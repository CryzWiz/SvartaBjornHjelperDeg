using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnATrainBase
    {
        [DisplayName("Spørsmål")]
        public string Query { get; set; }
        [DisplayName("Svar")]
        public string Answer { get; set; }
        [DisplayName("Chatbotnavn")]
        public string QnABotName { get; set; }
        [DisplayName("Abonnementsnøkkel")]
        public string SubscriptionKey { get; set; }
        [DisplayName("Kunnskapsbase Navn")]
        public string KnowledgeBaseName { get; set; }
        [DisplayName("Kunnskapsbase Id")]
        public string KnowledgeBaseId { get; set; }

    }
}
