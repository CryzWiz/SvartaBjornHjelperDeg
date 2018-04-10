using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IChatbotRepository
    {
        // Microsoft methods - To be removed
        Task<List<ChatbotDetails>> GetAllChatbots();
        Task<bool> RegisterNewChatbot(ChatbotDetails chatbotDetails);
        Task<ChatbotDetails> GetChatbotDetails(int id);
        Task<bool> UpdateChatbotDetails(ChatbotDetails chatbotDetails);
        Task<bool> DeleteChatbot(ChatbotDetails chatbot);
        Task<bool> CheckIfBotActive(int id);
        Task<string[]> ActivateBot(int id);
        Task<ChatbotDetails> GetActiveBot();

        // Chatbot Type I.E Microsoft Bot or QnAbot. Not used.
        Task<List<ChatbotTypes>> GetAllTypes();

        // QnA methods

        ///<summary>
        /// Method for fetching all the registered chatbots from the database
        ///</summary>
        ///<returns>A list of all the registered chatbots</returns>
        Task<List<QnABaseClass>> GetAllQnABots();

        /// <summary>
        /// Fetch chatbot by database-id
        /// </summary>
        /// <param name="id">database id for chatbot</param>
        /// <returns>QnABaseClass chatbot</returns>
        Task<QnADetails> GetQnABotDetails(int id);

        /// <summary>
        /// Register a new chatbot in the database
        /// </summary>
        /// <param name="qnabot">QnABaseClass chatbot to be registered</param>
        /// <returns>r[0] = operationresult, r[1] = chatbotname, r[2] = result from QnAKnowledgeBase creation</returns>
        Task<string[]> RegisterNewQnABotAsync(QnABaseClass qnabot);

        /// <summary>
        /// Fetch QnAKnowledgebase from db
        /// </summary>
        /// <param name="id">knowledgebase id in database</param>
        /// <returns>found QnAKnowledgeBase</returns>
        Task<QnAKnowledgeBase> GetQnAKnowledgeBaseAsync(int id);

        /// <summary>
        /// Add a new QnAKnowledgebase to the database, and call EFQnARepository to
        /// create the new knowledgebase to QnAMaker.ai
        /// </summary>
        /// <param name="b">QnAKnowledgeBase name</param>
        /// <returns>true if registered, false if not</returns>
        Task<bool> AddNewQnAKnowledgeBaseAsync(QnAKnowledgeBase b);

        /// <summary>
        /// Get the active QnABaseClass
        /// </summary>
        /// <returns>active QnABaseClass</returns>
        Task<QnABaseClass> GetActiveQnABaseClassAsync();

        /// <summary>
        /// Fetch active knowledgebase from db
        /// </summary>
        /// <returns>the active QnAKnowledgeBase</returns>
        Task<QnAKnowledgeBase> GetActiveQnAKnowledgeBaseAsync();

        /// <summary>
        /// Add a single QnA pair to the database, and call EFQnARepository to 
        /// add the pair to knowledgebase on QnAMaker.ai
        /// </summary>
        /// <param name="qna">the QnA to be added</param>
        /// <returns>true if added, false if not</returns>
        Task<bool> AddSingleQnAPairToBaseAsync(QnATrainBase qna);

        /// <summary>
        /// Delete the given QnAKnowledgeBase in the database, and call EFQnARepository
        /// to delete the knowledgebase on QnAMaker.ai
        /// </summary>
        /// <param name="id">knowledgeid in database</param>
        /// <returns></returns>
        Task<bool> DeleteQnAKnowledgeBaseByIdAsync(int id);

        /// <summary>
        /// Fetch the correct QnABaseClass from the given QnAMaker.ai subcription key
        /// </summary>
        /// <param name="subKey">subscription key</param>
        /// <returns>QnABaseClass</returns>
        Task<QnABaseClass> GetQnABotDetailsBySubscriptionAsync(string subKey);

        /// <summary>
        /// Fetch the correct QnAKnowledgeBase from the given QnAMaker.ai knowledgebase id
        /// </summary>
        /// <param name="knowledgeBaseId">knowledgebase id</param>
        /// <returns>QnAKnowledgeBase</returns>
        Task<QnAKnowledgeBase> GetQnAKnowledgeBaseByKnowledgeIdAsync(string knowledgeBaseId);
    }
}
