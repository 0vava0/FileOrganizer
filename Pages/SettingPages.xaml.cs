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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FileOrganizer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingPages.xaml
    /// </summary>

    public partial class SettingPages : Page
    {
        private CategoriesConfig _currentConfig;
        private string _JsonPath;
        public SettingPages()
        {
            InitializeComponent();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            List<string> NameCategiryList = new List<string>();
            try
            {
                _JsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "categories.json");
                if (!File.Exists(_JsonPath))
                {
                    System.Windows.MessageBox.Show($"Файл categories.json не найден по пути: {_JsonPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                string jsonContent = File.ReadAllText(_JsonPath);
                _currentConfig = JsonConvert.DeserializeObject<CategoriesConfig>(jsonContent);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке JSON: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            foreach (var categoryName in _currentConfig.Categories)
            {
                NameCategiryList.Add(categoryName.DisplayName);
            }
            NameCategiryList.Add("Добавить новую");
            SetCategory.ItemsSource = NameCategiryList;
            SetCategory.SelectedIndex = 0;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            bool ResultCheckHave = CheckHaveCategory(NameText.Text);
            try
            {
                string Name = NameText.Text.Trim();
                string DisplayName = DisplayNameText.Text.Trim();
                string Extensions = ExtensionsText.Text.Trim();

                if (string.IsNullOrEmpty(Name))
                {
                    MessageBox.Show("Введите название категории!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(DisplayName))
                {
                    MessageBox.Show("Введите отображаемое имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                List<string> extensionsList = new List<string>();
                if (!string.IsNullOrEmpty(Extensions))
                {
                   extensionsList.Add(ExtensionsText.Text.Replace(", ", ""));
                }

                string selectedItem = SetCategory.SelectedItem?.ToString();
                bool isAddNew = selectedItem == "Добавить новую";

                if (isAddNew != false)
                {
                    CreateNewCategory(Name, DisplayName, extensionsList);
                }
                else
                {
                    UpdateCategory(Name, DisplayName, extensionsList);
                }

                SaveEditFile();
                LoadComboBox();
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckHaveCategory(string categoryName)
        {
            return _currentConfig.Categories.Any(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        private async void SetCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButRemove.Visibility = Visibility.Visible;
            ButRemove.Width = 60;
            ClearText();
            await Task.Delay(10);
            string selectedCategoryName = SetCategory.SelectedItem.ToString();
            var existingCategory = _currentConfig.Categories.FirstOrDefault(c => string.Equals(c.DisplayName, selectedCategoryName, StringComparison.OrdinalIgnoreCase));
            if (existingCategory != null)
            {
                NameText.Text = existingCategory.Name;
                DisplayNameText.Text = existingCategory.DisplayName;
                ExtensionsText.Text = string.Join(", ", existingCategory.Extensions);
            }
            else
            {
                ButRemove.Visibility = Visibility.Hidden;
                ButRemove.Width = 0;
                ClearText();
            }
        }

        private void ClearText()
        {
            NameText.Text = string.Empty;
            DisplayNameText.Text = string.Empty;
            ExtensionsText.Text = string.Empty;
        }

        private void CreateNewCategory(string Name, string DisplayName, List<string> ExtensionsList)
        {
            bool nameExists = _currentConfig.Categories.Any(c => string.Equals(c.Name, Name, StringComparison.OrdinalIgnoreCase));
            if (nameExists)
            {
                MessageBox.Show($"Категория '{Name}' уже существует! Используйте редактирование.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var NewCategory = new Category
            {
                Name = Name,
                DisplayName = DisplayName,
                Extensions = ExtensionsList
            };
            _currentConfig.Categories.Add(NewCategory);
        }

        private void UpdateCategory(string Name, string DisplayName, List<string> ExtensionsList)
        {
            string selectedCategoryName = SetCategory.SelectedItem.ToString();
            var existingCategory = _currentConfig.Categories.FirstOrDefault(c => string.Equals(c.DisplayName, selectedCategoryName, StringComparison.OrdinalIgnoreCase));

            if (existingCategory == null)
            {
                MessageBox.Show("Категория не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.Equals(existingCategory.Name, Name, StringComparison.OrdinalIgnoreCase))
            {
                bool nameExists = _currentConfig.Categories.Any(c => string.Equals(c.Name, Name, StringComparison.OrdinalIgnoreCase));
                if (nameExists)
                {
                    MessageBox.Show($"Категория с именем '{Name}' уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            existingCategory.Name = Name;
            existingCategory.DisplayName = DisplayName;
            existingCategory.Extensions = ExtensionsList;
        }

        private void RemoveCategory()
        {
            string selectedCategoryName = SetCategory.SelectedItem.ToString();
            var existingCategory = _currentConfig.Categories.FirstOrDefault(c => string.Equals(c.DisplayName, selectedCategoryName, StringComparison.OrdinalIgnoreCase));
            var Handle = MessageBox.Show($"Уверенны в удалении категории '{existingCategory.DisplayName}'!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Hand);
            if(Handle == MessageBoxResult.Yes)
            {
                try
                {
                    _currentConfig.Categories.Remove(existingCategory);
                    SaveEditFile();
                    LoadComboBox();
                }
                catch
                {
                    MessageBox.Show($"Неудалось удалить категория '{selectedCategoryName}'!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ButRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveCategory();
            SetCategory.SelectedIndex = 0;
        }

        private void SaveEditFile()
        {
            string updatedJson = JsonConvert.SerializeObject(_currentConfig, Formatting.Indented);
            File.WriteAllText(_JsonPath, updatedJson);
        }
    }
}
