﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class ChatbotKeywordOptions
    {
        public ChatbotKeywordOptions()
        {
            Yes = "ja";
            No = "nei";
            RouteToChatWorker = "routeToChatWorker";
            Exit = "exit";
            SatisfiedQuestion = "Fikk du svar på det du lurte på?";
            TalkToChatWorkerQuestion = "Ønsker du å snakke med kundesenter?";
            Stop = "stop";
            NoMatch = "No good match found in the KB";
        }

        public string Yes { get; }
        public string No { get; }
        public string RouteToChatWorker { get; }

        public string Exit { get; }
        public string SatisfiedQuestion { get; }
        public string TalkToChatWorkerQuestion { get; }
        public string Stop { get; }
        public string NoMatch { get; }
    }
}
