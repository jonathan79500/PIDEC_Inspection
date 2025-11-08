using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Inspection.Domain.Entities.Common;

namespace Inspection.Persistence.Configurations.Entities
{
    public class InspectionSubjectConfiguration : IEntityTypeConfiguration<InspectionSubject>
    {
        public void Configure(EntityTypeBuilder<InspectionSubject> builder)
        {
            builder.ToTable("InspectionSubjects");
            var converter = new ValueConverter<InspectionSubjectId, Guid>(
          id => id.Value,
          guidValue => new InspectionSubjectId(guidValue));
            builder.HasKey(e => e.InspectionSubjectId);
            builder.Property(e => e.InspectionSubjectId)
                .HasConversion(converter)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired();
        }
    }
}
