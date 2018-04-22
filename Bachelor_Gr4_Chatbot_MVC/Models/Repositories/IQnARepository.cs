using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public interface IQnARepository
    {
        /// <summary>
        /// Testmethod
        /// </summary>
        /// <param name="Query">query as string</param>
        /// <returns>answer as string</returns>
        Task<string> PostQuery(string Query);

        /// <summary>
        /// Register the given knowledgebase at the given qna chatbot at QnAMaker.ai
        /// </summary>
        /// <param name="q">QnABaseClass q</param>
        /// <param name="b">QnAKnowledgeBase b</param>
        /// <returns>KnowledgeBase Id-string from QnAMaker.ai</returns>
        Task<string> RegisterNewQnAKnowledgeBaseAsync(QnABaseClass q, QnAKnowledgeBase b);

        /// <summary>
        /// Delete the given QnAKnowledgeBase from QnAMaker.ai
        /// </summary>
        /// <param name="q">QnABaseClass q</param>
        /// <param name="b">QnAKnowledgeBase b</param>
        /// <returns>true if deleted, false if not</returns>
        Task<bool> DeleteKnowledgeBase(QnABaseClass q, QnAKnowledgeBase b);

        /// <summary>
        /// Add a single QnA pair to the knowledgebase given to the QnATrainBase
        /// </summary>
        /// <param name="qna">QnATrainBase qna</param>
        /// <returns>true if added, false if not</returns>
        Task<bool> AddSingleQnAPairAsync(QnATrainBase qna);

        /// <summary>
        /// Publish given knowledgebase unpublished QnAPairs
        /// </summary>
        /// <param name="knowledgeBaseId">Id for knowledgebase at QnAMaker.ai</param>
        /// <returns>true if published, false if not</returns>
        Task<bool> PublishKnowledgeBase(int knowledgeBaseId);

        /// <summary>
        /// Download the given knowledgebase and return the QnAPairs found
        /// </summary>
        /// <param name="knowledgebase">knowledgebase id in database</param>
        /// <returns>QnAPairs found</returns>
        Task<List<QnAPairs>> DownloadKnowledgeBase(int knowledgebase);

        /// <summary>
        /// Post a comment to the given knowledgebase and return the response
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="knowledgebaseId"></param>
        /// <returns>(string) response</returns>
        Task<string> PostCommentToGivenKnowledgebase(string comment, int knowledgebaseId);

        /// <summary>
        /// Post a comment to the active knowledgebase and return the response
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="knowledgebaseId"></param>
        /// <returns>(string) response</returns>
        Task<string> PostCommentToActiveKnowledgebase(string comment);

        /// <summary>
        /// Delete the given QnAPair from the knowledgebase it belongs to
        /// </summary>
        /// <param name="qnaPair">QnAPair to be deleted</param>
        /// <returns><bool>True if ok, false if not</bool></returns>
        Task<bool> DeleteSingleQnAPairAsync(QnAPairs qnaPair);
    }
}
