using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;

namespace Inspection.Persistence.Configurations.Entities
{

    public class InspectionTypeConfiguration : IEntityTypeConfiguration<InspectionType>
    {
        public void Configure(EntityTypeBuilder<InspectionType> builder)
        {
            builder.ToTable("InspectionTypes");
            var converter = new ValueConverter<InspectionTypeId, Guid>(
          id => id.Value,
          guidValue => new InspectionTypeId(guidValue));
            builder.HasKey(e => e.InspectionTypeId);
            builder.Property(e => e.InspectionTypeId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
        }
    }
}
