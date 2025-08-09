using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Exceptions;
using TaskManagement.DataAccess;
using TaskManagement.Domain.Entities;
using TaskManagement.Entities;

namespace TaskManagement.Application.Services.Impl
{
    public class UserProfileService : IUserProfileService
    {
        private readonly DataContext _context;
        public UserProfileService(DataContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> CreateUserProfileAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == userId);
            if(user is null)
            {
                throw new NotFoundException("User not found");
            }

            var userProfile = new UserProfile
            {
                UserId = userId,
                Username = user.Username,
                Bio = "",
                ProfilePictureUrl = "",
                CurrentStreak = 0,
                LongestStreak = 0,
                CurrentTitle = "The Beginning",
                TotalExp = 0,
                LastLevelUp = DateTime.UtcNow,
                MainLevel = 1,
                CreatedDate = DateTime.UtcNow
            };

            var userCategories = new UserCategory[] {
                new UserCategory
                {
                    UserId = userProfile.Id,
                    CategoryId = 1
                },
                new UserCategory
                {
                    UserId = userProfile.Id,
                    CategoryId = 2
                },
                new UserCategory
                {
                    UserId = userProfile.Id,
                    CategoryId = 3
                },
                new UserCategory
                {
                    UserId = userProfile.Id,
                    CategoryId = 4
                }
            };

            foreach(var userCategory in userCategories)
            {
                await _context.UserCategories.AddAsync(userCategory);
            }

            userProfile.CategoryLevels = userCategories.ToList();
            await _context.UserProfiles.AddAsync(userProfile);

            await _context.SaveChangesAsync();
            return userProfile;
        }
    }
}
