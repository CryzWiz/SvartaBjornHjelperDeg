using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatbotTypes
    {
        [Key]
        public int chatBotTypeId { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
    }
}
