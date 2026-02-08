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


    //public class InspectionReportAttachmentConfiguration : IEntityTypeConfiguration<InspectionReportAttachment>
    //{
    //    public void Configure(EntityTypeBuilder<InspectionReportAttachment> builder)
    //    {
    //        ConfigureInspectionReportAttachments(builder);
    //    }

    //    private void ConfigureInspectionReportAttachments(EntityTypeBuilder<InspectionReportAttachment> builder)
    //    {
    //        builder.ToTable("InspectionReportAttachments");


    //        var converter = new ValueConverter<InspectionReportAttachmentId, Guid>(
    //        id => id.Value,
    //        guidValue => new InspectionReportAttachmentId(guidValue));
    //        builder.HasKey(e => e.InspectionReportAttachmentId);

    //        builder.Property(e => e.InspectionReportAttachmentId)
    //            .HasConversion(converter)
    //            .ValueGeneratedOnAdd();




    //    }
    //}
    public class InspectionReportAttachmentConfiguration : IEntityTypeConfiguration<InspectionReportAttachment>
    {
        public void Configure(EntityTypeBuilder<InspectionReportAttachment> builder)
        {
            builder.ToTable("InspectionReportAttachments");

            // Primary key
            builder.HasKey(e => e.InspectionReportAttachmentId);

            builder.Property(e => e.InspectionReportAttachmentId)
                .HasConversion(
                    id => id.Value,
                    guid => new InspectionReportAttachmentId(guid))
                .ValueGeneratedOnAdd();

            // Foreign key to InspectionReport
            builder.Property(e => e.InspectionReportId)
                .HasConversion(
                    id => id.Value,
                    guid => new InspectionReportId(guid));

            builder.HasOne(e => e.InspectionReport)
                .WithMany(r => r.Attachments) // assuming InspectionReport has ICollection<InspectionReportAttachment>
                .HasForeignKey(e => e.InspectionReportId);

            // Foreign key to Attachment
            builder.Property(e => e.AttachmentId)
                .HasConversion(
                    id => id.Value,
                    guid => new AttachmentId(guid));

            builder.HasOne(e => e.Attachment)
                .WithMany(a => a.InspectionReports) // matches Attachment.InspectionReports
                .HasForeignKey(e => e.AttachmentId);
        }
    }

}
