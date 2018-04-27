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
        private readonly ChatbotKeywordOptions _keywordOptions;

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

        /*
        // Wait time in queue
        private static int _waitTimeCounter = 0;
        private static TimeSpan _waitTimeSum;
        private static string _currentWaitTime;
        */

        // Keep track of all Chat-employees connected to the hub
        // ConcurrentDictionary<UserGroup, Status>
        //public readonly static ConnectionMapping<string> _chatWorkers = new ConnectionMapping<string>();
        private static ConcurrentDictionary<string, int> _chatEmployeeStatus = new ConcurrentDictionary<string, int>();

        /*
        // Keep track of all users in queue to chat with a chat-worker
        private static ConcurrentQueue<int> _queue = new ConcurrentQueue<int>();
        private static ConcurrentDictionary<string, int> _inQueue = new ConcurrentDictionary<string, int>();
        */

        //private static ChatQueue _fullChatQueue = new ChatQueue();

        IEnumerable<ChatGroup> _allChatGroups;
       
        public ChatHub(IChatRepository chatRepository,
            IChatbotRepository chatbotRepository,
            IChatBot chatBot, 
            UserManager<ApplicationUser> userManager,
            IOptions<RoleOptions> roleOptions,
            IOptions<ChatbotKeywordOptions> keywordOptions)
        {
            _chatRepository = chatRepository;
            _chatBotRepository = chatbotRepository;
            _chatBot = chatBot;
            _userManager = userManager;
            _roleOptions = roleOptions.Value;
            _keywordOptions = keywordOptions.Value;
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

            // Add user to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);

            // Add Employee to necessary groups

            if (Context.User.Identity.IsAuthenticated)
            {
                await AddEmployeeToWorkGroupsBasedOnRole();
                await AddEmployeeToCustomChatGroups();
                await DisplayUsersChatQueues();

                //TODO: DENNE SKAL FLYTTES
                //await GetAllChatGroups();
                //await DisplayAllChatQueues();
            }
            
            // TODO: Map chat workers status

            // TODO: When logged in, display chat workers queues

            // Add Chat-workers to list
            //if(Context.User.Identity.Name.)
            
            // Check if user has active conversations and display conversation if it exists
            if (_activeConversations.TryGetValue(key, out int conversationId))
            {
                // TODO: Testcode, replace with functional code
                Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                await Clients.Client(Context.ConnectionId).InvokeAsync("displayConversation", conversation);

            }

            //await DisplayConnectedUsers();
            // await DisplayQueueCount();

            await DisplayChatNumbersForAdmin();
        }

        /// <summary>
        /// Display for Admin how many users and chat workers are connected to the chat. 
        /// </summary>
        public async Task DisplayChatNumbersForAdmin()
        {
            string adminGroup = _roleOptions.AdminRole;

            // TODO: Update numbers to be correct, this is wrong!!!!
            await Clients.Group(adminGroup).InvokeAsync("numberOfChatWorkersConnected", 5);
            await Clients.Group(adminGroup).InvokeAsync("numberOfClientsConnected", 4);
            await Clients.Group(adminGroup).InvokeAsync("numberInQueue", 3);
        }



        public async Task CreateQueueForEachChatGroup()
        {
            IEnumerable<ChatQueue> chatQueues = await _chatRepository.GetAllChatGroupsAsQueueAsync();
        }

        public async Task SetChatEmployeeStatus(string userGroup, int status)
        {

        }


        /// <summary>
        /// Add employee to necessary groups based on their role. 
        /// There are defined groups for Admin and ChatWorker. 
        /// </summary>
        private async Task AddEmployeeToWorkGroupsBasedOnRole()
        {
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(Context.User.Identity.Name);

                // Chat employee groups
                if (await _userManager.IsInRoleAsync(user, _roleOptions.ChatEmployeeRole))
                {
                    await Groups.AddAsync(Context.ConnectionId, _roleOptions.ChatEmployeeRole);
                    // TODO: SetChatEmployeeStatus
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

        /// <summary>
        /// Add employee to groups based on their group membership. 
        /// </summary>
        private async Task AddEmployeeToCustomChatGroups()
        {
            var groups = await _chatRepository.GetUsersChatGroups(Context.User.Identity.Name);
            foreach(string group in groups)
            {
                await Groups.AddAsync(Context.ConnectionId, group);
            }
        }
        
        /// <summary>
        /// Get key used to map connection in Single-user group and In-memory connection mapping. 
        /// </summary>
        /// <returns>A key used to map the users connections</returns>
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
                    displayName = "Kundesenter";
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
            } catch (Exception e)
            {
                string msg = e.Message;
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

        public async Task EndConversation(int conversationId, string groupTo)
        {
            Conversation conversation = null;
            try
            {
                conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                conversation.EndTime = DateTime.Now;
                conversation.Result = true;
                await _chatRepository.UpdateConversationAsync(conversation);
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

       /* public async Task<Conversation> SaveConversationWithGivenResult(int conversationId, bool result)
        {
            try
            {

                return conversation;
            } catch(Exception e)
            {
                throw;
            }
        }*/

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

            // TODO: DISPLAY NUMBER OF USERS CONNECTED ETC; IN QUEUE FOR DASHBORD

            // TODO: Gjør endringer her!
            if(ChatQueue.RemoveFromFullQueue(key))
            {
                await DisplayQueueCount();
            }
            //await DisplayConnectedUsers();
            await DisplayChatNumbersForAdmin();
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
        

        public async Task JoinQueue(Conversation conversation)
        {
            try
            {
                int? placeInQueue = ChatQueue.Enqueue(conversation.ConversationId, conversation.UserGroup1);
                if(placeInQueue != null)
                {
                    string chatGroupsString = "IT, Bygg"; // TODO: FJERN HARDKODING, hent fra liste
                    string message = String.Format("Du er nå lagt i kø, en medarbeider vil svare deg så raskt som mulig. Din plass i køen er: {0}." +
                        "<br /> Gjelder din henvendelse et av følgende tema: {1}?" +
                        "<br /> Skriv inn ønsket tema så havner du til rett kundebehandler og vi kan behandle spørsmålet ditt raskerer." +
                        "<br /> Dersom ingen tema passer kan du vente på svar så vil første ledige kundebehandler ta kontakt.", placeInQueue, chatGroupsString);
                    await Clients.Group(conversation.UserGroup1).InvokeAsync("displayPlaceInQueue", message);
                    //await Clients.Group(conversation.UserGroup1).InvokeAsync("setChatGroupsQuestionBool", true);
                    await SetConversationId(conversation.UserGroup1, conversation.ConversationId);
                }
                /*_queue.Enqueue(conversation.ConversationId);
                if (_inQueue.TryAdd(conversation.UserGroup1, conversation.ConversationId))
                {
                    await DisplayQueueCount();
                }

                int placeInQueue = _inQueue.Count();
                await Clients.Group(conversation.UserGroup1).InvokeAsync("displayPlaceInQueue", placeInQueue);
                await SetConversationId(conversation.UserGroup1, conversation.ConversationId);*/
            }
            catch (Exception exception)
            {
                await DisplayErrorMessageInChat(conversation.UserGroup1, "Feil under tilkobling av chat. "); // TODO: Error messages...
            }

        }

        /// <summary>
        /// Method invoked from client to add to specific chat-queue. 
        /// Check if given ChatQueue name exists and call JoinSpecificQueue
        /// to add user to the correct queue. 
        /// </summary>
        /// <param name="queueName"></param>
        public async Task JoinSpecificChatQueue(string queueName)
        {
            string name = queueName.ToLower();
            
            try
            {
                // For queue in queues, check if match
                IEnumerable<ChatQueue> queues = await _chatRepository.GetAllChatGroupsAsQueueAsync();
                ChatQueue joinQueue = null;
                foreach(ChatQueue queue in queues)
                {
                    if((queue.ChatGroupName.ToLower()).Equals(name))
                    {
                        joinQueue = queue;
                    }
                }
                // Get queue from active queues

                // JoinSpecificQueue
                if(joinQueue != null)
                {
                    await JoinSpecificQueue(joinQueue);
                }
            }
            catch (Exception e)
            {
                // TODO: 
            }
        }

        public async Task JoinSpecificQueue(ChatQueue chatQueue)
        {

        }

        /*public async Task JoinQueue()
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
            }
            catch (Exception exception)
            {
                await DisplayErrorMessageInChat(userGroup, "Feil under tilkobling av chat. "); // TODO: Error messages...
            }

        }*/

        /*  public async Task JoinSpecificQueue(ChatQueue queue, int conversationId)
          {
              string userGroup = GetConnectionKey();
             // int? placeInQueue = queue.Enqueue(conversationId, userGroup);
              if(placeInQueue != null)
              {
                  await DisplayQueueCount();
              }


          }*/

        /*public async Task<bool> MessageIsKeyword(string message, int conversationId)
        {
            string msg = message.ToLower();

            if(msg.Equals(Exit))
            {
                try
                {
                    await EndConversationWithChatBot(conversationId);
                    Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                    conversation.EndTime = DateTime.Now;
                    conversation.Result = true;
                    await _chatRepository.UpdateConversationAsync(conversation);
                }
                catch (Exception exception)
                {
                    // TODO
                }
                return true;
            } else if (msg.Equals(RouteToChatWorker))
            {
                await EndConversationWithChatBot(conversationId);
                await RedirectToChatWorker(conversationId);
                return true;
            }
            return false;
        }*/



        /// <summary>
        /// Check response message from chatbot to see if it contains keyword defined 
        /// to change chat logic. 
        /// If a keyword is found, the change in logic is implemented. 
        /// </summary>
        /// <param name="response">Response message returned by chatbot</param>
        /// <param name="conversationId">Conversation ID for active conversation</param>
        /// <returns>true if keyword is found, false otherwise</returns>
        public async Task<bool> ResponseIsKeyword(string response, int conversationId)
        {
            if (response.Equals(_keywordOptions.Exit))// TODO: ER DENNE NØDVENDIG?
            { 
                try
                {
                    
                    string userGroup = GetConnectionKey();
                    await Clients.Group(userGroup).InvokeAsync("endBotConversation", conversationId);

                    Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                    conversation.EndTime = DateTime.Now;
                    conversation.Result = false;
                    await _chatRepository.UpdateConversationAsync(conversation);
                } catch (Exception e)
                {
                    // TODO
                }
            } else if(response.Equals(_keywordOptions.RouteToChatWorker))
            {
                string userGroup = GetConnectionKey();
                await Clients.Group(userGroup).InvokeAsync("endBotConversation", conversationId);
                await RedirectToChatWorker(conversationId);
            } else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Redirect conversation from chatbot to a chat with chat-worker. 
        /// Conversation with chatbot is ended and stored in the database with negative result. 
        /// JoinQueue is called to add user to chat queue. 
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        public async Task RedirectToChatWorker(int conversationId)
        {
            try
            {
                string userGroup = GetConnectionKey();

                Conversation chatbotConversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                chatbotConversation.EndTime = DateTime.Now;
                chatbotConversation.Result = false;
                
                Conversation chatWorkerConversation = new Conversation
                {
                    IsChatBot = false,
                    StartTime = DateTime.Now,
                    UserGroup1 = userGroup,
                    LinkedConversation = chatbotConversation.ConversationId
                };

                int chatworkerConversationId = await _chatRepository.AddConversationAsync(chatWorkerConversation);

                chatbotConversation.LinkedConversation = chatworkerConversationId;
                await _chatRepository.UpdateConversationAsync(chatbotConversation);
                await JoinQueue(chatWorkerConversation);
            } catch (Exception e)
            {
                // TODO: 
            }
        }



        private int? Dequeue()
        {
            return ChatQueue.Dequeue();
        }
        
        /*public async Task PickFromSpecificQueue(string queueId)
        {

            ChatQueue queue = new ChatQueue(); // TODO: DENNE MÅ HENTES FRA ET ANNET STED
            string chatWorkerId = GetConnectionKey();
            //int? conversationId = queue.Dequeue();
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

                    //_inQueue.Remove(conversation.UserGroup1, out int value);
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
        }*/

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
                    ChatQueue.AddFullWaitTime(thisWaitTime);

                    await DisplayWaitTime();
                    conversation.UserGroup2 = chatWorkerId;
                    await _chatRepository.UpdateConversationAsync(conversation);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, conversation.UserGroup1);
                    await SetGroupId(conversation.UserGroup1, chatWorkerId);

                    bool removed = ChatQueue.RemoveFromFullQueue(conversation.UserGroup1);
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
            await Clients.All.InvokeAsync("displayWaitTime", ChatQueue.FullWaitTime);
        }

        public IEnumerable<int> GetQueue()
        {
            return null;
            //return _queue.ToArray();
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

                    if(! await ResponseIsKeyword(response, id))
                    {
                        // Display response message
                        await DisplayMessage(from, ChatBot, response);
                    }
                }
                catch (Exception exception)
                {
                    await DisplayChatBotConnectionError(from);
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
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("displayQueueCount", ChatQueue.FullQueueCount);
            await Clients.Group(_roleOptions.ChatEmployeeRole).InvokeAsync("displayQueueCount", ChatQueue.FullQueueCount);
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
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("displayAllChatQueues", queues);
            await Clients.Group(_roleOptions.ChatEmployeeRole).InvokeAsync("displayAllChatQueues", queues);
        }

        public async Task DisplayUsersChatQueues()
        {
            string key = GetConnectionKey();
            try
            {
                IEnumerable<ChatQueue> queues = await _chatRepository.GetUsersChatGroupsAsQueueAsync(Context.User.Identity.Name);
                await Clients.Group(key).InvokeAsync("displayAllChatQueues", queues);
                await Clients.Group(key).InvokeAsync("displayAllChatQueues", queues);
            } catch
            {
                // TODO
            }
           

        }
    }
}
