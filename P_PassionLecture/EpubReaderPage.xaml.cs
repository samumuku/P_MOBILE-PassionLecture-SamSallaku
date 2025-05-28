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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(FilePath) && !string.IsNullOrWhiteSpace(BookTitle))
        {
            try
            {
                // Create a dummy Book object with minimal required data
                var book = new Book
                {
                    livre_id = int.Parse(Path.GetFileNameWithoutExtension(FilePath)),
                    titre = BookTitle
                    // You can set more properties if required
                };

                _viewModel = new EpubReaderViewModel(book);
                BindingContext = _viewModel;

                // If you had a method like LoadBookAsync, you could call it here.
                // But since ReadEpub is called in the constructor, this may not be necessary.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load EPUB: {ex.Message}");
                // Optional: Display an alert
            }
        }
    }
}
