using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAEditAndStoreMessagePairToBot
    {
        public string Question{ get; set; }
        public string Answer { get; set; }
        public string Type { get; set; }

        public int ConversationId { get; set; }
    }
}
