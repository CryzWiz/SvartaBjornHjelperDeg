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
using Microsoft.AspNetCore.SignalR;
using Bachelor_Gr4_Chatbot_MVC.Hubs;
using Bachelor_Gr4_Chatbot_MVC.Models.AdministratorViewModel;
using Bachelor_Gr4_Chatbot_MVC.Hubs;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;

/// <summary>
/// Controller holding all the Administrator functions / pages
/// </summary>
namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{
    public class AdministratorController : Controller
    {
        private IAdminRepository repository;
        private IChatbotRepository chatbotRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<Hub> _chatHub;

        public AdministratorController(
            IAdminRepository repository,
            IChatbotRepository chatbotRepository,
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager,
            IHubContext<Hub> chatHub)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            this.repository = repository;
            _chatHub = chatHub;
            this.chatbotRepository = chatbotRepository;
        }

        /// <summary>
        /// Display the dashboard
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ChatbotDetails c = await chatbotRepository.GetActiveBot();
            var model = new AdministratorIndexViewModel();
            if(c != null)
            {
                model.ChatbotName = c.chatbotName;
                model.ChatbotId = c.chatbotId;
            }
            else
            {
                model.ChatbotId = 0;
            }

            // Fetch chathub data
            model.ChatWorkers = ChatHubHandler.ConnectedWorkers.Count;
            model.ChatQue = ChatHubHandler.inQue;
            model.ChatUsers = ChatHubHandler.ConnectedUsers.Count;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult MorrisCharts()
        {

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult FlotCharts()
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
        
        /// <summary>
        /// Fetch and display data regarding given username
        /// </summary>
        /// <paramref name="username"/></param>
        /// <returns>One User</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ManageUser(string username)
        {
            User u = await repository.GetUser(username);
            return View(u);
        }

        /// <summary>
        /// Updata data for given user
        /// </summary>
        /// <paramref name="username"/></param>
        /// <returns>One User</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ManageUser(User user)
        {
            var r = await repository.UpdateUserData(user);
            if (r)
            {
                TempData["success"] = String.Format("Bruker {0} ble oppdatert.", user.Username);
                return RedirectToAction("Users");
            }
            else {
                TempData["error"] = String.Format("Bruker {0} ble ikke oppdatert.", user.Username);
                return RedirectToAction("Users");
            }
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

       /* public async Task<IActionResult> DisplayActiveChatWorkers()
        {
          
        }*/

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








        /// <summary>
        /// Code below is for Microsoft bots
        /// </summary>
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Chatbots()
        {
            var u = await chatbotRepository.GetAllChatbots();
            return View(u);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterNewChatbot()
        {
            var c = new ChatbotDetails();
            List<ChatbotTypes> types = await chatbotRepository.GetAllTypes();
            c.chatbotTypes = types;
            return View(c);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterNewChatbot([FromForm][Bind("chatbotName", "BotSecret")] ChatbotDetails chatbotDetails)
        {
            if (ModelState.IsValid)
            {
                var u = await chatbotRepository.RegisterNewChatbot(chatbotDetails);
                TempData["success"] = String.Format("Chatbot {0} ble registrert.", chatbotDetails.chatbotName);
                return RedirectToAction("Chatbots");
            }

            TempData["error"] = String.Format("Chatbot {0} ble ikke registrert.", chatbotDetails.chatbotName);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChatbot(int id)
        {
            var c = await chatbotRepository.GetChatbotDetails(id);
            var r = await chatbotRepository.DeleteChatbot(c);
            if (r)
            {
                TempData["success"] = String.Format("Chatbot {0} ble slettet.", c.chatbotName);
                return RedirectToAction("Chatbots");
            }
            TempData["error"] = String.Format("Chatbot {0} ble ikke slettet.", c.chatbotName);
            return RedirectToAction("Chatbots");

        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditChatbotDetails(int id)
        {
            var c = await chatbotRepository.GetChatbotDetails(id);
            return View(c);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChatbotDetails([FromForm][Bind("chatbotId", "chatbotName", "contentType", "BotSecret",
            "baseUrl","tokenUrlExtension","conversationUrlExtension","botAutorizeTokenScheme", "isActive")] ChatbotDetails chatbotDetails)
        {
            //if (ModelState.IsValid)
            //{
            var c = await chatbotRepository.UpdateChatbotDetails(chatbotDetails);
            if (c)
            {
                TempData["success"] = String.Format("Chatbot {0} ble oppdatert.", chatbotDetails.chatbotName);
                return RedirectToAction("Chatbots");
            }
            TempData["error"] = String.Format("Chatbot {0} ble ikke oppdatert.", chatbotDetails.chatbotName);
            return RedirectToAction("Chatbots");
            //}

            //TempData["error"] = String.Format("modelstate unvalid for {0}", chatbotDetails.chatbotName);
            //return RedirectToAction("Chatbots");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewChatbotDetails(int id)
        {
            var c = await chatbotRepository.GetChatbotDetails(id);
            return View(c);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateBot(int id)
        {
            string[] r;

            var a = await chatbotRepository.CheckIfBotActive(id);

            if (!a)
            {
                r = await chatbotRepository.ActivateBot(id);
                if (r[0].Equals("false"))
                {
                    TempData["error"] = String.Format("Chatbot {0} ble ikke aktivert.", r[1]);
                    return RedirectToAction("Chatbots");
                }
                else
                {
                    TempData["success"] = String.Format("Chatbot {0} ble aktivert.", r[1]);
                    return RedirectToAction("Chatbots");
                }
            }
            else
            {
                TempData["error"] = String.Format("Chatbot er allerede aktiv.");
                return RedirectToAction("Chatbots");
            }
        }




        /// <summary>
        /// Code below is for QnA bots
        /// </summary>
     
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABots()
        {
            var q = await chatbotRepository.GetAllQnABots();
            return View(q);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABotDetails(int id)
        {
            var q = await chatbotRepository.GetQnABotDetails(id);
            return View(q);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterNewQnaBotAsync()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterNewQnABotAsync([FromForm][Bind("chatbotName", "subscriptionKey", "knowledgeBaseID")] QnABaseClass qnabot)
        {
            var r = await chatbotRepository.RegisterNewQnABotAsync(qnabot);
            if (r[0].Equals("success")) {
                TempData[r[0]] = String.Format("QnA-bot {0} ble registrert.", r[1]);
                return RedirectToAction("QnABots");
            }
            else if(r[0].Equals("success") & r[2].Equals("success"))
            {
                TempData[r[0]] = String.Format("QnA-bot {0} ble registrert med ny kunnskapsbase.", r[1]);
                return RedirectToAction("QnABots");
            }
            else if (r[0].Equals("success") & r[2].Equals("error"))
            {
                TempData[r[0]] = String.Format("QnA-bot {0} ble registrert uten ny kunnskapsbase.", r[1]);
                return RedirectToAction("QnABots");
            }
            else if(r[0].Equals("error"))
            {
                TempData[r[0]] = String.Format("QnA-bot {0} ble ikke registrert.", r[1]);
                return RedirectToAction("QnABots");
            }
            else
            {
                TempData[r[0]] = String.Format("Noe gikk galt", r[1]);
                return RedirectToAction("QnABots");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABaseDetails(int id)
        {
            var q = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
            return View(q);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNewQnAKnowledgeBaseAsync(int id)
        {
            var q = new QnAKnowledgeBase
            {
                QnABotId = id
            };
            return View(q);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewQnAKnowledgeBaseAsync([FromForm][Bind("QnABotId", "QnAKnowledgeName")]QnAKnowledgeBase b)
        {
            var r = await chatbotRepository.AddNewQnAKnowledgeBaseAsync(b);
            if (r)
            {
                TempData["success"] = String.Format("Kunnskapsbase med navn {0} er registrert!", b.QnAKnowledgeName);
                return RedirectToAction("QnABots", new { id = b.QnABotId });
            }
            else
            {
                TempData["error"] = String.Format("Kunnskapsbase med navn {0} ble ikke registrert!", b.QnAKnowledgeName);
                return RedirectToAction("QnABots", new { id = b.QnABotId });
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQnAPairsToBaseAsync()
        {
            var q = await chatbotRepository.GetActiveQnABaseClassAsync();
            var b = await chatbotRepository.GetActiveQnAKnowledgeBaseAsync();

            var t = new QnATrainBase
            {
                KnowledgeBaseId = b.KnowledgeBaseID,
                KnowledgeBaseName = b.QnAKnowledgeName,
                SubscriptionKey = q.subscriptionKey,
                QnABotName = q.chatbotName
            };

            return View(t);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQnAPairsToBaseAsync([FromForm][Bind("Query", "Answer", "SubscriptionKey", "KnowledgeBaseId", "QnABotName", "KnowledgeBaseName")] QnATrainBase qna)
        {
            var r = await chatbotRepository.AddSingleQnAPairToBaseAsync(qna);
            if (r)
            {
                TempData["success"] = String.Format("Kunnskapsbase med navn {0} er oppdatert med nytt QnA par!", qna.QnABotName);
                return RedirectToAction("AddQnAPairsToBaseAsync");
            }
            else
            {
                TempData["error"] = String.Format("Kunnskapsbase med navn {0} er ikke oppdatert med nytt QnA par!", qna.QnABotName);
                return RedirectToAction("AddQnAPairsToBaseAsync");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQnAKnowledgeBaseByIdAsync(int id)
        {
            var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
            var r = await chatbotRepository.DeleteQnAKnowledgeBaseByIdAsync(id);
            if (r)
            {

                TempData["success"] = String.Format("Kunnskapsbase med navn {0} er slettet!", b.QnAKnowledgeName);
                return RedirectToAction("QnABots", new { id = b.QnABotId });
            }
            else
            {
                TempData["error"] = String.Format("Kunnskapsbase med navn {0} er ikke slettet!", b.QnAKnowledgeName);
                return RedirectToAction("QnABots", new { id = b.QnABotId });
            }
            
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ViewUnPublishedQnAPairs(int id)
        {

            var qnaPairs = await chatbotRepository.GetUnPublishedQnAPairsAsync(id);
            return View(qnaPairs);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewPublishedQnAPairs(int id)
        {

            var qnaPairs = await chatbotRepository.GetPublishedQnAPairsAsync(id);
            return View(qnaPairs);
        }


    }
}