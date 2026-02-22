using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.Contacts;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
using Inspection.Domain.Entities.Inspector;
using Inspection.Domain.Entities.Orders;

using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Domain.Entities.Vendor;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Inspection.Persistence;

public class InspectionDbContext : IdentityDbContext<SystematicUser, SystematicRole, string>
{
    public InspectionDbContext(DbContextOptions<InspectionDbContext> options):base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {       
         modelBuilder.Ignore<InspectionNotificationId>();
        modelBuilder.Ignore<ProjectId>();
        modelBuilder.Ignore<OrderId>();
         modelBuilder.Ignore<AttachmentId>();
        modelBuilder.Ignore<VendorDocumentId>();  
        modelBuilder.Entity<InspectionNotification>().Ignore("NotificationNumberParts");
        modelBuilder.Ignore<InspectionSubjectId>();
        modelBuilder.Ignore<InspectionTypeId>();
        modelBuilder.Ignore<ContactId>();
        modelBuilder.Ignore<InspectionReportId>();
        modelBuilder.Ignore<InspectorId>();
        modelBuilder.Ignore<InspectionReportAttachmentId>();


        //modelBuilder.Ignore("Attachments");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InspectionDbContext).Assembly);

        var converterAttachmentId = new ValueConverter<AttachmentId, Guid>(
            id => id.Value,
        guidValue => new AttachmentId(guidValue));


//        modelBuilder.Entity<InspectionNotification>()
//.HasOne(n => n.Revision)
//.WithMany(r => r.InspectionNotifications)
//.HasForeignKey(n => n.RevisionId)
//.OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.Restrict

    //    modelBuilder.Entity<InspectionReportAttachment>()
    //.HasOne(a => a.InspectionReport)
    //.WithMany(r => r.inspectionReportAttachments)
    //.HasForeignKey(a => a.InspectionReportId)
    //.IsRequired(); // or false if optional

        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<SystematicUserRole>()
       .HasOne(ur => ur.User)
       .WithMany(u => u.UserRoles)
       .HasForeignKey(ur => ur.UserId)
       .IsRequired();

        modelBuilder.Entity<SystematicUserRole>()
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();


        modelBuilder.Entity<CirculationAttachments>()
        .ToTable("CirculationAttachments")
        .HasKey(ca => ca.Id);


        modelBuilder.Entity<CirculationAttachments>()
            .HasOne(ca => ca.Circulation)
            .WithMany(ca => ca.Attachments)
            .HasForeignKey(ca => ca.CirculationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CirculationAttachments>()
           .HasOne(ca => ca.Attachment)
           .WithMany(ca => ca.Circulations)
           .HasForeignKey(ca => ca.AttachmentId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InspectorOrder>()
           .ToTable("InspectorOrders")
           .HasKey(ca => ca.Id);

        modelBuilder.Entity<InspectorOrder>()
            .HasOne(ca => ca.Inspector)
            .WithMany(ca => ca.inspectedOrders)
            .HasForeignKey(ca => ca.InspectorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InspectorOrder>()
           .HasOne(ca => ca.Order)
           .WithMany(ca => ca.Inspectors)
           .HasForeignKey(ca => ca.OrderId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InspectorProject>()
           .ToTable("InspectorProjects")
           .HasKey(ca => ca.Id);

        modelBuilder.Entity<InspectorProject>()
            .HasOne(ca => ca.Project)
            .WithMany(ca => ca.Inspectors)
            .HasForeignKey(ca => ca.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InspectorProject>()
           .HasOne(ca => ca.Inspector)
           .WithMany(ca => ca.inspectedProjects)
           .HasForeignKey(ca => ca.InspectorId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PersonAttachment>()
            .ToTable("PersonAttachments")
            .HasKey(ca => ca.Id);

        modelBuilder.Entity<PersonAttachment>()
            .HasOne(ca => ca.Person)
            .WithMany(ca => ca.Attachments)
            .HasForeignKey(ca => ca.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PersonAttachment>()
           .HasOne(ca => ca.Attachment)
           .WithMany(ca => ca.Persons)
           .HasForeignKey(ca => ca.AttachmentId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderContact>()
         .ToTable("OrderContacts")
         .HasKey(pq => pq.Id);
      

        modelBuilder.Entity<OrderContact>()
            .HasOne(pq => pq.Order)
            .WithMany(pq => pq.Contacts)
            .HasForeignKey(pq => pq.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderContact>()
           .HasOne(pq => pq.Contact)
           .WithMany(pq => pq.Orders)
           .HasForeignKey(pq=>pq.ContactId)
           .OnDelete(DeleteBehavior.Restrict);



        modelBuilder.Entity<OrdersettingException>()
      .ToTable("OrdersettingExceptions")
      .HasKey(pq => pq.Id);


        modelBuilder.Entity<OrdersettingException>()
            .HasOne(pq => pq.Order)
            .WithMany(pq => pq.ExceptionContacts)
            .HasForeignKey(pq => pq.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrdersettingException>()
           .HasOne(pq => pq.ProjectContact)
           .WithMany(pq => pq.OrderExceptions)
           .HasForeignKey(pq => pq.ProjectContactId)
           .OnDelete(DeleteBehavior.Restrict);



        
   


        var converterProjectId = new ValueConverter<ProjectId, Guid>(
        id => id.Value,
        guidValue => new ProjectId(guidValue));

        modelBuilder.Entity<ProjectUser>()
          .ToTable("ProjectUsers")
              .Property(e => e.ProjectId)
             .HasConversion(converterProjectId)
             .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProjectUser>().HasKey(pu=>pu.Id);

        modelBuilder.Entity<ProjectContact>()
          .ToTable("ProjectContacts")
          .HasKey(pc => pc.Id);
       

        modelBuilder.Entity<ProjectContact>()
            .HasOne(pq => pq.Project)
            .WithMany(pq => pq.Contacts)
            .HasForeignKey(pq => pq.ProjectId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectContact>()
           .HasOne(pq => pq.Contact)
           .WithMany(pq => pq.Projects)
           .HasForeignKey(pq => pq.ContactId)
           .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<OrderInspectionSubject>()
        .ToTable("OrderInspectionSubjects")
        .HasKey(pq => pq.Id); 

        modelBuilder.Entity<OrderInspectionSubject>()
            .HasOne(pq => pq.Order)
            .WithMany(pq => pq.Subjects)
            .HasForeignKey(pq => pq.OrderId);

        modelBuilder.Entity<OrderInspectionSubject>()
           .HasOne(pq => pq.InspectionSubject)
           .WithMany(pq => pq.Orders)
           .HasForeignKey(pq => pq.InspectionSubjectId);

        modelBuilder.Entity<InspectionNotificationRevision>()
        .ToTable("InspectionNotificationRevisions")
        .HasKey(insr => insr.Id);

        modelBuilder.Entity<InspectionNotificationSubject>()
        .ToTable("InspectionNotificationSubjects")
        .HasKey(pq => pq.Id);

        modelBuilder.Entity<InspectionNotificationLocation>()
       .ToTable("InspectionNotificationLocations")
       .HasKey(inl => inl.Id);


        modelBuilder.Entity<InspectionNotificationType>()
        .ToTable("InspectionNotificationTypes")
        .HasKey(inst => inst.Id);


        modelBuilder.Entity<InspectionReportPerson>()
            .HasOne(ins => ins.InspectionReport)
            .WithMany(ins => ins.InspectionPersons)
            .HasForeignKey(ins => ins.InspectionReportId);

        modelBuilder.Entity<InspectionNotificationRevision>()
            .HasOne(ins => ins.InspectionNotification)
            .WithMany(ins => ins.Revisions)
            .HasForeignKey(ins => ins.InspectionNotificationId);

        modelBuilder.Entity<InspectionNotificationRevision>()
            .HasOne(ins => ins.Revision)
            .WithMany(ins => ins.InspectionNotifications)
            .HasForeignKey(ins => ins.RevisionId);

        modelBuilder.Entity<InspectionNotificationSubject>()
            .HasOne(ins => ins.InspectionNotification)
            .WithMany(ins => ins.Subjects)
            .HasForeignKey(ins => ins.InspectionNotificationId);

        modelBuilder.Entity<InspectionNotificationSubject>()
            .HasOne(ins=>ins.InspectionSubject)
            .WithMany(ins=>ins.InspectionNotifications)
            .HasForeignKey(ins=>ins.InspectionSubjectId);

        modelBuilder.Entity<InspectionNotificationLocation>()
           .HasOne(ins => ins.InspectionNotification)
           .WithMany(ins => ins.Locations)
           .HasForeignKey(ins => ins.InspectionNotificationId);

        modelBuilder.Entity<InspectionNotificationLocation>()
            .HasOne(ins => ins.Location)
            .WithMany(ins => ins.InspectionNotifications)
            .HasForeignKey(ins => ins.AddressId);

        modelBuilder.Entity<InspectionNotificationType>()
            .HasOne(it => it.InspectionNotification)
            .WithMany(it => it.Types)
            .HasForeignKey(it => it.InspectionNotificationId);

        modelBuilder.Entity<InspectionNotificationType>()
            .HasOne(it=>it.InspectionType)
            .WithMany(it=>it.InspectionNotifications)
            .HasForeignKey(it=>it.InspectionTypeId);



        modelBuilder.Entity<CirculationInspector>()
        .ToTable("CirculationInspectors")
        .HasKey(cin => cin.Id);


        modelBuilder.Entity<Person>()
      .ToTable("Persons")
      .HasKey(cin => cin.Id);

        modelBuilder.Entity<CirculationInspector>()
            .HasOne(cin => cin.Circulation)
            .WithMany(cin => cin.Inspectors)
            .HasForeignKey(ins => ins.CirculationId);

        modelBuilder.Entity<CirculationInspector>()
            .HasOne(cin => cin.Inspector)
            .WithMany(cin => cin.Circulations)
            .HasForeignKey(ins => ins.InspectorId);
        //modelBuilder.Entity<InspectionNotificationContact>()
        //.ToTable("InspectionNotificationContacts")
        //.HasKey(pq => pq.Id);

        //modelBuilder.Entity<InspectionNotificationContact>()
        //    .HasOne(ic => ic.InspectionNotification)
        //    .WithMany(ic => ic.Contacts)
        //    .HasForeignKey(ic => ic.InspectionNotificationId);

        //modelBuilder.Entity<InspectionNotificationContact>()
        //    .HasOne(ic => ic.Contact)
        //    .WithMany(ic => ic.InspectionNotifications)
        //    .HasForeignKey(ic => ic.ContactId);



        base.OnModelCreating(modelBuilder);

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        foreach (var item in ChangeTracker.Entries<BaseDomainEntity<Guid>>())
        {
            item.Entity.ModifiedDate = DateTime.Now;
            if(item.State == EntityState.Added) {
                item.Entity.CreatedDate = DateTime.Now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<InspectionNotification> InspectionNotifications { get; set; } = null!;
    public DbSet<InspectionType> InspectionTypes { get; set; }
    public DbSet<InspectionSubject> InspectionSubjects { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Contact> Contacts { get; set; }    
    public DbSet<Project> Projects { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<VendorDocument> vendorDocuments { get; set; }
    public DbSet<NotificationNumber> NotificationNumbers { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Circulation> Circulations { get; set; }
    public DbSet<ActionType> ActionTypes { get; set; }
    public DbSet<OrderContact> OrderContacts { get; set; }
    public DbSet<OrderInspectionSubject> OrderInspectionSubjects { get; set; }
    public DbSet<ProjectContact> ProjectContacts { get; set; }
    public DbSet<InspectionNotificationContact> InspectionNotificationsContacts { get; set;}
    public DbSet<InspectionNotificationSubject> InspectionNotificationSubjects { get; set; }
    public DbSet<InspectionNotificationRevision> InspectionNotificationRevisions { get; set; }
    public DbSet<InspectionNotificationType> InspectionNotificationsTypes { get; set;}
    public DbSet<InspectionNotificationLocation> InspectionNotificationsLocations { get; set; }
    public DbSet<CirculationAttachments> CirculationAttachments { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<OrdersettingException> OrdersettingExceptions { get; set; }
    public DbSet<InspectionReport>  InspectionReports{ get; set; }
    public DbSet<InspectionReportAttachment>  InspectionReportAttachments{ get; set; }
    public DbSet<Inspector> Inspectors { get; set; }  
    public DbSet<CirculationInspector> CirculationInspectors { get; set; }
    public DbSet<Revision> Revisions { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<InspectionReportPerson> InspectionReportPersons { get; set; }
    public DbSet<InspectorOrder> InspectorOrders { get; set; }
    public DbSet<InspectorProject> InspectorProjects { get; set; }
}
