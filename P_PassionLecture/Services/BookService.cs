using P_PassionLecture.Models;
using System.Diagnostics;
using System.Text.Json;

namespace P_PassionLecture.Services
{
    public class BookService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        private const string baseAdress = "https://8c8b-193-5-249-1.ngrok-free.app";
        private const string booksUrl = $"{baseAdress}/api/books";

        public BookService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                //va relier les propriétes du modèle en CamelCase avec celui du JSON
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        public async Task<List<Book>> GetBooksAsync(int page = 1, int pageSize = 10)
        {
            var books = new List<Book>();

            string url = $"{booksUrl}?page={page}&limit={pageSize}";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var root = JsonDocument.Parse(content);
                    var dataElement = root.RootElement.GetProperty("data");
                    books = JsonSerializer.Deserialize<List<Book>>(dataElement.GetRawText(), _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex}");
            }

            return books;
        }

    }
}
