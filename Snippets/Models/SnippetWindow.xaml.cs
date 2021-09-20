using System.Windows;

namespace Snippets.Models
{
    /// <summary>
    /// Interaction logic for SnippetWindow.xaml
    /// </summary>
    public partial class SnippetWindow : Window
    {
        public Snippet Snippet { get; set; } = new();

        public SnippetWindow()
        {
            InitializeComponent();

            DataContext = Snippet;
        }

        public SnippetWindow(Snippet snippetToEdit)
        {
            InitializeComponent();

            Title = "Edit Snippet";
            submitTxt.Text = "Edit";
            submitIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.Pencil;

            Snippet = snippetToEdit;

            DataContext = Snippet;
        }

        public void AddSnippetClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }
    }
}
