using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Inserts data needed in the database
/// </summary>
public class SeedData
{
    private const string AdminRole = "Admin";
    private const string ChatEmployeeRole = "ChatEmployee";

    /// <summary>
    /// Initialize seeding of the database
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    /// <returns></returns>
    public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!context.Roles.Any())
        {
            await CreateRolesAsync(context, roleManager);
        }

        if(!context.Users.Any())
        {
            await CreateAdminAsync(context, userManager);
        }
    }

    /// <summary>
    /// Creates roles for the application. 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="roleManager"></param>
    /// <returns></returns>
    private static async Task CreateRolesAsync(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        await roleManager.CreateAsync(new IdentityRole(AdminRole));
        await roleManager.CreateAsync(new IdentityRole(ChatEmployeeRole));
        await context.SaveChangesAsync();
    }


    /// <summary>
    /// Creates an admin for the system
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    /// <returns></returns>
    private static async Task CreateAdminAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@nk.no",
            Email = "admin@nk.no",
            IsActive = true
        };
        string password = userManager.PasswordHasher.HashPassword(admin, "Password1");
        admin.PasswordHash = password;
        
        await userManager.CreateAsync(admin);
        await context.SaveChangesAsync();

        //await userManager.AddToRoleAsync(admin, AdminRole);
        //await context.SaveChangesAsync();
    }



}
