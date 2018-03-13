using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.BotModels
{
    public class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public DateTime created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }
}
