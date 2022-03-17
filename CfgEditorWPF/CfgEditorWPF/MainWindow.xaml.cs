using System;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
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

namespace CfgEditorWPF
{
    public partial class MainWindow : Window
    {
        internal string path = @"";

        public MainWindow()
        {
            InitializeComponent();
            loadPath();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cfg files (*.cfg)|*.cfg";
            if (openFileDialog.ShowDialog() == true)
            {
                path = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                pathLabel.Content = path;
                savePath(path);
            }
            else
            {
                return;
            }
        }

        private void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            TextRange txtr = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);

            string saveTxt = txtr.Text;
            StreamWriter sw = new StreamWriter(System.IO.Path.Combine(path, $"{cfgSelector.SelectedItem}"));
            sw.WriteLine(saveTxt);
            sw.Close();

            MessageBox.Show($"{cfgSelector.SelectedItem} was saved successfully");
        }

        private void ReloadButton_Clicked(object sender, RoutedEventArgs e)
        {
            cfgSelector.Items.Clear();
            fillListBox(cfgSelector, path, "*.cfg");
        }

        private void TerminateButton(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cfgSelector.SelectedItem != null)
                fillRichTextBox(cfgSelector);
            else return;
        }

        internal void loadPath()
        {
            try
            {
                StreamReader reader = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + "\\savedPath\\" + "Saved_path.txt");
                path = reader.ReadLine();
                pathLabel.Content = path;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Files missing D:\nCreating missing files :D\n\n {ex.Message}");
                pathLabel.Content = ex.Message;

                if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "savedPath"))
                {
                    Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "savedPath");
                    File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\savedPath\\" + "Saved_path.txt");
                }
                else if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\savedPath\\" + "Saved_path.txt"))
                {
                    File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\savedPath\\" + "Saved_path.txt");
                }
            }

            loadGuide();
        }

        internal void savePath(string pathToSave)
        {
            StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\savedPath\\" + "Saved_path.txt");
            sw.WriteLine(pathToSave);
            sw.Close();
        }

        internal void fillListBox(ListBox lbs, string folder, string fileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(folder);
            FileInfo[] files = dinfo.GetFiles(fileType);
            foreach (FileInfo file in files)
            {
                lbs.Items.Add(file.Name);
            }
        }

        internal void fillRichTextBox(ListBox lbs)
        {
            Editor.Document.Blocks.Clear();
            Editor.AppendText(File.ReadAllText(path + $"\\{lbs.SelectedItem}"));
        }

        internal void loadGuide()
        {
            Editor.AppendText("--=== How to use ===--\n\nPress the browse button to find your .cfg files I.e.  steamapps\\common\\Gorilla Tag\\BepInEx\\config" +
            "\n\nPress the reload button to load every config file in" +
            "\n\nPress a file in the left hierarchy and edit the text in the right window" +
            "\n\nRemember to save when you are done" +
            "\n\nThis text will go away when you open a file or delete it");
        }
    }
}
