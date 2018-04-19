using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class UserChatGroup
    {
        public string ChatGroupId { get; set; }
        public ChatGroup ChatGroup { get; set; }
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
