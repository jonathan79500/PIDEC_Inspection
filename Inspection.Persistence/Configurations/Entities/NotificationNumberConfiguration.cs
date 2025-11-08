using Inspection.Domain.Entities.InspectionNotification.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Persistence.Configurations.Entities
{
    public class NotificationNumberConfiguration : IEntityTypeConfiguration<NotificationNumber>
    {
        public void Configure(EntityTypeBuilder<NotificationNumber> builder)
        {

            ConfigureNotificationNumberTable(builder);
          

        }

        private void ConfigureNotificationNumberTable(EntityTypeBuilder<NotificationNumber> builder)
        {
            builder.ToTable("NotificationNumbers");
            builder.HasKey(x => x.Id);       

            builder.Property(e => e.Revision).HasDefaultValue(0);
            builder.Property(e => e.Version).HasDefaultValue(0);
            builder.Property(e => e.InfoPart).HasMaxLength(3);
            builder.Property(e => e.OrderNumber);
            builder.Property(e => e.ProjectNumber);            
        }
    }
}
