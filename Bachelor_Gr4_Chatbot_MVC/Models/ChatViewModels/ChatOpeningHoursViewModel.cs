using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Bachelor_Gr4_Chatbot_MVC.Controllers.ChatController;

namespace Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels
{
    public class ChatOpeningHoursViewModel
    {
        [Display(Name = "Velg ukedag(er) åpningstiden skal gjelde for")]
        public IEnumerable<SelectListItem> DaysOfWeek { get; set; }
        public string SelectedDayOfWeek { get; set; }
        [DisplayFormat(ApplyFormatInEditMode=true, DataFormatString ="{0:yyyy/MM/dd}")]
        [DataType(DataType.Date), Display(Name = "Fra dato", Description = "Velg dato åpningstidene skal være gyldig fra")]
        public DateTime DateFrom { get; set; }
        [DataType(DataType.Date), Display(Name = "Til dato", Description = "Velg dato åpningstidene skal være gyldig til")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        public DateTime DateTo { get; set; }
        [Display(Name = "Fra klokken", Description = "Velg tidspunkt det er åpent fra")]
        public TimeSpan OpenFrom { get; set; }
        [Display(Name = "Til klokken", Description = "Velg tidspunkt det er åpent til")]
        public TimeSpan OpenTo { get; set; }

    }
}
