using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Entities;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class CompleteTaskCommand(int id) : IRequest<string>
    {
        public int Id { get; } = id;
    }

    public class CompleteTaskCommandHandler(
        IAchievementRepository achievementRepo,
        ITaskRepository taskRepo,
        ITaskLogRepository taskLogRepo,
        ICategoryLevelRepository categoryRepo,
        IUserProfileRepository userRepo,
        IDistributedCache cache) : IRequestHandler<CompleteTaskCommand, string>
    {
        public async Task<string> Handle(CompleteTaskCommand command, CancellationToken token)
        {
            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task is null)
                throw new NotFoundException($"Task with ID {command.Id} was not found.");

            var taskLog = new TaskLog()
            {
                TaskId = command.Id,
                TaskTitle = task.Title,
                Category = task.Category,
                CompletedAt = DateTime.UtcNow,
                EXP_Gained = task.EXPValue,
                UserId = task.UserId,
            };

            //Update category level

            var category = await categoryRepo.GetByCategoryAsync(task.Category);
            if (category is null)
                throw new NotFoundException($"Category '{task.Category}' not found.");

            UpdateCategoryLevel(category, task.EXPValue);
            await categoryRepo.SaveChangesAsync();

            //Update user main level

            var user = await userRepo.GetUserByIdAsync(task.UserId);
            var categories = await categoryRepo.GetAllAsync();

            var totalCategoryLevels = categories.Sum(c => c.Level) - 4;
            var calculatedMainLevel = (totalCategoryLevels / 5) + 1;

            if (calculatedMainLevel > user.MainLevel)
                user.MainLevel = calculatedMainLevel;

            await taskLogRepo.AddTaskLogAsync(taskLog);
            task.Status = Domain.Enums.TaskStatus.Completed; 
            await taskLogRepo.SaveChangesAsync();
            await taskRepo.SaveChangesAsync();
            await userRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", token);
            await cache.RemoveAsync($"task_{task.Id}", token);
            await cache.RemoveAsync("task_logs_list", token);
            await cache.RemoveAsync("categories_list", token);
            await cache.RemoveAsync("users_list", token);

            // Step 1: Fetch necessary data
            var userAchievements = await achievementRepo.GetUserAchievementsAsync();
            var userTitles = await achievementRepo.GetUserTitlesAsync();
            var taskLogs = await taskLogRepo.GetTaskLogsAsync();
            var completedToday = taskLogs.Count(x => x.CompletedAt.Date == DateTime.UtcNow.Date);
            var totalTasks = taskLogs.Count();
            var hour = DateTime.UtcNow.Hour;
            var allCategories = await categoryRepo.GetAllAsync();
            var categoryLevel = category.Level;
            var allCategories5Plus = allCategories.All(c => c.Level >= 5);
            var allCategories10Plus = allCategories.All(c => c.Level >= 10);
            var allCategories15Plus = allCategories.All(c => c.Level >= 15);

            // Step 2: Define all 20 achievements
            var achievementRules = new List<(int Id, Func<bool> Condition)>
            {
                (1, () => totalTasks == 1),
                (2, () => user.MainLevel >= 10),
                (3, () => user.MainLevel >= 20),
                (4, () => user.MainLevel >= 30),
                (5, () => user.MainLevel >= 50),
                (6, () => user.MainLevel >= 100),
                (7, () => completedToday >= 10),
                (8, () => totalTasks >= 50),
                (9, () => totalTasks >= 100),
                (10, () => totalTasks >= 200),
                (11, () => categoryLevel >= 5),
                (12, () => categoryLevel >= 10),
                (13, () => categoryLevel >= 20),
                (14, () => allCategories5Plus),
                (15, () => task.EXPValue >= 50),
                (16, () => completedToday >= 10 && hour < 4),
                (17, () => hour >= 4 && hour < 6),
                (18, () => totalTasks % 100 == 0),
                (19, () => allCategories10Plus),
                (20, () => user.MainLevel >= 75),
            };

            // Step 3: Define all 10 titles
            var titleRules = new List<(int Id, Func<bool> Condition)>
            {
                (1, () => user.MainLevel >= 5),                       // Disciplined Soul
                (2, () => user.MainLevel >= 10),                      // Awakened One
                (3, () => user.MainLevel >= 20),  // Shadow Walker
                (4, () => user.MainLevel >= 30), // Skill Reaper
                (5, () => allCategories5Plus),                        // Master of Balance
                (6, () => totalTasks >= 100 && user.MainLevel >= 40), // Relentless Mind
                (7, () => completedToday >= 20),                      // Grinder
                (8, () => task.EXPValue >= 100),                      // Titan of Focus
                (9, () => allCategories15Plus),                       // Sage of Mastery
                (10, () => user.MainLevel >= 100),                    // The One Who Made Impossible
            };

            // Step 4: Evaluate and insert unlocked ones
            var newAchievements = achievementRules
                .Where(rule => rule.Condition() && !userAchievements.Any(x => x.AchievementId == rule.Id))
                .Select(rule => new UserAchievement
                {
                    UserId = user.Id,
                    AchievementId = rule.Id,
                    UnlockedAt = DateTime.UtcNow // if this property exists
                })
                .ToList();

            var newTitles = titleRules
                .Where(rule => rule.Condition() && !userTitles.Any(x => x.TitleId == rule.Id))
                .Select(rule => new UserTitle
                {
                    UserId = user.Id,
                    TitleId = rule.Id,
                    UnlockedAt = DateTime.UtcNow // if this property exists
                })
                .ToList();

            if (newAchievements.Any())
                foreach (var achievement in newAchievements)
                    await achievementRepo.UnlockAchievementAsync(achievement.AchievementId);

            if (newTitles.Any())
                foreach (var title in newTitles)
                    await achievementRepo.UnlockTitleAsync(title.TitleId);

            return "Task completed successfully!";
        }

        private static void UpdateCategoryLevel(CategoryLevel category, int expGained)
        {
            category.CurrentEXP += expGained;
            category.EXPToNextLevel -= expGained;

            while (category.EXPToNextLevel <= 0)
            {
                category.Level++;
                category.NeededEXP += 10;
                category.EXPToNextLevel += category.NeededEXP;
            }

        }
    }

    
}
