using P_PassionLecture.Models;
using P_PassionLecture.Services;
using System.Collections.ObjectModel;

namespace P_PassionLecture;

public partial class MainPage : ContentPage
{
    private readonly BookService _bookService = new();
    private readonly List<Book> _allBooks = new();                // Full list fetched once
    private readonly ObservableCollection<Book> _visibleBooks = new(); // What we show in UI
    private int _loadedCount = 0;
    private const int PageSize = 10;
    private bool _isLoading = false;

    public MainPage()
    {
        InitializeComponent();
        BooksCollection.ItemsSource = _visibleBooks;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        _loadedCount = 0;
        _visibleBooks.Clear();
        _allBooks.Clear();

        var books = await _bookService.GetBooksAsync();
        if (books != null)
        {
            _allBooks.AddRange(books);
            LoadNextChunk();
        }
    }

    private void CollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        LoadNextChunk();
    }
    private async void BooksCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Book selectedBook)
        {
            // Clear selection to allow re-selection of the same item later
            BooksCollection.SelectedItem = null;

            await Navigation.PushAsync(new Views.BookDetailPage(selectedBook));
        }
    }

    private void LoadNextChunk()
    {
        if (_isLoading || _loadedCount >= _allBooks.Count)
            return;

        _isLoading = true;

        int remaining = _allBooks.Count - _loadedCount;
        int count = Math.Min(PageSize, remaining);

        var nextItems = _allBooks.Skip(_loadedCount).Take(count);
        foreach (var book in nextItems)
        {
            _visibleBooks.Add(book);
        }

        _loadedCount += count;
        _isLoading = false;
    }
}
