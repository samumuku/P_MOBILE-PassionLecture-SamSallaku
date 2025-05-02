using Microsoft.VisualBasic;
using P_PassionLecture.Models;
using P_PassionLecture.Services;
using System.Diagnostics;
using System.Text.Json;

namespace P_PassionLecture
{
    public partial class MainPage : ContentPage
    {
        private readonly BookService _bookService = new BookService();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var books = await _bookService.GetBooksAsync();

            BooksCollection.ItemsSource = books;
        }
    }

}
