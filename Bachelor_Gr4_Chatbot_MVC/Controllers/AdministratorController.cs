﻿using System;
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
using Bachelor_Gr4_Chatbot_MVC.Models.ChatViewModels;

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
        /// Display the dashboard for the admin
        /// </summary>
        /// <returns>All users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var c = await chatbotRepository.GetActiveQnABaseClassAsync();
            var b = await chatbotRepository.GetActiveQnAKnowledgeBaseAsync();
            var model = new AdministratorIndexViewModel();
            if(c != null)
            {
                model.ChatbotName = c.chatbotName;
                model.ChatbotId = c.QnAId;
                model.KnowledgeBaseId = b.QnAKnowledgeBaseId;
                model.UnPublishedQnAPairs = await chatbotRepository.GetPublishedQnAPairsToActiveBotAsync();
            }
            else
            {
                model.ChatbotId = 0;
            }

            return View(model);
        }

        /// <summary>
        /// Demo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult MorrisCharts()
        {

            return View();
        }

        /// <summary>
        /// Demo
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RegisterNewUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            RegisterNewUserViewModel model = new RegisterNewUserViewModel();
            return View(model);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="model">The user to be registered</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deactivate useraccount
        /// </summary>
        /// <param name="username">username to deactivate</param>
        /// <returns></returns>
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

        /// <summary>
        /// Activate useraccount
        /// </summary>
        /// <param name="username">username to activate</param>
        /// <returns></returns>
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterChatGroup()
        {
            return View(new RegisterChatGroupViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterChatGroup(RegisterChatGroupViewModel model)
        {
            if(ModelState.IsValid)
            {
                ChatGroup chatGroup = new ChatGroup
                {
                    ChatGroupName = model.ChatGroupName
                };

                var result = await repository.AddChatGroup(chatGroup);
                if(result)
                {
                    TempData["success"] = String.Format("Chat gruppen: '{0}' ble opprettet", model.ChatGroupName);
                    return RedirectToAction("RegisterChatGroup");
                }

            }
            // Something went wrong, show form again
            TempData["error"] = "Feil under oppretting av char gruppe.";
            return View(model);

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChatGroups()
        {
            IEnumerable<ChatGroupViewModel> chatGroups = await repository.GetAllChatGroupsVM();
            return View(chatGroups);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditChatGroup(string id)
        {
            ChatGroup chatGroup = await repository.GetChatGroupByIdAsync(id);
            ChatGroupEditViewModel model = new ChatGroupEditViewModel
            {
                ChatGroupId = chatGroup.ChatGroupId,
                ChatGroupName = chatGroup.ChatGroupName
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateChatGroup(ChatGroupEditViewModel model)
        {
            if(ModelState.IsValid)
            {
                ChatGroup chatGroup = new ChatGroup
                {
                    ChatGroupId = model.ChatGroupId,
                    ChatGroupName = model.ChatGroupName
                };
                bool success = await repository.UpdateChatGroupAsync(chatGroup);
                if(success)
                {
                    TempData["success"] = "Chat gruppe ble oppdatert";
                    return RedirectToAction("ChatGroups");
                }
 
            }
            // Something went wrong, show form again
            TempData["error"] = "Feil under oppdatering av chat gruppe";
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteChatGroup(string id)
        {
            bool success = await repository.DeleteChatGroupAsync(id);
            if(success)
            {
                TempData["success"] = "Chat gruppe ble slettet";
                return RedirectToAction("ChatGroups");
            }
            // Something went wrong, show form again
            TempData["error"] = "Feil under sletting av chat gruppe";
            return RedirectToAction("ChatGroups");
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


        ///////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Activate the given QnAKnowledgebase
        /// </summary>
        /// <param name="id"><int>id for given knowledgebase</int></param>
        /// <returns>True if activated, false if not</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateQnAKnowledgeBase(int id)
        {
            var r = await chatbotRepository.ActivateQnAKnowledgeBaseAsync(id);
            var b = await chatbotRepository.GetActiveQnAKnowledgeBaseAsync();
            if (r)
            {
                TempData["success"] = String.Format("Kunnskapsbase {0} er nå aktiv", b.QnAKnowledgeName);
                return RedirectToAction("QnABotDetails", new { id = b.QnABotId });
            }
            else
            {
                TempData["error"] = String.Format("Kunnskapsbase {0} er fortsatt aktiv. Aktivering feilet!", b.QnAKnowledgeName);
                return RedirectToAction("QnABotDetails", new { id = b.QnABotId });
            }
        }

        /// <summary>
        /// Fetch all the QnAbots in the database and display them
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABots()
        {
            var q = await chatbotRepository.GetAllQnABots();
            return View(q);
        }

        /// <summary>
        /// Fetch details for QnABot and display them
        /// </summary>
        /// <param name="id">for QnABot</param>
        /// <returns>View with QnABot details</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABotDetails(int id)
        {
            var q = await chatbotRepository.GetQnABotDetails(id);
            return View(q);
        }

        /// <summary>
        /// Register a new chatbot view
        /// </summary>
        /// <returns>Register a new chatbotview</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterNewQnaBotAsync()
        {
            return View();
        }

        /// <summary>
        /// Register a new chatbot and return to QnABot View
        /// </summary>
        /// <param name="qnabot">QnABot to be registered</param>
        /// <returns>QnAbot View</returns>
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

        /// <summary>
        /// Fetch QnABase Details and return the view
        /// </summary>
        /// <param name="id"><int>id for QnABase</int></param>
        /// <returns>View with QnADetails</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QnABaseDetails(int id)
        {
            var q = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
            return View(q);
        }

        /// <summary>
        /// Create a new knowledgebase
        /// </summary>
        /// <param name="id"><int>Id for QnABase the knowledgebase belongs to</int></param>
        /// <returns>View for new knowledgebase</returns>
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

        /// <summary>
        /// Add a new knowledgebase
        /// </summary>
        /// <param name="b"><QnAKnwoledgeBase>QnAKnowledgebase to add</QnAKnwoledgeBase></param>
        /// <returns>Overview for QnABase</returns>
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

        /// <summary>
        /// Add a new QnApair to the active knowledgebase
        /// </summary>
        /// <returns>View for adding a new <QnAPair>qnapair</QnAPair></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQnAPairsToBaseAsync()
        {
            var q = await chatbotRepository.GetActiveQnABaseClassAsync();
            var b = await chatbotRepository.GetActiveQnAKnowledgeBaseAsync();

            var t = new QnATrainBase
            {
                KnowledgeBaseId = b.QnAKnowledgeBaseId,
                KnowledgeBaseName = b.QnAKnowledgeName,
                SubscriptionKey = q.subscriptionKey,
                QnABotName = q.chatbotName
            };

            return View(t);

        }

        /// <summary>
        /// Add a new QnAPair to the active knowledgebase
        /// </summary>
        /// <param name="qna">QnAPair to be added</param>
        /// <returns>view for adding a new <QnAPair>qnapair</QnAPair></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQnAPairsToBaseAsync([FromForm][Bind("Query", "Answer", "Dep", "SubscriptionKey", "KnowledgeBaseId", "QnABotName", "KnowledgeBaseName")] QnATrainBase qna)
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

        /// <summary>
        /// Delete the given QnAKnowledgebase
        /// </summary>
        /// <param name="id"><int>Id for the knowledgebase to be deleted</int></param>
        /// <returns>QnABot details view</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQnAKnowledgeBaseByIdAsync(int id)
        {
            var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
            if (!b.IsActive)
            {
                var r = await chatbotRepository.DeleteQnAKnowledgeBaseByIdAsync(id);
                if (r)
                {

                    TempData["success"] = String.Format("Kunnskapsbase med navn {0} er slettet!", b.QnAKnowledgeName);
                    return RedirectToAction("QnABots", new { id = b.QnABotId });
                }
                else
                {
                    TempData["error"] = String.Format("Kunnskapsbase med navn {0} er ikke slettet da noe gikk galt!", b.QnAKnowledgeName);
                    return RedirectToAction("QnABots", new { id = b.QnABotId });
                }
            }
            else
            {
                TempData["error"] = String.Format("Kunnskapsbase med navn {0} er ikke slettet siden den er aktiv!", b.QnAKnowledgeName);
                return RedirectToAction("QnABots", new { id = b.QnABotId });
            }
            
            
        }

        /// <summary>
        /// View all unpublished QnAPairs
        /// </summary>
        /// <param name="id"><int>id for QnAKnowledgeBase</int></param>
        /// <returns>All unpublished QnAPairs</returns>
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ViewUnPublishedQnAPairs(int id)
        {
            // Fetch UnPublished QnAPairs
            var p = await chatbotRepository.GetUnPublishedQnAPairsAsync(id);
            // Fetch KnowledgeBase
            var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);

            QnAPairsView QnAPairs = new QnAPairsView
            {
                QnAId = b.QnABotId,
                QnAKnowledgeBaseId = id,
                QnAKnowledgeBaseName = b.QnAKnowledgeName,
                QnAPairs = p
            };
            return View(QnAPairs);
        }

        /// <summary>
        /// View published qnapairs 
        /// </summary>
        /// <param name="id"><int>id for QnAKnowledgebase</int></param>
        /// <returns>All QnAPairs published for given knowledgebase</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewPublishedQnAPairs(int id)
        {
            // Fetch published QnAPairs
            var p = await chatbotRepository.GetPublishedQnAPairsAsync(id);
            // Fetch KnowledgeBase
            var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);

            QnAPairsView QnAPairs = new QnAPairsView
            {
                QnAId = b.QnABotId,
                QnAKnowledgeBaseId = id,
                IsActive = b.IsActive,
                QnAKnowledgeBaseName = b.QnAKnowledgeName,
                QnAPairs = p
            };
            return View(QnAPairs);
        }

        /// <summary>
        /// Publish all changes to the knowledgebase
        /// </summary>
        /// <param name="id"><int>Id for knowledgebase</int></param>
        /// <returns>QnABaseDetails for knowledgebase</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PublishTrainedQnAPairs(int id)
        {
            var r = await chatbotRepository.PublishTrainedQnAPairs(id);
            if (r)
            {
                var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
                TempData["success"] = String.Format("Kunnskapsbase med navn {0} er publisert!", b.QnAKnowledgeName);
                return RedirectToAction("QnABaseDetails", new { id = b.QnAKnowledgeBaseId });
            }
            else
            {
                var b = await chatbotRepository.GetQnAKnowledgeBaseAsync(id);
                TempData["error"] = String.Format("Kunnskapsbase med navn {0} ble ikke publisert!", b.QnAKnowledgeName);
                return RedirectToAction("QnABaseDetails", new { id = b.QnAKnowledgeBaseId });
            }
        }

        /// <summary>
        /// Verify the local db QnAPairs to the QnAMaker.ai db and store the missing QnAPairs to the local db
        /// </summary>
        /// <param name="id"><int>id for knowledgebase to verify</int></param>
        /// <returns>QnABaseDetails view</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyLocalKnowledgeBaseToOnlineKnowledgeBase(int id)
        {
            var r = await chatbotRepository.VerifyLocalDbToPublishedDb(id);
            //TempData["success"] = String.Format("{0}", r);
            //return RedirectToAction("QnABaseDetails", new { id = id });

            if (r == 0)
            {
                TempData["success"] = String.Format("Databasen er verifisert", r);
                return RedirectToAction("QnABaseDetails", new { id = id });
            }
            else if (r < 0)
            {
                TempData["error"] = String.Format("Noe gikk galt eller ingen nye QnA par funnet.", r);
                return RedirectToAction("QnABaseDetails", new { id = id });
            }
            else
            {
                TempData["success"] = String.Format("{0} QnA par ble lagt til i databasen", r);
                return RedirectToAction("QnABaseDetails", new { id = id });
            }
        }

        /// <summary>
        /// Delete the given QnAPair from local db and the QnAMaker.ai db
        /// </summary>
        /// <param name="id"><int>Id for QnAPair to delete</int></param>
        /// <returns>ViewPublishedQnAPairs</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQnAPair(int id)
        {
            var b = await chatbotRepository.GetKnowledgebaseIdToQnAPair(id);
            var r = await chatbotRepository.DeleteQnAPair(id);
            

            if (r)
            {
                TempData["success"] = String.Format("QnA paret {0} er slettet", id);
                return RedirectToAction("ViewPublishedQnAPairs", new { id = b });
            }
            else
            {
                TempData["error"] = String.Format("Noe gikk galt..QnA paret {0} ble ikke slettet", id);
                return RedirectToAction("ViewPublishedQnAPairs", new { id = b });
            }
            
        }

        /// <summary>
        /// Edit info for QnAPair
        /// </summary>
        /// <param name="id"><int>id for <QnAPair>QnaPair</QnAPair> to edit</int></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQnAPair(int id)
        {
            var m = await chatbotRepository.GetSingleQnAPairAsync(id);
            return View(m);
        }

        /// <summary>
        /// Edit info for QnAPair
        /// </summary>
        /// <param name="id"><int>id for <QnAPair>QnaPair</QnAPair> to edit</int></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditQnAPair([FromForm][Bind("QnAPairsId", "Query", "Answer", "Dep", "KnowledgeBaseId")]QnAPairs qna)
        {
            var m = await chatbotRepository.UpdateQnAPairAsync(qna);
            if (ModelState.IsValid)
            {
                TempData["success"] = String.Format("QnA par ble oppdatert");
                return RedirectToAction("ViewPublishedQnAPairs", new { id = qna.KnowledgeBaseId });
            }
            else
            {
                TempData["error"] = String.Format("QnA par ble ikke oppdatert");
                return RedirectToAction("ViewPublishedQnAPairs", new { id = qna.KnowledgeBaseId });
            }
        }

        /// <summary>
        /// View all conversations with active bot
        /// </summary>
        /// <returns>ViewConversationWithActiveBotAsync</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewConversationsWithActiveBot()
        {
            var conversations = await chatbotRepository.GetConversationsWithActiveBotAsync();
            return View(conversations);
        }

        /// <summary>
        /// View all conversation details. That means all the messages
        /// </summary>
        /// <param name="id"><int>id for conversation to view</int></param>
        /// <returns>ViewConversationDetails</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewConversationDetails(int id)
        {
            var messages = await chatbotRepository.GetMessagesForConversationAsync(id);
            var conversation = await chatbotRepository.GetConversationByIdAsync(id);

            ViewMessagesForConversation viewmodel = new ViewMessagesForConversation
            {
                messages = messages,
                conversation = conversation
            };

            return View(viewmodel);
        }

        /// <summary>
        /// Fetch and display a single message. Purpose is to add a desired answer to that question/message
        /// and train it to the active bot
        /// </summary>
        /// <param name="q"><int>id for given message/question</int></param>
        /// <returns>View with question</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAndStoreMessagePairToBot(int q)
        {
            //var knowledgebase = await chatbotRepository.GetActiveQnAKnowledgeBaseAsync();
            var message = await chatbotRepository.GetSingleMessageByIdAsync(q);

            var model = new QnAEditAndStoreMessagePairToBot
            {
                Question = message.Content,
                Answer = "",
                ConversationId = message.ConversationId
            };

            return View(model);
        }

        /// <summary>
        /// Recive QnAPair from the Get EditAndStoreMessagePairToBot method and
        /// store the QnAPair to the active knowledgebase
        /// </summary>
        /// <param name="q"><int>id for given message/question</int></param>
        /// <returns>View with question</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAndStoreMessagePairToBot([FromForm][Bind("Question", "Answer", "Dep", "ConversationId")] QnAEditAndStoreMessagePairToBot qna)
        {
            var c = await chatbotRepository.GetConversationByIdAsync(qna.ConversationId);
            var knowledgebase = await chatbotRepository.GetQnAKnowledgeBaseAsync(c.KnowledgebaseId);
            var qnabot = await chatbotRepository.GetQnABaseClassById(knowledgebase.QnABotId);
            var QnA = new QnATrainBase
            {
                Query = qna.Question,
                Answer = qna.Answer,
                Dep = qna.Type,
                QnABotName = qnabot.chatbotName,
                KnowledgeBaseId = knowledgebase.QnAKnowledgeBaseId,
                KnowledgeBaseName = knowledgebase.QnAKnowledgeName,
                SubscriptionKey = qnabot.subscriptionKey
            };
            var message = await chatbotRepository.AddSingleQnAPairToBaseAsync(QnA);

            if (message)
            {
                TempData["success"] = String.Format("QnA paret er lagt til kunnskapsbase {0}", knowledgebase.QnAKnowledgeName);
                return RedirectToAction("ViewConversationDetails", new { id = qna.ConversationId });
            }
            else
            {
                TempData["error"] = String.Format("Klarte ikke å legge QnA til kunnskapsbase {0}", knowledgebase.QnAKnowledgeName);
                return RedirectToAction("ViewConversationDetails", new { id = qna.ConversationId });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewConversationsAndTrainBot(int id)
        {
            List<Conversation> conversations = await chatbotRepository.GetAllConversationsForKnowledgeBase(id);
            return View(conversations);
        }

    }
}