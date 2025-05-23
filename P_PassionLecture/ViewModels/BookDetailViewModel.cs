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
}
