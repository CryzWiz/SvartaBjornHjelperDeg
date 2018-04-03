﻿using Bachelor_Gr4_Chatbot_MVC.Data;
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
        private UserManager<ApplicationUser> manager;

        public EFChatbotRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            this.db = db;
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
    }
}
