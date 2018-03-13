using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.SignalR;
using System;
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

        private Queue<string> _queue = new Queue<string>();


        public override async Task OnConnectedAsync()
        {
            /*
             * Map connections using in-memory connection mapping
             * for keeping track of all active connections. 
             */
            string connectionId = Context.ConnectionId;
            string key = (Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name: connectionId);
            _connections.Add(key, connectionId);

            // Add to single-user group
            await Groups.AddAsync(Context.ConnectionId, key);
            
            await DisplayConnectedUsers();
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
            _queue.Enqueue(Context.ConnectionId);
            int placeInQueue = _queue.Count();

            await Clients.All.InvokeAsync("displayQueue", GetQueue());
            await Clients.Client(Context.ConnectionId).InvokeAsync("displayQueueNumber", placeInQueue);
        }

        public string PickFromQueue()
        {
            return _queue.Dequeue();
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
            await Clients.Group(groupName).InvokeAsync("Send", message, from); 
        }
    }
}
