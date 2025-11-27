using Inspection.Domain.Entities.Contacts;
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
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts");
            var converter = new ValueConverter<ContactId, Guid>(
          id => id.Value,
          guidValue => new ContactId(guidValue));
            builder.HasKey(e => e.ContactId);
            //builder.Property(e => e.ContactId)
            //    .HasConversion(converter)
            //    .ValueGeneratedOnAdd();

            builder.Property(e => e.UserName).IsRequired();
            builder.Property(e => e.DomainId).IsRequired();
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e=> e.Email).IsRequired();
            builder.Property(e=>e.Phone).IsRequired();
            builder.Property(e=>e.Mobile).IsRequired();
        }
    }
}
