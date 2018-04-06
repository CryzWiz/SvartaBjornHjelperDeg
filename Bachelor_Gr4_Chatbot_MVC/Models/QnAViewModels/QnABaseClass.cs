using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnABaseClass
    {
        // Name and id for database and display purposes
        [Key]
        [DisplayName("QnA Bot Id")]
        public int QnAId { get; set; }
        [DisplayName("Bot Navn")]
        public string chatbotName { get; set; }

        // Registration details
        [DisplayName("Registrert Dato")]
        public DateTime regDate { get; set; }
        [DisplayName("Sist Oppdatert")]
        public DateTime lastEdit { get; set; }
        [DisplayName("Aktiv")]
        public bool isActive { get; set; }


        // QnA Details
        [DisplayName("Abonnements-nøkkel")]
        public string subscriptionKey { get; set; }

        [DisplayName("Kunnskapsbase Id")]
        public string knowledgeBaseID { get; set; }

    }
}
