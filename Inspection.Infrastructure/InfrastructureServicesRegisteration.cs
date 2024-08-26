using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Models;
using Inspection.Infrastructure.EDMS;
using Inspection.Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspection.Infrastructure
{
    public static class InfrastructureServicesRegisteration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,IConfiguration configuration) {

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<EDMSServiceSettings>(configuration.GetSection("EMSServiceSettings"));

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IEDMSService, EMSService>();
            return services;
        }
    }
}
