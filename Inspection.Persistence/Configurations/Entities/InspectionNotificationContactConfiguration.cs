using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Inspection.Persistence.Configurations.Entities
{
    public class InspectionNotificationContactConfiguration : IEntityTypeConfiguration<InspectionNotificationContact>
    {
        public void Configure(EntityTypeBuilder<InspectionNotificationContact> builder)
        {
            builder.ToTable("InspectionNotificationContacts")
                .HasKey(insc=>insc.Id);

            var converter = new ValueConverter<ContactId, Guid>(
           CId => CId.Value,
          guidValue => new ContactId(guidValue));

            var converter1 = new ValueConverter<InspectionNotificationId, Guid>(
                 CId => CId.Value,
        guidValue => new InspectionNotificationId(guidValue));



            // builder.HasKey(e =>new { e.ContactId , e.InspectionNotificationId});
            builder.Property(e => e.ContactId)
                .HasConversion(converter);

            builder.Property(e => e.InspectionNotificationId)
                .HasConversion(converter1);


           // builder.HasKey(inst => new { inst.InspectionNotificationId, inst.ContactId });
            //.ValueGeneratedOnAdd();


            builder
           .HasOne(ic => ic.InspectionNotification)
           .WithMany(ic => ic.Contacts)
           .HasForeignKey(ic => ic.InspectionNotificationId);

            builder
          .HasOne(ic => ic.Contact)
          .WithMany(ic => ic.InspectionNotifications)
          .HasForeignKey(ic => ic.ContactId);
        }
    }
}
