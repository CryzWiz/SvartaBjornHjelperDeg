using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAPairs
    {
        [Key]
        public int QnAPairsId { get; set; }

        public string Query { set; get; }
        public string Answer { set; get; }
        public int KnowledgeBaseId { set; get; }
        public bool Trained { set; get; }
        public DateTime Published { get; set; }
    }
}
