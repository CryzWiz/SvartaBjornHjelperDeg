using Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IChatRepository
    {
        Task SaveOpeningHours(OpeningHours openingHours);
        Task SaveOpeningHours(IEnumerable<OpeningHours> openingHours);
        Task<IEnumerable<ChatOpeningHoursViewModel>> GetAllOpeningHours();

        Task<int> AddConversationAsync(Conversation conversation);
        Task<string> GetName(string userId);
        Task AddMessageAsync(Message message);
        Task AddMessagesAsync(IEnumerable<Message> messages);
        Task<Conversation> GetConversationByIdAsync(int conversationId);
        Task UpdateConversationAsync(Conversation conversation);
    }
}
