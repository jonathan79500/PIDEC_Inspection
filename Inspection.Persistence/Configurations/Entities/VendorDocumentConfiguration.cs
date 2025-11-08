
using Inspection.Domain.Entities.ValueObjects;
using Inspection.Domain.Entities.Vendor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


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
            builder.Property(e => e.edmsId).HasMaxLength(12);
            builder.Property(e => e.PDFSize).HasMaxLength(20);
            builder.Property(e => e.Content).HasColumnType("image");
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);

            //we do this OwnsOne for value objects which should flatten to InspectionNotification Table and has not any unique ids
            //  builder.HasOne(e => e.Order).WithMany().HasForeignKey("OrderId");


        }
    }
}
