using Snippets.Models;
using System;
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
        private static readonly string SNIPPET_SEPARATOR = "======================  SNIPPET  =======================";
        private static readonly string SNIPPET_FOOTER = "========================================================";
        private static readonly string NAME_PREFIX = "//  Name         :  ";
        private static readonly string DESCRIPTION_PREFIX = "//  Description  :  ";
        private static readonly string USE_COUNT_PREFIX = "//  Used         :  ";
        private static readonly string PREREQUISITES = "===================  [PREREQUISITES]  ==================";
        private static readonly string CODE_START = "====================  [CODE START]  ====================";
        private static readonly string CODE_END = "=====================  [CODE END]  =====================";
        private readonly ObservableCollection<Snippet> snippetsList = new();

        private static readonly string APPLICATION_FOLDER = "Snippets";
        private static readonly string SNIPPETS_FILE = "snippets.txt";
        private readonly string snippetsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, SNIPPETS_FILE);

        public MainWindow()
        {
            InitializeComponent();

            ReadSnippetsFile();
        }

        private void ReadSnippetsFile()
        {
            if (File.Exists(snippetsPath))
            {
                List<string> snippets = File.ReadAllText(snippetsPath).Split(SNIPPET_SEPARATOR).ToList().Skip(1).ToList();

                foreach (string snippetText in snippets)
                {
                    ParseSnippetText(snippetText);
                }
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

        private int ExtractUseCount(string text)
        {
            int useCount = 0;
            string pattern = $"{USE_COUNT_PREFIX}(.+)\r";
            Match m = Regex.Match(text, pattern, RegexOptions.Multiline);
            if (m.Success)
            {
                if (int.TryParse(m.Groups[1].Value, out int result))
                {
                    useCount = result;
                }
            }
            return useCount;
        }

        private string ExtractPrerequisites(string text)
        {
            string prerequisites = "";
            
            if (text.IndexOf(PREREQUISITES) > -1)
            {
                int startIndex = text.IndexOf(PREREQUISITES) + PREREQUISITES.Length + 2;
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

        private void ListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem is Snippet selectedSnippet)
            {
                snippetTxt.Text = ConvertSnippetToText(selectedSnippet);
            }
        }

        private void CopyCodeToClipboard(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedIndex != -1)
            {
                Snippet selectedSnippet = (Snippet)snippetsListView.SelectedItem;
                selectedSnippet.UseCount++;
                Clipboard.SetText(selectedSnippet.Code);
                snippetTxt.Text = ConvertSnippetToText(selectedSnippet);
            }
        }

        private void ShowAddWindowClick(object sender, RoutedEventArgs e)
        {
            AddSnippetWindow snippetWindow = new();
            snippetWindow.Owner = this;
            if (snippetWindow.ShowDialog() == true)
            {
                snippetsList.Add(snippetWindow.GetSnippet());
            }
        }

        private string ConvertSnippetToText(Snippet snippet)
        {
            StringBuilder snippetOutput = new();
            return snippetOutput
                .AppendLine(SNIPPET_SEPARATOR)
                .AppendLine($"{NAME_PREFIX}{snippet.Title}")
                .AppendLine($"{DESCRIPTION_PREFIX}{snippet.Description}")
                .AppendLine($"{USE_COUNT_PREFIX}{snippet.UseCount}")
                .AppendLine(SNIPPET_FOOTER)
                .AppendLine()
                .AppendLine(PREREQUISITES)
                .AppendLine(snippet.Prerequisites)
                .AppendLine(CODE_START)
                .AppendLine(snippet.Code)
                .AppendLine(CODE_END)
                .AppendLine()
                .AppendLine()
                .AppendLine()
                .ToString();
        }

        private void SaveSnippetsOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StringBuilder snippetsBuilder = new();
            foreach (Snippet snippet in snippetsList)
            {
                snippetsBuilder = snippetsBuilder.AppendLine(ConvertSnippetToText(snippet));
            }

            string appDirectory = Path.GetDirectoryName(snippetsPath);
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }
            File.WriteAllText(snippetsPath, snippetsBuilder.ToString());
        }

        private void DeleteSnippetClick(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedIndex != -1)
            {
                snippetsList.RemoveAt(snippetsListView.SelectedIndex);
                snippetTxt.Text = string.Empty;
            }
        }
    }
}
