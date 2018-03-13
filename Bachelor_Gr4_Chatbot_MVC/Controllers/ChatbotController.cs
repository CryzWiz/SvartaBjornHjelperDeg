//using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Bachelor_Gr4_Chatbot_MVC.Extensions;
using Bachelor_Gr4_Chatbot_MVC.Models.BotModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static string directLineAddress = "https://directline.botframework.com/v3/directline/conversations";
        private static string BaseAddress = "https://chatbotsvartabjorn.azurewebsites.net/";
        private static string ApiExt = "api/messages";
        private static string DiretlineUrl = @"https://chatbotsvartabjorn.azurewebsites.net/api/messages";
        private static string directLineSecret = "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0";
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

            bool IsReplyReceived = false;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com/v3/directline");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "SGOlKUQmphg.cwA.0ho.CYEuXR9VGPjZ19d7n7zKKjSYpVamhVYgh7qEdE_gxn0");
            
            response = await client.PostAsync("/v3/directline/tokens/generate", null);

            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                response = await client.PostAsJsonAsync("/v3/directline/conversations", conversation);
                //if (response.IsSuccessStatusCode)
                //{
                //    Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                //    string conversationUrl = ConversationInfo.ConversationId + "/messages/";
                //    Message msg = new Message() { text = message };
                //    response = await client.PostAsJsonAsync(conversationUrl, msg);
                //    if (response.IsSuccessStatusCode)
                //    {
                //        response = await client.GetAsync(conversationUrl);
                //        if (response.IsSuccessStatusCode)
                //        {
                //            MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                //            ViewBag.Messages = BotMessage;
                //            IsReplyReceived = true;
                //        }
                //    }
                //}

            }
            return Json(response.Content.ReadAsStringAsync());
        }





        // POST api/values
        //[HttpPost]
        //public virtual async Task<IActionResult> Post(Activity activity)
        //{
        //    Activity a = activity as Activity;

        //    var result = new { test = "failed -> Vi kom ikke inn i noen" };

        //    if (activity.Type == ActivityTypes.Message)
        //    {
        //        // calculate something for us to return
        //        int length = (activity.Text ?? string.Empty).Length;
        //        await ReplyMessage(activity, $"You sent {activity.Text} which was {length} characters");
        //        result = new { test = "suksess - vi har kontakt og er i ReplyMessage!" };
        //    }
        //    else
        //    {
        //        await HandleSystemMessage(activity);
        //        if (activity is Activity)
        //        {
        //            var t = "Do I work?";
        //            result = new { test = "suksess - vi har kontakt og er i HandleSystemMessage! Vi har en Activity. " +
        //                "Activity.Type == " + a.Text};
        //        }
        //        else
        //        {
        //            result = new { test = "suksess - vi har kontakt og er i HandleSystemMessage! Activity == null" };
        //        }
                
        //    }
            
        //    //return Ok();
        //    return Json(result);
        //}

        /// <summary>
        /// Handles the system message.
        /// </summary>
        /// <param name="activity">The activity.</param>
        //private async Task<Activity> HandleSystemMessage(Activity activity)
        //{
        //    switch (activity.Type)
        //    {
        //        case ActivityTypes.DeleteUserData:
        //            // Implement user deletion here
        //            // If we handle user deletion, return a real message
        //            break;

        //        case ActivityTypes.ConversationUpdate:
        //            // Handle conversation state changes, like members being added and removed
        //            // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
        //            // Not available in all channels
        //            break;

        //        case ActivityTypes.ContactRelationUpdate:
        //            // Handle add/remove from contact lists
        //            // Activity.From + Activity.Action represent what happened
        //            break;

        //        case ActivityTypes.Typing:
        //            // Handle knowing that the user is typing
        //            break;

        //        case ActivityTypes.Ping:
        //            await ReplyMessage(activity, "Pong");
        //            break;

        //        default:
        //            break;
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Replies the message.
        ///// </summary>
        ///// <param name="activity">The activity.</param>
        ///// <param name="message">The message.</param>
        //private async Task ReplyMessage(Activity activity, string message)
        //{
        //    var serviceEndpointUri = new Uri(activity.ServiceUrl);
        //    var connector = new ConnectorClient(serviceEndpointUri, appCredentials);
        //    var reply = activity.CreateReply(message);

        //    await connector.Conversations.ReplyToActivityAsync(reply);
        //}
    }
}