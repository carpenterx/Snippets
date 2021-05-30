using Snippets.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Snippets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string SNIPPETS_PATH = @"C:\Users\jorda\Desktop\snippets.txt";
        private static readonly string SNIPPET_SEPARATOR = "======================  SNIPPET  =======================";
        private static readonly string NAME_PREFIX = "//  Name         :  ";
        private static readonly string DESCRIPTION_PREFIX = "//  Description  :  ";
        private static readonly string USE_COUNT_PREFIX = "//  Used         :  ";
        private static readonly string PREREQUISITES = "===================  [PREREQUISITES]  ==================";
        private static readonly string CODE_START = "====================  [CODE START]  ====================";
        private static readonly string CODE_END = "=====================  [CODE END]  =====================";
        private readonly ObservableCollection<Snippet> snippetsList = new();

        public MainWindow()
        {
            InitializeComponent();

            ReadSnippetsFile(SNIPPETS_PATH);
        }

        private void ReadSnippetsFile(string path)
        {
            List<string> snippets = File.ReadAllText(path).Split(SNIPPET_SEPARATOR).ToList().Skip(1).ToList();
            
            foreach (string snippetText in snippets)
            {
                ParseSnippetText(snippetText);
            }

            snippetsListView.ItemsSource = snippetsList;
        }

        private void ParseSnippetText(string text)
        {
            snippetsList.Add(new Snippet
            {
                Title = ExtractName(text),
                Description = ExtractDescription(text),
                UseCount = ExtractUseCount(text),
                Prerequisites = ExtractPrerequisites(text),
                Code = ExtractCode(text)
            });
        }

        private string ExtractName(string text)
        {
            string name = "";
            string pattern = $"{NAME_PREFIX}(.+)\r";
            Match m = Regex.Match(text, pattern, RegexOptions.Multiline);
            if (m.Success)
            {
                name = m.Groups[1].Value;
            }
            return name;
        }

        private string ExtractDescription(string text)
        {
            string description = "";
            string pattern = $"{DESCRIPTION_PREFIX}(.+)\r";
            Match m = Regex.Match(text, pattern, RegexOptions.Multiline);
            if (m.Success)
            {
                description = m.Groups[1].Value;
            }
            return description;
        }

        private string ExtractUseCount(string text)
        {
            string useCount = "";
            string pattern = $"{USE_COUNT_PREFIX}(.+)\r";
            Match m = Regex.Match(text, pattern, RegexOptions.Multiline);
            if (m.Success)
            {
                useCount = m.Groups[1].Value;
            }
            return useCount;
        }

        private string ExtractPrerequisites(string text)
        {
            string prerequisites = "";
            int startIndex = text.IndexOf(PREREQUISITES) + PREREQUISITES.Length + 2;
            if(startIndex > -1)
            {
                int endIndex = text.IndexOf(CODE_START) - 2;
                prerequisites = text.Substring(startIndex, endIndex - startIndex);
            }
            return prerequisites;
        }

        private string ExtractCode(string text)
        {
            string code;
            int startIndex = text.IndexOf(CODE_START) + CODE_START.Length + 2;
            int endIndex = text.IndexOf(CODE_END) - 2;
            code = text[startIndex..endIndex];
            return code;
        }

        private void DisplaySnippet(Snippet snippet)
        {
            StringBuilder snippetStringBuilder = new();
            snippetStringBuilder.AppendLine("===========================");
            snippetStringBuilder.AppendLine(snippet.Title);
            snippetStringBuilder.AppendLine("===========================");
            snippetStringBuilder.AppendLine(snippet.Description);
            snippetStringBuilder.AppendLine("===========================");
            snippetStringBuilder.AppendLine(snippet.UseCount);
            snippetStringBuilder.AppendLine("===========================");
            snippetStringBuilder.AppendLine(snippet.Prerequisites);
            snippetStringBuilder.AppendLine("===========================");
            snippetStringBuilder.AppendLine(snippet.Code);
            snippetStringBuilder.AppendLine("===========================");
            snippetTxt.Text = snippetStringBuilder.ToString();
        }

        private void ListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
            Snippet selectedSnippet = (sender as ListView).SelectedItem as Snippet;
            DisplaySnippet(selectedSnippet);
        }

        private void CopyCodeToClipboard(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedIndex != -1)
            {
                Snippet selectedSnippet = (Snippet)snippetsListView.SelectedItem;
                Clipboard.SetText(selectedSnippet.Code);
            }
        }

        private void ShowAddWindowClick(object sender, RoutedEventArgs e)
        {
            var snippetWindow = new AddSnippetWindow();
            snippetWindow.Owner = this;
            if (snippetWindow.ShowDialog() == true)
            {
                snippetsList.Add(snippetWindow.GetSnippet());
            }
        }
    }
}
