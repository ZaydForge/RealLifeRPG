using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryLevel>
{
    public void Configure(EntityTypeBuilder<CategoryLevel> builder)
    {
        builder.HasData(
            new CategoryLevel { Id = 1, Category = Domain.Enums.Category.Intelligence, Level = 1, CurrentEXP = 0, EXPToNextLevel = 100, NeededEXP = 100, UserId = 1 },
            new CategoryLevel { Id = 2, Category = Domain.Enums.Category.Strength, Level = 1, CurrentEXP = 0, EXPToNextLevel = 100, NeededEXP = 100, UserId = 1 },
            new CategoryLevel { Id = 3, Category = Domain.Enums.Category.Wisdom, Level = 1, CurrentEXP = 0, EXPToNextLevel = 100, NeededEXP = 100, UserId = 1 },
            new CategoryLevel { Id = 4, Category = Domain.Enums.Category.Soul, Level = 1, CurrentEXP = 0, EXPToNextLevel = 100, NeededEXP = 100, UserId = 1 });

    }
}
