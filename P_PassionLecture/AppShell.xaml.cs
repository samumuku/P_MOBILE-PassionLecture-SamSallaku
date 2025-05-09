namespace P_PassionLecture
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("BookDetailPage", typeof(P_PassionLecture.Views.BookDetailPage));

        }
    }
}
