using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatGroup
    {
        [Key]
        public string ChatGroupId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserChatGroup> GroupMembers { get; set; }
    }
}
