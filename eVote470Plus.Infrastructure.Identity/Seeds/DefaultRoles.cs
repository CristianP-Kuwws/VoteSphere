using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
           if (!await roleManager.Roles.AnyAsync(r => r.Name == Roles.Administrator.ToString()))
           {
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));
           }

           if (!await roleManager.Roles.AnyAsync(r => r.Name == Roles.PoliticalLeader.ToString()))
           {
                await roleManager.CreateAsync(new IdentityRole(Roles.PoliticalLeader.ToString()));
           }
        }
    }
}
