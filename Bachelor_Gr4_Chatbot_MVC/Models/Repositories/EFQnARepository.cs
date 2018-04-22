using Bachelor_Gr4_Chatbot_MVC.Data;
using Bachelor_Gr4_Chatbot_MVC.Models.QnAViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    /// <summary>
    /// This repository only holds methods for calling the QnAbot. 
    /// 
    /// </summary>
    public class EFQnARepository : IQnARepository
    {
        public ApplicationDbContext db;

        public EFQnARepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Delete the given QnAKnowledgeBase from QnAMaker.ai
        /// </summary>
        /// <param name="q">QnABaseClass q</param>
        /// <param name="b">QnAKnowledgeBase b</param>
        /// <returns>true if deleted, false if not</returns>
        public async Task<bool> DeleteKnowledgeBase(QnABaseClass q, QnAKnowledgeBase b)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", q.subscriptionKey);

            HttpResponseMessage response;
            var uri = b.DeleteQnAKnowledgeBase;

            response = await client.DeleteAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Testmethod
        /// </summary>
        /// <param name="Query">query as string</param>
        /// <returns>answer as string</returns>
        public async Task<String> PostQuery(string Query)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(String.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "7d26f05ae72842478df8fdca921de66d");

            var uri = "https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/025fd52b-e8d7-43aa-a10f-e8f9bde3e369/generateAnswer?" + queryString;

            HttpResponseMessage response;

            string c = "{'question':'" + Query + "'}";
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(c);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Register the given knowledgebase at the given qna chatbot at QnAMaker.ai
        /// </summary>
        /// <param name="q">QnABaseClass q</param>
        /// <param name="b">QnAKnowledgeBase b</param>
        /// <returns>KnowledgeBase Id-string from QnAMaker.ai</returns>
        public async Task<String> RegisterNewQnAKnowledgeBaseAsync(QnABaseClass q, QnAKnowledgeBase b)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", q.subscriptionKey);

            HttpResponseMessage response;
            var uri = b.CreateNewQnAKnowledgeBase;

            string c = "{'name':'" + b.QnAKnowledgeName +"'}";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(c);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
            JObject o = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            var kbId = (string)o.SelectToken("['kbId']");
            return kbId;
        }

        /// <summary>
        /// Add a single QnA pair to the knowledgebase given to the QnATrainBase
        /// </summary>
        /// <param name="qna">QnATrainBase qna</param>
        /// <returns>true if added, false if not</returns>
        public async Task<bool> AddSingleQnAPairAsync(QnATrainBase qna)
        {
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.subscriptionKey == qna.SubscriptionKey));
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.QnAKnowledgeBaseId == qna.KnowledgeBaseId));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            string pair = "{'add': {'qnaPairs': [{'answer': '"+qna.Answer+"','question': '"+qna.Query+"'}]},}";

            HttpResponseMessage response;

            var uri = b.AddQnAPairUrl;

            var method = new HttpMethod("PATCH");
            HttpContent content = new StringContent(pair, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = content
            };

            response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Delete the given QnAPair from the knowledgebase it belongs to
        /// </summary>
        /// <param name="qnaPair">QnAPair to be deleted</param>
        /// <returns><bool>True if ok, false if not</bool></returns>
        public async Task<bool> DeleteSingleQnAPairAsync(QnAPairs qnaPair)
        {
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.QnAKnowledgeBaseId == qnaPair.KnowledgeBaseId));
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.QnAId == b.QnABotId));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            string pair = "{'delete': {'qnaPairs': [{'answer': '" + qnaPair.Answer + "','question': '" + qnaPair.Query + "'}]},}";

            HttpResponseMessage response;

            var uri = b.AddQnAPairUrl;

            var method = new HttpMethod("PATCH");
            HttpContent content = new StringContent(pair, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = content
            };

            response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;

        }

        /// <summary>
        /// Publish given knowledgebase unpublished QnAPairs
        /// </summary>
        /// <param name="knowledgeBaseId">Id for knowledgebase at QnAMaker.ai</param>
        /// <returns>true if published, false if not</returns>
        public async Task<bool> PublishKnowledgeBase(int knowledgeBaseId)
        {
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.QnAKnowledgeBaseId == knowledgeBaseId));
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.QnAId == b.QnABotId));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            var uri = b.PublishKnowledgeBaseUrl;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PutAsync(uri, content);
            }
            if (response.IsSuccessStatusCode) return true;
            else return false;


        }

        /// <summary>
        /// Download the given knowledgebase and return the QnAPairs found
        /// </summary>
        /// <param name="knowledgebase">knowledgebase id in database</param>
        /// <returns>QnAPairs found</returns>
        public async Task<List<QnAPairs>> DownloadKnowledgeBase(int knowledgebase)
        {
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.QnAKnowledgeBaseId == knowledgebase));
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.QnAId == b.QnABotId));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            var uri = b.PublishKnowledgeBaseUrl;

            HttpResponseMessage response;
            response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string url = await response.Content.ReadAsStringAsync();
                string urlClean = url.Replace("\"", "");
                string result = GetCSV(urlClean);

                var clean = result.Replace("Editorial", "\t");

                string[] temp = clean.Split("\t");
                temp[2] = temp[2].Replace("Source", "");
                for (int x = 2; x < temp.Count(); x++)
                {
                    temp[x] = temp[x].Trim();

                }
                result = temp.Count().ToString();
                List<QnAPairs> qnas = new List<QnAPairs>();
                int i = 2;
                while (i < temp.Count()-1)
                {
                    var qna = new QnAPairs
                    {
                        Query = temp[i],
                        Answer = temp[i + 1],
                        Trained = true,
                        Published = true,
                        TrainedDate = DateTime.Now,
                        PublishedDate = DateTime.Now,
                        Dep = "Web",
                        KnowledgeBaseId = b.QnAKnowledgeBaseId
                    };
                    qnas.Add(qna);
                    i = i + 3;
                }

                return qnas;
            }
            else return null;
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        /// <summary>
        /// Post comment to a given knowledgebase and return the response
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="knowledgebaseId"></param>
        /// <returns></returns>
        public async Task<string> PostCommentToGivenKnowledgebase(string comment, int knowledgebaseId)
        {
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.QnAKnowledgeBaseId == knowledgebaseId));
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.QnAId == b.QnABotId));


            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(String.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            var uri = b.AskQuestionUrl;

            HttpResponseMessage response;

            string q = "{'question':'" + comment + "'}";
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(q);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            return response.Content.ReadAsStringAsync().Result;

        }

        /// <summary>
        /// Post comment to active knowledgebase and return the response
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<string> PostCommentToActiveKnowledgebase(string comment)
        {
            var b = await Task.Run(() => db.QnAKnowledgeBase.FirstOrDefault(X => X.IsActive == true));
            var c = await Task.Run(() => db.QnABaseClass.FirstOrDefault(X => X.QnAId == b.QnABotId));


            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(String.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", c.subscriptionKey);

            var uri = b.AskQuestionUrl;

            HttpResponseMessage response;

            string q = "{'question':'" + comment + "'}";
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(q);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
            JObject o = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            JArray a = (JArray)o["answers"];
            var result = (string)o.SelectToken("['answers'][0]['answer']");
            return result;

        }

    }
}
