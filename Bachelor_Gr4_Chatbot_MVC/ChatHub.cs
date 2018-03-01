using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Bachelor_Gr4_Chatbot_MVC
{
    /// Referanser: 
    /// https://code.msdn.microsoft.com/ASPNET-CORE-20-uses-7a771742
    /// https://docs.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/mapping-users-to-connections
    public class ChatHub : Hub
    {
        /*public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message);
        }*/

        /// <summary>
        /// Send a message to everyone. 
        /// </summary>
        /// <param name="name">Name of the author of the message</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        public Task Send(string name, string message)
        {
            return Clients.All.InvokeAsync("broadcastMessage", name, message);
        }

        /// <summary>
        /// Send a message to a specified user group. 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        public void SendChatMessage(string groupName, string message)
        {
            string name = Context.User.Identity.Name;
            Clients.Group(groupName).InvokeAsync("SendMessage", name, message);
        }

        /*public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;
            Groups.AddAsync(Context.ConnectionId, name);
            return base.OnConnectedAsync();
        }*/

 

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Connected user leaves the specified group. 
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveAsync(Context.ConnectionId, groupName);

        }
    }
}
