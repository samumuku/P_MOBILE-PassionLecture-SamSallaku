using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P_PassionLecture.Models;
using System.Diagnostics;
using VersOne.Epub;

namespace P_PassionLecture.ViewModels;

public partial class EpubReaderViewModel : ObservableObject
{
    [ObservableProperty]
    private string title;
    public Book Book { get; }

    [ObservableProperty]
    private string content;

    public async Task LoadBookAsync(string filePath, string title)
    {
        Title = title;
        var book = await EpubReader.ReadBookAsync(filePath);
        Content = string.Join("\n\n", book.ReadingOrder.Select(ch => ch.Content));
    }

[RelayCommand]
private async Task ReadBookAsync()
{
    // URL of your EPUB on API (replace with your actual API URL for the EPUB file)
    string epubUrl = $"http://10.0.2.2:3000/api/books/{Book.livre_id}/epub";
        if (Book == null)
        {
            Debug.WriteLine("Book is null, cannot read EPUB.");
            return;
        }

        // Download EPUB and get local path
        string localPath = await DownloadEpubAsync(epubUrl, $"{Book.titre}.epub");

    await Shell.Current.GoToAsync($"epubreader?filepath={Uri.EscapeDataString(localPath)}&title={Uri.EscapeDataString(Book.titre)}");
}


    public async Task<string> DownloadEpubAsync(string epubUrl, string fileName)
    {
        try
        {
            using HttpClient client = new();
            var response = await client.GetAsync(epubUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to download EPUB. Status code: {response.StatusCode}");

            var bytes = await response.Content.ReadAsByteArrayAsync();

            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            File.WriteAllBytes(localPath, bytes);

            return localPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Download error: {ex.Message}");
            throw;
        }
    }


}
