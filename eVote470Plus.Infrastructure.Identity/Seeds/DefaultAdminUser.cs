using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace eVote470Plus.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)        
        {
            // Check if the admin user already exists
            //await DefaultRoles.SeedAsync(roleManager);

            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@evote.com",
                Name = "John",
                LastName = "Doe",
                IsActive = true,
                EmailConfirmed = true
            };

            var user = await userManager.FindByNameAsync(defaultUser.UserName);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin123!");
                await userManager.AddToRoleAsync(defaultUser, Roles.Administrator.ToString());
            }
        }
    }
}
