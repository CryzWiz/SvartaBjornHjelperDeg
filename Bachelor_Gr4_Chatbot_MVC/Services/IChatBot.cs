using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;

namespace Bachelor_Gr4_Chatbot_MVC.Services
{
    public interface IChatBot
    {

        string GetConversationTokenAsString();
        /*Task<Conversation> GetActiveConversation(string token);
        Task<string> GetConversationTokenAsString();
        Task<HttpClient> GetHttpClient(string token);
       // Task<IActionResult> Post([Bind(new[] { "query" }), System.Web.Http.FromBody] QnAIndexViewModel Qna);
        Task<string> PostCommentByToken(string token, string comment);
        Task<Conversation> StartAndGetNewConversation();

        string TestDependency();*/
    }
}