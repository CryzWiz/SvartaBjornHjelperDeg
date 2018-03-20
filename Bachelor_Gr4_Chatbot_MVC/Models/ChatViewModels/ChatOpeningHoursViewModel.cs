using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels
{
    public class ChatOpeningHoursViewModel
    {
        public int OpeningHoursId { get; set; }
        public int WeekDay { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date), Display(Name = "Gyldig fra dato")]
        public DateTime DateFrom { get; set; }
        [DataType(DataType.Date), Display(Name = "Gyldig til dato")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        public DateTime DateTo { get; set; }

        /*[Display(Name = "Åpent fra")]

        public TimeSpan OpenFrom { get; set; }
        [Display(Name = "Åpent til")]
        public TimeSpan OpenTo { get; set; }*/


        [Display(Name = "Åpent fra")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime OpenFrom { get; set; }
        [Display(Name = "Åpent til")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime OpenTo { get; set; }

        public bool IsStandard { get; set; }





    }
}
