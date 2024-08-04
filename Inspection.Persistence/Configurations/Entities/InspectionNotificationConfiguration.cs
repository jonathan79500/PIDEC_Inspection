

using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;

namespace Inspection.Persistence.Configurations.Entities;

public class InspectionNotificationConfiguration : IEntityTypeConfiguration<InspectionNotification>
{
    public void Configure(EntityTypeBuilder<InspectionNotification> builder)
    {

        ConfigureInspectionNotificationTable(builder);
    }

    private void ConfigureInspectionNotificationTable(EntityTypeBuilder<InspectionNotification> builder)
    {
        builder.ToTable("InspectionNotifications");          
        builder.Ignore(e=>e.InspectionNotificationId);

        var converter = new ValueConverter<InspectionNotificationId, Guid>(
        id => id.Value,
          guidValue => InspectionNotificationId.Create(guidValue));
             
       
        builder.Property(e => e.Id)
            .HasConversion(converter)
            .ValueGeneratedOnAdd();

        builder.HasOne(e => e.InspectionType);
        builder.Property(e => e.InspectionSubject);
        builder.Property(e => e.ContractShippingDueDate);
        builder.ComplexProperty(i => i.NotificationNumber).IsRequired();
        builder.HasOne(i => i.Owner);
        builder.HasOne(i => i.Order);
        

        builder.HasMany(inf => inf.Attachments);
        builder.Metadata.FindNavigation(nameof(InspectionNotification.Contacts));
        builder.Metadata.FindNavigation(nameof(InspectionNotification.Owner));

        //when we want flatten an object and spread it to mother object as one or more fields we use OwnesOne() Method
        // builder.ComplexProperty(e => e.Owner);
        // builder.OwnsOne(e => e.Owner);

        // var InspectionTypeIdconverter = new ValueConverter<InspectionTypeId, Guid>(
        //id => id.Value,
        //  guidValue => InspectionTypeId.Create(guidValue));
        // builder.Property(e => e.InspectionType.InspectionTypeId).HasConversion(InspectionTypeIdconverter) .ValueGeneratedOnAdd();
    }
}
