using Inspection.Application.Behaviors;
using Inspection.Application.Profiles;
using Inspection.Application.User;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Inspection.Application
{
    public static class ApplicationServicesRegisteration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            //services.addMaps
            //services.AddAutoMapper(typeof(MappingProfile));

            var config = new TypeAdapterConfig();
            new MappingProfile().Register(config);
            //config.Apply(new MappingProfile());
            config.Compile();

            services.AddSingleton(config);
            //services.AddScoped<IMapper, ServiceMapper>();

            //MappingProfile profile = new MappingProfile();

            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddScoped<IUserContext, UserContext>();
            services.AddTransient(typeof(IPipelineBehavior<,>),typeof(TransactionBehavior<,>));
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
