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
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ToTable("Attachments");
            var converter = new ValueConverter<AttachmentId, Guid>(
          id => id.Value,
          guidValue => new AttachmentId(guidValue));
            builder.HasKey(e => e.AttachmentId);
            builder.Property(e => e.AttachmentId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();
        }
    }
}
