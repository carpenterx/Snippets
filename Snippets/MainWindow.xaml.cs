using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snippets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string SNIPPETS_PATH = @"C:\Users\jorda\Desktop\snippets.txt";
        public MainWindow()
        {
            InitializeComponent();

            ReadSnippetsFile(SNIPPETS_PATH);
        }

        private void ReadSnippetsFile(string path)
        {
            snippetTxt.Text = File.ReadAllText(path);
        }
    }
}
