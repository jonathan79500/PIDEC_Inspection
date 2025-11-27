using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
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
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> atb)
        {
            atb.ToTable("Attachments");

            atb.HasKey(a => a.AttachmentId);

            var converter = new ValueConverter<AttachmentId, Guid>(
                id => id.Value,
                guidValue => AttachmentId.Create(guidValue));

            atb.Property(a => a.AttachmentId)
                .HasColumnName("AttachmentId")              
                .HasConversion(converter);

            atb.Property(a => a.Name).HasMaxLength(100);
            atb.Property(a => a.Description).HasMaxLength(255);
            atb.Property(a => a.PDFSize).HasMaxLength(20);
            atb.Property(a => a.Content).HasColumnType("image");
            atb.Property(e => e.CreatedBy).HasMaxLength(100);
            atb.Property(e => e.ModifiedBy).HasMaxLength(100);
          
        }
    }

}
