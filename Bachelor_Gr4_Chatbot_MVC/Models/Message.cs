using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        // Connection ID
        public string From { get; set; }
        public string To { get; set; }

        public bool IsChatBot { get; set; }
        public bool IsChatWorker { get; set; }

        public DateTime DateTime { get; set; }

    }
}
