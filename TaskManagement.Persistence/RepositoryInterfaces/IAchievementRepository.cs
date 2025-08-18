using TaskManagement.Domain.Entities;

namespace TaskManagement.Persistence.RepositoryInterfaces
{
    public interface IAchievementRepository
    {
        Task<IEnumerable<UserTitle>> GetUserTitlesAsync();
        Task<IEnumerable<UserTitle>> GetUserTitlesAsync(int userId);

        Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync();
        Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync(int userId);

        Task<IEnumerable<Achievement>> GetAchievementsAsync();

        Task<IEnumerable<Title>> GetTitlesAsync();

        Task<bool> UnlockAchievementAsync(int achievementId, int userId);

        Task<bool> UnlockTitleAsync(int titleId, int userId);
    }
}
