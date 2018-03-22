using Bachelor_Gr4_Chatbot_MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    /// <summary>
    /// Repository for Administrator functions -> Database access and management
    /// </summary>
    public class EFAdminRepository : IAdminRepository
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;

        public EFAdminRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.db = db;
            manager = userManager;
        }

        /// <summary>
        /// Deactivate the user-account that is related to given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> DeactivateUser(string username)
        {
            var u = await Task.Run(() => db.Users.FirstOrDefault(X => X.UserName == username));
            if(u != null)
            {
                u.IsActive = false;
                await db.SaveChangesAsync();               
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Activate user-account that is related to given username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> ActivateUser(string username)
        {
            var u = await Task.Run(() => db.Users.FirstOrDefault(X => X.UserName == username));
            if (u != null)
            {
                u.IsActive = true;
                await db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fetch all the users in the database and store the information we want to
        /// display as a User in a List<User>
        /// </summary>
        /// <returns>All users</returns>
        public async Task<List<User>> GetAllUsers()
        {
            //return await manager.Users.ToListAsync();
            var r = await (from u in manager.Users
                           select new User
                           {
                               Email = u.Email,
                               Username = u.UserName,
                               Active = u.IsActive
                           }).ToListAsync();
            return r;
        }

        /// <summary>
        /// Fetch all the user with the given username
        /// </summary>
        /// <paramref name="username"/></param>
        /// <returns>One User</returns>
        public async Task<User> GetUser(String username)
        {
            var u = await Task.Run(() => manager.Users.FirstOrDefault(X => X.Email == username));
            if(u != null)
            {
                User user = new User();
                user.Active = u.IsActive;
                user.Email = u.Email;
                user.Username = u.UserName;
                return user;
            }
            else
            {
                return null;
            }
            
        }

        public async Task<List<ChatbotDetails>> GetAllChatbots()
        {
            //var c = await Task.Run(() => db.ChatbotDetails);
            var r = await (from c in db.ChatbotDetails
                           select new ChatbotDetails
                           {
                               chatbotId = c.chatbotId,
                               chatbotName = c.chatbotName,
                               isActive = c.isActive,
                               regDate = c.regDate,
                               lastEdit = c.lastEdit,
                               contentType = c.contentType,
                               base_Url = c.base_Url,
                               tokenUrlExtension = c.tokenUrlExtension,
                               converationUrlExtenison = c.converationUrlExtenison,
                               botAutorizeTokenScheme = c.botAutorizeTokenScheme,
                               BotSecret = c.BotSecret
                           }).ToListAsync();

            return r;
        }

        public async Task<bool> RegisterNewChatbot(ChatbotDetails chatbotDetails)
        {
            await db.ChatbotDetails.AddAsync(chatbotDetails);
            if (await db.SaveChangesAsync() < 0) return true;
            else return false;

        }

        public async Task<ChatbotDetails> GetChatbotDetails(int id)
        {
            var c = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.chatbotId == id));
            return c;
        }
    }
}
