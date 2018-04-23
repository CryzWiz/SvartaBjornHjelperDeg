using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModel
{
    public class AdministratorIndexViewModel
    {
        public string ChatbotName { get; set; }

        public int ChatbotId { get; set; }

        public int ChatWorkers { get; set; }

        public int ChatQue { get; set; }

        public int ChatUsers { get; set; }

        public int UnPublishedQnAPairs { get; set; }
    }
}
