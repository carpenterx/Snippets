using System.Windows;

namespace Snippets.Models
{
    /// <summary>
    /// Interaction logic for AddSnippetWindow.xaml
    /// </summary>
    public partial class AddSnippetWindow : Window
    {
        private Snippet snippet = new();

        public AddSnippetWindow()
        {
            InitializeComponent();

            DataContext = snippet;
        }

        public AddSnippetWindow(Snippet snippetToEdit)
        {
            InitializeComponent();

            snippet = snippetToEdit;

            DataContext = snippet;
        }

        public void AddSnippetClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }

        public Snippet GetSnippet()
        {
            return snippet;
        }
    }
}
