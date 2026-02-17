using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Db
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<BlogWebAppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            string[] roles = { "SuperUser", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var superUserEmail = "Super@local.com";
            var superUserPassword = "Super&2000";

            var superUser = await userManager.FindByNameAsync(superUserEmail);

            if(superUser == null)
            {
                superUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = superUserEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(superUser, superUserPassword);
                await userManager.AddToRoleAsync(superUser, "SuperUser");
            }
        }
    }
}
