using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.RepositoryInterfaces;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class CompleteTaskCommand(int id) : IRequest<string>
    {
        public int Id { get; } = id;
    }

    public class CompleteTaskCommandHandler(
        ITaskRepository taskRepo,
        ITaskLogRepository taskLogRepo,
        ICategoryLevelRepository categoryRepo,
        IUserRepository userRepo,
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

            //Update user main level

            var user = await userRepo.GetUserByIdAsync(task.UserId);
            var categories = await categoryRepo.GetAllAsync();

            var totalCategoryLevels = categories.Sum(c => c.Level) - 4;
            var calculatedMainLevel = (totalCategoryLevels / 5) + 1;

            if (calculatedMainLevel > user.MainLevel)
                user.MainLevel = calculatedMainLevel;

            await taskLogRepo.AddTaskLogAsync(taskLog);
            await taskRepo.Delete(task);
            await taskLogRepo.SaveChangesAsync();
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", token);
            await cache.RemoveAsync($"task_{task.Id}", token);
            await cache.RemoveAsync("task_logs_list", token);
            await cache.RemoveAsync("categories_list", token);
            await cache.RemoveAsync("users_list", token);

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
