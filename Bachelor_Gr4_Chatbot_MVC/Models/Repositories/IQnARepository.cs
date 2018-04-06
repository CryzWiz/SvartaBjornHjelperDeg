using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IQnARepository
    {
        Task<string> PostQuery(string Query);
        Task<string> RegisterNewQnAKnowledgeBaseAsync(QnABaseClass q, QnAKnowledgeBase b);
        Task<bool> DeleteKnowledgeBase(QnABaseClass q, QnAKnowledgeBase b);
    }
}
