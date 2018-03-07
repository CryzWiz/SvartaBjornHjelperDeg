using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ConversationContent
    {
        [Key]
        public int ConversationContentId { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime Registered { get; set; }
    }
}
