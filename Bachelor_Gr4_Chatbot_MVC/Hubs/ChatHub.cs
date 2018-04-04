using Bachelor_Gr4_Chatbot_MVC.Controllers;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

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
        private readonly IChatRepository _repository;
        private readonly IChatBot _chatBot;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleOptions _roleOptions;

        // Chat keyword constants
        private const string Exit = "avslutt";

        // ChatBot user constant
        private const string ChatBot = "ChatBot";

        // Keep track of all SignalR Connections made towards the hub
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        // ChatWorkers status
        public enum LogInStatus
        {
            Available,
            Gone,
            Disconnected
        };

        // Keep track of all Chat-employees connected to the hub
        // ConcurrentDictionary<UserGroup, Status>
        //public readonly static ConnectionMapping<string> _chatWorkers = new ConnectionMapping<string>();
        private static ConcurrentDictionary<string, int> _chatEmployeeStatus = new ConcurrentDictionary<string, int>();

        // Keep track of all users in queue to chat with a chat-worker
        private static ConcurrentQueue<int> _queue = new ConcurrentQueue<int>();
        private static ConcurrentDictionary<string, int> _inQueue = new ConcurrentDictionary<string, int>();



       
        public ChatHub(IChatRepository repository, 
            IChatBot chatBot, 
            UserManager<ApplicationUser> userManager,
            IOptions<RoleOptions> roleOptions)
        {
            _repository = repository;
            _chatBot = chatBot;
            _userManager = userManager;
            _roleOptions = roleOptions.Value;
        }

        public override async Task OnConnectedAsync()
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string connectionId = Context.ConnectionId;
            string key = GetConnectionKey();
            _connections.Add(key, connectionId);

            //var test = Context.Request.Cookies["ASP.NET_SessionId"].Value;
            await AddEmployeeToWorkGroupsBasedOnRole();


            // TODO: 
            if (key.Equals(connectionId))
            {
                ChatHubHandler.ConnectedUsers.Add(key);
            }
            else
            {
                ChatHubHandler.ConnectedWorkers.Add(key);
            }

            // Add Chat-workers to list
            //if(Context.User.Identity.Name.)

            // Add to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);
            
            await DisplayConnectedUsers();
           // await DisplayQueueCount();
        }

        public async Task AddEmployeeToWorkGroupsBasedOnRole()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(Context.User.Identity.Name);

                    // Chat employee groups
                    if (await _userManager.IsInRoleAsync(user, _roleOptions.ChatEmployeeRole))
                    {
                        await Groups.AddAsync(Context.ConnectionId, _roleOptions.ChatEmployeeRole);
                    }

                    // Administrator groups
                    if (await _userManager.IsInRoleAsync(user, _roleOptions.AdminRole))
                    {
                        await Groups.AddAsync(Context.ConnectionId, _roleOptions.AdminRole);
                    }
                }
                catch (Exception e)
                {
 
                }

            }
        }


        public async Task<Conversation> ConnectWithChatBot(string userGroup)
        {
            try
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

                conversation.ConversationId = conversationId;
                return conversation;
            } catch(Exception exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get key used to map connection in Single-user group and In-memory connection mapping. 
        /// </summary>
        /// <returns></returns>
        private string GetConnectionKey()
        {
            string connectionId = Context.ConnectionId;
            return (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : connectionId);
        }

        /// <summary>
        /// Get name to display with messages. 
        /// If user is logged in it will return first name. 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDisplayName()
        {
            string displayName = "Gjest";
            if (Context.User.Identity.IsAuthenticated)
            {
                try
                {
                    displayName = await _repository.GetName(Context.User.Identity.Name);
                } catch(Exception e) {
                    displayName = "Chat-medarbeider";
                }
            }
            return displayName;
        }

        /// <summary>
        /// Connects user with chatbot and diplays standard ChatBot "Hello" message. 
        /// </summary>
        /// <returns></returns>
        public async Task StartConversationWithChatBot()
        {
            string connectionId = Context.ConnectionId;
            string userGroup = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : connectionId);
            Conversation conversation = await ConnectWithChatBot(userGroup);

            if(conversation == null)
            {
                await DisplayChatBotConnectionError(userGroup);
                return;
            }
            await SetChatBotToken(userGroup, conversation.ConversationToken);
            await SetConversationId(userGroup, conversation.ConversationId);
            // TODO: GetStandardChatBotHello should be changed
            await DisplayMessage(userGroup, ChatBot, GetStandardChatBotHello());
        }

        public async Task EndConversationWithChatBot(int conversationId)
        {
            try
            {
                await SaveConversationWithResultSetToTrue(conversationId);
                // TODO: Trengs det "stenges" noe i ChatBot??
            }
            catch (Exception exception)
            {
                // TODO
            }
            string userGroup = GetConnectionKey();
            await Clients.Group(userGroup).InvokeAsync("endBotConversation", "Samtale med ChatBot avsluttet.", conversationId);
        }

        public async Task EndConversation(int conversationId, string groupTo)
        {
            Conversation conversation = null;
            try
            {
                conversation = await SaveConversationWithResultSetToTrue(conversationId);
            } catch (Exception e)
            {
                // TODO
            }
            string user = GetConnectionKey();
            string displayName = await GetDisplayName();
            string message = displayName + " forlot samtalen. ";
            await Clients.Group(conversation.UserGroup1).InvokeAsync("conversationEnded", message, conversationId);
            await Clients.Group(conversation.UserGroup2).InvokeAsync("conversationEnded", message, conversationId);
        }

        public async Task<Conversation> SaveConversationWithResultSetToTrue(int conversationId)
        {
            try
            {
                Conversation conversation = await _repository.GetConversationByIdAsync(conversationId);
                conversation.EndTime = DateTime.Now;
                conversation.Result = true; // Standard result set to true, but user can modify
                await _repository.UpdateConversationAsync(conversation);
                return conversation;
            } catch(Exception e)
            {
                throw;
            }
        }

        private async Task DisplayErrorMessageInChat(string userGroup, string message)
        {
            await Clients.Group(userGroup).InvokeAsync("errorMessage", message);
        }

        private async Task DisplayChatBotConnectionError(string userGroup)
        {
            await DisplayErrorMessageInChat(userGroup, "Klarer ikke opprette tilkobling til ChatBot." +
                "<button id='startChat' class='btn btn-success btn-block'> Start Chat</button>");
        }

        /*
        private async Task SetChatWorkerStatus(string userName, string status)
        {
            //_chatWorkerStatus.AddOrUpdate(userName, status);
        }

        private async Task DisconnectChatWorker(string userName)
        {
            
        }
        */

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string key = GetConnectionKey();
            _connections.Remove(key, Context.ConnectionId);
            if (key.Equals(Context.ConnectionId))
            {
                ChatHubHandler.ConnectedUsers.Remove(key);
            }
            else
            {
                ChatHubHandler.ConnectedWorkers.Remove(key);
            }
            // TODO: Gjør endringer her!
            if(_inQueue.Remove(key, out int value))
            {
                await DisplayQueueCount();
            }
            await DisplayConnectedUsers();

        }

        public string GetStandardChatBotHello()
        {
            // TODO: Denne skal hentes fra et annet sted, kun testkode
            return "Hei, mitt navn er Svarta Bjørn, hva kan jeg hjelpe deg med?";
        }

        public async Task RegisterConversationResult(int conversationId, bool result)
        {
            Conversation conversation = await _repository.GetConversationByIdAsync(conversationId);
            conversation.Result = result;
            await _repository.UpdateConversationAsync(conversation);
        }

             
        public async Task JoinQueue()
        {
            string userGroup = GetConnectionKey();
            //Create conversation
            Conversation conversation = new Conversation
            {
                IsChatBot = false,
                StartTime = DateTime.Now,
                UserGroup1 = userGroup
            };

            try
            {
                int conversationId = await _repository.AddConversationAsync(conversation);
                //_queue.Enqueue(userGroup);
                _queue.Enqueue(conversationId);

                ChatHubHandler.inQue += 1; // TODO: ChatHubHandler -------------------------------------
                
               if(_inQueue.TryAdd(userGroup, conversationId))
               {
                    await DisplayQueueCount();
               }



                int placeInQueue = _inQueue.Count();
                await Clients.Group(userGroup).InvokeAsync("displayPlaceInQueue", placeInQueue);
                await SetConversationId(userGroup, conversationId);

            } catch (Exception exception)
            {
                await DisplayErrorMessageInChat(userGroup, "Feil under tilkobling av chat. "); // TODO: Error messages...
            }
        }


        public async Task<bool> MessageIsKeyword(string message, int conversationId)
        {
            string msg = message.ToLower();

            if(msg.Equals(Exit))
            {
                await EndConversationWithChatBot(conversationId);
                return true;
            }
            return false;
        }

        private int? Dequeue()
        {
            while (!_queue.IsEmpty)
            {
                if(_queue.TryDequeue(out int conversationId))
                {
                    if(_inQueue.Values.Contains(conversationId))
                    {
                        return conversationId;
                    }
                }

            }
            return null;
        }

        public async Task PickFromQueue()
        {
            string chatWorkerId = GetConnectionKey();
            // Be sure queue is not empty
            int? conversationId = Dequeue();
            if(conversationId != null)
            {
                string displayName = await GetDisplayName();
                string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);

                try
                {
                    Conversation conversation = await _repository.GetConversationByIdAsync((int)conversationId);
                    conversation.UserGroup2 = chatWorkerId;
                    await _repository.UpdateConversationAsync(conversation);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, conversation.UserGroup1);
                    await SetGroupId(conversation.UserGroup1, chatWorkerId);

                    _inQueue.Remove(conversation.UserGroup1, out int value);
                    await SetConversationId(chatWorkerId, conversationId);
                    await DisplayMessage(conversation.UserGroup1, chatWorkerId, message);
                    await DisplayQueueCount();

                } catch (Exception e)
                {
                    await DisplayErrorMessageInChat(chatWorkerId, "Feil under henting fra kø. ");
                }

            } else
            {
                await DisplayErrorMessageInChat(chatWorkerId, "Køen er tom ");
            }
            /*if(!_queue.IsEmpty)
            {
                if(_queue.TryDequeue(out string userId))
                {
                    
                    string displayName = await GetDisplayName();
                    string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, userId);
                    await SetGroupId(userId, chatWorkerId);

                    await DisplayMessage(userId, chatWorkerId, message);
                    await DisplayQueue();
                    ChatHubHandler.inQue -= 1;// TODO: ChatHubHandler -------------------------------------

                } else
                {
                    await DisplayErrorMessageInChat(chatWorkerId, "Feil under henting fra kø. ");
                }
            } else
            {
                await DisplayErrorMessageInChat(chatWorkerId, "Køen er tom.");
            }      */
            
        }

        public IEnumerable<int> GetQueue()
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

        /*public async Task JoinChat(string groupName) {
            // TODO: Skal ikke hardkodes men hentes fra databasen: Denne meldingen hører hjemme i pickfromque, ikke joinchat
            string displayName = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "Kundesenter");
            string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);


            /*Message msg = new Message
            {
                Content = "test",
                From = "fra meg"
            };
            string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

            await DisplayMessage(groupName, from, message);
        }*/



        /// <summary>
        /// Send a message to a specified single-user group.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="message">Message content</param>
        public async Task SendToGroup(string groupName, string message, string conversationId)
        {
            string from = GetConnectionKey();

            if (Int32.TryParse(conversationId, out int id))
            {

                string displayName = await GetDisplayName();

                Message msg = new Message
                {
                    ConversationId = id,
                    From = from,
                    To = groupName,
                    DisplayName = displayName,
                    DateTime = DateTime.Now,
                    Content = message,
                    IsChatBot = false
                };
                await DisplayMessage2(msg);

                try
                {
                    await _repository.AddMessageAsync(msg);
                    
                } catch (Exception e)
                {
                    // TODO
                }
                
            } else
            {
                await DisplayErrorMessageInChat(from, "Sending av melding feilet, du er ikke koblet på noen chat");
            }           
        }

        public async Task SendToChatBot(string conversationId, string conversationToken, string message)
        {
            List<Message> messages = new List<Message>();
            string from = GetConnectionKey();
            await DisplayMessage(ChatBot, from, message);

            if(Int32.TryParse(conversationId, out int id))
            {
                if (! await MessageIsKeyword(message, id))
                {
                   
                    Message msg = new Message
                    {
                        ConversationId = id,
                        To = ChatBot,
                        From = from,
                        DisplayName = await GetDisplayName(),
                        Content = message,
                        IsChatBot = false,
                        IsChatWorker = false,
                        DateTime = DateTime.Now
                    };

                    messages.Add(msg);

                    try
                    {

                        string response = await _chatBot.PostCommentByToken(conversationToken, message);

                        Message responseMsg = new Message
                        {
                            ConversationId = id,
                            From = ChatBot,
                            DisplayName = "ChatBot",
                            Content = response,
                            IsChatBot = true,
                            IsChatWorker = false,
                            DateTime = DateTime.Now
                        };

                        messages.Add(responseMsg);

                        // Display response message
                        await DisplayMessage(from, ChatBot, response);

                    }
                    catch (Exception exception)
                    {
                        await DisplayChatBotConnectionError(from);
                    }
                }
            } else
            {
                await DisplayChatBotConnectionError(from);
            }

            try
            {
                await _repository.AddMessagesAsync(messages);
            } catch (Exception exception)
            {
                // ChatBot should continue to work even if messages does not get stored to db. 
                // So do nothing here.
            }
        }

        public async Task DisplayMessage(string groupTo, string groupFrom, string message)
        {
            if(!groupFrom.Equals(ChatBot))
            {
                await Clients.Group(groupFrom).InvokeAsync("sendMessage", message);
            }

            if(!groupTo.Equals(ChatBot))
            {
                await Clients.Group(groupTo).InvokeAsync("receiveMessage", groupFrom, message);
            }




        }

        public async Task DisplayMessage2(Message message)
        {
            //await Clients.Group(message.To).InvokeAsync("message", message);
            await Clients.Group(message.To).InvokeAsync("receiveMessage", message.From, message.Content);
            await Clients.Group(message.From).InvokeAsync("sendMessage", message.Content);
        }
       /* public async Task DisplayQueue()
        {
            IEnumerable<int> queue = GetQueue();
            await Clients.All.InvokeAsync("displayQueue", queue);
        }*/

        public async Task DisplayQueueCount()
        {
            await Clients.All.InvokeAsync("displayQueueCount", _inQueue.Count);
        }
        public async Task SetChatBotToken(string userGroup, string token)
        {
            await Clients.Group(userGroup).InvokeAsync("setChatBotToken", token);
        }
        private async Task SetConversationId(string userGroup, int? conversationId)
        {
            await Clients.Group(userGroup).InvokeAsync("setConversationId", conversationId);
        }

        private async Task SetGroupId(string toUser, string groupId)
        {
            await Clients.Group(toUser).InvokeAsync("setGroupId", groupId);
        }

        private async Task LogIn()
        {

        }

        /*
        // This method is copied from: 
        // https://stackoverflow.com/questions/5177755/how-to-get-get-a-c-sharp-enumeration-in-javascript
        private void ExportEnum<T>()
        {
            var type = typeof(T);
            var values = Enum.GetValues(type).Cast<T>();
            var dictionary = values.ToDictionary(x => x.ToString(), x => Convert.ToInt32(x));
            var json = new JavaScriptSerializer().Serialize(dictionary);
            var script = string.Format("{0}={1};", type.Name, json);
            //System.Web.UI.ScriptManager.RegistertStartupScripts(this, GetType(), "")

        }
        */
    }
}
