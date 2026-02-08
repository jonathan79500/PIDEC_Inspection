using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
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
    public class PersonAttachmentConfiguration : IEntityTypeConfiguration<PersonAttachment>
    {
        public void Configure(EntityTypeBuilder<PersonAttachment> builder)
        {
            builder.ToTable("PersonAttachments");

            builder.HasKey(e => e.Id);

            // Configure AttachmentId as FK
            builder.Property(e => e.AttachmentId)
                .HasConversion(
                    id => id.Value,
                    guid => guid == Guid.Empty ? null : new AttachmentId(guid));

            builder.HasOne(e => e.Attachment)
                .WithMany(a => a.Persons) // matches Attachment.Persons collection
                .HasForeignKey(e => e.AttachmentId);

           

            builder.HasOne(e => e.Person)
                .WithMany(i => i.Attachments) // assuming Person has ICollection<PersonAttachment>
                .HasForeignKey(e => e.PersonId);
        }
    }

}
