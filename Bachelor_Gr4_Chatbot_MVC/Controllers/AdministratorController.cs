using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            ViewData["Message"] = "User page.";

            return View();
        }

        public IActionResult ManageUser()
        {
            ViewData["Message"] = "Manage User Page.";

            return View();
        }
    }
}