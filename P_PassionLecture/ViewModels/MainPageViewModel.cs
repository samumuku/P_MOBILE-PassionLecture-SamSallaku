using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P_PassionLecture.Models;
using P_PassionLecture.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace P_PassionLecture.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly BookService _bookService = new();
    private readonly List<Book> _allBooks = new();

    [ObservableProperty]
    string debugText = "Nothing loaded";


    private int _loadedCount = 0;
    private const int PageSize = 20;
    private bool _isLoading = false;

    public ObservableCollection<Book> VisibleBooks { get; } = new();

    public MainPageViewModel()
    {
        // rien
    }

    [RelayCommand]
    public async Task LoadBooksAsync()
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadedCount = 0;
        _allBooks.Clear();

        await MainThread.InvokeOnMainThreadAsync(() => VisibleBooks.Clear());

        var books = await _bookService.GetBooksAsync();
        if (books != null)
        {
            _allBooks.AddRange(books);
            _isLoading = false;
            LoadNextChunk();
        }
        Debug.WriteLine($"VisibleBooks Count = {VisibleBooks.Count}");
        DebugText = $"Retrieved {_allBooks.Count} books. VisibleBooks: {VisibleBooks.Count}";
    }


    public async void LoadNextChunk()
    {
        if (_isLoading || _loadedCount >= _allBooks.Count)
            return;

        int remaining = _allBooks.Count - _loadedCount;
        int count = Math.Min(PageSize, remaining);

        var nextItems = _allBooks.Skip(_loadedCount).Take(count).ToList();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            foreach (var book in nextItems)
            {
                VisibleBooks.Add(book);
            }
        });

        _loadedCount += count;
    }

}
