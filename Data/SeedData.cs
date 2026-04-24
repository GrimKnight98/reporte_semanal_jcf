using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using DemoMvc.Models; // ApplicationUser

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string roleName = "SysAdmin";
        string adminEmail = "admin@demo.com";
        string adminPassword = "Admin123!";

        // --- Crear rol SysAdmin si no existe ---
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
                Console.WriteLine($"✅ Rol '{roleName}' creado.");
            else
                Console.WriteLine($"⚠️ Error creando rol: {string.Join(", ", roleResult.Errors)}");
        }
        else
        {
            Console.WriteLine($"ℹ️ Rol '{roleName}' ya existía.");
        }

        // --- Crear usuario admin si no existe ---
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PersonNumber = "0001", // 👈 obligatorio
                FirstName = "Admin",
                LastName = "System",
                FullName = "System Admin",
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                Console.WriteLine($"✅ Usuario admin '{adminEmail}' creado con PersonNumber={adminUser.PersonNumber}.");
                await userManager.AddToRoleAsync(adminUser, roleName);
                Console.WriteLine($"✅ Usuario admin asignado al rol '{roleName}'.");
            }
            else
            {
                foreach (var err in createResult.Errors)
                {
                    Console.WriteLine($"Error creando usuario admin: {err.Code} - {err.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($" Usuario admin '{adminEmail}' ya existía.");
            if (!await userManager.IsInRoleAsync(adminUser, roleName))
            {
                await userManager.AddToRoleAsync(adminUser, roleName);
                Console.WriteLine($"✅ Usuario admin asignado al rol '{roleName}'.");
            }
        }
    }
}
