using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.AspNetCore.Http;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class Chat
    {
        public string ChatMessage { get; set; }
        public string ChatResponse { get; set; }
        public string watermark { get; set; }
    }

    public class ChatbotController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        private static string DiretlineUrl
            = @"https://directline.botframework.com";
        private static string directLineSecret =
            "0EHMWUP76KI.cwA.Sds.wy7NypTmRGbx3oXxgjUc-4OHWwBikhE0rSUVxjIvkhs";
        private static string botId =
            "Chatbot_SvartaBjorn_Azure";
        private ChannelAccount channel = new ChannelAccount(botId);

        #region public async Task<ActionResult> Index()
        public async Task<ActionResult> Index()
        {
            // Create an Instance of the Chat object
            Chat objChat = new Chat();

            // Only call Bot if logged in
            //if (User.Identity.IsAuthenticated)
            if (true)
            {
                // Pass the message to the Bot 
                // and get the response
                objChat = await TalkToTheBot("Hello");
            }
            else
            {
                objChat.ChatResponse = "Must be logged in";
            }

            // Return response
            return View(objChat);
        }
        #endregion

        #region public async Task<ActionResult> Index(Chat model)
        [HttpPost]
        public async Task<ActionResult> Index(Chat model)
        {
            // Create an Instance of the Chat object
            Chat objChat = new Chat();

            // Only call Bot if logged in
            //if (User.Identity.IsAuthenticated)
            if (true)
            {
                // Pass the message to the Bot 
                // and get the response
                objChat = await TalkToTheBot(model.ChatMessage);
            }
            else
            {
                objChat.ChatResponse = "Must be logged in";
            }

            // Return response
            return View(objChat);
        }
        #endregion

        // Utility

        #region private async Task<Chat> TalkToTheBot(string paramMessage)
        private async Task<Chat> TalkToTheBot(string paramMessage)
        {
            // Connect to the DirectLine service
            DirectLineClient client = new DirectLineClient(directLineSecret);

            HttpContext context = this.ControllerContext.HttpContext;

            // Try to get the existing Conversation
            Conversation conversation =
                context.Session["conversation"] as Conversation;
            // Try to get an existing watermark 
            // the watermark marks the last message we received
            string watermark = context.Session["watermark"] as string;

            if (conversation == null)
            {
                // There is no existing conversation
                // start a new one
                conversation = await client.Conversations.StartConversationAsync();
            }

            // Use the text passed to the method (by the user)
            // to create a new message
            Activity message = new Activity
            {
                From = channel,
                Text = paramMessage,
                Type = ActivityTypes.Message
            };

            // Post the message to the Bot
            await client.Conversations.PostActivityAsync(conversation.ConversationId, message);

            // Get the response as a Chat object
            Chat objChat =
                await ReadBotMessagesAsync(client, conversation.ConversationId, watermark);

            // Save values
            context.Session["conversation"] = conversation;
            context.Session["watermark"] = objChat.watermark;

            // Return the response as a Chat object
            return objChat;
        }
        #endregion

        #region private async Task<Chat> ReadBotMessagesAsync(DirectLineClient client, string conversationId, string watermark)
        private async Task<Chat> ReadBotMessagesAsync(
            DirectLineClient client, string conversationId, string watermark)
        {
            // Create an Instance of the Chat object
            Chat objChat = new Chat();

            // We want to keep waiting until a message is received
            bool messageReceived = false;
            while (!messageReceived)
            {
                // Get any messages related to the conversation since the last watermark 
                var messages =
                    await client.Conversations.GetActivitiesAsync(conversationId, watermark);

                // Set the watermark to the message received
                watermark = messages?.Watermark;

                // Get all the messages 
                var messagesFromBotText = from message in messages.Activities
                                          where message.From == channel
                                          select message;

                // Loop through each message
                foreach (Activity message in messagesFromBotText)
                {
                    // We have Text
                    if (message.Text != null)
                    {
                        // Set the text response
                        // to the message text
                        objChat.ChatResponse
                            += " "
                            + message.Text.Replace("\n\n", "<br />");
                    }

                    //// We have an Image
                    //if (message.Images.Count > 0)
                    //{
                    //    // Set the text response as an HTML link
                    //    // to the image
                    //    objChat.ChatResponse
                    //        += " "
                    //        + RenderImageHTML(message.Images[0]);
                    //}
                }

                // Mark messageReceived so we can break 
                // out of the loop
                messageReceived = true;
            }

            // Set watermark on te Chat object that will be 
            // returned
            objChat.watermark = watermark;

            // Return a response as a Chat object
            return objChat;
        }
        #endregion

        #region private static string RenderImageHTML(string ImageLocation)
        private static string RenderImageHTML(string ImageLocation)
        {
            // Construct a URL to the image
            string strReturnHTML =
                String.Format(@"<img src='{0}/{1}'><br />",
                DiretlineUrl,
                ImageLocation);

            return strReturnHTML;
        }
        #endregion
    }
}