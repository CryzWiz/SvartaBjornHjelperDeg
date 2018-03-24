using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class RoleOptions
    {
        public RoleOptions()
        {
            AdminRole = "Admin";
            ChatEmployeeRole = "ChatEmployee";
        }

        public string AdminRole { get; set; }
        public string ChatEmployeeRole { get; set; }

    }
}
