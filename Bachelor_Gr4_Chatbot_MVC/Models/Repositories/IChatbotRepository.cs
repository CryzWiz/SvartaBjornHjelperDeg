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

        /// <summary>
        /// Fetch all the unpublished QnAPairs for the given knowledgebase
        /// </summary>
        /// <param name="id">knowledge database id</param>
        /// <returns>List<QnAPairs>unpublished</QnAPairs></returns>
        Task<List<QnAPairs>> GetUnPublishedQnAPairsAsync(int id);

        /// <summary>
        /// Fetch all QnAPairs to a given knowledgebase
        /// </summary>
        /// <param name="id">knowledgebase id in database</param>
        /// <returns>List<QnAPairs>allQnAPairs</QnAPairs></returns>
        Task<List<QnAPairs>> GetPublishedQnAPairsAsync(int id);

        /// <summary>
        /// Mark all unpublished QnA pairs as published, and call
        /// EFQnARepository to perform the publish method on QnAMaker.ai
        /// </summary>
        /// <param name="knowledgebaseId">QnAMaker.ai knowledgebase id</param>
        /// <returns>true if stored and published, false if not</returns>
        Task<bool> PublishTrainedQnAPairs(int knowledgebaseId);

        /// <summary>
        /// Check the local knowledgebase to the published knowledgebase and
        /// add the missing QnAPairs to the local db, if any are missing.
        /// This function is added since you can add QnA pairs on QnAMaker.ai. With this function you
        /// can get them and add them to the local db so you can manage them (delete, view)
        /// </summary>
        /// <param name="id">KnowledgeBase id in database</param>
        /// <returns>number of pairs added</returns>
        Task<int> VerifyLocalDbToPublishedDb(int id);

        /// <summary>
        /// Post a query to the qnabot and return the reply
        /// </summary>
        /// <param name="comment">query asked</param>
        /// <returns><string>answer from active knowledgebase</string></returns>
        Task<string> PostToActiveKnowledgeBase(string comment);

        /// <summary>
        /// Delete the given QnA pair from knowledgebase db. If it is published, delete it from the
        /// QnAmake.ai base aswell.
        /// </summary>
        /// <param name="id">QnAPair to be deleted</param>
        /// <returns>true if deleted, false if not</returns>
        Task<bool> DeleteQnAPair(int id);

        /// <summary>
        /// Fetch the id for the knowledgebase the QnAPair belongs to
        /// </summary>
        /// <param name="id"><int>id for qnapair</int></param>
        /// <returns><int>knowledgebase id</int></returns>
        Task<int> GetKnowledgebaseIdToQnAPair(int id);
        
        /// <summary>
        /// Get all conversations with the active bot
        /// </summary>
        /// <returns>view over all conversations</returns>
        Task<List<Conversation>> GetConversationsWithActiveBotAsync();

        /// <summary>
        /// Get all the messages for a given conversation
        /// </summary>
        /// <param name="id"><int>id for conversation</int></param>
        /// <returns>View over messages for given conversation</returns>
        Task<List<Message>> GetMessagesForConversationAsync(int id);

        /// <summary>
        /// Get conversation by id
        /// </summary>
        /// <param name="id"><int>id for conversation</int></param>
        /// <returns><Conversation>found conversation</Conversation></returns>
        Task<Conversation> GetConversationByIdAsync(int id);

        /// <summary>
        /// Fetch the number of unpublished qnapairs to the active knowledgebase
        /// </summary>
        /// <returns><int>number of unpublished qnapairs</int></returns>
        Task<int> GetPublishedQnAPairsToActiveBotAsync();

        /// <summary>
        /// Fetch a single message and return it
        /// </summary>
        /// <param name="q"><int>Message id</int></param>
        /// <returns><Message>Message found</Message></returns>
        Task<Message> GetSingleMessageByIdAsync(int q);

        /// <summary>
        /// Activate the given knowledgebase and return the result
        /// </summary>
        /// <param name="id"><int>id for knowledgebase</int></param>
        /// <returns>true if activated, false if not</returns>
        Task<bool> ActivateQnAKnowledgeBaseAsync(int id);
    }
}
