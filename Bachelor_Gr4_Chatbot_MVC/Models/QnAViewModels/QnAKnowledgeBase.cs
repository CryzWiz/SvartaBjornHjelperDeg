using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels
{
    public class QnAKnowledgeBase
    {
        [Key]
        [DisplayName("Kunnskapsbase Id")]
        public int QnAKnowledgeBaseId { get; set; }
        [DisplayName("Bot Id")]
        public int QnABotId { get; set; }
        [DisplayName("Kunnskapsbase Navn")]
        public string QnAKnowledgeName { get; set; }
        [DisplayName("Registrert Dato")]
        public DateTime RegDate { get; set; }
        [DisplayName("Sist Oppdatert")]
        public DateTime LastEdit { get; set; }
        [DisplayName("Aktiv")]
        public bool IsActive { get; set; }
        [DisplayName("Abonnements-nøkkel")]
        public string KnowledgeBaseID { get; set; }




        [DisplayName("Base Url")]
        // https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases
        public string RequestUrl {
            get
            {
                //RequestUrl = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases";
                return "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases";
            }

        }
        [DisplayName("Post spørsmål Url")]
        // requestUrl + /{knowledgeBaseID}/generateAnswer
        public string AskQuestionUrl {
            get
            {
                return String.Format("{0}/{1}/generateAnswer", RequestUrl, KnowledgeBaseID);
            }
        }
        [DisplayName("Trenings Url")]
        // requestUrl + /{knowledgeBaseID}/train
        public string TrainknowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}/train", RequestUrl, KnowledgeBaseID);
            }
        }
        [DisplayName("Publiserings Url")]
        // requestUrl + /{knowledgeBaseID}
        public string PublishKnowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }
        [DisplayName("Oppdaterings Url")]
        // requestUrl + /{knowledgeBaseID}
        public string UpdateKnowledgeBaseUrl {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }

        [DisplayName("Lag Kunnskapsbase Url")]
        public string CreateNewQnAKnowledgeBase
        {
            get
            {
                return String.Format("{0}/create", RequestUrl);
            }
        }

        [DisplayName("Slett Kunnskapsbase Url")]
        public string DeleteQnAKnowledgeBase
        {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }

        [DisplayName("Legg til QnA par Url")]
        public string AddQnAPairUrl
        {
            get
            {
                return String.Format("{0}/{1}", RequestUrl, KnowledgeBaseID);
            }
        }
        //public string contentType { get; set; }
    }
}
