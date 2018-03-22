using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatbotDetails
    {
        // Name and id for database and display purposes
        [Key]
        public int chatbotId { get; set; }
        public string chatbotName { get; set; }

        // Registration details
        public DateTime regDate { get; set; }
        public DateTime lastEdit { get; set; }
        public bool isActive { get; set; }



        // Microsoft bot details
        public string contentType { get; set; }
        public string BotSecret { get; set; }
        public string base_Url { get; set; }
        public string tokenUrlExtension { get; set; }
        public string converationUrlExtenison { get; set; }
        public string botAutorizeTokenScheme { get; set; }

        
    }
}
