using Inspection.Domain.Entities.Inspector;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Configurations.Entities
{
    public class InspectorConfiguration : IEntityTypeConfiguration<Inspector>
    {
        public void Configure(EntityTypeBuilder<Inspector> builder)
        {
            builder.ToTable("Inspectors");

            builder.HasKey(x => x.InspectorId);

            builder.Property(x => x.InspectorId)
                   .ValueGeneratedNever();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.HasOne(x => x.InspectionCompanyContact)
                   .WithMany()
                   .HasForeignKey(x => x.InspectionCompanyContactId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.InspectionNotifications)
                   .WithOne(x => x.Inspector)
                   .HasForeignKey(x => x.InspectorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.Phone).HasMaxLength(30);

            builder.HasIndex(x => x.InspectionCompanyContactId);
        }

        }
    public class InspectionSkillConfiguration : IEntityTypeConfiguration<InspectionSkill>
    {
        public void Configure(EntityTypeBuilder<InspectionSkill> builder)
        {
            builder.ToTable("InspectionSkills");

            builder.HasKey(x => x.SkillId); 

            builder.Property(x => x.SkillId)
                   .ValueGeneratedNever();

                builder.HasKey(x => x.SkillId);
                builder.Property(x => x.Code).HasMaxLength(10).IsRequired();
                builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
                builder.HasIndex(x => x.Code).IsUnique();
                
                builder.HasData(
                    new InspectionSkill { SkillId = Guid.Parse("22222222-2222-2222-2222-000000000001"), Code = "VT", Name = "Visual Testing", IsActive = true },
                    new InspectionSkill { SkillId = Guid.Parse("22222222-2222-2222-2222-000000000002"), Code = "RT", Name = "Radiographic Testing", IsActive = true },
                    new InspectionSkill { SkillId = Guid.Parse("22222222-2222-2222-2222-000000000003"), Code = "UT", Name = "Ultrasonic Testing", IsActive = true },
                    new InspectionSkill { SkillId = Guid.Parse("22222222-2222-2222-2222-000000000004"), Code = "MT", Name = "Magnetic Particle Testing", IsActive = true });
            
        }
    }

    public class InspectorSkillConfiguration : IEntityTypeConfiguration<InspectorSkill>
    {
        public void Configure(EntityTypeBuilder<InspectorSkill> builder)
        {
            builder.ToTable("InspectorSkills");

            builder.HasKey(x => x.InspectorSkillId);

            builder.Property(x => x.InspectorSkillId)
                   .ValueGeneratedNever();

            builder.HasOne(x => x.Inspector)
                 .WithMany(i => i.Skills)
                 .HasForeignKey(x => x.InspectorId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Skill)
             .WithMany()
             .HasForeignKey(x => x.SkillId)
             .OnDelete(DeleteBehavior.Restrict);



            builder.Property(x => x.Level).HasConversion<string>().HasMaxLength(20);


            builder.HasIndex(x => new { x.InspectorId, x.SkillId }).IsUnique();
        }
    }


  
 
}
