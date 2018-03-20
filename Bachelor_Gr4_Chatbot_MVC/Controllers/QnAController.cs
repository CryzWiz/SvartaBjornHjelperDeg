using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Net.Http.Headers;
using Bachelor_Gr4_Chatbot_MVC.Models.Repositories;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;

namespace Bachelor_Gr4_Chatbot_MVC.Controllers
{

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class QnAController : Controller
    {
        private IQnARepository repository;

        public QnAController(IQnARepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody][Bind("query")] QnAIndexViewModel Qna)
        {
            var result = await repository.PostQuery(Qna.query);
            return Json(result);
        }
    }
}