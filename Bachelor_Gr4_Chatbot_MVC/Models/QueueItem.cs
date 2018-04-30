using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class QueueItem
    {
        public int ConversationId { get; set; }
        public DateTime TimeAddedToQueue { get; set; }

        public string Key { get; set; }

    }
}
