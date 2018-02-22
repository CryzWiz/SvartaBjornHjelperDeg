using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class EmailSenderOptions
    {
        public EmailSenderOptions()
        {
            User = "gr4-chatbot";
            APIKey = "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";
        }

        public string User { get; set; }
        public string APIKey { get; set; }
    }
}
