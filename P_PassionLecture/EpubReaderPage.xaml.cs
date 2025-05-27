using P_PassionLecture.ViewModels;
using System.Diagnostics;

namespace P_PassionLecture;

[QueryProperty(nameof(FilePath), "filepath")]
[QueryProperty(nameof(BookTitle), "title")]
public partial class EpubReaderPage : ContentPage
{
    private readonly EpubReaderViewModel _viewModel = new();

    public string FilePath { get; set; }
    public string BookTitle { get; set; }

    public EpubReaderPage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrWhiteSpace(FilePath) && !string.IsNullOrWhiteSpace(BookTitle))
        {
            try
            {
                await _viewModel.LoadBookAsync(FilePath, BookTitle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load EPUB: {ex.Message}");
                // You could show an alert here
            }
        }
    }

}
