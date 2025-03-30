using Microsoft.AspNetCore.Identity;

namespace PromptArchive.Database;

public static class IdentitySeeder
{
    public static async Task SeedBaseRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }

    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var initialUserConfig = configuration.GetSection("InitialUser");
        var email = initialUserConfig.GetValue<string>("Email")!;
        var username = initialUserConfig.GetValue<string>("Username")!;
        var password = initialUserConfig.GetValue<string>("Password")!;

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            var adminUser = new ApplicationUser()
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
            else
                throw new InvalidOperationException("Failed to create the admin user: " +
                                                    string.Join(", ", result.Errors.Select(x => x.Description)));
        }
    }
}