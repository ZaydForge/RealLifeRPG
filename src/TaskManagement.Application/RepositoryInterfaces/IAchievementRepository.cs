using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.RepositoryInterfaces
{
    public interface IAchievementRepository
    {
        Task<IEnumerable<UserTitle>> GetUserTitlesAsync();

        Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync();

        Task<IEnumerable<Achievement>> GetAchievementsAsync();

        Task<IEnumerable<Title>> GetTitlesAsync();

        Task<bool> UnlockAchievementAsync(int achievementId);

        Task<bool> UnlockTitleAsync(int titleId);
    }
}
