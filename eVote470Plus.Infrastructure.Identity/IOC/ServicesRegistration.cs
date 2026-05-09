using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eVote470Plus.Infrastructure.Identity.Contexts;
using eVote470Plus.Infrastructure.Identity.Entities;
using eVote470Plus.Infrastructure.Identity.Seeds;
using eVote470Plus.Core.Application.Interfaces.ApplicationUser;
using eVote470Plus.Infrastructure.Identity.Services;

namespace eVote470Plus.Infrastructure.Identity.IOC
{
    public static class ServicesRegistration
    {
        public static void AddIdentityInfrastructureLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            ConfigureDatabase(services, config);

            services.Configure<IdentityOptions>(opt =>
            {
                // Password
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;

                // Lockout

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 5;

                // Email

                opt.User.RequireUniqueEmail = true;

            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<IdentityContext>()
               .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12);
            });

            services.ConfigureApplicationCookie(opt =>
            {
                opt.LoginPath = "/Login/Index";
                opt.AccessDeniedPath = "/Login/AccessDenied";
                opt.ExpireTimeSpan = TimeSpan.FromHours(3);
                opt.SlidingExpiration = true;
            });

            services.AddScoped<IAccountServicesForWebApp, AccountServicesForWebApp>();

        }

        public static async Task RunIdentitySeedAsync(this IServiceProvider service)
        {
            using var scoped = service.CreateScope();
            var servicesProvider = scoped.ServiceProvider;

            var userManager = servicesProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultRoles.SeedAsync(roleManager);
            await DefaultAdminUser.SeedAsync(userManager);
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration config)
        {
            if (config.GetValue<bool>("Use in memory database."))
            {
                services.AddDbContext<IdentityContext>(options =>
                {
                    options.UseInMemoryDatabase("eVote470Db");
                });
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");

                services.AddDbContext<IdentityContext>(
                    (serviceProvider, options) =>
                    {
                        options.EnableSensitiveDataLogging();
                        options.UseMySql(
                            connectionString,
                            ServerVersion.AutoDetect(connectionString),
                            mysqloptions => mysqloptions.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)
                        );
                    },

                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                );
                
            }
        }
    }
}
