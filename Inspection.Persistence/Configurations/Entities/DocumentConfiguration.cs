using Inspection.Domain.Entities.Common;
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
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(x => x.DocumentId);

            builder.HasKey(x => x.DocumentId);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Number).HasMaxLength(100);
            builder.Property(x => x.Status).HasMaxLength(30);
            builder.Property(x => x.Version).HasMaxLength(20);

            builder.HasOne<DocumentType>()
             .WithMany()
             .HasForeignKey(x => x.DocumentTypeId)
             .OnDelete(DeleteBehavior.Restrict);

        }
    }
    public class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("DocumentTypes");

            builder.HasKey(x => x.DocumentTypeId);
            builder.Property(x => x.DocumentTypeName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.AllowedExtensions).HasMaxLength(200);
            builder.HasIndex(x => x.DocumentTypeCode).IsUnique();

        }
    }

    public class DocumentTypeVisibilityConfiguration : IEntityTypeConfiguration<DocumentTypeVisibility>
    {
        public void Configure(EntityTypeBuilder<DocumentTypeVisibility> builder)
        {
            builder.ToTable("DocumentTypeVisibilities");

            builder.HasKey(x => new { x.DocumentTypeId, x.ContactTypeCode });
            builder.HasOne<DocumentType>()
             .WithMany()
             .HasForeignKey(x => x.DocumentTypeId)
             .OnDelete(DeleteBehavior.Cascade);

        }
    }

    public class DocumentLinkConfiguration : IEntityTypeConfiguration<DocumentLink>
    {
        public void Configure(EntityTypeBuilder<DocumentLink> builder)
        {
            builder.ToTable("DocumentLinks");

            builder.HasKey(x => new { x.DocumentId, x.OwnerType, x.OwnerId });
            builder.HasOne<Document>()
             .WithMany()
             .HasForeignKey(x => x.DocumentId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.OwnerType, x.OwnerId });

        }
    }


}
