using P_PassionLecture.Models;
using P_PassionLecture.ViewModels;
namespace P_PassionLecture.Views;

public partial class BookDetailPage : ContentPage
{
    public BookDetailPage(Book selectedBook)
    {
        InitializeComponent();
        BindingContext = new BookDetailViewModel(selectedBook); ;
    }
}
