using Snippets.Models;
using System;
using System.Collections.ObjectModel;
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
                snippetGrid.DataContext = selectedSnippet;
            }
        }

        private void CopyCodeToClipboard(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedIndex != -1)
            {
                Snippet selectedSnippet = (Snippet)snippetsListView.SelectedItem;
                selectedSnippet.Used++;
                Clipboard.SetText(selectedSnippet.Code);
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

        private void SaveSnippetsOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string appDirectory = Path.GetDirectoryName(snippetsPath);
            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(snippetsList);
            File.WriteAllText(snippetsPath, yaml);
        }

        private void DeleteSnippetClick(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedIndex != -1)
            {
                snippetsList.RemoveAt(snippetsListView.SelectedIndex);
                snippetGrid.DataContext = null;
            }
        }
    }
}
