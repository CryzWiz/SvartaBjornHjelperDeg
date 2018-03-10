using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatGroupOptions
    {
        private static int counter = 0;
        public string ChatWorkerGroup = "ChatWorkerGroup";
        public string ChatBotGroup = "ChatBotGroup";

        public string ChatWorkerGroupName
        {
            get {
                counter++;
                return ChatWorkerGroup + counter;
            }
        }

        public string ChatBotGroupName
        {
            get
            {
                counter++;
                return String.Format("{0} - {1}", ChatBotGroup, counter);
            }
        }

    }
}
