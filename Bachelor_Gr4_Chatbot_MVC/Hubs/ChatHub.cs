﻿using Bachelor_Gr4_Chatbot_MVC.Controllers;
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

        // ChatBot user constant
        private const string ChatBot = "ChatBot";

        // Keep track of all SignalR Connections made towards the hub using in-memory connection mapping
        public readonly static ConnectionMapping<string> _connectedUsers =
            new ConnectionMapping<string>();

        public readonly static ConnectionMapping<string> _connectedChatWorkers =
            new ConnectionMapping<string>();

        // Keep track of all active conversations
        public readonly static ConcurrentDictionary<string, int> _activeConversations 
            = new ConcurrentDictionary<string, int>();

        // Keep track of all queues
        private static IEnumerable<ChatQueue> _allChatQueues = null;
        
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

        //IEnumerable<ChatGroup> _allChatGroups;
        //IEnumerable<ChatQueue> _allChatQueues;

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
            if(_allChatQueues == null)
            {
                _allChatQueues = await _chatRepository.GetAllChatGroupsAsQueueAsync();
            }

            // Variables used to map connections
            string connectionId = Context.ConnectionId;
            string key = GetConnectionKey();
            
            // Add user to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);

            // Add Employee to necessary groups and map connected chat workers
            if (Context.User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(Context.User.Identity.Name);
                    // Chat Worker
                    if (await _userManager.IsInRoleAsync(user, _roleOptions.ChatEmployeeRole))
                    {
                        _connectedChatWorkers.Add(key, connectionId); // In-memory connection mapping
                        await Groups.AddAsync(Context.ConnectionId, _roleOptions.ChatEmployeeRole);
                        // TODO: SetChatEmployeeStatus
                        await SetChatEmployeeStatus(GetConnectionKey(), (int)LogInStatus.Available);
                    }

                    // Administrator
                    if (await _userManager.IsInRoleAsync(user, _roleOptions.AdminRole))
                    {
                        await Groups.AddAsync(Context.ConnectionId, _roleOptions.AdminRole);
                    }
                    await AddEmployeeToCustomChatGroups();
                    //await DisplayUsersChatQueues();
                } catch {

                }
            } else
            {
                _connectedUsers.Add(key, connectionId); // In-memory connection mapping
            } 
            
            // TODO: Map chat workers status

            // TODO: When logged in, display chat workers queues
            
            // Check if user has active conversations and display conversation if it exists
            if (_activeConversations.TryGetValue(key, out int conversationId))
            {
                Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                if(conversation.Messages.Count > 0) 
                    await Clients.Client(Context.ConnectionId).InvokeAsync("displayConversation", conversation.Messages);
                await SetConversationId(key, conversation.ConversationId);
            }
            await DisplayAllChatQueues();
            await DisplayQueueCount();
            await DisplayChatNumbersForAdmin();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string key = GetConnectionKey();

            // Remove from in-memory connection mapping
            if (Context.User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(Context.User.Identity.Name);
                    // Chat Worker
                    if (await _userManager.IsInRoleAsync(user, _roleOptions.ChatEmployeeRole))
                    {
                        _connectedChatWorkers.Remove(key, Context.ConnectionId); // In-memory connection mapping
                    }
                }
                catch
                {
                    // TODO
                }
            }
            else
            {
                _connectedUsers.Remove(key, Context.ConnectionId); // In-memory connection mapping
            }

            // Check if user is in any queues and remove from queue
            if(ChatQueue.RemoveFromFullQueue(key))
            {
                // Display queue information for admin and chatworkers
                await DisplayAllChatQueues();
                await DisplayQueueCount();
            }
            await DisplayChatNumbersForAdmin();

        }


        /// <summary>
        /// Add employee to groups based on their group membership. 
        /// </summary>
        private async Task AddEmployeeToCustomChatGroups()
        {
            var groups = await _chatRepository.GetUsersChatGroups(Context.User.Identity.Name);
            foreach (string group in groups)
            {
                await Groups.AddAsync(Context.ConnectionId, group);
            }
        }


        /*
         * ChatBot methods
         */

        /// <summary>
        /// Start conversation with ChatBot and display ChatBot "Hello" message. 
        /// </summary>
        /// <returns></returns>
        public async Task StartConversationWithChatBot()
        {
            string userGroup = GetConnectionKey();
            if (!_activeConversations.TryRemove(userGroup, out int id))
            {
                var knowledgebase = await _chatBotRepository.GetActiveQnAKnowledgeBaseAsync();
                var qnabase = await _chatBotRepository.GetActiveQnABaseClassAsync();
                int conversationId;
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
                    conversationId = await _chatRepository.AddConversationAsync(conversation);
                    conversation.ConversationId = conversationId;
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    await DisplayChatBotConnectionError(userGroup);
                    return;
                }

                // Add to active conversation
                _activeConversations.TryAdd(userGroup, conversationId);

                await SetConversationId(userGroup, conversation.ConversationId);
                await DisplayMessage(userGroup, ChatBot, GetStandardChatBotHello());
            }


        }

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
            if (response.Equals(_keywordOptions.Exit))
            {
                // TODO: Remove this? 
                /*try
                {

                    string userGroup = GetConnectionKey();
                    await Clients.Group(userGroup).InvokeAsync("endBotConversation", conversationId);

                    Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                    conversation.EndTime = DateTime.Now;
                    conversation.Result = false;
                    await _chatRepository.UpdateConversationAsync(conversation);
                }
                catch (Exception e)
                {
                    // TODO
                }*/
            }
            else if (response.Equals(_keywordOptions.RouteToChatWorker))
            {
                string userGroup = GetConnectionKey();
                if (_connectedChatWorkers.Count > 0)
                {
                    string availableGroups = "Velg et tema for henvendelsen: <br />";
                    for (int i = 0; i < _allChatQueues.Count(); i++)
                    {
                        availableGroups += i + 1 + ": " + _allChatQueues.ElementAt(i).ChatGroupName + "<br />";
                    }
                    availableGroups += "0: Annet";
                    await Clients.Group(userGroup).InvokeAsync("receiveMessage", "", availableGroups);
                    await Clients.Group(userGroup).InvokeAsync("setMessageIsChatGroup", true);
                } else
                {
                    await Clients.Group(userGroup).InvokeAsync("receiveMessage", "", "Det er ingen chat medarbeidere tilgjengelig, men du kan fortsatt be chatboten om hjelp. ");
              
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        /*
         * Methods used for live chat with chat-worker
         */
         
        /// <summary>
        /// Method invoked from client to add to specific chat-queue. 
        /// Check if given ChatQueue name or number exists and call RedirectToChatWorker
        /// to add user to the correct queue. 
        /// </summary>
        /// <param name="queueName"></param>
        public async Task SelectChatGroup(string conversationId, string chatGroup)
        {
            string userGroup = GetConnectionKey();
            await Clients.Group(userGroup).InvokeAsync("setMessageIsChatGroup", false);

            ChatQueue selectedQueue = null;

            for (int i = 0; i < _allChatQueues.Count(); i++)
            {
                ChatQueue thisQueue = _allChatQueues.ElementAt(i);
                if (chatGroup.Equals(thisQueue.ChatGroupName)
                    || chatGroup.Equals((i + 1).ToString()))
                {
                    selectedQueue = thisQueue;
                    break;
                }
            }
            if (Int32.TryParse(conversationId, out int id))
            {
                await RedirectToChatWorker(id, selectedQueue);
            }

        }

        /// <summary>
        /// Redirect conversation from chatbot to a chat with chat-worker-
        /// Conversation with chatbot is ended and stored in the database with negative result. 
        /// JoinQueue is called to add user to chat queue. 
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="queueu">ChatQueue being sent to JoinQueue</param>
        public async Task RedirectToChatWorker(int conversationId, ChatQueue queue)
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
                await Clients.Group(userGroup).InvokeAsync("endBotConversation", conversationId);

                // Update active conversations
                _activeConversations.TryUpdate(userGroup, conversationId, chatworkerConversationId);

                await JoinQueue(chatWorkerConversation, queue);
            }
            catch (Exception e)
            {
                // TODO: 
            }
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

        public async Task JoinQueue(Conversation conversation, ChatQueue queue)
        {
            try
            {

                if (queue != null)
                {
                    int? placeInQueue = queue.Enqueue(conversation.ConversationId, conversation.UserGroup1);
                    if (placeInQueue != null)
                    {
                        string message = String.Format("Du er nå lagt i kø, en medarbeider vil svare deg så raskt som mulig. Din plass i køen er: {0}.", placeInQueue);
                        await Clients.Group(conversation.UserGroup1).InvokeAsync("displayPlaceInQueue", message);
                        await SetConversationId(conversation.UserGroup1, conversation.ConversationId);
                    }
                }
                else
                {
                    int? placeInQueue = ChatQueue.EnqueueFullQueue(conversation.ConversationId, conversation.UserGroup1);
                    if (placeInQueue != null)
                    {
                        string message = String.Format("Du er nå lagt i kø, en medarbeider vil svare deg så raskt som mulig. Din plass i køen er: {0}.", placeInQueue);
                        await Clients.Group(conversation.UserGroup1).InvokeAsync("displayPlaceInQueue", message);
                        //await Clients.Group(conversation.UserGroup1).InvokeAsync("setChatGroupsQuestionBool", true);
                        await SetConversationId(conversation.UserGroup1, conversation.ConversationId);
                    }
                }

                await DisplayChatNumbersForAdmin();
                await DisplayAllChatQueues();
                await DisplayQueueCount();

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

        public async Task PickFromQueue()
        {
            string chatWorkerId = GetConnectionKey();
            // Be sure queue is not empty
            await Dequeue(null);

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

        public async Task PickFromSpecificQueue(string queueId)
        {
            ChatQueue queue = null;
            foreach(ChatQueue q in _allChatQueues)
            {
                if(q.ChatGroupId.Equals(queueId))
                {
                    queue = q;
                }
            }

            await Dequeue(queue);


        }

        private async Task Dequeue(ChatQueue queue)
        {
            string chatWorkerId = GetConnectionKey();

            int? conversationId;
            if (queue != null)
            {
                conversationId = queue.DequeFromSpecifiedQueue();
            }
            else
            {
                QueueItem item = ChatQueue.Dequeue();
                conversationId = item.ConversationId;
            }


            if (conversationId != null)
            {
                string displayName = await GetDisplayName();
                string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);

                try
                {
                    Conversation conversation = await _chatRepository.GetConversationByIdAsync((int)conversationId);

                    //AddConversationToActiveConversations(conversation.UserGroup1, (int)conversationId);

                    TimeSpan thisWaitTime = DateTime.Now - conversation.StartTime;
                    ChatQueue.AddFullWaitTime(thisWaitTime);

                    await DisplayWaitTime();
                    conversation.UserGroup2 = chatWorkerId;
                    await _chatRepository.UpdateConversationAsync(conversation);

                    // Set groupId for chatworker and user
                    await SetGroupId(chatWorkerId, conversation.UserGroup1);
                    await SetGroupId(conversation.UserGroup1, chatWorkerId);

                    //bool removed = ChatQueue.RemoveFromFullQueue(conversation.UserGroup1);
                    await SetConversationId(chatWorkerId, conversationId);
                    await DisplayMessage(conversation.UserGroup1, chatWorkerId, message);
                    //await Clients.Group(conversation.UserGroup1).InvokeAsync("enableInputField", "hei");
                    await DisplayQueueCount();
                    await DisplayChatNumbersForAdmin();
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


        public async Task EndConversation(int conversationId, string groupTo)
        {
            Conversation conversation = null;
            try
            {
                conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
                conversation.EndTime = DateTime.Now;
                conversation.Result = true;
                await _chatRepository.UpdateConversationAsync(conversation);
            }
            catch (Exception e)
            {
                // TODO
            }
            string user = GetConnectionKey();
            string displayName = await GetDisplayName();
            string message = displayName + " forlot samtalen. ";

            // Remove from active conversations
            _activeConversations.TryRemove(user, out int id);

            await Clients.Group(conversation.UserGroup1).InvokeAsync("conversationEnded", message, conversationId);
            await Clients.Group(conversation.UserGroup2).InvokeAsync("conversationEnded", message, conversationId);
        }


        /*
         * Send invoked from client
         */

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
        public async Task SendToChatBot(string conversationId, string message)
        {

            List<Message> messages = new List<Message>();
            string from = GetConnectionKey();
            await DisplayMessage(ChatBot, from, message);



            if (Int32.TryParse(conversationId, out int id))
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

                    if (!await ResponseIsKeyword(response, id))
                    {
                        // Display response message
                        await DisplayMessage(from, ChatBot, response);
                    }
                }
                catch (Exception exception)
                {
                    await DisplayChatBotConnectionError(from);
                }

            }
            else
            {
                await DisplayChatBotConnectionError(from);
            }


            try
            {
                // Save messages
                await _chatRepository.AddMessagesAsync(messages);
            }
            catch (Exception exception)
            {
                // ChatBot should continue to work even if messages does not get stored to db. 
                // So do nothing here.
            }
        }


        public async Task RegisterConversationResult(int conversationId, bool result)
        {
            Conversation conversation = await _chatRepository.GetConversationByIdAsync(conversationId);
            conversation.Result = result;
            await _chatRepository.UpdateConversationAsync(conversation);
        }


        public string GetStandardChatBotHello()
        {
            // TODO: Denne skal hentes fra et annet sted, kun testkode
            return "Hei, mitt navn er Svarta Bjørn, hva kan jeg hjelpe deg med?";
        }
        /// <summary>
        /// Get key used to map connection in Single-user group and In-memory connection mapping. 
        /// </summary>
        /// <returns>A key used to map the users connections</returns>
        private string GetConnectionKey()
        {
            string connectionId = Context.ConnectionId;

            string connectionKey = connectionId;
            if (Context.User.Identity.IsAuthenticated)
            {
                connectionKey = Context.User.Identity.Name;
            }
            else
            {
                var context = Context.Connection.GetHttpContext();
                string signalRCookie = context.Request.Cookies["SignalRCookie"];
                if (signalRCookie != null)
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
                    if (displayName.Length < 1)
                    {
                        displayName = "Kundesenter";
                    }
                }
                catch (Exception e)
                {
                    displayName = "Kundesenter";
                }
            }
            return displayName;
        }


        /*
         * Set variables on client
         */

        private async Task SetConversationId(string userGroup, int? conversationId)
        {
            await Clients.Group(userGroup).InvokeAsync("setConversationId", conversationId);
        }

        private async Task SetGroupId(string toUser, string groupId)
        {
            await Clients.Group(toUser).InvokeAsync("setGroupId", groupId);
        }


        /*
         * Display on client
         */

        public async Task DisplayMessage(string groupTo, string groupFrom, string message)
        {
            if (!groupFrom.Equals(ChatBot))
            {
                await Clients.Group(groupFrom).InvokeAsync("sendMessage", message);
            }

            if (!groupTo.Equals(ChatBot))
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

        /// <summary>
        /// Display for Admin how many users and chat workers are connected to the chat. 
        /// </summary>
        public async Task DisplayChatNumbersForAdmin()
        {
            string adminGroup = _roleOptions.AdminRole;
            await Clients.Group(adminGroup).InvokeAsync("numberOfChatWorkersConnected", _connectedChatWorkers.Count);
            await Clients.Group(adminGroup).InvokeAsync("numberOfClientsConnected", _connectedUsers.Count);
            await Clients.Group(adminGroup).InvokeAsync("numberInQueue", ChatQueue.FullQueueCount);
        }

        /// <summary>
        /// Display all chat queues for admin and chat-worker
        /// </summary>
        /// <returns></returns>
        public async Task DisplayAllChatQueues()
        {
            if(_allChatQueues != null)
            {
                await Clients.Group(_roleOptions.AdminRole).InvokeAsync("displayAllChatQueues", _allChatQueues);
                await Clients.Group(_roleOptions.ChatEmployeeRole).InvokeAsync("displayAllChatQueues", _allChatQueues);
            }
        }

        /// <summary>
        /// Display useres own chat queues
        /// </summary>
        /// <returns></returns>
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

        private async Task DisplayWaitTime()
        {
            await Clients.All.InvokeAsync("displayWaitTime", ChatQueue.FullWaitTime);
        }

        /// <summary>
        /// Display for admin and chat-worker how many users are in queue to talk to a chat worker
        /// </summary>
        /// <returns></returns>
        public async Task DisplayQueueCount()
        {
            await Clients.Group(_roleOptions.AdminRole).InvokeAsync("displayQueueCount", ChatQueue.FullQueueCount);
            await Clients.Group(_roleOptions.ChatEmployeeRole).InvokeAsync("displayQueueCount", ChatQueue.FullQueueCount);
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
         * Code below is unused for now, not finished implementing 
         */
   
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

        public async Task SetChatEmployeeStatus(string userGroup, int status)
        {

        }

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


    }
}
