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

        //S
        [HttpGet]
        public async Task<ActionResult> Users()
        {
            ViewData["Message"] = "User page.";
            var u = await repository.GetAllUsers();
            return View(u);
        }

        [HttpGet]
        public async Task<ActionResult> ManageUser(string username)
        {
            ViewData["Message"] = "Manage User Page." + username;
            // ViewData just used for debugging.. Can be deleted, Not used!
            User u = await repository.GetUser(username);
            return View(u);
        }

        [HttpGet]
        public ActionResult Register_New_User()
        {
            ViewData["Message"] = "Register new user page";
            // ViewData just used for debugging.. Can be deleted, Not used!
            return View();
        }
    }
}