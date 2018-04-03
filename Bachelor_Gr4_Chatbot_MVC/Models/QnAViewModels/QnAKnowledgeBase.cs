using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAKnowledgeBase
    {
        public int QnAKnowledgeBaseId { get; set; }
        public int QnABotId { get; set; }
        public string QnAKnowledgeName { get; set; }

        public DateTime RegDate { get; set; }
        public DateTime LastEdit { get; set; }

        public bool IsActive { get; set; }

        public string KnowledgeBaseID { get; set; }





        // https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases
        public string RequestUrl {
            get
            {
                return RequestUrl;
            }
            set
            {
                RequestUrl = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases";
            }

        }
        // requestUrl + /{knowledgeBaseID}/generateAnswer
        public string AskQuestionUrl {
            get
            {
                return String.Format("{0}/{1}/generateAnswer", RequestUrl, KnowledgeBaseID);
            }
        }
        // requestUrl + /{knowledgeBaseID}/train
        public string TrainknowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}/train", RequestUrl, KnowledgeBaseID);
            }
        }
        // requestUrl + /{knowledgeBaseID}
        public string PublishKnowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }
        // requestUrl + /{knowledgeBaseID}
        public string UpdateKnowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }
        //public string contentType { get; set; }
    }
}
