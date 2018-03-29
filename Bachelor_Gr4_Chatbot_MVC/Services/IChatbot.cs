using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Services
{
    interface IChatbot
    {
        Task<String> GetConversationTokenAsString();
        Task<Conversation> StartAndGetNewConversation();
        Task<Conversation> GetActiveConversation(string token);
        Task<String> PostCommentByToken(string token, string comment);
    }
}
