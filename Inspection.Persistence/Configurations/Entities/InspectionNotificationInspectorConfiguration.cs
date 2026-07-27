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
    public class InspectionNotificationInspectorConfiguration : IEntityTypeConfiguration<InspectionNotificationInspector>
    {
        public void Configure(
            EntityTypeBuilder<InspectionNotificationInspector> builder)
        {
            builder.HasKey(x => x.InspectionInspectorId);

            builder.Property(x => x.InspectionNotificationId)
                   .HasConversion(
                       x => x.Value,
                       x => new Domain.Entities.InspectionNotification.ValueObjects.InspectionNotificationId(x));

            builder.HasOne(x => x.InspectionNotification)
                   .WithMany(x => x.Inspectors)
                   .HasForeignKey(x => x.InspectionNotificationId);

            builder.HasOne(x => x.Inspector)
                   .WithMany(x => x.InspectionNotifications)
                   .HasForeignKey(x => x.InspectorId);
        }
    }
}
