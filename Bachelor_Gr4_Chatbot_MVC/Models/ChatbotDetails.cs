﻿using System;
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

        public string contentType { get; set; }

        // Chatbot type
        public string TypeName { get; set; }
        public int TypeId { get; set; }

        // Microsoft bot details
        public string BotSecret { get; set; }
        public string baseUrl { get; set; }
        public string tokenUrlExtension { get; set; }
        public string conversationUrlExtension { get; set; }
        public string conversationUrlExtensionEnding { get; set; }
        public string botAutorizeTokenScheme { get; set; }

        // ChatbotType Details

        public virtual List<ChatbotTypes> chatbotTypes { get; set; }
    }
}
