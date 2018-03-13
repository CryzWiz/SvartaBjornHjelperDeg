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
            Conversation conversationinfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
            // Clear the headers and set the new token
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(botAutorizeTokenScheme_V3, conversationinfo.Token);
            // Start the conversation
            response = await client.PostAsync(directLineConversation_V3, null);
            Conversation currentConversation = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
            // Set the conversation url
            string conversationUrl = directLineConversation_V3 + conversationinfo.ConversationId + "/activities";
            // Create activity
            Activity thisActivity = new Activity { Type = "message", Text = "tester tester", From = new ChannelAccount { Id = currentConversation.ConversationId }};
            var myContent = JsonConvert.SerializeObject(thisActivity);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Post the activity
            response = await client.PostAsync(conversationUrl, byteContent);

            // Fetch messages
            response = await client.GetAsync(conversationUrl);

            //response = await client.PostAsJsonAsync(conversationUrl, requestMessage);


            if (response.IsSuccessStatusCode) // Yey -> We got a connection and a reply
            {
                
                //Conversation conversationinfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;

                
                //string conversationUrl = "/v3/directline/conversations/" + conversationinfo.ConversationId + "/activities";

                //ChannelAccount user = new ChannelAccount { Id = conversationinfo.ConversationId, Name = "allan_name" };
                //ConversationAccount conversationAccount = new ConversationAccount { Id = conversationinfo.ConversationId };
                //activity = new Activity { From = user, Text = "Hei", Type = "message", Conversation = conversationAccount };
                //Message msg = new Message() { text = message, from = user, conversationId = conversationinfo.ConversationId,
                //type = "message"};
                //var a = Activity.CreateMessageActivity();
                //a.From = user;
                //a.Text = "hello";
                //a.Type = "message";
                //HttpRequestMessage requestMessage = new HttpRequestMessage();
                //requestMessage.Content = new StringContent("{\"type\":\"message\",\"from\": {\"id\": \"user1\" }, \"text\":\"Hello\"}", Encoding.UTF8, "application/json");
                //response = await client.PostAsJsonAsync(conversationUrl, requestMessage);
                //response = await client.GetAsync(conversationUrl);
            }
            


            
            //if (response.IsSuccessStatusCode)
            //{
            //    var conversation = new Conversation();
            //    response = await client.PostAsJsonAsync("/v3/directline/conversations", conversation);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
            //        string conversationUrl = "/v3/directline/conversations/"+ConversationInfo.ConversationId + " / activities/";
            //        Message msg = new Message() { text = message };
            //        response = await client.PostAsJsonAsync(conversationUrl, msg);
            //        if (response.IsSuccessStatusCode)
            //        {
            //            response = await client.GetAsync(conversationUrl);
            //            if (response.IsSuccessStatusCode)
            //            {
            //                MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
            //                ViewBag.Messages = BotMessage;
            //                IsReplyReceived = true;
            //            }
            //        }
            //    }
            //}
            return Json(response.Content.ReadAsStringAsync().Result);
        }
    }
}