using Bachelor_Gr4_Chatbot_MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public class EFAdminRepository : IAdminRepository
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;

        public EFAdminRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.db = db;
            manager = userManager;
        }

        public List<User> GetAllUsers()
        {
            //return await manager.Users.ToListAsync();
            var r = (from u in manager.Users

                                         select new User
                                         {
                                             Email = u.Email,
                                             Username = u.UserName,
                                             Active = u.IsActive
                                         }).ToList();
            return r;
        }

        public User GetUser(String username)
        {
            var u = manager.Users.FirstOrDefault(X => X.Email == username);
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
