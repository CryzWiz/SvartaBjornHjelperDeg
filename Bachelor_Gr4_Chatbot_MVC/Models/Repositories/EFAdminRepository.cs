using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModel;
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
                               UserId = u.Id,
                               Email = u.Email,
                               Username = u.UserName,
                               Active = u.IsActive
                           }).ToListAsync();
            return r;
        }

        /// <summary>
        /// Fetch all the users in the database and store the information we want to
        /// display as a User in a List<User>
        /// </summary>
        /// <returns>All users</returns>
        public async Task<bool> UpdateUserData(User user)
        {
            var u = await Task.Run(() => manager.Users.FirstOrDefault(X => X.Id == user.UserId));
            

            IdentityResult r = null;

            // Check if there is a change in the Username
            if(u.Email != user.Email)
            {
                r = await manager.SetEmailAsync(u, user.Email);
                r = await manager.SetUserNameAsync(u, user.Email);

                // Fetch the user and re-set the email to confirmed
                // This might be a potential security problem. Should be re-confirmed.

                var dbU = await Task.Run(() => db.Users.FirstOrDefault(X => X.Id == user.UserId));
                dbU.EmailConfirmed = true;
                await Task.Run(() => db.Update(dbU));
                var dbR = db.SaveChangesAsync();
            }

            if(r.Succeeded) return true;
            else return false;
        }

        /// <summary>
        /// Fetch all the user with the given username
        /// </summary>
        /// <paramref name="username"/></param>
        /// <returns>One User</returns>
        public async Task<User> GetUser(String username)
        {
            var u = await Task.Run(() => manager.Users.FirstOrDefault(X => X.UserName == username));
            if(u != null)
            {
                User user = new User();
                user.UserId = u.Id;
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

        /// <summary>
        /// Add ChatGroup to the database
        /// </summary>
        /// <param name="chatGroup"></param>
        /// <returns>bool result</returns>
        public async Task<bool> AddChatGroup(ChatGroup chatGroup)
        {
            await db.ChatGroups.AddAsync(chatGroup);
            return ((await db.SaveChangesAsync() > 0) ? true: false);
        }

        public async Task<IEnumerable<ChatGroupViewModel>> GetAllChatGroupsVM()
        {
            IEnumerable<ChatGroupViewModel> chatGroups = await (from chatGroup in db.ChatGroups
                                                          select new ChatGroupViewModel
                                                          {
                                                              ChatGroupName = chatGroup.ChatGroupName
                                                          }).ToListAsync();
            return chatGroups;
                            
        }
    }
}
