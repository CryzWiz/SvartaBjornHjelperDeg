//using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Bachelor_Gr4_Chatbot_MVC.Extensions;
using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
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
using System.Web;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ChatbotController : Controller
    {
        private readonly Microsoft.Bot.Connector.MicrosoftAppCredentials appCredentials;
        private HttpResponseMessage response;

        private static string contentType = "application/json";
        private static string BotSecret = "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0";

        private static string directLineAddress_V3 = "https://directline.botframework.com";
        private static string tokenAddress_V3 = "/v3/directline/tokens/generate";
        private static string botAutorizeTokenScheme_V3 = "Bearer";
        private static string directLineConversation_V3 = "/v3/directline/conversations/";

        private static string DiretlineUrl = @"https://chatbotsvartabjorn.azurewebsites.net/api/messages";

        private static string botId = "Chatbot_SvartaBjorn_Azure";

        public ActivitySet activitySet;
        private IChatbotRepository chatbotRepository;

        public ChatbotController(IChatbotRepository chatbotRepository)
        {
            this.chatbotRepository = chatbotRepository;
        }


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



        /// <summary>
        /// Make a connection to the chatbot and get a token that a user can use to have a conversation with the bot
        /// </summary>
        /// <returns>(String)Securitytoken to be used in conversations with the chatbot</returns>
        public async Task<String> GetConversationTokenAsString()
        {
            Models.ChatbotDetails activeBot = await chatbotRepository.GetActiveBot();   // fetch the active bot

            // Create the connection using the secret token
            HttpClient client = new HttpClient();   // new httpclient
            client.BaseAddress = new Uri(activeBot.baseUrl);    // set base url
            client.DefaultRequestHeaders.Accept.Clear();    // clear headers
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contentType
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, activeBot.BotSecret);  // make connection
            // Fetch a new token for just this chat
            response = await client.PostAsync(activeBot.tokenUrlExtension, null);   // make connection and await response
            if (response.IsSuccessStatusCode) // Yey -> We got a connection and a reply
            {
                Conversation conversationinfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;  // read response as a conversation
                return conversationinfo.Token;  // return the new security token
            }
            else
            {
                return null;    // else we return null
            }
        }

        /// <summary>
        /// Start a new conversation by contacting the chatbot, exchange tokens, change token, start the conversation for the user
        /// </summary>
        /// <returns>(Conversation)The started conversation details</returns>
        public async Task<Conversation> StartAndGetNewConversation()
        {
            Models.ChatbotDetails activeBot = await chatbotRepository.GetActiveBot();   // fetch the active bot
            if(activeBot != null)
            {
                // Create the connection using the secret token
                HttpClient client = new HttpClient();   // httpclient
                client.BaseAddress = new Uri(activeBot.baseUrl);    // set base url
                client.DefaultRequestHeaders.Accept.Clear();    // clear all header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contentType
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, activeBot.BotSecret);  // set secyrity headers
                // Fetch a new token for just this chat
                response = await client.PostAsync(activeBot.tokenUrlExtension, null);   // make token exchange and await response
                if (response.IsSuccessStatusCode) // Yey -> We got a connection and a reply
                {
                    Conversation conversationinfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;  // read response as a conversation
                    // Clear the headers and set the new token
                    client.DefaultRequestHeaders.Accept.Clear();    // clear the headers
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contenttype
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, conversationinfo.Token);   // set the new secyrity token
                    // Start the conversation
                    response = await client.PostAsync(activeBot.conversationUrlExtension, null); // make connection and get response
                    if (response.IsSuccessStatusCode) // Yey -> we managed to change the token and initiate the chat
                    {
                        Conversation currentConversation = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;   // read response as a Conversation
                        return currentConversation;     // return conversation
                    }
                    else
                    {
                        return null;    // could not make a connection
                    }
                }
                else
                {
                    return null;    // Could not make a connection
                }
            }
            else
            {
                return null;    // we dont have a active bot
            }
        }


        /// <summary>
        /// Takes the token for a specific chat-channel and get the conversation-details
        /// </summary>
        /// <param name="token"></param>
        /// <returns>(Conversation)Conversation for the user whom the token belongs to</returns>
        public async Task<Conversation> GetActiveConversation(string token)
        {
            Models.ChatbotDetails activeBot = await chatbotRepository.GetActiveBot();   // Fetch the active bot
            if(activeBot != null)
            {
                // Create the connection using the given token
                HttpClient client = new HttpClient();   // create httpclient
                client.BaseAddress = new Uri(activeBot.baseUrl);    // set base url address
                client.DefaultRequestHeaders.Accept.Clear();    // clear all headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contenttype
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, token); // set security bearer and token
                response = await client.PostAsync(activeBot.tokenUrlExtension, null);   // make connection and get reponse
                if (response.IsSuccessStatusCode) // Yey -> we managed to change the token and initiate the chat
                {
                    Conversation currentConversation = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;   // read response as a Conversation
                    return currentConversation; // return the conversation
                }
                else
                {
                    return null; // else we return null
                }
            }
            else
            {
                return null;    // we dont have any active bots
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody][Bind("query")] QnAIndexViewModel Qna)
        {
            // Create the connection using the secret token
            HttpClient client = new HttpClient();
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
                    Activity thisActivity = new Activity { Type = "message", Text = Qna.query, From = new ChannelAccount { Id = currentConversation.ConversationId } };
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
            string responseString = null;
            foreach (Activity a in activitySet.Activities)
            {
                responseString = a.Text;
            }
            // Send the entire activityset in return for now. 
            return Json(responseString);
        }
    }
}