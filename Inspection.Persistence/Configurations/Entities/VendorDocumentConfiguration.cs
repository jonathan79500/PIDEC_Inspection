using Inspection.Domain.Entities;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inspection.Persistence.Configurations.Entities
{
    public class VendorDocumentConfiguration : IEntityTypeConfiguration<VendorDocument>
    {
        public void Configure(EntityTypeBuilder<VendorDocument> builder)
        {
            builder.ToTable("VendorDocuments");
            builder.HasKey(x => x.Id);

         

            var converter = new ValueConverter<VendorDocumentId, Guid>(
            id => id.Value,
        guidValue => VendorDocumentId.Create(guidValue));



            builder.Property(e => e.Id)
                .HasColumnName("Id")
                 .ValueGeneratedNever()
                .HasConversion(converter);

            builder.Property(e => e.Name).HasMaxLength(200);
            builder.Property(e => e.Content).HasColumnType("image");

            //we do this OwnsOne for value objects which should flatten to InspectionNotification Table and has not any unique ids
          //  builder.HasOne(e => e.Order).WithMany().HasForeignKey("OrderId");


        }
    }
}
