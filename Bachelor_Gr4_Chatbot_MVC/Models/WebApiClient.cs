using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bachelor_Gr4_Chatbot_MVC.Models
{
    public class WebApiClient
    {
        private readonly HttpClient _client;
        // TODO: Endre denne: 
        private const string _baseAddress = "https://localhost:44365/";

        public WebApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> PostJson<T>(string requestUri, T content)
        {
            _client.BaseAddress = new Uri(_baseAddress);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsJsonAsync(requestUri, content);
            return response;
        }


    }
}
