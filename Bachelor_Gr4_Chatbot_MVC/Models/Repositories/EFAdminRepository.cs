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
    }
}
