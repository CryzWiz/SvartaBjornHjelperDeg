using Bachelor_Gr4_Chatbot_MVC.Controllers;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
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
        private readonly IChatRepository _chatRepository;
        private readonly IChatbotRepository _chatBotRepository;
        private readonly IChatBot _chatBot;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleOptions _roleOptions;

        // Chat keyword constants
        private const string Exit = "avslutt";
        private const string RouteToChatWorker = "medarbeider";

        // ChatBot user constant
        private const string ChatBot = "ChatBot";

        // Keep track of all SignalR Connections made towards the hub
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        // Keep track of all active conversations
        public readonly static ConcurrentDictionary<string, int> _activeConversations 
            = new ConcurrentDictionary<string, int>();

        // ChatWorkers status
        public enum LogInStatus
        {
            Available,
            Gone,
            Disconnected
        };

        // Wait time in queue
        private static int _waitTimeCounter = 0;
        private static TimeSpan _waitTimeSum;
        private static string _currentWaitTime;

        // Keep track of all Chat-employees connected to the hub
        // ConcurrentDictionary<UserGroup, Status>
        //public readonly static ConnectionMapping<string> _chatWorkers = new ConnectionMapping<string>();
        private static ConcurrentDictionary<string, int> _chatEmployeeStatus = new ConcurrentDictionary<string, int>();

        // Keep track of all users in queue to chat with a chat-worker
        private static ConcurrentQueue<int> _queue = new ConcurrentQueue<int>();
        private static ConcurrentDictionary<string, int> _inQueue = new ConcurrentDictionary<string, int>();

        // Move chat queue to a seperate file


        IEnumerable<ChatGroup> _allChatGroups;
       
        public ChatHub(IChatRepository chatRepository,
            IChatbotRepository chatbotRepository,
            IChatBot chatBot, 
            UserManager<ApplicationUser> userManager,
            IOptions<RoleOptions> roleOptions)
        {
            _chatRepository = chatRepository;
            _chatBotRepository = chatbotRepository;
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





            //TODO: DENNE SKAL FLYTTES
            await GetAllChatGroups();

            //await AddEmployeeToWorkGroupsBasedOnRole();
            await DisplayAllChatQueues(); // TODO: LLLLL

            // TODO: ChatHubHandler skal fjernes
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


            // Check if user has active conversations and display conversation if it exists
            if (_activeConversations.TryGetValue(key, out int conversationId))
            {
                // TODO: Testcode, replace with functional code
                Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                await Clients.Client(Context.ConnectionId).InvokeAsync("displayConversation", conversation);

            }

            //await DisplayConnectedUsers();
            // await DisplayQueueCount();
        }



        public async Task CreateQueueForEachChatGroup()
        {
            IEnumerable<ChatQueue> chatQueues = await _chatRepository.GetAllChatGroupsAsQueueAsync();
        }

        public async Task SetChatEmployeeStatus(string userGroup, int status)
        {

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
                        await SetChatEmployeeStatus(GetConnectionKey(), (int)LogInStatus.Available);
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




        /// <summary>
        /// Get key used to map connection in Single-user group and In-memory connection mapping. 
        /// </summary>
        /// <returns></returns>
        private string GetConnectionKey()
        {
            string connectionId = Context.ConnectionId;

            string connectionKey = connectionId;
            if(Context.User.Identity.IsAuthenticated)
            {
                connectionKey = Context.User.Identity.Name;
            } else
            {
                var context = Context.Connection.GetHttpContext();
                string signalRCookie = context.Request.Cookies["SignalRCookie"];
                if(signalRCookie != null)
                {
                    connectionKey = signalRCookie;
                }
            }

            return connectionKey;
            //return (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : connectionId);
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
                    displayName = await _chatRepository.GetName(Context.User.Identity.Name);
                } catch(Exception e) {
                    displayName = "Chat-medarbeider";
                }
            }
            return displayName;
        }

        /// <summary>
        /// Start conversation with ChatBot and display ChatBot "Hello" message. 
        /// </summary>
        /// <returns></returns>
        public async Task StartConversationWithChatBot()
        {
            string userGroup = GetConnectionKey();
            var knowledgebase = await _chatBotRepository.GetActiveQnAKnowledgeBaseAsync();
            var qnabase = await _chatBotRepository.GetActiveQnABaseClassAsync();

            Conversation conversation = new Conversation
            {
                UserGroup1 = userGroup,
                IsChatBot = true,
                StartTime = DateTime.Now,
                KnowledgebaseId = knowledgebase.QnAKnowledgeBaseId,
                QnABaseId = qnabase.QnAId
            };

            try
            {
                int conversationId = await _chatRepository.AddConversationAsync(conversation);
                conversation.ConversationId = conversationId;
            } catch
            {
                await DisplayChatBotConnectionError(userGroup);
                return;
            }

            await SetConversationId(userGroup, conversation.ConversationId);
            await DisplayMessage(userGroup, ChatBot, GetStandardChatBotHello());
        }


        /// <summary>
        /// Add conversation to list over all active conversations. 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="conversationId"></param>
        public void AddConversationToActiveConversations(string connectionKey, int conversationId)
        {
            _activeConversations.TryAdd(connectionKey, conversationId);
        }

        /// <summary>
        /// Remove conversation from list over all active conversations. 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="conversationId"></param>
        public void RemoveConversationFromActiveConversations(string connectionKey)
        {
            _activeConversations.TryRemove(connectionKey, out int conversationId);
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
            RemoveConversationFromActiveConversations(user);

            await Clients.Group(conversation.UserGroup1).InvokeAsync("conversationEnded", message, conversationId);
            await Clients.Group(conversation.UserGroup2).InvokeAsync("conversationEnded", message, conversationId);
        }

        public async Task<Conversation> SaveConversationWithResultSetToTrue(int conversationId)
        {
            try
            {
                Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                conversation.EndTime = DateTime.Now;
                conversation.Result = true; // Standard result set to true, but user can modify
                await _chatRepository.UpdateConversationAsync(conversation);
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
            //await DisplayConnectedUsers();

        }

        public string GetStandardChatBotHello()
        {
            // TODO: Denne skal hentes fra et annet sted, kun testkode
            return "Hei, mitt navn er Svarta Bjørn, hva kan jeg hjelpe deg med?";
        }

        public async Task RegisterConversationResult(int conversationId, bool result)
        {
            Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
            conversation.Result = result;
            await _chatRepository.UpdateConversationAsync(conversation);
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
                int conversationId = await _chatRepository.AddConversationAsync(conversation);
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

        public async Task JoinSpecificQueue(ChatQueue queue)
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
                int conversationId = await _chatRepository.AddConversationAsync(conversation);

                int? placeInQueue = queue.Enqueue(conversationId, userGroup);
                if(placeInQueue != null)
                {
                    await DisplayQueueCount();
                }
                await Clients.Group(userGroup).InvokeAsync("displayPlaceInQueue", placeInQueue);
                await SetConversationId(userGroup, conversationId);
            }
            catch (Exception exception)
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

        public async Task PickFromSpecificQueue(string queueId)
        {

            ChatQueue queue = new ChatQueue(); // TODO: DENNE MÅ HENTES FRA ET ANNET STED
            string chatWorkerId = GetConnectionKey();
            int? conversationId = queue.Dequeue();
            if (conversationId != null)
            {
                string displayName = await GetDisplayName();

                // TODO: Hent fra databasen:
                string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);

                try
                {
                    Conversation conversation = await _chatRepository.GetConversationByIdAsync((int)conversationId);
                    queue.AddWaitTime(DateTime.Now - conversation.StartTime);
  
                    await DisplayWaitTime();
                    conversation.UserGroup2 = chatWorkerId;
                    await _chatRepository.UpdateConversationAsync(conversation);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, conversation.UserGroup1);
                    await SetGroupId(conversation.UserGroup1, chatWorkerId);

                    _inQueue.Remove(conversation.UserGroup1, out int value);
                    ChatQueue.RemoveFromFullQueue(conversation.UserGroup1, (int)conversationId);
                    await SetConversationId(chatWorkerId, conversationId);
                    await DisplayMessage(conversation.UserGroup1, chatWorkerId, message);
                    await DisplayQueueCount();

                }
                catch (Exception e)
                {
                    await DisplayErrorMessageInChat(chatWorkerId, "Feil under henting fra kø. ");
                }

            }
            else
            {
                await DisplayErrorMessageInChat(chatWorkerId, "Køen er tom ");
            }
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
                    Conversation conversation = await _chatRepository.GetConversationByIdAsync((int)conversationId);

                    AddConversationToActiveConversations(conversation.UserGroup1, (int)conversationId);

                    TimeSpan thisWaitTime = DateTime.Now - conversation.StartTime;
                    _waitTimeSum += thisWaitTime;
                    _waitTimeCounter += 1;

                    await DisplayWaitTime();
                    conversation.UserGroup2 = chatWorkerId;
                    await _chatRepository.UpdateConversationAsync(conversation);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, conversation.UserGroup1);
                    await SetGroupId(conversation.UserGroup1, chatWorkerId);

                    _inQueue.Remove(conversation.UserGroup1, out int value);
                    await SetConversationId(chatWorkerId, conversationId);
                    await DisplayMessage(conversation.UserGroup1, chatWorkerId, message);
                    await Clients.Group(conversation.UserGroup1).InvokeAsync("enableInputField", "hei");
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

        private async Task DisplayWaitTime()
        {
            int avrageWaitTime = (int)_waitTimeSum.TotalSeconds / _waitTimeCounter;
            TimeSpan span = new TimeSpan(0, 0, avrageWaitTime);
            _currentWaitTime = String.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
            await Clients.All.InvokeAsync("displayWaitTime", _currentWaitTime);
        }

        public IEnumerable<int> GetQueue()
        {
            return _queue.ToArray();
        }




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
                    await _chatRepository.AddMessageAsync(msg);
                    
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

                        string response = await _chatBotRepository.PostToActiveKnowledgeBase(message);

                        Message responseMsg = new Message
                        {
                            ConversationId = id,
                            From = ChatBot,
                            DisplayName = ChatBot,
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
                // Save messages
                await _chatRepository.AddMessagesAsync(messages);
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
            await SetChatEmployeeStatus(GetConnectionKey(), (int)LogInStatus.Available);
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("updateEmployeeList");
        }

        private async Task LogOut()
        {
            await SetChatEmployeeStatus(GetConnectionKey(), (int)LogInStatus.Disconnected);
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("updateEmployeeList");
        }

        private async Task SetStatusToGone()
        {
            await SetChatEmployeeStatus(GetConnectionKey(), (int)LogInStatus.Gone);
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("updateEmployeeList");
        }
        
        private async Task GetAllChatGroups()
        {
            IEnumerable<ChatGroup> chatGroups = await _chatRepository.GetAllChatGroupsAsync();
            _allChatGroups = chatGroups;
        }

        public async Task DisplayAllChatQueues()
        {
            IEnumerable<ChatQueue> queues = await _chatRepository.GetAllChatGroupsAsQueueAsync();
            await Clients.All.InvokeAsync("displayAllChatQueues", queues);
        }

        public async Task Test()
        {
            await Clients.All.InvokeAsync("test2", "test melding fra chathub");
        }














        // UNUSED CODE -- TO BE DELETED: 


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
            };
            string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

            await DisplayMessage(groupName, from, message);
        }*/

    }
}
