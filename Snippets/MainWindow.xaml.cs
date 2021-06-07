using Snippets.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using YamlDotNet.Serialization;

namespace Snippets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string SNIPPET_FOOTER = "========================================================";
        private static readonly string NAME_PREFIX = "//  Name         :  ";
        private static readonly string DESCRIPTION_PREFIX = "//  Description  :  ";
        private static readonly string USE_COUNT_PREFIX = "//  Used         :  ";
        private static readonly string PREREQUISITES = "===================  [PREREQUISITES]  ==================";
        private static readonly string CODE_START = "====================  [CODE START]  ====================";
        private static readonly string CODE_END = "=====================  [CODE END]  =====================";
        private ObservableCollection<Snippet> snippetsList = new();

        private static readonly string APPLICATION_FOLDER = "Snippets";
        private static readonly string SNIPPETS_FILE = "snippets.yml";
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
                var input = new StringReader(File.ReadAllText(snippetsPath));
                var deserializer = new DeserializerBuilder().Build();

                ObservableCollection<Snippet> snippets = deserializer.Deserialize<ObservableCollection<Snippet>>(input);
                snippetsList = new ObservableCollection<Snippet>(snippets);
            }

            snippetsListView.ItemsSource = snippetsList;
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
                selectedSnippet.Used++;
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
                .AppendLine($"{NAME_PREFIX}{snippet.Name}")
                .AppendLine($"{DESCRIPTION_PREFIX}{snippet.Description}")
                .AppendLine($"{USE_COUNT_PREFIX}{snippet.Used}")
                .AppendLine(SNIPPET_FOOTER)
                .AppendLine()
                .AppendLine(PREREQUISITES)
                .AppendLine(snippet.Prerequisites)
                .AppendLine(CODE_START)
                .AppendLine(snippet.Code)
                .AppendLine(CODE_END)
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

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(snippetsList);
            File.WriteAllText(snippetsPath, yaml);
            Process.Start("explorer.exe", snippetsPath);
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
