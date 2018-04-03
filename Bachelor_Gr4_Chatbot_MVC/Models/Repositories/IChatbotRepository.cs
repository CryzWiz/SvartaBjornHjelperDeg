using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IChatbotRepository
    {
        // Microsoft methods
        Task<List<ChatbotDetails>> GetAllChatbots();
        Task<bool> RegisterNewChatbot(ChatbotDetails chatbotDetails);
        Task<ChatbotDetails> GetChatbotDetails(int id);
        Task<bool> UpdateChatbotDetails(ChatbotDetails chatbotDetails);
        Task<bool> DeleteChatbot(ChatbotDetails chatbot);
        Task<bool> CheckIfBotActive(int id);
        Task<string[]> ActivateBot(int id);
        Task<ChatbotDetails> GetActiveBot();

        Task<List<ChatbotTypes>> GetAllTypes();

        // QnA methods
        Task<List<QnABaseClass>> GetAllQnABots();
        Task<QnADetails> GetQnABotDetails(int id);
        Task<string[]> RegisterNewQnABotAsync(QnABaseClass qnabot);
    }
}
