using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnADetails
    {
        public int QnAId { get; set; }
        public string ChatbotName { get; set; }

        // Registration details
        public DateTime RegDate { get; set; }
        public DateTime LastEdit { get; set; }
        public bool IsActive { get; set; }


        // QnA Details
        public string SubscriptionKey { get; set; }

        public virtual IEnumerable<QnAKnowledgeBase> KnowledgeBases { get; set;}
    }
}
