using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Inspection.Domain.Entities.Inspector;
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


            ConfigureInspectors(builder);

        }

      
        private void ConfigureInspectors(EntityTypeBuilder<Inspector> builder)
        {
            builder.ToTable("Inspectors");
         

            var converter = new ValueConverter<InspectorId, Guid>(
            id => id.Value,
            guidValue => new InspectorId(guidValue));
            builder.HasKey(e => e.InspectorId);

            builder.Property(e => e.InspectorId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();

            
            builder.HasMany(e => e.inspectorAttachments);
        }
    }

}
