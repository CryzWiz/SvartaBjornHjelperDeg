
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class OpeningHours
    {
        [Key]
        public int OpeningHoursId { get; set; }
        public int DayOfWeek{ get; set; }
        public DateTime OpenFrom { get; set; }
        public DateTime OpenTo { get; set; }
        public bool StandardOpeningHours { get; set; }
    }
}
