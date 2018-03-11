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

       
        //private List<string> _connections2 = new List<string>();

        private static int _counter = 0;

        public override async Task OnConnectedAsync()
        {
            // Map connections using in-memory ConnectionMapping
            string connectionId = Context.ConnectionId;
            string connectionName = SetConnectionName();

            // Add to connection list
            _connections.Add(connectionName, Context.ConnectionId);

            // Join persons own group 
            // TODO: Change this later! TESTCODE
            await JoinGroup(connectionName);

            string displayName = GetDisplayName();
            await Clients.All.InvokeAsync("broadcastMessage", $"{displayName} joined");
            await DisplayConnectedUsers();
         }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // Map connections using in-memory ConnectionMapping
            string connectionId = Context.ConnectionId;
            string connectionName = SetConnectionName();

            // Remove from connection list
            _connections.Remove(connectionName, Context.ConnectionId);

            string displayName = GetDisplayName();
            await Clients.All.InvokeAsync("broadcastMessage", $"{displayName} left");
            await DisplayConnectedUsers();
        }

        public string SetConnectionName()
        {
            string name = Context.ConnectionId;
            if (Context.User.Identity.IsAuthenticated)
            {
                name = Context.User.Identity.Name;
            }
            return name;
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

        
        /// <summary>
        /// Send a message to everyone connected to the chat. 
        /// </summary>
        /// <param name="message">Message content.</param>
        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("broadcastMessage", $"{Context.ConnectionId}: {message}");
        }

        /// <summary>
        /// Send a message to a specified user group. 
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="message">Message content</param>
        public Task SendToGroup(string groupName, string message)
        {
            // TODO: Testkode som må fjernes: 
            if(groupName.Length == 0)
            {
                return Send(message);
            }

            return Clients.Group(groupName).InvokeAsync("Send", $"{Context.ConnectionId}@{groupName}: {message}");
            
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddAsync(Context.ConnectionId, groupName);
            //await Clients.Group(groupName).InvokeAsync("Send", $"{Context.ConnectionId} joined {groupName}");
            
        }

        /// <summary>
        /// Connected user leaves the specified group
        /// </summary>
        /// <param name="groupName">Group name</param>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).InvokeAsync("broadcastMessage", $"{Context.ConnectionId} left {groupName}");
            await Clients.Group(groupName).InvokeAsync("testMessageToUser", $"{Context.ConnectionId} left {groupName}");
        }

        public Task Echo(string message)
        {
            return Clients.Client(Context.ConnectionId).InvokeAsync("broadcastMessage", $"{Context.ConnectionId}: {message}");
        }

        // TODO: Trenger å oppdateres
        public string GetDisplayName()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                return Context.User.Identity.Name;
            }

            _counter++;
            return "Guest " + _counter;
        }


    }
}
