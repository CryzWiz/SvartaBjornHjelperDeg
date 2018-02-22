using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Bachelor_Gr4_Chatbot_MVC.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Bachelor_Gr4_Chatbot_MVC.Services;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class AdministratorController : Controller
    {
        private IAdminRepository repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AdministratorController(
            IAdminRepository repository,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            this._emailSender = emailSender;
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
        //[ValidateAntiForgeryToken]
        public ActionResult RegisterNewUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            RegisterNewUserViewModel model = new RegisterNewUserViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewUser(RegisterNewUserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, IsActive = true};
                var result = await _userManager.CreateAsync(user);

                if(result.Succeeded)
                {
                    // Email example from AccountController: 
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
                    return RedirectToLocal(returnUrl);
                }
            }

            // Something went wrong, show form again
            return View(model);
        }

        // Copied from Auto generated code in AccountController
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}