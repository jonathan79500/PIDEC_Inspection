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
        }
    }
}
