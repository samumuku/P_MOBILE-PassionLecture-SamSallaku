using P_PassionLecture.Models;
using System.Diagnostics;
using System.Text.Json;

namespace P_PassionLecture.Services
{
    public class BookService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        private const string baseAdress = "http://10.0.2.2:3000";
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

        public async Task<List<Book>> GetBooksAsync()
        {
            var books = new List<Book>(); // créé une nouvelle liste de livres

            Uri uri = new Uri(booksUrl); // créé une uri pour stocker l'URL du backend (json)

            try
            {
                //envoie une requête http GET
                HttpResponseMessage response = await _client.GetAsync(uri);

                // si la réponse est 200
                if (response.IsSuccessStatusCode)
                {
                    //va lire le contenu en tant que string, qui sera donc le JSON
                    string content = await response.Content.ReadAsStringAsync();

                    //retourne le JSON propre
                    var root = JsonDocument.Parse(content);
                    //va chercher les premières valeurs du tableau JSON, et va chercher le "data"
                    var dataElement = root.RootElement.GetProperty("data");
                    //deserializer la liste de livres, donc prend le JSON et le transforme en une liste de livres
                    books = JsonSerializer.Deserialize<List<Book>>(dataElement.GetRawText(), _serializerOptions);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return books;
        }
    }
}
