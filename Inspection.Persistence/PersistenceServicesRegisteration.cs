

using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inspection.Persistence;

public static class PersistenceServicesRegisteration
{
    public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<InspectionDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("InspectionConnectionString")));

        //services.AddScoped<IGenericRepository,IGenericRepository>();
        services.AddScoped(typeof(IGenericRepository<,>),typeof(GenericRepository<,>));
        services.AddScoped <IInspectionNotificationRepository,InspectionNotificationRepository>();

        return services;

    }
}
