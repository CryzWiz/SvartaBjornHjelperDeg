using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnABaseClass
    {
        // Name and id for database and display purposes
        [Key]
        public int QnAId { get; set; }
        public string chatbotName { get; set; }

        // Registration details
        public DateTime regDate { get; set; }
        public DateTime lastEdit { get; set; }
        public bool isActive { get; set; }


        // QnA Details
        public string subscriptionKey { get; set; }


        public string knowledgeBaseID { get; set; }
        // https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/
        public string requestUrl { get; set; }
        // requestUrl + /{knowledgeBaseID}/generateAnswer
        public string askQuestionUrl { get; set; }
        // requestUrl + /{knowledgeBaseID}/train
        public string trainknowledgeBaseUrl { get; set; }
        // requestUrl + /{knowledgeBaseID}
        public string publishKnowledgeBaseUrl { get; set; }
        // requestUrl + /{knowledgeBaseID}
        public string updateKnowledgeBaseUrl { get; set; }
        //public string contentType { get; set; }
    }
}
