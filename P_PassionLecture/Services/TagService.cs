using System.Text.Json;
using P_PassionLecture.Models;

namespace P_PassionLecture.Services
{
    public class TagService
    {
        private readonly string _tagFile = Path.Combine(FileSystem.AppDataDirectory, "tags.json");
        private readonly string _bookTagFile = Path.Combine(FileSystem.AppDataDirectory, "booktags.json");

        private List<Tag> _tags = new();
        private List<BookTag> _bookTags = new();

        public TagService()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_tagFile))
                _tags = JsonSerializer.Deserialize<List<Tag>>(File.ReadAllText(_tagFile)) ?? new();

            if (File.Exists(_bookTagFile))
                _bookTags = JsonSerializer.Deserialize<List<BookTag>>(File.ReadAllText(_bookTagFile)) ?? new();
        }

        private void SaveData()
        {
            File.WriteAllText(_tagFile, JsonSerializer.Serialize(_tags));
            File.WriteAllText(_bookTagFile, JsonSerializer.Serialize(_bookTags));
        }

        public List<Tag> GetTagsForBook(int bookId)
        {
            var tagIds = _bookTags.Where(bt => bt.BookId == bookId).Select(bt => bt.TagId);
            return _tags.Where(t => tagIds.Contains(t.Id)).ToList();
        }

        public List<Tag> GetAllTags() => _tags;

        public Tag? FindTagByName(string name) => _tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public Tag CreateTag(string name)
        {
            var tag = new Tag { Id = _tags.Count > 0 ? _tags.Max(t => t.Id) + 1 : 1, Name = name };
            _tags.Add(tag);
            SaveData();
            return tag;
        }

        public void AssignTagToBook(int bookId, int tagId)
        {
            if (!_bookTags.Any(bt => bt.BookId == bookId && bt.TagId == tagId))
            {
                _bookTags.Add(new BookTag { BookId = bookId, TagId = tagId });
                SaveData();
            }
        }

    }
}
