using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAPairs
    {
        [Key]
        public int QnAPairsId { get; set; }

        [DisplayName("Spørsmål")]
        public string Query { set; get; }
        [DisplayName("Svar")]
        public string Answer { set; get; }
        [DisplayName("Kunnskapsbase Id")]
        public string KnowledgeBaseId { set; get; }
        [DisplayName("Oppdatert")]
        public bool Trained { set; get; }
        [DisplayName("Publisert")]
        public bool Published { set; get; }
        [DisplayName("Publisert Dato")]
        public DateTime PublishedDate { set; get; }
        [DisplayName("Oppdatert Dato")]
        public DateTime TrainedDate { set; get; }
    }
}
