using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels
{
    public class ViewMessagesForConversation
    {
        public List<Message> messages { get; set; }
        public Conversation conversation { get; set; }
    }
}
