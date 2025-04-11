using System.Diagnostics;
using System.Text.Json;

namespace P_PassionLecture
{
    public partial class MainPage : ContentPage
    {
        public static string baseAdress = "http://10.0.2.2:3000";
        public static string booksUrl = $"{baseAdress}/api/books";

        HttpClient _client;

        public MainPage()
        {
            InitializeComponent();

            _client = new HttpClient();
        }

        public async void getBooks()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(booksUrl);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(@"\tBooks : {0}", content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            getBooks();
        }
    }

}
