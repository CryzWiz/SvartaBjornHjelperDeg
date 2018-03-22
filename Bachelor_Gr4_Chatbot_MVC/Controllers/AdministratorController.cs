using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Bachelor_Gr4_Chatbot_MVC.Models.AccountViewModels;
using Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModels;
using Microsoft.AspNetCore.Identity;
using Bachelor_Gr4_Chatbot_MVC.Services;


/// <summary>
/// Controller holding all the Administrator functions / pages
/// </summary>
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

        /// <summary>
        /// Display the dashboard
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Fetch all the users and display them to the user(admin)
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Users()
        {
            ViewData["Message"] = "User page.";
            var u = await repository.GetAllUsers();
            return View(u);
        }

        public async Task<IActionResult> Chatbots()
        {
            var u = await repository.GetAllChatbots();
            return View(u);
        }

        [HttpGet]
        public IActionResult RegisterNewChatbot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewChatbot([FromForm][Bind("chatbotName", "contentType", "BotSecret",
            "base_url","tokenUrlExtension","conversationExtension","botAutorizeTokenScheme")] ChatbotDetails chatbotDetails)
        {
            var u = repository.RegisterNewChatbot(chatbotDetails);
            return View();
        }

        public async Task<IActionResult> EditChatbotDetails(int id)
        {
            var c = await repository.GetChatbotDetails(id);
            return View(c);
        }

        public async Task<IActionResult> ViewChatbotDetails(int id)
        {
            var c = await repository.GetChatbotDetails(id);
            return View(c);
        }

        /// <summary>
        /// Fetch and display data regarding given username
        /// </summary>
        /// <paramref name="username"/></param>
        /// <returns>One User</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ManageUser(string username)
        {
            ViewData["Message"] = "Manage User Page." + username;
            // ViewData just used for debugging.. Can be deleted, Not used!
            User u = await repository.GetUser(username);
            return View(u);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RegisterNewUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            RegisterNewUserViewModel model = new RegisterNewUserViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterNewUser(RegisterNewUserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, IsActive = true};
                var result = await _userManager.CreateAsync(user);

                if(result.Succeeded)
                {
                    TempData["success"] = String.Format("Bruker ble opprettet for {0} {1} og e-post med instrukser for fullføring av registreringen er sendt til {2}.", model.FirstName, model.LastName, model.Email);
                    // Email example from AccountController: 
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
                    return RedirectToAction("RegisterNewUser");
                }
            }

            // Something went wrong, show form again
            TempData["error"] = "Feil under oppretting av bruker.";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(string username)
        {
            var result = await repository.DeactivateUser(username);
            if (result)
            {
                TempData["success"] = String.Format("Bruker {0} ble deaktivert.", username);
            }
            else
            {
                TempData["error"] = String.Format("Bruker {0} ble IKKE deaktivert.", username);
            }
            return RedirectToAction("Users");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(string username)
        {
            var result = await repository.ActivateUser(username);
            if (result)
            {
                TempData["success"] = String.Format("Bruker {0} ble aktivert.", username);
            }
            else
            {
                TempData["error"] = String.Format("Bruker {0} ble IKKE aktivert.", username);
            }
            return RedirectToAction("Users");
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