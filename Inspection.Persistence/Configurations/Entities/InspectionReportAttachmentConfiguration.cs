using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Configurations.Entities
{


    public class InspectionReportAttachmentConfiguration : IEntityTypeConfiguration<InspectionReportAttachment>
    {
        public void Configure(EntityTypeBuilder<InspectionReportAttachment> builder)
        {

          
            ConfigureInspectionReportAttachments(builder);

        }

      
        private void ConfigureInspectionReportAttachments(EntityTypeBuilder<InspectionReportAttachment> builder)
        {
            builder.ToTable("InspectionReportAttachments");
         

            var converter = new ValueConverter<InspectionReportAttachmentId, Guid>(
            id => id.Value,
            guidValue => new InspectionReportAttachmentId(guidValue));
            builder.HasKey(e => e.InspectionReportAttachmentId);

            builder.Property(e => e.InspectionReportAttachmentId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name).HasMaxLength(200);


        }
    }

}
