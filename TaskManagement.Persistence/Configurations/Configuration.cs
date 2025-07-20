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

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasData(
            new AppUser
            {
                Id = 1,
                Username = "Zayd",
                Email = "zaydabduxamidov2008@gmail.com",
                PasswordHash = "Password",
                MainLevel = 1,
                CreatedDate = new DateTime(2025, 7, 19, 0, 0, 0, DateTimeKind.Utc) // ✅ explicitly UTC
            });
        builder.HasKey(x => x.Id);

        builder.Property(r => r.Username)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(r => r.Email)
            .IsRequired();

        builder.Property(r => r.PasswordHash)
            .IsRequired();
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
