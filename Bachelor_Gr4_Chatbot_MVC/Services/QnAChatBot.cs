using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bachelor_Gr4_Chatbot_MVC.Services
{
    public class QnAChatBot : IChatBot
    {
        private HttpResponseMessage response; // Bør ligge i de klassene de brukes
        public ActivitySet activitySet; // Bør ligge i de klassene de brukes
        private IChatbotRepository _chatBotRepository;

        // TODO: Dette skulle vel kanskje hentes fra repository???
        private static string directLineConversation_V3 = "/v3/directline/conversations/";


        public QnAChatBot(IChatbotRepository chatBotRepository)
        {
            _chatBotRepository = chatBotRepository;
        }


        /// <summary>
        /// Make a connection to the chatbot and get a token that a user can use to have a conversation with the bot
        /// </summary>
        /// <returns>(String)Securitytoken to be used in conversations with the chatbot</returns>
        public async Task<String> GetConversationTokenAsString()
        {
            Models.ChatbotDetails activeBot = await _chatBotRepository.GetActiveBot();   // fetch the active bot

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
            Models.ChatbotDetails activeBot = await _chatBotRepository.GetActiveBot();   // fetch the active bot
            if (activeBot != null)
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
        /// Using the token for a specific discusion, make a post
        /// </summary>
        /// <param name="token"></param>
        /// <param name="comment"></param>
        /// <returns>(string)The response from the chatbot</returns>
        public async Task<String> PostCommentByToken(string token, string comment)
        {
            // Get HttpClient
            HttpClient httpClient = await GetHttpClient(token);
            // Get active conversation
            Conversation conversationinfo = await GetActiveConversation(token);
            // Set the conversation url
            string conversationUrl = directLineConversation_V3 + conversationinfo.ConversationId + "/activities";
            // Create activity
            Activity thisActivity = new Activity { Type = "message", Text = comment, From = new ChannelAccount { Id = "idToGoHere" } };
            var myContent = JsonConvert.SerializeObject(thisActivity);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // Post the activity
            response = await httpClient.PostAsync(conversationUrl, byteContent);

            if (response.IsSuccessStatusCode) // Yey -> It was posted
            {
                // Fetch messages
                response = await httpClient.GetAsync(conversationUrl);
                // set ActivitySet from the response
                activitySet = response.Content.ReadAsAsync(typeof(ActivitySet)).Result as ActivitySet;
                string responseString = null;
                // For each activity in activitySet, get comment.
                foreach (Activity a in activitySet.Activities)
                {
                    responseString = a.Text;
                }
                // return the last comment in activitySet
                return responseString;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create and return a HttpClient with the correct token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>(HttpClient)HttpClient with the correct security token</returns>
        public async Task<HttpClient> GetHttpClient(string token)
        {
            Models.ChatbotDetails activeBot = await _chatBotRepository.GetActiveBot();   // Fetch the active bot
            if (activeBot != null)
            {
                // Create the connection using the given token
                HttpClient client = new HttpClient();   // create httpclient
                client.BaseAddress = new Uri(activeBot.baseUrl);    // set base url address
                client.DefaultRequestHeaders.Accept.Clear();    // clear all headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contenttype
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, token); // set security bearer and token
                return client;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Takes the token for a specific chat-channel and get the conversation-details
        /// </summary>
        /// <param name="token"></param>
        /// <returns>(Conversation)Conversation for the user whom the token belongs to</returns>
        public async Task<Conversation> GetActiveConversation(string token)
        {
            Models.ChatbotDetails activeBot = await _chatBotRepository.GetActiveBot();   // Fetch the active bot
            if (activeBot != null)
            {
                // Create the connection using the given token
                HttpClient client = new HttpClient();   // create httpclient
                client.BaseAddress = new Uri(activeBot.baseUrl);    // set base url address
                client.DefaultRequestHeaders.Accept.Clear();    // clear all headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(activeBot.contentType));    // set contenttype
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(activeBot.botAutorizeTokenScheme, token); // set security bearer and token
                response = await client.PostAsync(activeBot.conversationUrlExtension, null);   // make connection and get reponse
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
    }
}

