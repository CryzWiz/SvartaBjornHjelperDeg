using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets;
using Bachelor_Gr4_Chatbot_MVC.Hubs;


namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class ChatController : Controller
    {
        IChatRepository _repository;
        ConnectionManager _connectionManager;
        private readonly IHubContext<ChatHub> _chatHub;
        

        public ChatController(IChatRepository repository, ConnectionManager connectionManager, IHubContext<ChatHub> chatHub)
        {
            _repository = repository;
            _connectionManager = connectionManager;
            _chatHub = chatHub;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConnectToChat(string chatGroup)
        {
            //IHubContext context = GlobalHost.GetHubContext("/chat");
            //var testHub = _connectionManager.GetHubContext<TestChat>();

            // Slettes: 

            return View();
        }

        

   
    }
}