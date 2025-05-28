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
                livre_id = 0
            };

            _viewModel = new EpubReaderViewModel(book, FilePath);
            BindingContext = _viewModel;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.PageText))
                {
                    // Wrap the HTML content inside a simple HTML page
                    var html = $"<html><body style='padding:10px;font-size:16px;font-family:sans-serif;'>{_viewModel.PageText}</body></html>";
                    EpubWebView.Source = new HtmlWebViewSource { Html = html };
                }
            };
        }
    }
}


