using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Bachelor_Gr4_Chatbot_MVC
{
    public class ChatHub : Hub
    {
        /*public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message);
        }*/

        public Task Send(string name, string message)
        {
            return Clients.All.InvokeAsync("broadcastMessage", name, message);
        }
    }
}
