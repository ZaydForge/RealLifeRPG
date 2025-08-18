using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;
using TaskManagement.Entities;

namespace TaskManagement.Rules;

public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.EXPValue)
            .IsRequired();
    }

}

public class UserConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasData(
            new UserProfile
            {
                Id = 1,
                UserId = 1,
                Username = "Zayd",
                Bio = "Persistence, consistency and gratitude - key to success",
                ProfilePictureUrl = "https://example.com/profile.jpg",
                CurrentStreak = 0,
                LongestStreak = 0,
                CurrentTitle = "The Beginning",
                TotalExp = 0,
                LastLevelUp = new DateTime(2025, 7, 24, 0, 0, 0, DateTimeKind.Utc), // ✅ explicitly UTC
                MainLevel = 1,
                CreatedDate = new DateTime(2025, 7, 24, 0, 0, 0, DateTimeKind.Utc) // ✅ explicitly UTC
            });
        builder.HasKey(x => x.Id);

        builder.HasOne(r => r.User)
            .WithOne(r => r.Profile)
            .HasForeignKey<UserProfile>(p => p.UserId);

        builder.Property(r => r.Username)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.Bio)
            .HasMaxLength(100);

    }

}

//public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
//{
//    public void Configure(EntityTypeBuilder<UserAchievement> builder)
//    {

//        builder.HasOne(r => r.User)
//            .WithMany(r => r.Achievements);
//    }

//}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new Category {Id = 1, CategoryName = Domain.Enums.CategoryName.Intelligence },
            new Category {Id = 2, CategoryName = Domain.Enums.CategoryName.Strength },
            new Category {Id = 3, CategoryName = Domain.Enums.CategoryName.Wisdom },
            new Category { Id = 4, CategoryName = Domain.Enums.CategoryName.Soul});
    }
}
