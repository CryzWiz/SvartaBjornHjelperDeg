﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Bachelor_Gr4_Chatbot_MVC.Models.Repositories
{
    public class EFQnARepository : IQnARepository
    {

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
    }
}
