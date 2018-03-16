using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }

        // User Group to the user initiating 
        public int UserGroup1 { get; set; }
        public int UserGroup2 { get; set; }
        public bool IsChatBot { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Result { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
