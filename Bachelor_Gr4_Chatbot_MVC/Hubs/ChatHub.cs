using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Hubs
{

    /// Referanser: 
    /// https://code.msdn.microsoft.com/ASPNET-CORE-20-uses-7a771742
    /// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
    public class ChatHub : Hub
    {
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        private static ConcurrentQueue<string> _queue = new ConcurrentQueue<string>();
        private static ConcurrentDictionary<string, string> _chatWorkerStatus = new ConcurrentDictionary<string, string>();
        

        public override async Task OnConnectedAsync()
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string connectionId = Context.ConnectionId;
            string key = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name: connectionId);

            if(Context.User.IsInRole())
            _connections.Add(key, connectionId);

            // Add to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);
            
            await DisplayConnectedUsers();
         }

        private async Task SetChatWorkerStatus(string userName, string status)
        {

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
                    string message = String.Format("Hei! Mitt navn er {0}, hva kan jeg hjelpe deg med?", displayName);
                    string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);

                    await Clients.Group(from).InvokeAsync("pickFromQueue", groupId);
                    await DisplayMessage(groupId, from, message);

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
        public async Task SendToGroup(string groupName, string message)
        {
            string from = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : Context.ConnectionId);
            Message msg = new Message
            {
                From = from,
                To = groupName,
                DisplayName = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "Guest"),
                DateTime = DateTime.Now,
                Content = message
            };

            
            await DisplayMessage(groupName, from, message);

        }
        public async Task DisplayMessage(string groupTo, string groupFrom, string message)
        {
            await Clients.Group(groupTo).InvokeAsync("receiveMessage", groupFrom, message);
            await Clients.Group(groupFrom).InvokeAsync("sendMessage", message);
            
        }
    }
}
