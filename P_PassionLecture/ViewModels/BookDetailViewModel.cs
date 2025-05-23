using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using P_PassionLecture.Models;
using P_PassionLecture.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace P_PassionLecture.ViewModels
{
    public class BookDetailViewModel : BindableObject
    {
        public Book Book { get; }
        public ObservableCollection<Tag> Tags { get; } = new();

        private string _newTag;
        public string NewTag
        {
            get => _newTag;
            set
            {
                _newTag = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTagCommand { get; }

        private readonly MysqliteContext _db;

        public BookDetailViewModel(Book book)
        {
            Book = book;
            _db = new MysqliteContext(Path.Combine(FileSystem.AppDataDirectory, "books.db"));

            AddTagCommand = new Command(async () => await AddTagAsync());
            LoadTags();
        }

        private async void LoadTags()
        {
            var tagIds = await _db.BookTags
                .Where(bt => bt.BookId == Book.livre_id)
                .Select(bt => bt.TagId)
                .ToListAsync();

            var tags = await _db.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();

            Tags.Clear();
            foreach (var tag in tags)
                Tags.Add(tag);
        }

        private async Task AddTagAsync()
        {
            if (string.IsNullOrWhiteSpace(NewTag)) return;

            // Find or create tag
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == NewTag);
            if (tag == null)
            {
                tag = new Tag { Name = NewTag };
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
            }

            // Check if link already exists
            bool alreadyLinked = await _db.BookTags.AnyAsync(bt =>
                bt.BookId == Book.livre_id && bt.TagId == tag.Id);

            if (!alreadyLinked)
            {
                _db.BookTags.Add(new BookTag { BookId = Book.livre_id, TagId = tag.Id });
                await _db.SaveChangesAsync();
                Tags.Add(tag);
            }

            NewTag = string.Empty;
        }
    }
}
