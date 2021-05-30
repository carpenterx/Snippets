using System.Windows;

namespace Snippets.Models
{
    /// <summary>
    /// Interaction logic for AddSnippetWindow.xaml
    /// </summary>
    public partial class AddSnippetWindow : Window
    {
        private Snippet snippet;
        //public Snippet Snippet { get; set; }

        public AddSnippetWindow()
        {
            InitializeComponent();
        }

        public void AddSnippetClick(object sender, RoutedEventArgs e)
        {

            snippet = new Snippet
            {
                Title = titleTxt.Text,
                Description = descTxt.Text,
                Prerequisites = prereqTxt.Text,
                Code = codeTxt.Text,
                UseCount = "0"
            };
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        public Snippet GetSnippet()
        {
            return snippet;
        }
    }
}
