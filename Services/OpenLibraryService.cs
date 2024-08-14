using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace NookApp.Services
{
    public class OpenLibraryService
    {
        private readonly HttpClient _httpClient;

        public OpenLibraryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JObject> GetBookInfoByIsbnAsync(string isbn)
        {
            string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&jscmd=data&format=json";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }

        public async Task<JObject> GetBookInfoByTitleAsync(string title)
        {
            string url = $"https://openlibrary.org/search.json?title={title}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }

        public async Task<JObject> GetBookInfoByAuthorAsync(string author)
        {
            string url = $"https://openlibrary.org/search.json?author={author}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }
    }
}
