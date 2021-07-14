using Snippets.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void FindSnippetClick(object sender, RoutedEventArgs e)
        {
            FindSnippet();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                FindSnippet();
            }
        }

        private void FindSnippet()
        {
            string[] searchTerms = searchTxt.Text.ToLowerInvariant().Split(" ");

            List<int> searchScores = new();
            for (int i = 0; i < snippetsListView.Items.Count; i++)
            {
                Snippet snippet = snippetsListView.Items[i] as Snippet;
                searchScores.Add(GetSearchScore(snippet.Name.ToLowerInvariant(), searchTerms) + GetSearchScore(snippet.Description.ToLowerInvariant(), searchTerms));
            }
            int searchResultIndex = IndexOfMaxValue(searchScores);
            if (searchScores[searchResultIndex] > 0)
            {
                snippetsListView.ScrollIntoView(snippetsListView.Items[searchResultIndex]);
                snippetsListView.SelectedIndex = searchResultIndex;
            }
        }

        private int GetSearchScore(string input, string[] searchTerms)
        {
            int score = 0;
            foreach (string searchTerm in searchTerms)
            {
                if (input.Contains(searchTerm))
                {
                    score++;
                }
            }

            return score;
        }

        private static int IndexOfMaxValue(IList<int> list)
        {
            int size = list.Count;

            if (size < 2)
            {
                return size - 1;
            }

            int maxValue = list[0];
            int maxIndex = 0;

            for (int i = 1; i < size; ++i)
            {
                int thisValue = list[i];
                if (thisValue > maxValue)
                {
                    maxValue = thisValue;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private void ShowEditWindowClick(object sender, RoutedEventArgs e)
        {
            if (snippetsListView.SelectedItem is Snippet selectedSnippet)
            {
                AddSnippetWindow snippetWindow = new(selectedSnippet);
                snippetWindow.Owner = this;
                if (snippetWindow.ShowDialog() == true)
                {
                    Snippet editedSnippet = snippetWindow.GetSnippet();
                    int index = snippetsList.IndexOf(selectedSnippet);
                    snippetsList.Remove(selectedSnippet);
                    snippetsList.Insert(index, editedSnippet);
                }
            }
        }
    }
}
