using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.BotModels
{
    public class Conversation
    {
        public String conversationId { get; set; }
        public String token { get; set; }
        public int expires_in { get; set; }
        public String streamUrl { get; set; }

    }
}
