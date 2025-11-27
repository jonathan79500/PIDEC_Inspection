using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Inspection.Persistence.Configurations.Entities;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {

        ConfigureOrderTable(builder);
       
        
    }

    //private void ConfigureAttachments(EntityTypeBuilder<InspectionNotification> builder)
    //{
       
    //    builder.OwnsMany(inf => inf.Attachments
    //    , 
    //        atb =>
    //        {
    //            atb.ToTable("Attachments");

              
    //            //or
    //            atb.HasKey(nameof(Attachment.Id), "InspectionNotificationId");

    //            atb.WithOwner().HasForeignKey("InspectionNotificationId");

    //            var converter = new ValueConverter<AttachmentId, Guid>(
    //             id => id.Value,
    //              guidValue => AttachmentId.Create(guidValue));

    //            atb.Property(a => a.Id)
    //                .HasColumnName("Id")
    //                .ValueGeneratedNever()
    //                .HasConversion(converter);

    //          //  atb.HasKey("AttachmentId", "InspectionNotificationId");

    //            atb.Property(a => a.Name).HasMaxLength(100);
    //            atb.Property(a => a.Description).HasMaxLength(255);
    //            atb.Property(a => a.Content).HasColumnType("image");

    //            // if attchment record had many children so again like above we could define atb.OwnsMany(....) and etc
    //        }
    //    );

    //   // builder.Navigation(i => i.Attachments).Metadata.SetField("_attachments");
    //    builder.Navigation(i => i.Contacts).Metadata.SetField("_contacts");
    //    builder.Metadata.FindNavigation(nameof(InspectionNotification.Attachments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    //    builder.Metadata.FindNavigation(nameof(InspectionNotification.Contacts))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    //}

    public void ConfigureOrderTable(EntityTypeBuilder<Order> builder)
    {
        var converter = new ValueConverter<OrderId, Guid>(
     id => id.Value,
     guidValue => new OrderId(guidValue));
        builder.HasKey(e => e.OrderId);
        builder.Property(e => e.OrderId)
            .HasConversion(converter)
            .ValueGeneratedOnAdd();

        // Configure primary key
        // builder.HasKey(o => o.No);
        //Configure properties
        builder.Property(o => o.No).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Title).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Version).IsRequired();
        builder.Property(o => o.EdmsId).HasMaxLength(100);
        builder.Property(o => o.EquipmentTagNo).HasMaxLength(100);
        builder.Property(o => o.ITP_NO).HasMaxLength(50);    

        // Configure relationships

        builder.HasOne(o => o.Project)
            .WithMany(p => p.Orders); // Assuming Project has a collection of Orders


        
        builder.HasMany(o => o.Subjects)
            .WithOne()
            .HasForeignKey(s => s.InspectionSubjectId); // Assuming OrderSubject has an OrderId foreign key property

        //builder.HasMany(o => o.QCMembers)
        //    .WithOne()
        //    .HasForeignKey(q => q.OrderId); // Assuming OrderQCMember has an OrderId foreign key property

        builder.HasMany(o => o.Contacts)
            .WithOne()
            .HasForeignKey(q => q.ContactId); // Assuming OrderQCContactPerson has an OrderId foreign key property

        builder.HasMany(o => o.ExceptionContacts)
            .WithOne()  
            .HasForeignKey(e => e.ProjectContactId);

        builder.HasMany(o => o.VendorDocuments);
            //.WithOne()
            //.HasForeignKey(vd => vd.OrderId); // Assuming VendorDocument has an OrderId foreign key property 

        // Configure table name if different from the entity name
        builder.ToTable("Orders");
    }   
}
