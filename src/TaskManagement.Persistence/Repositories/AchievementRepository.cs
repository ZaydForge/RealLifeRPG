using Microsoft.EntityFrameworkCore;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Persistence.Repositories
{
    public class AchievementRepository(DataContext context) : IAchievementRepository
    {
        public async Task<IEnumerable<Achievement>> GetAchievementsAsync()
        {
            return await context.Achievements.ToListAsync();
        }

        public async Task<IEnumerable<Title>> GetTitlesAsync()
        {
            return await context.Titles.ToListAsync();
        }

        public async Task<IEnumerable<UserAchievement>> GetUserAchievementsAsync()
        {
            return await context.UserAchievements.ToListAsync();
        }

        public async Task<IEnumerable<UserTitle>> GetUserTitlesAsync()
        {
            return await context.UserTitles.ToListAsync();
        }

        public async Task<bool> UnlockAchievementAsync(int achievementId)
        {
            var achievement = await context.Achievements.FirstOrDefaultAsync(r => r.Id == achievementId);
            if (achievement == null)
            {
                return false; // Achievement not found
            }

            var userAchievement = new UserAchievement
            {
                UserId = 1,
                AchievementId = achievementId,
                UnlockedAt = DateTime.UtcNow
            };

            await context.UserAchievements.AddAsync(userAchievement);
            await context.SaveChangesAsync();
            return true; // Achievement unlocked successfully
        }

        public async Task<bool> UnlockTitleAsync(int titleId)
        {
            var title = await context.Titles.FirstOrDefaultAsync(r => r.Id == titleId);
            if (title == null)
            {
                return false; // Title not found
            }

            var userTitle = new UserTitle
            {
                UserId = 1,
                TitleId = titleId,
                UnlockedAt = DateTime.UtcNow
            };

            await context.UserTitles.AddAsync(userTitle);
            await context.SaveChangesAsync();
            return true; // Title unlocked successfully
        }
    }
}
