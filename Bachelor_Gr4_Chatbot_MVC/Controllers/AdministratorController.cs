using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Models;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class AdministratorController : Controller
    {
        private IAdminRepository repository;

        public AdministratorController(IAdminRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Users()
        {
            ViewData["Message"] = "User page.";
            var u = repository.GetAllUsers();
            return View(u);
        }

        [HttpGet]
        public ActionResult ManageUser()
        {
            ViewData["Message"] = "Manage User Page.";

            return View();
        }

        [HttpGet]
        public ActionResult Register_New_User()
        {
            ViewData["Message"] = "Register new user page";

            return View();
        }
    }
}