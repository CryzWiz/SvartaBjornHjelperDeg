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
        public bool IsChatBot { get; set; }
        public DateTime Registered { get; set; }
        public bool Result { get; set; }
    }
}
