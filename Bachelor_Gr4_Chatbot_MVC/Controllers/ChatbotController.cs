//using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Bachelor_Gr4_Chatbot_MVC.Extensions;
using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    
    public class Chat
    {
        public string Text { get; set; }
        public string ChatResponse { get; set; }
        public string watermark { get; set; }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ChatbotController : Controller
    {
        private readonly Microsoft.Bot.Connector.MicrosoftAppCredentials appCredentials;
        private object result;
        private HttpResponseMessage response;
        private HttpResponseMessage response2;
        private HttpResponseMessage response3;
        private HttpClient client;

        private static string contentType = "application/json";
        private static string BotSecret = "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0";

        private static string directLineAddress_V3 = "https://directline.botframework.com/v3/directline/conversations";
        private static string tokenAddress_V3 = "/v3/directline/tokens/generate";
        private static string botAutorizeTokenScheme_V3 = "Bearer";
        private static string directLineConversation_V3 = "/v3/directline/conversations/";

        private static string directLineAddress_V1 = "https://directline.botframework.com/api/conversations";
        private static string tokenAddress_V1 = "/api/tokens/conversation";
        private static string botAutorizeTokenScheme_V1 = "BotConnector";
        private static string directLineConversation_V1 = "/api/conversations";

        private static string ApiExt = "api/messages";
        private static string ApiEndPoint = "https://chatbotsvartabjorn.azurewebsites.net/";


        private static string DiretlineUrl = @"https://chatbotsvartabjorn.azurewebsites.net/api/messages";
        private static string directLineSecret = "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0";
        String messagereturn;
        Activity activity;
        private static string botId = "Chatbot_SvartaBjorn_Azure";

        public ActivitySet activitySet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatbotController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ChatbotController(IConfiguration configuration)
        {
            appCredentials = new Microsoft.Bot.Connector.MicrosoftAppCredentials(configuration);
        }
        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }
        [HttpPost]
        public virtual async Task<IActionResult> Post(string message)
        {
            // Create the connection using the secret token
            client = new HttpClient();
            client.BaseAddress = new Uri(directLineAddress_V3);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(botAutorizeTokenScheme_V3, BotSecret);
            // Fetch a new token for just this chat
            response = await client.PostAsync(tokenAddress_V3, null);
            if (response.IsSuccessStatusCode) // Yey -> We got a connection and a reply
            {
                Conversation conversationinfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                // Clear the headers and set the new token
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(botAutorizeTokenScheme_V3, conversationinfo.Token);
                // Start the conversation
                response = await client.PostAsync(directLineConversation_V3, null);

                if (response.IsSuccessStatusCode) // Yey -> we managed to change the token and initiate the chat
                {
                    Conversation currentConversation = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                    // Set the conversation url
                    string conversationUrl = directLineConversation_V3 + conversationinfo.ConversationId + "/activities";
                    // Create activity
                    Activity thisActivity = new Activity { Type = "message", Text = "tester tester", From = new ChannelAccount { Id = currentConversation.ConversationId } };
                    var myContent = JsonConvert.SerializeObject(thisActivity);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    // Post the activity
                    response = await client.PostAsync(conversationUrl, byteContent);

                    if (response.IsSuccessStatusCode) // Yey -> It was posted
                    {
                        // Fetch messages
                        response = await client.GetAsync(conversationUrl);
                        activitySet = response.Content.ReadAsAsync(typeof(ActivitySet)).Result as ActivitySet;
                    }
                }
            }
            return Json(response.Content.ReadAsStringAsync().Result);
        }
    }
}