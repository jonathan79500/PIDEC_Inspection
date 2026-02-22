using Inspection.Domain.Entities.InspectionNotification.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.InspectionReport;
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


    public class InspectionReportConfiguration : IEntityTypeConfiguration<InspectionReport>
    {
        public void Configure(EntityTypeBuilder<InspectionReport> builder)
        {

          
            ConfigureInspectionReports(builder);

        }

      
        private void ConfigureInspectionReports(EntityTypeBuilder<InspectionReport> builder)
        {
            builder.ToTable("InspectionReports");
         

            var converter = new ValueConverter<InspectionReportId, Guid>(
            id => id.Value,
            guidValue => new InspectionReportId(guidValue));
            builder.HasKey(e => e.InspectionReportId);

            builder.Property(e => e.InspectionReportId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();

            builder.HasOne(x => x.Creator)
       .WithMany()
       .HasForeignKey(x => x.CreatorContactId);
            
        }
    }

}
