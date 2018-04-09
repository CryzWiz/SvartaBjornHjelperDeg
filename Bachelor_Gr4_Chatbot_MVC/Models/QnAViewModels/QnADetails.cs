using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnADetails
    {
        [DisplayName("QnA Bot Id")]
        public int QnAId { get; set; }
        [DisplayName("Bot Navn")]
        public string ChatbotName { get; set; }

        // Registration details
        [DisplayName("Registrert Dato")]
        public DateTime RegDate { get; set; }
        [DisplayName("Sist Oppdatert")]
        public DateTime LastEdit { get; set; }
        [DisplayName("Aktiv")]
        public bool IsActive { get; set; }


        // QnA Details
        [DisplayName("Abonnements-nøkkel")]
        public string SubscriptionKey { get; set; }
        [DisplayName("QnA Kunnskapsbaser")]
        public virtual IEnumerable<QnAKnowledgeBase> KnowledgeBases { get; set;}
    }
}
