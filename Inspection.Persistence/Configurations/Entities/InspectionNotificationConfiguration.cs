

using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;

namespace Inspection.Persistence.Configurations.Entities;

public class InspectionNotificationConfiguration : IEntityTypeConfiguration<InspectionNotification>
{
    public void Configure(EntityTypeBuilder<InspectionNotification> builder)
    {

        ConfigureInspectionNotificationTable(builder);
        ConfigureAttachments(builder);
        
    }

    private void ConfigureAttachments(EntityTypeBuilder<InspectionNotification> builder)
    {
        

        builder.OwnsMany(inf => inf.Attachments
        , 
            atb =>
            {
                atb.ToTable("Attachments");

              
                //or
                atb.HasKey(nameof(Attachment.Id), "InspectionNotificationId");

                atb.WithOwner().HasForeignKey("InspectionNotificationId");

                var converter = new ValueConverter<AttachmentId, Guid>(
                 id => id.Value,
                  guidValue => AttachmentId.Create(guidValue));

                atb.Property(a => a.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedNever()
                    .HasConversion(converter);

              //  atb.HasKey("AttachmentId", "InspectionNotificationId");

                atb.Property(a => a.Name).HasMaxLength(100);
                atb.Property(a => a.Description).HasMaxLength(255);
                atb.Property(a => a.PDFSize).HasMaxLength(20);
                atb.Property(a => a.Content).HasColumnType("image");               
                atb.Property(e => e.CreatedBy).HasMaxLength(100);
                atb.Property(e => e.ModifiedBy).HasMaxLength(100);

                // if attchment record had many children so again like above we could define atb.OwnsMany(....) and etc
            }
        );

        builder.Navigation(i => i.Attachments).Metadata.SetField("_attachments");
       // builder.Navigation(i => i.InspectionNotificationContacts).Metadata.SetField("_contacts");
        builder.Metadata.FindNavigation(nameof(InspectionNotification.Attachments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        //builder.Metadata.FindNavigation(nameof(InspectionNotification.InspectionNotificationContacts))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private void ConfigureInspectionNotificationTable(EntityTypeBuilder<InspectionNotification> builder)
    {
        builder.ToTable("InspectionNotifications");
        //builder.HasKey(x => x.Id);       

        //var converter = new ValueConverter<InspectionNotificationId, Guid>(
        //id => id.Value,
        //  guidValue => InspectionNotificationId.Create(guidValue));



        //builder.Property(e => e.Id)
        //    .HasColumnName("Id")
        //     .ValueGeneratedNever()
        //    .HasConversion(converter);

        var converter = new ValueConverter<InspectionNotificationId, Guid>(
        id => id.Value,
        guidValue => new InspectionNotificationId(guidValue));
        builder.HasKey(e => e.InspectionNotificationId);
      
        builder.Property(e => e.InspectionNotificationId)
            .HasConversion(converter)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name).HasMaxLength(200);
       // builder.Property(e => e.InspectionSubject).HasMaxLength(600);
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.ModifiedBy).HasMaxLength(100);
        builder.Property(e => e.RequestInspectionDateStart).HasColumnName("RequestInspectionDateStart");
        builder.Property(e => e.RequestInspectionDateEnd).HasColumnName("RequestInspectionDateEnd");
        builder.Property(e => e.RequestInspectionPersianDateStart).HasColumnName("RequestInspectionPersianDateStart").HasMaxLength(50);
        builder.Property(e => e.RequestInspectionPersianDateEnd).HasColumnName("RequestInspectionPersianDateEnd").HasMaxLength(50);
        builder.Property(e => e.RequestInspectionDueDate).HasColumnName("RequestInspectionDueDate");
        builder.Property(e => e.RequestInspectionTimesInPeriod).HasColumnName("RequestInspectionTimesInPeriod");
        builder.Property(e => e.RequestInspectionPeriodDays).HasColumnName("RequestInspectionPeriodDays");
        

        // builder.Property(e => e.MainInspectionNotificationId).HasConversion(converter);


        //we do this OwnsOne for value objects which should flatten to InspectionNotification Table and has not any unique ids
        //builder.OwnsOne(e => e.NotificationNumber, nnb =>
        //{
        //    nnb.Property(n => (n.ProjectNumber+"-"+n.OrderNumber+"-NFT-"+n.Version.ToString()+"."+n.Revision.ToString())).HasColumnName("NotificationNumber").IsRequired();
        //});

        builder.Property(e=>e.NotificationNumber).HasColumnName("NotificationNumber").IsRequired().HasMaxLength(100);

       // builder.HasOne(e => e.InspectionType).WithMany().HasForeignKey("InspectionTypeId");
      
       // builder.Property(e => e.ContractShippingDueDate);
        //builder.ComplexProperty(i => i.NotificationNumber).IsRequired();        
        builder.HasOne(i => i.Order);//.WithMany().HasForeignKey("OrderId");
        builder.HasOne(i => i.Request);//.WithMany().OnDelete(DeleteBehavior.NoAction);
      
    }
}
