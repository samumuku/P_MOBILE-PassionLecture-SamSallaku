using P_PassionLecture.Models;

namespace P_PassionLecture.Views;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage(Book selectedBook)
    {
        InitializeComponent();
        BindingContext = selectedBook;
    }
}
