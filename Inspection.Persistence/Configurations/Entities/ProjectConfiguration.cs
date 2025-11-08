using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Inspection.Persistence.Configurations.Entities;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {

        ConfigureProjectTable(builder);
       
        
    }

   

    private void ConfigureProjectTable(EntityTypeBuilder<Project> builder)
    {

        var converter = new ValueConverter<ProjectId, Guid>(
  id => id.Value,
  guidValue => new ProjectId(guidValue));
        builder.HasKey(e => e.ProjectId);
        builder.Property(e => e.ProjectId)
            .HasConversion(converter)
            .ValueGeneratedOnAdd();


        //builder.ToTable("Projects");   
        //builder.HasKey(x => x.Id);

        //builder.Property(e => e.Id)
        //    .HasColumnName("Id")
        //     .ValueGeneratedOnAdd();
            

        builder.Property(e => e.Title).HasMaxLength(400);
        builder.Property(e => e.Number).HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(500);
    
       //FsId means ForeignSystem Id
        builder.Property(e => e.FSId).HasMaxLength(100);
        //builder.Property(e => e.CreatedBy).HasMaxLength(100);
        //builder.Property(e => e.ModifiedBy).HasMaxLength(100);

        builder.HasMany(p => p.Contacts)
            .WithOne()
            .HasForeignKey(p => p.ContactId);
 




    }
}
