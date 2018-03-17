
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
        public TimeSpan OpenFrom { get; set; }
        public TimeSpan OpenTo { get; set; }

        /*
         * DateFrom and DateTo are set if special opening hours are required. 
         * Otherwise they are set to null. 
         */
        [Column(TypeName = "date")]
        public DateTime? DateFrom { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateTo { get; set; }

    }
}
