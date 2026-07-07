using Newtonsoft.Json;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileOrganizer.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    /// 

    public class Category
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> Extensions { get; set; }
    }

    public class CategoriesConfig
    {
        public List<Category> Categories { get; set; }
    }


    public partial class MainPage : Page
    {
        public string rootPath;
        private CategoriesConfig _currentConfig;
        private List<string> _processedFilesLog = new List<string>();
        public MainPage()
        {
            InitializeComponent();
            ParseJson();
            FilePathTxt.Text = Properties.Settings.Default.LastUsedPath;

        }

        private void SearchPath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку для организации файлов";
                dialog.ShowNewFolderButton = true;

                if (!string.IsNullOrEmpty(FilePathTxt.Text) && System.IO.Directory.Exists(FilePathTxt.Text))
                {
                    dialog.SelectedPath = FilePathTxt.Text;
                }
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    rootPath = dialog.SelectedPath;
                    FilePathTxt.Text = dialog.SelectedPath;
                }
            }
        }

        private void Perform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Perform.IsEnabled = false;
                if (string.IsNullOrEmpty(rootPath))
                {
                    System.Windows.MessageBox.Show("Пожалуйста, выберите папку для организации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Perform.IsEnabled = true;
                    return;
                }

                Properties.Settings.Default.LastUsedPath = rootPath;
                Properties.Settings.Default.Save();

                int processedCount = OrganizeFiles();
                System.Windows.MessageBox.Show($"Организация файлов завершена!\nОбработано файлов: {processedCount}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Perform.IsEnabled = true;
            }
        }

        private int OrganizeFiles()
        {
            string[] files = System.IO.Directory.GetFiles(rootPath);
            int processedCount = 0;
            foreach (string filePath in files)
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(filePath);
                    if (string.IsNullOrEmpty(extension))
                        continue;
                    extension = extension.ToLowerInvariant();
                    string targetCategory = GetCategoryForExtension(extension);
                    if (targetCategory == null)
                    {
                        targetCategory = "Other";
                    }

                    string destinationPath = System.IO.Path.Combine(rootPath, targetCategory);
                    if (!System.IO.Directory.Exists(destinationPath))
                    {
                        System.IO.Directory.CreateDirectory(destinationPath);
                    }
                    string fileName = System.IO.Path.GetFileName(filePath);
                    string destinationFile = System.IO.Path.Combine(destinationPath, fileName);

                    if (System.IO.File.Exists(destinationFile))
                    {
                        string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        string ext = System.IO.Path.GetExtension(filePath);
                        string newFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
                        destinationFile = System.IO.Path.Combine(destinationPath, newFileName);
                    }
                    System.IO.File.Move(filePath, destinationFile);
                    processedCount++;
                    string logMessage = $"Перемещен: {fileName} -> {targetCategory}";
                    _processedFilesLog.Add(logMessage);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Ошибка при обработке {System.IO.Path.GetFileName(filePath)}: {ex.Message}";
                    _processedFilesLog.Add(errorMessage);
                }
            }
            return processedCount;
        }

        private string GetCategoryForExtension(string extension)
        {
            foreach (var category in _currentConfig.Categories)
            {
                if (category.Extensions.Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return category.Name;
                }
            }
            return null;
        }

        private CategoriesConfig ParseJson()
        {
            try
            {
                string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "categories.json");
                if (!File.Exists(jsonPath))
                {
                    System.Windows.MessageBox.Show($"Файл categories.json не найден по пути: {jsonPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                string jsonContent = File.ReadAllText(jsonPath);
                _currentConfig = JsonConvert.DeserializeObject<CategoriesConfig>(jsonContent);
                return _currentConfig;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ButSetting_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.SettingPages());
        }

    }
}
