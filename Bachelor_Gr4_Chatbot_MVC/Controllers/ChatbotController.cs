using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using System;
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
        private readonly MicrosoftAppCredentials appCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatbotController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ChatbotController(IConfiguration configuration)
        {
            appCredentials = new MicrosoftAppCredentials(configuration);
        }
        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }

        // POST api/values
        [HttpPost]
        public virtual async Task<IActionResult> Post(Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;
                await ReplyMessage(activity, $"You sent {activity.Text} which was {length} characters");
            }
            else
            {
                await HandleSystemMessage(activity);
            }
            var result = new { test = "suksess - vi har kontakt!" };
            //return Ok();
            return Json(result);
        }

        /// <summary>
        /// Handles the system message.
        /// </summary>
        /// <param name="activity">The activity.</param>
        private async Task<Activity> HandleSystemMessage(Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;

                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;

                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;

                case ActivityTypes.Typing:
                    // Handle knowing that the user is typing
                    break;

                case ActivityTypes.Ping:
                    await ReplyMessage(activity, "Pong");
                    break;

                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Replies the message.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="message">The message.</param>
        private async Task ReplyMessage(Activity activity, string message)
        {
            var serviceEndpointUri = new Uri(activity.ServiceUrl);
            var connector = new ConnectorClient(serviceEndpointUri, appCredentials);
            var reply = activity.CreateReply(message);

            await connector.Conversations.ReplyToActivityAsync(reply);
        }
    }
}