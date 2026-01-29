using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PetCare_system.Models;
using System;
using System.Threading.Tasks;

namespace PetCare_system
{
    public static class DbInitializer
    {
        public static async Task SeedAsync()
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                // 1. Ensure the Admin role exists
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // 2. Check if the admin user exists
                string adminEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        CellphoneNumber = "0123456789",
                        IdNumber = "1234567890123",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newAdmin, "AdminPassword123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin.Id, "Admin");
                    }
                    else
                    {
                        throw new Exception("Failed to create admin: " + string.Join(", ", result.Errors));
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
