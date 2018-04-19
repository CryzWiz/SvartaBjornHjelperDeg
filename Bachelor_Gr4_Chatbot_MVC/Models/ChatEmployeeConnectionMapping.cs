using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatEmployeeConnectionMapping<T>
    {
        private readonly ConcurrentDictionary<T, ChatEmployeeConnectionStatus> 
            _connectionStatus = new ConcurrentDictionary<T, ChatEmployeeConnectionStatus>();

        public int Count
        {
            get
            {
                return _connectionStatus.Count;
            }
        }

        public void AddConnection(T key, string connectionId)
        {

        }


    }
}
