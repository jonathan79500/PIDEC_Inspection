using System;
using System.Threading;
using System.Threading.Tasks;
using Inspection.Application.Contracts.Persistence;
using Inspection.Domain.Entities.User.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inspection.Persistence.Repositories
{



    public class UserCreationBackGroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public UserCreationBackGroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait before starting a new cycle.
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    // Resolve your scoped services here.
                    var userManagerRepository = scope.ServiceProvider.GetRequiredService<IUserManagerRepository>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SystematicUser>>();

                    // Simulate getting a vendor contact – in your actual code, retrieve this from the database or another service.
                    var vendorContact = new VendorContact
                    {
                        Name = "VendorUser",
                        Email = "vendor@example.com"
                    };

                    if (vendorContact != null)
                    {
                        var vendorUser = new SystematicUser
                        {
                            UserName = vendorContact.Name,
                            Email = vendorContact.Email
                        };

                        // Create the user using UserManager (or via your IUserManagerRepository wrapper).
                        var createResult = await userManager.CreateAsync(vendorUser);
                        if (createResult.Succeeded)
                        {
                            // Assign the new user to the "vendor" role.
                            await userManager.AddToRoleAsync(vendorUser, "vendor");
                        }
                        else
                        {
                            // Log errors from createResult.Errors as needed.
                            foreach (var error in createResult.Errors)
                            {
                                Console.WriteLine($"Error: {error.Description}");
                            }
                        }
                    }
                }
            }
        }
    }

    public class VendorContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}