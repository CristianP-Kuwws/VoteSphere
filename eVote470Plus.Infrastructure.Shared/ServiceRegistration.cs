using eVote470Plus.Core.Application.Interfaces.Email;
using eVote470Plus.Core.Domain.Settings;
using eVote470Plus.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eVote470Plus.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        //Extension method - decorator pattern
        public static void AddSharedLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Configurations
            services.Configure<MailSettings>(config.GetSection("MailSettings"));

            #endregion

            #region Services IOC

            services.AddScoped<IEmailService, EmailService>();

            #endregion

        }
    }
}
