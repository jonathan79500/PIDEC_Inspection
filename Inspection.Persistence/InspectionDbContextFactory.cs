using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Inspection.Persistence;

public class InspectionDbContextFactory:IDesignTimeDbContextFactory<InspectionDbContext>
{
    public InspectionDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration =new ConfigurationBuilder()
                              .SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json").Build();
            

        var builder = new DbContextOptionsBuilder<InspectionDbContext>();
        var connectionString = configuration.GetConnectionString("InspectionConnectionString");
        builder.UseSqlServer(connectionString);

        return new InspectionDbContext(builder.Options);
    }
    
}
