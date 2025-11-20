

using Inspection.Application.Common.Interfaces.Persistence;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.InspectionNotification.ValueObjects;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
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
        services.AddIdentityApiEndpoints<SystematicUser>()      
            .AddRoles<SystematicRole>()
            .AddRoleManager<RoleManager<SystematicRole>>()
            .AddEntityFrameworkStores<InspectionDbContext>()
            .AddDefaultTokenProviders();

       
        services.AddScoped<IUserManagerRepository, UserManagerRepository>();
        services.AddIdentityCore<SystematicUser>();

        services.AddScoped<IUserManagerRepository, UserManagerRepository>();
        services.AddScoped(typeof(IGenericRepository<,>),typeof(GenericRepository<,>));
        services.AddScoped <IInspectionNotificationRepository,InspectionNotificationRepository>();
        services.AddScoped<IInspectionReportRepository, InspectionReportRepository>();
        services.AddScoped<IBasicInformationRepository, BasicInformationRepository>();
        services.AddScoped<IUnitOfWork,EFUnitOfWork>();

        return services;

    }
}
