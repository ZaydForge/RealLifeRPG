using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Entities;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Application.Services;

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
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo) : IRequestHandler<CompleteTaskCommand, string>
    {
        public async Task<string> Handle(CompleteTaskCommand command, CancellationToken token)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task is null || task.UserId != profileId)
                throw new NotFoundException($"Task with ID {command.Id} was not found.");

            var taskLog = new TaskLog()
            {
                TaskId = command.Id,
                TaskTitle = task.Title,
                TaskDescription = task.Description ?? "",
                Category = task.Category,
                CompletedAt = DateTime.UtcNow,
                EXP_Gained = task.EXPValue,
                UserId = task.UserId,
            };

            //Update category level

            var category = await categoryRepo.GetByCategoryAsync(task.Category, profileId);
            if (category is null)
                throw new NotFoundException($"Category '{task.Category}' not found.");

            UpdateCategoryLevel(category, task.EXPValue);
            await categoryRepo.SaveChangesAsync();

            //Update user main level and profile stats

            var user = await userRepo.GetProfileByIdAsync(task.UserId);
            var categories = (await categoryRepo.GetAllAsync()).Where(c => c.UserId == profileId);

            var totalCategoryLevels = categories.Sum(c => c.Level) - 4;
            var calculatedMainLevel = (totalCategoryLevels / 5) + 1;

            var previousMainLevel = user.MainLevel;
            if (calculatedMainLevel > user.MainLevel)
            {
                user.MainLevel = calculatedMainLevel;
                user.LastLevelUp = DateTime.UtcNow;
            }

            // Update total experience
            user.TotalExp += task.EXPValue;

            // Update streak logic
            await UpdateUserStreak(user, taskLogRepo, profileId);

            await taskLogRepo.AddTaskLogAsync(taskLog);
            task.Status = Domain.Enums.TaskStatus.Completed; 
            await taskLogRepo.SaveChangesAsync();
            await taskRepo.SaveChangesAsync();
            await userRepo.SaveChangesAsync();

            await cache.RemoveAsync($"tasks_list_user_{profileId}", token);
            await cache.RemoveAsync($"task_{task.Id}_user_{profileId}", token);
            await cache.RemoveAsync($"task_logs_list_user_{profileId}", token);
            await cache.RemoveAsync($"categories_list_user_{profileId}", token);
            await cache.RemoveAsync($"users_list_user_{profileId}", token);

            // Step 1: Fetch necessary data
            var achievemets = await achievementRepo.GetAchievementsAsync();
            var titles = await achievementRepo.GetTitlesAsync();
            var userAchievements = await achievementRepo.GetUserAchievementsAsync(profileId);
            var userTitles = await achievementRepo.GetUserTitlesAsync(profileId);
            var taskLogs = await taskLogRepo.GetTaskLogsAsync(profileId);
            var completedToday = taskLogs.Count(x => x.CompletedAt.Date == DateTime.UtcNow.Date);
            var totalTasks = taskLogs.Count();
            var hour = DateTime.UtcNow.Hour;
            var allCategories = (await categoryRepo.GetAllAsync()).Where(c => c.UserId == profileId);
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
                (1, () => user.MainLevel == 1 && totalTasks >= 1),
                (2, () => user.MainLevel >= 5), 
                (3, () => user.MainLevel >= 10),
                (4, () => user.MainLevel >= 20),
                (5, () => user.MainLevel >= 30),
                (6, () => user.MainLevel >= 40),
                (7, () => user.MainLevel >= 50),
                (8, () => user.MainLevel >= 100),
                (9, () => task.EXPValue >= 1000), 
                (10, () => totalTasks >= 1000),    
            };

            // Step 4: Evaluate and insert unlocked ones
            var newAchievements = achievementRules
                .Where(rule => rule.Condition())
                .Select(rule => new UserAchievement
                {
                    UserId = profileId,
                    AchievementId = rule.Id,
                    UnlockedAt = DateTime.UtcNow
                })
                .ToList();

            var newTitles = titleRules
                .Where(rule => rule.Condition())
                .Select(rule => new UserTitle
                {
                    UserId = profileId,
                    TitleId = rule.Id,
                    UnlockedAt = DateTime.UtcNow
                })
                .ToList();

            if (newAchievements.Any())
            {
                foreach (var achievement in newAchievements)
                {
                    var exists = userAchievements.Any(x => x.AchievementId == achievement.AchievementId);
                    if (!exists)
                        await achievementRepo.UnlockAchievementAsync(achievement.AchievementId, profileId);
                }
                
                // Invalidate achievements cache
                await cache.RemoveAsync($"user_achievements_list_user_{profileId}", token);
                await cache.RemoveAsync($"achievements_list_user_{profileId}", token);
            }

            if (newTitles.Any())
            {
                foreach (var title in newTitles)
                {
                    var exists = userTitles.Any(x => x.TitleId == title.TitleId);
                    if (!exists)
                        await achievementRepo.UnlockTitleAsync(title.TitleId, profileId);
                }
                
                // Invalidate titles cache
                await cache.RemoveAsync($"user_titles_list_user_{profileId}", token);
                await cache.RemoveAsync($"titles_list_user_{profileId}", token);
            }

            return "Task completed successfully!";
        }

        private static void UpdateCategoryLevel(UserCategory category, int expGained)
        {
            category.CurrentEXP += expGained;
            category.EXPToNextLevel -= expGained;

            while (category.EXPToNextLevel <= 0)
            {
                category.Level++;
                category.NeededEXP += 10;
                category.EXPToNextLevel += category.NeededEXP;
                category.LastLevelUp = DateTime.UtcNow;
            }

        }

        private static async Task UpdateUserStreak(UserProfile user, ITaskLogRepository taskLogRepo, int userId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            
            // Get user's task logs for streak calculation
            var allTaskLogs = await taskLogRepo.GetTaskLogsAsync(userId);
            
            // Check if user completed any tasks today
            var completedToday = allTaskLogs.Any(log => log.CompletedAt.Date == today);
            
            // Check if user completed any tasks yesterday
            var completedYesterday = allTaskLogs.Any(log => log.CompletedAt.Date == yesterday);
            
            if (completedToday)
            {
                // If user completed tasks yesterday, continue the streak
                if (completedYesterday || user.CurrentStreak == 0)
                {
                    user.CurrentStreak++;
                }
                // If no tasks yesterday but tasks today, reset streak to 1
                else
                {
                    user.CurrentStreak = 1;
                }
                
                // Update longest streak if current streak is higher
                if (user.CurrentStreak > user.LongestStreak)
                {
                    user.LongestStreak = user.CurrentStreak;
                }
            }
        }
    }

    
}
