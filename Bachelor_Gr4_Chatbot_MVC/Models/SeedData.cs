using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
    public static async Task InitializeAsync(ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<RoleOptions> roleOptions)
    {
        if (!context.Roles.Any())
        {
            await CreateRolesAsync(context, roleManager, roleOptions.Value);
        }

        if(!context.Users.Any())
        {
            await CreateAdminAsync(context, userManager);
            await CreateUserAsync(context, userManager);
        }

        if(!context.OpeningHours.Any())
        {
            await CreateOpeningHours(context);
        }

        if (!context.ChatbotDetails.Any())
        {
            await AddChatbot(context);
        }
    }

    /// <summary>
    /// Creates roles for the application. 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="roleManager"></param>
    /// <returns></returns>
    private static async Task CreateRolesAsync(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, RoleOptions roleOptions)
    {
        await roleManager.CreateAsync(new IdentityRole(roleOptions.AdminRole));
        await roleManager.CreateAsync(new IdentityRole(roleOptions.ChatEmployeeRole));
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
            FirstName = "Admin",
            LastName = "1",
            IsActive = true,
            EmailConfirmed = true
        };
        string password = userManager.PasswordHasher.HashPassword(admin, "Password1");
        admin.PasswordHash = password;
        
        await userManager.CreateAsync(admin);
        await context.SaveChangesAsync();

        await userManager.AddToRoleAsync(admin, AdminRole);
        await context.SaveChangesAsync();
    }


    /// <summary>
    /// Creates two users for the system
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    /// <returns></returns>
    private static async Task CreateUserAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = "user1@nk.no",
            Email = "user1@nk.no",
            IsActive = true,
            EmailConfirmed = true
        };
        string password = userManager.PasswordHasher.HashPassword(user, "Password1");
        user.PasswordHash = password;

        await userManager.CreateAsync(user);
        await context.SaveChangesAsync();

        await userManager.AddToRoleAsync(user, ChatEmployeeRole);
        await context.SaveChangesAsync();

        var user2 = new ApplicationUser
        {
            UserName = "user2@nk.no",
            Email = "user2@nk.no",
            IsActive = true,
            EmailConfirmed = true
        };
        string password2 = userManager.PasswordHasher.HashPassword(user2, "Password2");
        user2.PasswordHash = password2;

        await userManager.CreateAsync(user2);
        await context.SaveChangesAsync();

        await userManager.AddToRoleAsync(user2, ChatEmployeeRole);
        await context.SaveChangesAsync();
    }

    private static async Task CreateOpeningHours(ApplicationDbContext context)
    {
        List<OpeningHours> hours = new List<OpeningHours>();
        for(int i = 1; i <= 5; i++)
        {

            OpeningHours openingHours = new OpeningHours
            {
                DayOfWeek = i,
                OpenFrom = DateTime.Today.Add(new TimeSpan(8, 30, 0)),
                OpenTo = new DateTime(2050, 1, 1, 16, 0, 0),
                StandardOpeningHours = true
            };
            hours.Add(openingHours);
        }

        await context.AddRangeAsync(hours);
        await context.SaveChangesAsync();
    }

    private static async Task AddChatbot(ApplicationDbContext context)
    {
        var chatbot1 = new ChatbotDetails
        {
            regDate = DateTime.Now,
            lastEdit = DateTime.Now,
            chatbotName = "Svarta Bjørn 1",
            isActive = false,
            contentType = "application/json",
            baseUrl = "https://directline.botframework.com",
            tokenUrlExtension = "/v3/directline/tokens/generate",
            conversationUrlExtension = "/v3/directline/conversations/",
            botAutorizeTokenScheme = "Bearer",
            BotSecret = "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0"

        };
        await context.AddAsync(chatbot1);
        await context.SaveChangesAsync();

        var chatbot2 = new ChatbotDetails
        {
            regDate = DateTime.Now,
            lastEdit = DateTime.Now,
            chatbotName = "Svarta Bjørn 2",
            isActive = true,
            contentType = "application/json",
            baseUrl = "https://directline.botframework.com",
            tokenUrlExtension = "/v3/directline/tokens/generate",
            conversationUrlExtension = "/v3/directline/conversations/",
            botAutorizeTokenScheme = "Bearer",
            BotSecret = "U3uIHr5dEVY.cwA.PbQ.IUlQuQsWrZHAg2wX4qM3wDa6i7W8buvXaGiSgFIAkRg"

        };
        await context.AddAsync(chatbot2);
        await context.SaveChangesAsync();
    }



}
