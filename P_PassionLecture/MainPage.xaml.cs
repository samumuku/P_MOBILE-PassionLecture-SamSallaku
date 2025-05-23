namespace P_PassionLecture;
using P_PassionLecture.Models;
using P_PassionLecture.ViewModels;
public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }

    /*protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainPageViewModel vm)
        {
            vm.LoadBooksCommand.Execute(null);
        }
    }*/

    private void CollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (BindingContext is MainPageViewModel vm)
        {
            vm.LoadNextChunk();
        }
    }

    private async void BooksCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Book selectedBook)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Navigation.PushAsync(new Views.BookDetailPage(selectedBook));
        }
    }
}
