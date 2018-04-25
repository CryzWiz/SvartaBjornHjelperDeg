using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Migrations;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
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
    /// <summary>
    /// Initialize seeding of the database
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    /// <param name="roleManager"></param>
    /// <param name="roleOptions"></param>
    /// <returns></returns>
    public static async Task InitializeAsync(ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<RoleOptions> roleOptions)
    {

        if (!context.ChatGroups.Any())
        {
            await CreateChatGroups(context);
        }

        if (!context.Roles.Any())
        {
            await CreateRolesAsync(context, roleManager, roleOptions.Value);
        }

        if(!context.Users.Any())
        {
            await CreateAdminAsync(context, userManager, roleOptions.Value);
            await CreateUserAsync(context, userManager, roleOptions.Value);
        }

        if(!context.OpeningHours.Any())
        {
            await CreateOpeningHours(context);
        }

        if (!context.ChatbotTypes.Any())
        {
            await AddChatbotTypes(context);
        }

        if (!context.ChatbotDetails.Any())
        {
            await AddChatbot(context);
        }
        
        if (!context.QnABaseClass.Any())
        {
            await AddQnA(context);
        }

        if (!context.QnAKnowledgeBase.Any())
        {
            await AddKnowledgeBase(context);
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
    private static async Task CreateAdminAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleOptions roleOptions)
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

        await userManager.AddToRoleAsync(admin, roleOptions.AdminRole);
        await context.SaveChangesAsync();

        // Add admin to all chatgroups
        List<ChatGroup> groups = context.ChatGroups.ToList();
        List<UserChatGroup> adminChatGrups = new List<UserChatGroup>();
        foreach(ChatGroup group in groups)
        {
            UserChatGroup gr = new UserChatGroup
            {
                UserId = admin.Email,
                ChatGroupId = group.ChatGroupId
            };
            adminChatGrups.Add(gr);
        }

        await context.AddRangeAsync(adminChatGrups);
        await context.SaveChangesAsync();
    }


    /// <summary>
    /// Creates two users for the system
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    /// <returns></returns>
    private static async Task CreateUserAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleOptions roleOptions)
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

        await userManager.AddToRoleAsync(user, roleOptions.ChatEmployeeRole);
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

        await userManager.AddToRoleAsync(user2, roleOptions.ChatEmployeeRole);
        await context.SaveChangesAsync();
    }

    private static async Task CreateChatGroups(ApplicationDbContext context)
    {
        List<ChatGroup> chatGroups = new List<ChatGroup>();

        var adminGroup = new ChatGroup
        {
            ChatGroupName = "Admin"
        };
        chatGroups.Add(adminGroup);

        var chatWorkerGroup = new ChatGroup
        {
            ChatGroupName = "Chat medarbeider"
        };
        chatGroups.Add(chatWorkerGroup);

        var it = new ChatGroup
        {
            ChatGroupName = "IT"
        };
        chatGroups.Add(it);

        await context.AddRangeAsync(chatGroups);
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
    private static async Task AddChatbotTypes(ApplicationDbContext context)
    {
        var chatbotType = new ChatbotTypes
        {
            Type = "Microsoft Bot Framework",
            TypeId = 1
        };

        await context.AddAsync(chatbotType);
        await context.SaveChangesAsync();

        var chatbotType2 = new ChatbotTypes
        {
            Type = "QnA Maker",
            TypeId = 2
        };

        await context.AddAsync(chatbotType2);
        await context.SaveChangesAsync();
    }
    private static async Task AddChatbot(ApplicationDbContext context)
    {
        var chatbot1 = new ChatbotDetails
        {
            regDate = DateTime.Now,
            lastEdit = DateTime.Now,
            chatbotName = "Svarta Bjørn 1",
            TypeId = 2,
            isActive = false,
            contentType = "application/json",
            baseUrl = "https://directline.botframework.com",
            conversationUrlExtensionEnding = "/activities",
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
            TypeId = 2,
            isActive = true,
            contentType = "application/json",
            baseUrl = "https://directline.botframework.com",
            conversationUrlExtensionEnding = "/activities",
            tokenUrlExtension = "/v3/directline/tokens/generate",
            conversationUrlExtension = "/v3/directline/conversations/",
            botAutorizeTokenScheme = "Bearer",
            BotSecret = "U3uIHr5dEVY.cwA.PbQ.IUlQuQsWrZHAg2wX4qM3wDa6i7W8buvXaGiSgFIAkRg"

        };
        await context.AddAsync(chatbot2);
        await context.SaveChangesAsync();
    }

    private static async Task AddQnA(ApplicationDbContext context)
    {
        var qna = new QnABaseClass
        {
            chatbotName = "Svarta Bjørn QnA",
            regDate = DateTime.Now,
            lastEdit = DateTime.Now,
            isActive = true,
            subscriptionKey = "7d26f05ae72842478df8fdca921de66d"

        };

        await context.AddAsync(qna);
        await context.SaveChangesAsync();
    }

    private static async Task AddKnowledgeBase(ApplicationDbContext context)
    {
        var qna = new QnAKnowledgeBase
        {
            QnABotId = 1,
            QnAKnowledgeName = "SvartaBjorn_QnA",
            RegDate = DateTime.Now,
            LastEdit = DateTime.Now,
            IsActive = true,
            //SubscriptionKey = "7d26f05ae72842478df8fdca921de66d",
            KnowledgeBaseID = "025fd52b-e8d7-43aa-a10f-e8f9bde3e369"

        };

        await context.AddAsync(qna);
        await context.SaveChangesAsync();
    }


}
