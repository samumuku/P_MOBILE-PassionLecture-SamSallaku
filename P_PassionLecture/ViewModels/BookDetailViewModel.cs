using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P_PassionLecture.Models;
using P_PassionLecture.Services;
using System.Collections.ObjectModel;

namespace P_PassionLecture.ViewModels;

public partial class BookDetailViewModel : ObservableObject
{
    private readonly TagService _tagService = new();

    public BookDetailViewModel(Book book)
    {
        Book = book;
        Tags = new ObservableCollection<Tag>(_tagService.GetTagsForBook(Book.livre_id));
    }

    public Book Book { get; }

    public ObservableCollection<Tag> Tags { get; }

    [ObservableProperty]
    private string newTag;

    [RelayCommand]
    private void AddTag()
    {
        if (string.IsNullOrWhiteSpace(NewTag)) return;

        var tag = _tagService.FindTagByName(NewTag) ?? _tagService.CreateTag(NewTag);
        _tagService.AssignTagToBook(Book.livre_id, tag.Id);

        if (!Tags.Any(t => t.Id == tag.Id))
            Tags.Add(tag);

        NewTag = string.Empty;
    }
    [RelayCommand]
    private async Task ReadBookAsync()
    {
        try
        {
            // Fetch BLOB from your API
            var client = new HttpClient();
            var url = $"http://10.0.2.2:3000/api/books/{Book.livre_id}/blob"; // Adjust this to match your API
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var epubBytes = await response.Content.ReadAsByteArrayAsync();

            // Save file
            var fileName = $"{Book.titre.Replace(" ", "_")}.epub";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            File.WriteAllBytes(filePath, epubBytes);

            // Navigate to EPUB reader page
            await Shell.Current.GoToAsync($"epubreader?filepath={Uri.EscapeDataString(filePath)}&title={Uri.EscapeDataString(Book.titre)}");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Erreur", $"Impossible de lire le livre: {ex.Message}", "OK");
        }
    }

}
