using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Hubs
{
    public interface IChatHub
    {
        Task<IEnumerable<string>> getUsers();
    }
}
