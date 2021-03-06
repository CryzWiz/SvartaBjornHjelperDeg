﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Services;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class HomeController : Controller
    {
        IEmailSender _emailSender;
        public HomeController(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Chat()
        {
            ViewData["Message"] = "Chat-client";

            return View();
        }

        public IActionResult Test()
        {
            ViewData["Message"] = "Chat-client";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
