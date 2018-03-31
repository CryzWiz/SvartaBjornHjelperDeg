using Bachelor_Gr4_Chatbot_MVC.Controllers;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Hubs
{

    public static class ChatHubHandler
    {
        public static HashSet<string> ConnectedUsers = new HashSet<string>();
        public static HashSet<string> ConnectedWorkers = new HashSet<string>();
        public static int inQue = 0;
    }



    /// Referanser: 
    /// https://code.msdn.microsoft.com/ASPNET-CORE-20-uses-7a771742
    /// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
    public class ChatHub : Hub
    {
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        private static ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        //private static ConcurrentDictionary<string, string> _chatWorkerStatus = new ConcurrentDictionary<string, string>();

        private IChatRepository _repository;
        private IChatBot _chatBot;

        public ChatHub(IChatRepository repository, IChatBot chatBot)
        {
            _repository = repository;
            _chatBot = chatBot;
        }

        public override async Task OnConnectedAsync()
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string connectionId = Context.ConnectionId;
            string key = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name: connectionId);
            _connections.Add(key, connectionId);

            // TODO: 
            if (key.Equals(connectionId))
            {
                ChatHubHandler.ConnectedUsers.Add(key);
            }
            else
            {
                ChatHubHandler.ConnectedWorkers.Add(key);
            }

            // Add to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);
            
            int conversationId = await ConnectWithChatBot(key);
            await Clients.Group(key).InvokeAsync("setConversationId", conversationId);

            await DisplayConnectedUsers();
         }

        public async Task<int> ConnectWithChatBot(string userGroup)
        {
            string conversationToken = await _chatBot.GetConversationTokenAsString();

            // Create conversation 
            Conversation conversation = new Conversation
            {
                ConversationToken = conversationToken,
                UserGroup1 = userGroup,
                IsChatBot = true,
                StartTime = DateTime.Now
            };

            // Save conversation
            int conversationId = await _repository.AddConversationAsync(conversation);
            return conversationId;
        }

        public async Task StartConversationWithChatBot()
        {
            Microsoft.Bot.Connector.DirectLine.Conversation conversation = await _chatBot.StartAndGetNewConversation();
            string test = "test";
        }

       

        private async Task SetChatWorkerStatus(string userName, string status)
        {
            //_chatWorkerStatus.AddOrUpdate(userName, status);
        }

        private async Task DisconnectChatWorker(string userName)
        {
            
        }
        

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string connectionId = Context.ConnectionId;
            string key = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : connectionId);
            _connections.Remove(key, Context.ConnectionId);
            if (key.Equals(connectionId))
            {
                ChatHubHandler.ConnectedUsers.Remove(key);
            }
            else
            {
                ChatHubHandler.ConnectedWorkers.Remove(key);
            }
            // TODO: Gjør endringer her!
            await DisplayConnectedUsers(); 
            await DisplayQueue();
        }

        

        public async Task JoinQueue()
        {
            /* Create conversation
            Conversation conversation = new Conversation
            {
                IsChatBot = false,
                StartTime = DateTime.Now
            };*/


            _queue.Enqueue(Context.ConnectionId);
            int placeInQueue = _queue.Count();
            ChatHubHandler.inQue += 1;
            await DisplayQueue();
            await Clients.Client(Context.ConnectionId).InvokeAsync("displayQueueNumber", placeInQueue);
        }

        public async Task PickFromQueue()
        {
            // Be sure queue is not empty
            if(!_queue.IsEmpty)
            {
                if(_queue.TryDequeue(out string groupId))
                {
                    // TODO: Skal ikke hardkodes men hentes fra databasen
                    string displayName = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "Kundesenter");
                    //var r = await _repository.PostQuery("hva er du");
                    //string message = r;
                    string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);
                    string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

                    await Clients.Group(from).InvokeAsync("pickFromQueue", groupId);
                    await DisplayMessage(groupId, from, message);
                    ChatHubHandler.inQue -= 1;
                    await DisplayQueue();
                } else
                {
                    // TODO: SLETTES
                    string message = String.Format("TryDeque failed");
                    string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

                    await DisplayMessage(from, from, message);
                }
            } else
            {
                // Queue is empty
                // TODO: SLETTES
                string message = String.Format("Køen er tom");
                string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

                await DisplayMessage(from, from, message);
            }            
        }

        public IEnumerable<string> GetQueue()
        {
            return _queue.ToArray();
        }

        /// <summary>
        /// Display a list of all connected users
        /// </summary>
        public async Task DisplayConnectedUsers()
        {
            // Display all current users
            // TODO: TESTCODE: This needs to be updated to only be shown inside Chat-workers sites
            IEnumerable<string> keys = _connections.GetConnectionKeys();
            await Clients.All.InvokeAsync("displayConnections", keys);
            
        }

        public async Task JoinChat(string groupName) {
            // TODO: Skal ikke hardkodes men hentes fra databasen: Denne meldingen hører hjemme i pickfromque, ikke joinchat
            string displayName = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "Kundesenter");
            string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);


            /*Message msg = new Message
            {
                Content = "test",
                From = "fra meg"
            };*/
            string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

            await DisplayMessage(groupName, from, message);
        }

        public async Task DisplayQueue()
        {
            IEnumerable<string> queue = GetQueue();
            await Clients.All.InvokeAsync("displayQueue", queue);
        }

        /// <summary>
        /// Send a message to a specified single-user group.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="message">Message content</param>
        public async Task SendToGroup(string groupName, string message, string conversationId)
        {
            string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

            //if (Int32.TryParse(conversationId, out int id))
            //{
                // Get DisplayName
                string displayName = "Gjest";
                if(Context.User.Identity.IsAuthenticated)
                {
                    displayName = await _repository.GetName(Context.User.Identity.Name);
                }
                


                Message msg = new Message
                {
                    //ConversationId = id,
                    From = from,
                    To = groupName,
                    DisplayName = displayName,
                    DateTime = DateTime.Now,
                    Content = message
                };
                /*try
                {
                    await _repository.AddMessageAsync(msg);*/
                    await DisplayMessage2(msg);
                /*} catch(Exception exception)
                {
                    await Clients.Group(from).InvokeAsync("sendMessage", "Sending av melding feilet.");
                }*/
                
            /*} else
            {
                await Clients.Group(from).InvokeAsync("sendMessage", "Sending av melding feilet, du er ikke koblet på noen chat");
            }*/
            
        }

        /*public async Task SendToGroup2(string groupName, bool talkWithChatBot, string message)
        {
            if(talkWithChatBot)
            {
                QnAIndexViewModel vm = new QnAIndexViewModel
                {
                    query = message
                };
                // TODO: ChathubController settings
                string baseAdress = "https://localhost:44365/";
                string requestUri = "api/ChatBot";

                // Call ChathubController
                using (HttpClient client = new HttpClient()) {
                    client.BaseAddress = new Uri(baseAdress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(_contentTypeJson));

                    var contentAsJson = JsonConvert.SerializeObject(vm);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(contentAsJson);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue(_contentTypeJson);

                    HttpResponseMessage response = await client.Post(requestUri, byteContent);

                    if(response.IsSuccessStatusCode)
                    {
                        await SendToGroup(groupName, response.ToString());
                    }
                    await SendToGroup(groupName, response.StatusCode.ToString());

                }

            }
            await SendToGroup(groupName, message);

        }*/
        public async Task DisplayMessage(string groupTo, string groupFrom, string message)
        {
            await Clients.Group(groupTo).InvokeAsync("receiveMessage", groupFrom, message);
            await Clients.Group(groupFrom).InvokeAsync("sendMessage", message);
        }

        public async Task DisplayMessage2(Message message)
        {
            //await Clients.Group(message.To).InvokeAsync("message", message);
            await Clients.Group(message.To).InvokeAsync("receiveMessage", message.From, message.Content);
            await Clients.Group(message.From).InvokeAsync("sendMessage", message.Content);
        }
    }
}
