using Inspection.Domain.Entities;
using Inspection.Domain.Entities.Common;
using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Inspection.Persistence;

public class InspectionDbContext:DbContext
{
    public InspectionDbContext(DbContextOptions<InspectionDbContext> options):base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {       
         modelBuilder.Ignore<InspectionNotificationId>();
         modelBuilder.Ignore<AttachmentId>();
        //modelBuilder.Ignore("Attachments");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InspectionDbContext).Assembly);

        //modelBuilder.Entity<InspectionNotification>(entity => {
        //    entity.HasKey(i=>i.InspectionNotificationId);
        //    entity.Property(i => i.NotificationNumber).IsRequired();
        //    entity.Property(i=>i.Owner).IsRequired();
        //    entity.Property(i=>i.Order).IsRequired();   
        //    });

        //modelBuilder.Entity<InspectionNotification>(entity => {  entity.HasKey(i=>i.Id); });

        //modelBuilder.Entity<InspectionNotification>().HasMany(inf => inf.Attachments);//.WithMany().UsingEntity(a=>a.);
        //  modelBuilder.Entity<InspectionNotification>().OwnsOne(inf => inf.InspectionType);

       
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
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Contact> Contacts { get; set; }    
    public DbSet<Project> Projects { get; set; }
    public DbSet<Order> Orders { get; set; }

}
