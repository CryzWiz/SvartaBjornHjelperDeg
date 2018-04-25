using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class QnAKeywordPair
    {
        [Key]
        public int QnAKeywordPairId { get; set; }
        public string Query { get; set; }
        public string Answer { get; set; }
    }
}
