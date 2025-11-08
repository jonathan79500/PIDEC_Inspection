using Inspection.Domain.Entities.Common.ValueObjects;
using Inspection.Domain.Entities.InspectionNotification.Entities;
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
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(x => x.Id);


            builder.Property(e => e.Id)
                .HasColumnName("Id")
                 .ValueGeneratedOnAdd();
                

            builder.HasKey(e => e.Id);
            builder.Property(e=>e.Country).IsRequired();
            builder.Property(e=>e.City).IsRequired();
            builder.Property(e=>e.ZipCode).IsRequired(false);
            builder.Property(e => e.State).IsRequired(false);
            builder.Property(e => e.StreetLine1).IsRequired(false);
            builder.Property(e => e.StreetLine2).IsRequired(false);           
            builder.Property(e => e.CreatedBy).HasMaxLength(100);
            builder.Property(e => e.ModifiedBy).HasMaxLength(100);

        }
    }
}
