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
    /// <summary>
    /// Repository for all interactions with the chatbot. If the interaction demands data
    /// to be transfere to QnAMaker.ai this repository will use the QnArepository to perform the calls. 
    /// </summary>
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



        /// QnA methods are gathered below, but all communication to the QnA API is
        /// put in its own repository - EFQnARepository. So you can with ease change the API calls without
        /// changing the logistic in this file.

 

        ///<summary>
        /// Method for fetching all the registered chatbots from the database
        ///</summary>
        ///<returns>A list of all the registered chatbots</returns>
        public async Task<List<QnABaseClass>> GetAllQnABots()
        {
            var q = await db.QnABaseClass.ToListAsync();
            return q;
        }

        /// <summary>
        /// Fetch all details for the given chatbot
        /// </summary>
        /// <param name="id">Database id for chatbot</param>
        /// <returns>QnADetails Chatbotdetails</returns>
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

        /// <summary>
        /// Fetch chatbot by name
        /// </summary>
        /// <param name="name">The stored name for the chatbot</param>
        /// <returns>QnABaseClass chatbot</returns>
        public async Task<QnABaseClass> GetQnABotByNameAsync(string name)
        {
            var qna = await Task.Run(() => db.QnABaseClass.FirstOrDefault(x => x.chatbotName == name));
            return qna;
        }

        /// <summary>
        /// Fetch chatbot by database-id
        /// </summary>
        /// <param name="id">database id for chatbot</param>
        /// <returns>QnABaseClass chatbot</returns>
        public async Task<QnABaseClass> GetQnABotByIdAsync(int id)
        {
            var q = await db.QnABaseClass.FirstOrDefaultAsync(X => X.QnAId == id);
            return q;
        }

        /// <summary>
        /// Register a new chatbot in the database
        /// </summary>
        /// <param name="qnabot">QnABaseClass chatbot to be registered</param>
        /// <returns>r[0] = operationresult, r[1] = chatbotname, r[2] = result from QnAKnowledgeBase creation</returns>
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

        /// <summary>
        /// Register a new knowledgebase in the database. This only happens if you
        /// register a new chatbot with a knowledgebase already exsisting
        /// </summary>
        /// <param name="klb">knowledgebase id from microsoft</param>
        /// <returns>true if ok, false if not</returns>
        private async Task<bool> RegisterNewQnAKnowledgeBaseAsync(QnAKnowledgeBase klb)
        {
            
            await db.QnAKnowledgeBase.AddAsync(klb);
            var r = await db.SaveChangesAsync();
            if(r < 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Fetch QnAKnowledgebase from db
        /// </summary>
        /// <param name="id">knowledgebase id in database</param>
        /// <returns>found QnAKnowledgeBase</returns>
        public async Task<QnAKnowledgeBase> GetQnAKnowledgeBaseAsync(int id)
        {
            QnAKnowledgeBase q = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.QnAKnowledgeBaseId == id);
            List<QnAPairs> qPairs = await db.QnAPairs.Where(X => X.KnowledgeBaseId == q.QnAKnowledgeBaseId).ToListAsync();
            q.QnAPairs = qPairs;
            var conversationCount = await Task.Run(() => db.Conversations.Count(x => x.KnowledgebaseId == q.QnAKnowledgeBaseId));
            q.ConversationCount = conversationCount;
            return q;
        }

        /// <summary>
        /// Fetch active knowledgebase from db
        /// </summary>
        /// <returns>the active QnAKnowledgeBase</returns>
        public async Task<QnAKnowledgeBase> GetActiveQnAKnowledgeBaseAsync()
        {
            var q = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.IsActive == true);
            return q;
        }

        /// <summary>
        /// Add a new QnAKnowledgebase to the database, and call EFQnARepository to
        /// create the new knowledgebase to QnAMaker.ai
        /// </summary>
        /// <param name="b">QnAKnowledgeBase name</param>
        /// <returns>true if registered, false if not</returns>
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

        /// <summary>
        /// Delete the given QnAKnowledgeBase in the database, and call EFQnARepository
        /// to delete the knowledgebase on QnAMaker.ai
        /// </summary>
        /// <param name="id">knowledgeid in database</param>
        /// <returns></returns>
        public async Task<bool> DeleteQnAKnowledgeBaseByIdAsync(int id)
        {
            var b = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.QnAKnowledgeBaseId == id);
            var q = await db.QnABaseClass.FirstOrDefaultAsync(X => X.QnAId == b.QnABotId);
            var pairs = await db.QnAPairs.Where(x => x.KnowledgeBaseId == b.QnAKnowledgeBaseId).ToListAsync();
            //var r = await qnaRepository.DeleteKnowledgeBase(q, b);
            // First remove the knowledgebase
            db.Remove(b);
            if (await db.SaveChangesAsync() > 0)
            {
                // then remove all pairs we might have
                if (pairs.Count() > 0)
                {
                    db.RemoveRange(pairs);
                    await db.SaveChangesAsync();
                }
                // Fetch conversations and messages
                var conversations = await db.Conversations.Where(x => x.KnowledgebaseId == id).ToListAsync();
                var messages = new List<Message>();
                // If there was any conversations
                if (conversations.Count() > 0)
                {
                    // Loop trough them and get all the messages
                    foreach (Conversation c in conversations)
                    {
                        var m = await db.Messages.Where(x => x.ConversationId == c.ConversationId).ToListAsync();
                        messages.AddRange(m);
                    }
                    // Remove the messages
                    db.RemoveRange(messages);
                    await db.SaveChangesAsync();
                    // Remove the conversations
                    db.RemoveRange(conversations);
                    await db.SaveChangesAsync();
                }
                // Finally delete the knowledgebase at QnAMaker.ai
                var r = await qnaRepository.DeleteKnowledgeBase(q, b);
                if (r)
                {
                    return true;
                }
                else
                    return true; // If this happens, the knowledgebase is deleted localy
            }                    // but not at QnAMaker.ai
            else
                return false; // Failed to delete knowledgebase in db
        }

        /// <summary>
        /// Get the active QnABaseClass
        /// </summary>
        /// <returns>active QnABaseClass</returns>
        public async Task<QnABaseClass> GetActiveQnABaseClassAsync()
        {
            var q = await db.QnABaseClass.FirstOrDefaultAsync(X => X.isActive == true);
            return q;
        }

        /// <summary>
        /// Add a single QnA pair to the database, and call EFQnARepository to 
        /// add the pair to knowledgebase on QnAMaker.ai
        /// </summary>
        /// <param name="qna">the QnA to be added</param>
        /// <returns>true if added, false if not</returns>
        public async Task<bool> AddSingleQnAPairToBaseAsync(QnATrainBase qna)
        {
            var r = await qnaRepository.AddSingleQnAPairAsync(qna);
            if (r)
            {
                QnAPairs pair = new QnAPairs
                {
                    Query = qna.Query,
                    Answer = qna.Answer,
                    Dep = qna.Dep,
                    KnowledgeBaseId = qna.KnowledgeBaseId,
                    Trained = true,
                    TrainedDate = DateTime.Now,
                    Published = false,
                    PublishingType = "add"
                };

                await db.AddAsync(pair);
                await db.SaveChangesAsync();

                return true;
            }
            else return false;
        }

        /// <summary>
        /// Delete the given QnA pair from knowledgebase db. If it is published, delete it from the
        /// QnAmake.ai base aswell.
        /// </summary>
        /// <param name="id">QnAPair to be deleted</param>
        /// <returns>true if deleted, false if not</returns>
        public async Task<bool> DeleteQnAPair(int id)
        {
            var qnaPair = await db.QnAPairs.FirstOrDefaultAsync(X => X.QnAPairsId == id);

            if (qnaPair.Published)
            {
                var r = await qnaRepository.DeleteSingleQnAPairAsync(qnaPair);
                if (r)
                {
                    qnaPair.Published = false;
                    qnaPair.PublishingType = "delete";
                    db.Update(qnaPair);
                    if (await db.SaveChangesAsync() > 0) return true;
                    else return false;
                }
                else return false;
            }
            else
            {
                db.Remove(qnaPair);
                if (await db.SaveChangesAsync() > 0) return true;
                else return false;
            }
        }

        /// <summary>
        /// Fetch the correct QnABaseClass from the given QnAMaker.ai subcription key
        /// </summary>
        /// <param name="subKey">subscription key</param>
        /// <returns>QnABaseClass</returns>
        public async Task<QnABaseClass> GetQnABotDetailsBySubscriptionAsync(string subKey)
        {
            var q = await db.QnABaseClass.FirstOrDefaultAsync(X => X.subscriptionKey == subKey);
            return q;
        }

        /// <summary>
        /// Fetch the correct QnAKnowledgeBase from the given QnAMaker.ai knowledgebase id
        /// </summary>
        /// <param name="knowledgeBaseId">knowledgebase id</param>
        /// <returns>QnAKnowledgeBase</returns>
        public async Task<QnAKnowledgeBase> GetQnAKnowledgeBaseByKnowledgeIdAsync(string knowledgeBaseId)
        {
            var b = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.KnowledgeBaseID == knowledgeBaseId);
            return b;
        }

        /// <summary>
        /// Fetch all QnAPairs belonging to a given knowledgebase
        /// </summary>
        /// <param name="id">knowledgebase id in knowledgebase</param>
        /// <returns>all QnAPairs for given knowledgebase</returns>
        public async Task<List<QnAPairs>> GetQnAPairsByKnowledgeBaseIdAsync(int id)
        {
            var b = await GetQnAKnowledgeBaseAsync(id);
            return await db.QnAPairs.Where(X => X.KnowledgeBaseId == b.QnAKnowledgeBaseId).ToListAsync();
              
        }

        /// <summary>
        /// Fetch all the unpublished QnAPairs for the given knowledgebase
        /// </summary>
        /// <param name="id">knowledge database id</param>
        /// <returns>List<QnAPairs>unpublished</QnAPairs></returns>
        public async Task<List<QnAPairs>> GetUnPublishedQnAPairsAsync(int id)
        {
            var b = await GetQnAKnowledgeBaseAsync(id);
            return await db.QnAPairs.Where(X => X.KnowledgeBaseId == b.QnAKnowledgeBaseId && X.Published == false).ToListAsync();
        }

        /// <summary>
        /// Mark all unpublished QnA pairs as published, and call
        /// EFQnARepository to perform the publish method on QnAMaker.ai
        /// </summary>
        /// <param name="knowledgebaseId">QnAMaker.ai knowledgebase id</param>
        /// <returns>true if stored and published, false if not</returns>
        public async Task<bool> PublishTrainedQnAPairs(int knowledgebaseId)
        {
            var r = await qnaRepository.PublishKnowledgeBase(knowledgebaseId);
            if (r)
            {
                var qnaPairs = db.QnAPairs.Where(X => X.KnowledgeBaseId == knowledgebaseId
                && X.Published == false);
                foreach (QnAPairs p in qnaPairs)
                {
                    if (p.PublishingType.Equals("add"))
                    {
                        p.Published = true;
                        p.PublishedDate = DateTime.Now;
                        db.QnAPairs.Update(p);
                    }
                    else
                    {
                        db.QnAPairs.Remove(p);
                    }                  
                }
                await db.SaveChangesAsync();

                var qbase = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.QnAKnowledgeBaseId == knowledgebaseId);
                qbase.LastEdit = DateTime.Now;
                db.QnAKnowledgeBase.Update(qbase);
                db.SaveChanges();

                return true;
            }
            else return false;
            
        }

        /// <summary>
        /// Fetch all QnAPairs to a given knowledgebase
        /// </summary>
        /// <param name="id">knowledgebase id in database</param>
        /// <returns>List<QnAPairs>publishedQnAPairs</QnAPairs></returns>
        public async Task<List<QnAPairs>> GetPublishedQnAPairsAsync(int id)
        {
            var kbase = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.QnAKnowledgeBaseId == id);
            try
            {
                var b = await Task.Run(() => db.QnAPairs.Where(X => X.Published == true
            && X.KnowledgeBaseId == kbase.QnAKnowledgeBaseId));
                if(b != null)
                {
                    var cb = await b.ToListAsync();
                    return cb;
                }
                else
                    return new List<QnAPairs>();
            }
            catch(Exception e){
                return new List<QnAPairs>();
            }
            
        }

        /// <summary>
        /// Fetch all QnAPairs to a given knowledgebase
        /// </summary>
        /// <param name="id">knowledgebase id in database</param>
        /// <returns>List<QnAPairs>allQnAPairs</QnAPairs></returns>
        public async Task<List<QnAPairs>> GetAllQnAPairsAsync(int id)
        {
            var b = await Task.Run(() => db.QnAPairs.Where(X => X.KnowledgeBaseId == id).ToList());
            return b;
        }

        /// <summary>
        /// Check the local knowledgebase to the published knowledgebase and
        /// add the missing QnAPairs to the local db, if any are missing.
        /// This function is added since you can add QnA pairs on QnAMaker.ai. With this function you
        /// can get them and add them to the local db so you can manage them (delete, view)
        /// </summary>
        /// <param name="id">KnowledgeBase id in database</param>
        /// <returns>number of pairs added</returns>
        public async Task<int> VerifyLocalDbToPublishedDb(int id)
        {
            // Fetch all QnAPairs active in the knowledgbase
            var onlineQnA = await qnaRepository.DownloadKnowledgeBase(id);
            // If no where found, return -1. Something is wrong
            if(onlineQnA == null) return -1;
            // Fetch local QnA
            var localQnA = await GetAllQnAPairsAsync(id);

            int number = 0;
            bool present = false;
            // If we already have some QnAPairs in local db
            if (localQnA.Count > 0)
            {
                foreach (QnAPairs external_qna in onlineQnA)
                {
                    for(int i = 0; i < onlineQnA.Count(); i++)
                    {
                        if(external_qna.Answer.ToLower().Equals(onlineQnA[i].Answer.ToLower())
                            && external_qna.Query.ToLower().Equals(onlineQnA[i].Query.ToLower()))
                        {
                            present = true;
                            break;
                        }
                    }
                    if (!present)
                    {
                        var newQnA = new QnAPairs
                        {
                            Query = external_qna.Query,
                            Answer = external_qna.Answer,
                            KnowledgeBaseId = id,
                            Trained = true,
                            Published = true,
                            PublishedDate = DateTime.Now,
                            TrainedDate = DateTime.Now,
                            Dep = "Web-sync - Må oppdateres"
                        };
                        await db.AddAsync(newQnA);
                        await db.SaveChangesAsync();
                        number++;
                    }
                }
            }
            else // Or if we dont have any in the local db, just store them all. No need to check them
            {
                foreach (QnAPairs external_qna in onlineQnA)
                {
                    var newQnA = new QnAPairs
                    {
                        Query = external_qna.Query,
                        Answer = external_qna.Answer,
                        KnowledgeBaseId = id,
                        Trained = true,
                        Published = true,
                        PublishedDate = DateTime.Now,
                        TrainedDate = DateTime.Now,
                        Dep = "Web-sync - Må oppdateres"
                    };
                    await db.AddAsync(newQnA);
                    await db.SaveChangesAsync();
                    number++;
                }
            }
            //update last edit for knowledgebase
            var b = await GetQnAKnowledgeBaseAsync(id);
            b.LastEdit = DateTime.Now;
            db.Update(b);
            db.SaveChanges();
            // Return result
            if (number > 0)
                return number;
            else if (number == 0)
                return 0;
            else
                return -1;

        }

        /// <summary>
        /// Post a query to the active knowledgebase and return the response
        /// </summary>
        /// <param name="comment"></param>
        /// <returns>(string) response</returns>
        public async Task<string> PostToActiveKnowledgeBase(string comment)
        {
            var kbase = await db.QnAKnowledgeBase.FirstOrDefaultAsync(X => X.IsActive == true);
            var response = await qnaRepository.PostCommentToActiveKnowledgebase(comment);
            return response;
        }

        /// <summary>
        /// Fetch the id for the knowledgebase the QnAPair belongs to
        /// </summary>
        /// <param name="id"><int>id for qnapair</int></param>
        /// <returns><int>knowledgebase id</int></returns>
        public async Task<int> GetKnowledgebaseIdToQnAPair(int id)
        {
            var r = await db.QnAPairs.FirstOrDefaultAsync(x => x.QnAPairsId == id);
            return r.KnowledgeBaseId;
        }

        /// <summary>
        /// Get all conversations with the active bot
        /// </summary>
        /// <returns>view over all conversations</returns>
        public async Task<List<Conversation>> GetConversationsWithActiveBotAsync()
        {
            var knowledgebase = await db.QnAKnowledgeBase.FirstOrDefaultAsync(x => x.IsActive == true);
            var conversations = db.Conversations.Where(x => x.KnowledgebaseId == knowledgebase.QnAKnowledgeBaseId).ToList();

            return conversations;
        }

        /// <summary>
        /// Get all the messages for a given conversation
        /// </summary>
        /// <param name="id"><int>id for conversation</int></param>
        /// <returns>View over messages for given conversation</returns>
        public async Task<List<Message>> GetMessagesForConversationAsync(int id)
        {
            var messages = await db.Messages.Where(x => x.ConversationId == id).ToListAsync();
            return messages;
        }

        /// <summary>
        /// Get conversation by id
        /// </summary>
        /// <param name="id"><int>id for conversation</int></param>
        /// <returns><Conversation>found conversation</Conversation></returns>
        public async Task<Conversation> GetConversationByIdAsync(int id)
        {
            var conversation = await db.Conversations.FirstOrDefaultAsync(x => x.ConversationId == id);
            return conversation;
        }


        public async Task<List<Conversation>> GetAllConversationsForKnowledgeBase(int id)
        {
            var c = await Task.Run(() => db.Conversations.Where(x => x.KnowledgebaseId == id).ToList());
            return c;
        }
        /// <summary>
        /// Fetch the number of unpublished qnapairs to the active knowledgebase
        /// </summary>
        /// <returns><int>number of unpublished qnapairs</int></returns>
        public async Task<int> GetPublishedQnAPairsToActiveBotAsync()
        {
            var b = await db.QnAKnowledgeBase.FirstOrDefaultAsync(x => x.IsActive == true);
            var qna = db.QnAPairs.Where(x => x.Published == false && x.KnowledgeBaseId == b.QnAKnowledgeBaseId);
            return qna.Count();
        }

        /// <summary>
        /// Fetch a single message and return it
        /// </summary>
        /// <param name="q"><int>Message id</int></param>
        /// <returns><Message>Message found</Message></returns>
        public async Task<Message> GetSingleMessageByIdAsync(int q)
        {
            var m = await db.Messages.FirstOrDefaultAsync(x => x.MessageId == q);
            return m;
        }

        /// <summary>
        /// Activate the given knowledgebase and return the result
        /// </summary>
        /// <param name="id"><int>id for knowledgebase</int></param>
        /// <returns>true if activated, false if not</returns>
        public async Task<bool> ActivateQnAKnowledgeBaseAsync(int id)
        {
            QnABaseClass q_active;
            QnAKnowledgeBase b_active;

            var b = await db.QnAKnowledgeBase.FirstOrDefaultAsync(x => x.QnAKnowledgeBaseId == id);
            if (!b.IsActive)
            {
                // Find the active knowledgebase and set it to un-active
                b_active = await db.QnAKnowledgeBase.FirstOrDefaultAsync(x => x.IsActive == true);
                b_active.IsActive = false;
                db.Update(b_active);
                await db.SaveChangesAsync();

                // Find the active baseclass (chatbot)
                q_active = await db.QnABaseClass.FirstOrDefaultAsync(x => x.isActive == true);

                // If the QnABotId the knowledgebase belongs to does not match the QnABotId for active bot
                if(b.QnABotId != q_active.QnAId)
                {
                    // Set the active bot to not active
                    q_active.isActive = false;
                    db.Update(q_active);
                    await db.SaveChangesAsync();
                    // find the correct QnABaseclass and set it to active
                    var q = await db.QnABaseClass.FirstOrDefaultAsync(x => x.QnAId == b.QnABotId);
                    q.isActive = true;
                    db.Update(q);
                    await db.SaveChangesAsync();
                }
                // Finally we update the correct knowledgebase with status active
                b.IsActive = true;
                db.Update(b);
                if(await db.SaveChangesAsync() > 0)
                {
                    // All is fine, return true
                    return true;
                }
                else // something failed, and we could not update db. Return false
                {
                    return false;
                }
            }
            else // Knowledgebase is already active, return fail.
            {
                return false;
            }
        }

        /// <summary>
        /// Get QnABaseClass by id
        /// </summary>
        /// <param name="id"><int>id for QnABaseClass</int></param>
        /// <returns><QnABaseClass>qnabase object found</QnABaseClass></returns>
        public async Task<QnABaseClass> GetQnABaseClassById(int id)
        {
            var qnaBase = await db.QnABaseClass.FirstOrDefaultAsync(x => x.QnAId == id);
            return qnaBase;
        }

        /// <summary>
        /// Fetch a single QnAPair from the db
        /// </summary>
        /// <param name="id"><int>id for given QnAPair</int></param>
        /// <returns><QnAPair>QnAPair found</QnAPair></returns>
        public async Task<QnAPairs> GetSingleQnAPairAsync(int id)
        {
            var qna = await db.QnAPairs.FirstOrDefaultAsync(x => x.QnAPairsId == id);
            return qna;
        }

        /// <summary>
        /// Update a single QnAPair
        /// </summary>
        /// <param name="qna">QnA to update</param>
        /// <returns>true if updated, false if not</returns>
        public async Task<bool> UpdateQnAPairAsync(QnAPairs qna)
        {
            var q = await db.QnAPairs.FirstOrDefaultAsync(x => x.QnAPairsId == qna.QnAPairsId);
            q.Dep = qna.Dep;
            db.Update(q);
            if(await db.SaveChangesAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
