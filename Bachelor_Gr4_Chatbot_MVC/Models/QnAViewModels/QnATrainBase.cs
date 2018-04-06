using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnATrainBase
    {
        public string Query { get; set; }
        public string Answer { get; set; }
        public string QnABotName { get; set; }
        public string SubscriptionKey { get; set; }
        public string KnowledgeBaseName { get; set; }
        public string KnowledgeBaseId { get; set; }

    }
}
