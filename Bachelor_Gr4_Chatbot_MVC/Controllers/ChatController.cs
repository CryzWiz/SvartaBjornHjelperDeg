using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets;
using Bachelor_Gr4_Chatbot_MVC.Hubs;
using Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bachelor_Gr4_Chatbot_MVC.Models;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{

    public class ChatController : Controller
    {
        IChatRepository _repository;
        ConnectionManager _connectionManager;
        private readonly IHubContext<ChatHub> _chatHub;
        public enum WeekDay { Mandag = 1, Tirsdag, Onsdag, Torsdag, Fredag, Lørdag, Søndag, Ukedager, Helg, Alle};


        public ChatController(IChatRepository repository, ConnectionManager connectionManager, IHubContext<ChatHub> chatHub)
        {
            _repository = repository;
            _connectionManager = connectionManager;
            _chatHub = chatHub;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConnectToChat(string chatGroup)
        {
            //IHubContext context = GlobalHost.GetHubContext("/chat");
            //var testHub = _connectionManager.GetHubContext<TestChat>();

            // Slettes: 

            return View();
        }

        public IActionResult UserChat()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OpeningHours()
        {
            IEnumerable<ChatOpeningHoursViewModel> modelList = await _repository.GetAllOpeningHours();
            return View(modelList);
        }


        [Authorize]
        [HttpGet]
        public IActionResult RegisterOpeningHours()
        {
            List<SelectListItem> list = new List<SelectListItem>{
                new SelectListItem { Selected = false, Text = WeekDay.Mandag.ToString(), Value =  ((int)WeekDay.Mandag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Tirsdag.ToString(), Value =  ((int)WeekDay.Tirsdag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Onsdag.ToString(), Value =  ((int)WeekDay.Onsdag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Torsdag.ToString(), Value =  ((int)WeekDay.Torsdag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Fredag.ToString(), Value =  ((int)WeekDay.Fredag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Lørdag.ToString(), Value =  ((int)WeekDay.Lørdag).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Søndag.ToString(), Value =  ((int)WeekDay.Søndag).ToString()},
                new SelectListItem { Selected = true, Text = WeekDay.Ukedager.ToString(), Value =  ((int)WeekDay.Ukedager).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Helg.ToString(), Value =  ((int)WeekDay.Helg).ToString()},
                new SelectListItem { Selected = false, Text = WeekDay.Alle.ToString(), Value =  ((int)WeekDay.Alle).ToString()}
            };
            ChatOpeningHoursRegisterViewModel model = new ChatOpeningHoursRegisterViewModel
            {
                DaysOfWeek = list,
               // DateFrom = (DateTime.Today).Date,
                DateFrom = DateTime.Today,
                DateTo = (DateTime.MaxValue).Date,
                OpenFrom = new TimeSpan(08,30,00),
                OpenTo = new TimeSpan(16,00,00)
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterOpeningHours(ChatOpeningHoursRegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                int[] daysOfWeek = GetOpeningHoursDays(Convert.ToInt32(model.SelectedDayOfWeek));
                if(daysOfWeek != null)
                {
                    List<OpeningHours> openingHours = new List<OpeningHours>();
                    DateTime from = model.DateFrom.Add(model.OpenFrom);
                    DateTime to = model.DateTo.Add(model.OpenTo);
                    for (int i = 0; i < daysOfWeek.Length; i++)
                    {
                        OpeningHours hours = new OpeningHours
                        {
                            DayOfWeek = daysOfWeek[i],
                            OpenFrom = from,
                            OpenTo = to,
                            StandardOpeningHours = false
                        };
                        openingHours.Add(hours);
                    }
                    await _repository.SaveOpeningHours(openingHours);
                    return RedirectToAction("Index", "Administrator");
                }

            }
            return View(model); // If we get here, something is wrong! Return view again
            

            
            
            /*ChatOpeningHoursViewModel model = new ChatOpeningHoursViewModel
            {
                DaysOfWeek = list,
                // DateFrom = (DateTime.Today).Date,
                DateFrom = DateTime.Today,
                DateTo = (DateTime.MaxValue).Date,
                OpenFrom = new TimeSpan(08, 30, 00),
                OpenTo = new TimeSpan(16, 00, 00)
            };*/
            //return View(model);
        }

        private int[] GetOpeningHoursDays(int dayOfWeek)
        {
            switch(dayOfWeek)
            {
                case (int)WeekDay.Mandag:
                case (int)WeekDay.Tirsdag:
                case (int)WeekDay.Onsdag:
                case (int)WeekDay.Torsdag:
                case (int)WeekDay.Fredag:
                case (int)WeekDay.Lørdag:
                case (int)WeekDay.Søndag:
                    return new int[] { dayOfWeek };
                case (int)WeekDay.Ukedager:
                    return new int[] { (int)WeekDay.Mandag, (int)WeekDay.Tirsdag, (int)WeekDay.Onsdag, (int)WeekDay.Torsdag, (int)WeekDay.Fredag };
                case (int)WeekDay.Helg:
                    return new int[] { (int)WeekDay.Lørdag, (int)WeekDay.Søndag };
                case (int)WeekDay.Alle:
                    return new int[] { (int)WeekDay.Mandag, (int)WeekDay.Tirsdag, (int)WeekDay.Onsdag, (int)WeekDay.Torsdag, (int)WeekDay.Fredag, (int)WeekDay.Lørdag, (int)WeekDay.Søndag };
                default:
                    return null;
            }

        }








    }
}