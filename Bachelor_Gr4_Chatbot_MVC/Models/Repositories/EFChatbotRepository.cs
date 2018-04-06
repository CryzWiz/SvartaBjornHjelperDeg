using Bachelor_Gr4_Chatbot_MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public class EFChatbotRepository : IChatbotRepository
    {

        private ApplicationDbContext db;
        private IQnARepository qnaRepository;
        private UserManager<ApplicationUser> manager;

        public EFChatbotRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IQnARepository qnARepository)
        {
            this.db = db;
            this.qnaRepository = qnARepository;
            manager = userManager;
        }

        /// <summary>
        /// Microsoft Bot Framework methods are gathered below
        /// </summary>
        


        public async Task<List<ChatbotDetails>> GetAllChatbots()
        {
            //var c = await Task.Run(() => db.ChatbotDetails);
            var r = await (from c in db.ChatbotDetails
                           select new ChatbotDetails
                           {
                               chatbotId = c.chatbotId,
                               chatbotName = c.chatbotName,
                               isActive = c.isActive,
                               regDate = c.regDate,
                               lastEdit = c.lastEdit,
                               contentType = c.contentType,
                               baseUrl = c.baseUrl,
                               tokenUrlExtension = c.tokenUrlExtension,
                               conversationUrlExtension = c.conversationUrlExtension,
                               botAutorizeTokenScheme = c.botAutorizeTokenScheme,
                               BotSecret = c.BotSecret
                           }).ToListAsync();

            return r;
        }

        public async Task<bool> RegisterNewChatbot(ChatbotDetails chatbotDetails)
        {
            chatbotDetails.contentType = "application/json";
            chatbotDetails.baseUrl = "https://directline.botframework.com";
            chatbotDetails.conversationUrlExtension = "/v3/directline/conversations/";
            chatbotDetails.conversationUrlExtensionEnding = "/activities";
            chatbotDetails.tokenUrlExtension = "/v3/directline/tokens/generate";
            chatbotDetails.botAutorizeTokenScheme = "Bearer";
            chatbotDetails.regDate = DateTime.Now;
            chatbotDetails.lastEdit = DateTime.Now;
            chatbotDetails.isActive = false;
            db.ChatbotDetails.Add(chatbotDetails);
            if (await db.SaveChangesAsync() < 0) return true;
            else return false;

        }

        public async Task<ChatbotDetails> GetChatbotDetails(int id)
        {
            var c = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.chatbotId == id));
            return c;
        }

        public async Task<List<ChatbotTypes>> GetAllTypes()
        {
            List<ChatbotTypes> s = await db.ChatbotTypes.ToListAsync();
            return s;
        }

        public async Task<bool> DeleteChatbot(ChatbotDetails chatbot)
        {
            var r = db.Remove(chatbot);
            if (await db.SaveChangesAsync() > 0) return true;
            else return false;
        }

        public async Task<bool> UpdateChatbotDetails(ChatbotDetails chatbotDetails)
        {
            var c = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.chatbotId == chatbotDetails.chatbotId));

            c.chatbotName = chatbotDetails.chatbotName;
            //c.baseUrl = chatbotDetails.baseUrl;
            //c.botAutorizeTokenScheme = chatbotDetails.botAutorizeTokenScheme;
            c.BotSecret = chatbotDetails.BotSecret;
            //c.contentType = chatbotDetails.contentType;
            //c.conversationUrlExtension = chatbotDetails.conversationUrlExtension;
            //c.conversationUrlExtensionEnding = chatbotDetails.conversationUrlExtensionEnding;
            //c.isActive = chatbotDetails.isActive;
            //c.tokenUrlExtension = chatbotDetails.tokenUrlExtension;
            c.lastEdit = DateTime.Now;


            await Task.Run(() => db.ChatbotDetails.Update(c));

            if (await db.SaveChangesAsync() > 0) return true;
            else return false;
        }

        public async Task<bool> CheckIfBotActive(int id)
        {
            var c = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.chatbotId == id));
            if (c.isActive)
                return true;
            else
                return false;
        }

        public async Task<string[]> ActivateBot(int id)
        {
            string[] r = new string[2];
            var isActive = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.isActive == true));
            if (isActive != null)
            {
                isActive.isActive = false;
                db.Update(isActive);
                db.SaveChanges();
            }

            var c = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.chatbotId == id));
            c.isActive = true;
            db.Update(c);
            if (db.SaveChanges() > 0)
            {
                r[0] = "true";
                r[1] = c.chatbotName;
                return r;
            }
            else
            {
                r[0] = "false";
                r[1] = c.chatbotName;
                return r;
            }
        }

        public async Task<ChatbotDetails> GetActiveBot()
        {
            var isActive = await Task.Run(() => db.ChatbotDetails.FirstOrDefault(X => X.isActive == true));
            return isActive;
        }



        /// <summary>
        /// QnA methods are gathered below
        /// </summary>
 


        public async Task<List<QnABaseClass>> GetAllQnABots()
        {
            var q = await db.QnABaseClass.ToListAsync();
            return q;
        }

        public async Task<QnADetails> GetQnABotDetails(int id)
        {
            var qna = await Task.Run(() => db.QnABaseClass.FirstOrDefault(x => x.QnAId == id));
            var bases = await db.QnAKnowledgeBase.Where(x => x.QnABotId == id).ToListAsync();

            var r = new QnADetails
            {
                QnAId = qna.QnAId,
                ChatbotName = qna.chatbotName,
                RegDate = qna.regDate,
                LastEdit = qna.lastEdit,
                IsActive = qna.isActive,
                SubscriptionKey = qna.subscriptionKey,
                KnowledgeBases = bases
            };

            return r;
        }

        public async Task<QnABaseClass> GetQnABotByNameAsync(string name)
        {
            var qna = await Task.Run(() => db.QnABaseClass.FirstOrDefault(x => x.chatbotName == name));
            return qna;
        }

        public async Task<QnABaseClass> GetQnABotByIdAsync(int id)
        {
            var q = await db.QnABaseClass.FirstOrDefaultAsync(X => X.QnAId == id);
            return q;
        }

        public async Task<string[]> RegisterNewQnABotAsync(QnABaseClass qnabot)
        {
            string[] r = new string[3];
            var qna = new QnABaseClass
            {
                chatbotName = qnabot.chatbotName,
                regDate = DateTime.Now,
                lastEdit = DateTime.Now,
                isActive = false,
                subscriptionKey = qnabot.subscriptionKey,
                knowledgeBaseID = qnabot.knowledgeBaseID
            };
            await db.QnABaseClass.AddAsync(qna);
            if(await db.SaveChangesAsync() > 0)
            {
                r[0] = "success";
                r[1] = qna.chatbotName;
            }
            else
            {
                r[0] = "error";
                r[1] = qna.chatbotName;
            }

            var q = await GetQnABotByNameAsync(qna.chatbotName);
            if(qna.knowledgeBaseID != null)
            {
                var klb = new QnAKnowledgeBase
                {
                    KnowledgeBaseID = q.knowledgeBaseID,
                    QnABotId = q.QnAId,
                    RegDate = DateTime.Now,
                    LastEdit = DateTime.Now,
                    IsActive = false,
                    QnAKnowledgeName = "navn mangler"

                };

                if(await RegisterNewQnAKnowledgeBaseAsync(klb))
                {
                    r[2] = "success";
                    return r;
                }
                else
                {
                    r[2] = "error";
                    return r;
                }
            }
            else
            {
                r[2] = "error";
                return r;
            }
        }

        private async Task<bool> RegisterNewQnAKnowledgeBaseAsync(QnAKnowledgeBase klb)
        {
            
            await db.QnAKnowledgeBase.AddAsync(klb);
            var r = await db.SaveChangesAsync();
            if(r < 0)
                return true;
            else
                return false;
        }

        public async Task<QnAKnowledgeBase> GetQnAKnowledgeBaseAsync(int id)
        {
            var q = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.QnAKnowledgeBaseId == id);
            return q;
        }

        public async Task<bool> AddNewQnAKnowledgeBaseAsync(QnAKnowledgeBase b)
        {
            string[] result = new string[2];

            var q = await GetQnABotByIdAsync(b.QnABotId);

            b.RegDate = DateTime.Now;
            b.LastEdit = DateTime.Now;
            b.IsActive = false;

            var r = await qnaRepository.RegisterNewQnAKnowledgeBaseAsync(q, b);
            if(r != null)
            {
                b.KnowledgeBaseID = r;
                await db.AddAsync(b);
                if (await db.SaveChangesAsync() > 0)
                    return true;
                else
                    return false;
            }
            else return false;

        }
    }

}
