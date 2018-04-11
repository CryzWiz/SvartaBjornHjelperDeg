using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAPairsView
    {
        public int QnAId { get; set; }
        public int QnAKnowledgeBaseId { get; set; }

        public List<QnAPairs> QnAPairs { get; set; }
    }
}
