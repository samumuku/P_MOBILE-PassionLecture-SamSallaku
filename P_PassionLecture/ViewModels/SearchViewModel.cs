using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P_PassionLecture.Models;
using P_PassionLecture.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace P_PassionLecture.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly TagService _tagService = new();
        private readonly BookService _bookService = new();

        [ObservableProperty]
        private string titleKeyword;

        [ObservableProperty]
        private Tag selectedTag;

        [ObservableProperty]
        private DateTime fromDate = new DateTime(1900, 1, 1);

        [ObservableProperty]
        private DateTime toDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<Tag> tags = new();

        [ObservableProperty]
        private ObservableCollection<Book> filteredBooks = new();

        private List<Book> _allBooks = new();

        public SearchViewModel()
        {
            LoadData();
        }

        private async void LoadData()
        {
            // Load tags
            var loadedTags = _tagService.GetAllTags();
            Tags = new ObservableCollection<Tag>(loadedTags);

            // Load books
            _allBooks = await _bookService.GetBooksAsync(page: 1, pageSize: 100);
            FilterBooks();
        }

        [RelayCommand]
        private void FilterBooks()
        {
            var keyword = TitleKeyword?.ToLower() ?? "";
            var from = FromDate.Date;
            var to = ToDate.Date;

            var query = _allBooks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(b => b.titre.ToLower().Contains(keyword));

            query = query.Where(b => b.datePublication.Date >= from && b.datePublication.Date <= to);

            if (SelectedTag != null)
            {
                query = query.Where(b => _tagService.GetTagsForBook(b.livre_id).Any(t => t.Id == SelectedTag.Id));
            }

            FilteredBooks = new ObservableCollection<Book>(query);
        }

        [RelayCommand]
        private void ClearTagFilter()
        {
            SelectedTag = null;
            FilterBooks();
        }

    }
}
