using P_PassionLecture.Models;
using System.Diagnostics;
using System.Text.Json;

namespace P_PassionLecture.Services
{
    public class BookService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly TagService _tagService;  // Add TagService

        private const string baseAdress = "http://10.0.2.2:3000";
        private const string booksUrl = $"{baseAdress}/api/books";

        public BookService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            _tagService = new TagService();  // Initialize TagService
        }

        public async Task<List<Book>> GetBooksAsync(int page = 1, int pageSize = 10)
        {
            var books = new List<Book>();
            Debug.WriteLine($"Fetching books: {booksUrl}?page={page}&limit={pageSize}");

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
                    foreach (var book in books)
                    {
                        book.Tags = _tagService.GetTagsForBook(book.livre_id);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: {ex}");
            }
            Debug.WriteLine($"Retrieved {books.Count} books");

            return books;
        }
    }
}
