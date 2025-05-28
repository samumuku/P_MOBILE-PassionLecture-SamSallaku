using P_PassionLecture.Models;
using P_PassionLecture.ViewModels;
using System.Diagnostics;

namespace P_PassionLecture;

[QueryProperty(nameof(FilePath), "filepath")]
[QueryProperty(nameof(BookTitle), "title")]
public partial class EpubReaderPage : ContentPage
{
    private EpubReaderViewModel _viewModel;

    public string FilePath { get; set; }
    public string BookTitle { get; set; }

    public EpubReaderPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(FilePath))
        {
            var book = new Book
            {
                titre = BookTitle ?? "Untitled",
                livre_id = 0 // You can leave this 0 if not needed for now
            };

            _viewModel = new EpubReaderViewModel(book, FilePath);
            BindingContext = _viewModel;
        }
    }
}

