using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    [Produces("application/json")]
    [Route("api/Chat")]
    public class ChatApiController : Controller
    {
        private static List<Message> _messages = new List<Message>();
        private IChatRepository _repository;

        public ChatApiController(IChatRepository repository)
        {
            _repository = repository;
        }

        public void JoinChatGroup()
        {

        }


    }
}