using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Indicates if the account is active. If not, it should not be possible to log in. 
        public bool IsActive { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
