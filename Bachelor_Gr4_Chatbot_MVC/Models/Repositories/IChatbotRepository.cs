using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IChatbotRepository
    {
        // Microsoft methods - To be removed
        Task<List<ChatbotDetails>> GetAllChatbots();
        Task<bool> RegisterNewChatbot(ChatbotDetails chatbotDetails);
        Task<ChatbotDetails> GetChatbotDetails(int id);
        Task<bool> UpdateChatbotDetails(ChatbotDetails chatbotDetails);
        Task<bool> DeleteChatbot(ChatbotDetails chatbot);
        Task<bool> CheckIfBotActive(int id);
        Task<string[]> ActivateBot(int id);
        Task<ChatbotDetails> GetActiveBot();

        // Chatbot Type I.E Microsoft Bot or QnAbot. Not used.
        Task<List<ChatbotTypes>> GetAllTypes();

        // QnA methods
        Task<List<QnABaseClass>> GetAllQnABots();
        Task<QnADetails> GetQnABotDetails(int id);
        Task<string[]> RegisterNewQnABotAsync(QnABaseClass qnabot);
        Task<QnAKnowledgeBase> GetQnAKnowledgeBaseAsync(int id);
        Task<bool> AddNewQnAKnowledgeBaseAsync(QnAKnowledgeBase b);
        Task<QnABaseClass> GetActiveQnABaseClassAsync();
        Task<QnAKnowledgeBase> GetActiveQnAKnowledgeBaseAsync();
        Task<bool> AddSingleQnAPairToBaseAsync(QnATrainBase qna);
        Task<bool> DeleteQnAKnowledgeBaseByIdAsync(int id);
        Task<QnABaseClass> GetQnABotDetailsBySubscriptionAsync(string subKey);
        Task<QnAKnowledgeBase> GetQnAKnowledgeBaseByKnowledgeIdAsync(string knowledgeBaseId);
    }
}
