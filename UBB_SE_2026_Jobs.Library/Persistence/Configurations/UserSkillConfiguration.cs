using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UBB_SE_2026_Jobs.Library.Domain;

namespace UBB_SE_2026_Jobs.Library.Persistence.Configurations;

public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("UserSkills");

        // User side is already configured in UserConfiguration.HasMany(userWithSkills => userWithSkills.Skills)
        // Only configure the Skill side here
        builder.HasOne(userSkillToLook => userSkillToLook.Skill)
            .WithMany()
            .HasForeignKey("SkillId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasKey("UserId", "SkillId");

        builder.HasIndex("SkillId");
    }
}

