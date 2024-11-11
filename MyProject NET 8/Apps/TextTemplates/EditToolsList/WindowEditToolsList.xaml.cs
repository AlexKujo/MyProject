using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyProject_NET_8.TextTemplates.AddTool;

namespace MyProject_NET_8.TextTemplates.EditToolsList
{
    /// <summary>
    /// Логика взаимодействия для WindowEditToolsList.xaml
    /// </summary>
    public partial class WindowEditToolsList : Window
    {
        private string _jsonName;
        private string _jsonFilePath;

        private ObservableCollection<Tool> _editedTools;

        public WindowEditToolsList(ObservableCollection<Tool> tools)
        {
            _jsonName = "Tools.json";
            _jsonFilePath = @"D:\VisualStudio Projects\WorkProject\Main\TextTemplates\" + _jsonName;

            _editedTools = tools;

            InitializeComponent();
            InitializeData();

        }

        private void InitializeData()
        {
            ListBoxTools.DisplayMemberPath = "Name";
            ListBoxTools.ItemsSource = _editedTools;
        }

        private void ButtonAddTool_Click(object sender, RoutedEventArgs e)
        {
            WindowAddTool addToolWindow = new WindowAddTool(_editedTools);

            addToolWindow.ShowDialog();

            SaveJson(_editedTools);
        }

        private void ButtonDeleteTool_Click(object sender, RoutedEventArgs e)
        {
            // Получаем список выбранных элементов
            string message;

            if (ListBoxTools.SelectedItem != null)
            {
                message = "Вы уверены, что хотите удалить выбранный инструмент?";

                MessageBoxResult result = MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var deletedTool = (Tool)ListBoxTools.SelectedItem;
                    _editedTools.Remove(deletedTool);
                }

                SaveJson(_editedTools);
            }
            else
            {
                MessageBox.Show("Выберите хотя бы один инструмент для удаления.");
            }


        }

        private void SaveJson<T>(ObservableCollection<T> data)
        {
            //string relativePath = $"TextTemplates/{jsonName}.json";
            //string jsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_jsonFilePath, jsonString);
        }


        private void ListBoxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Tool selectedTool = (Tool)ListBoxTools.SelectedItem;

            if (selectedTool != null)
            {
                StackPanelDetails.DataContext = ListBoxTools.SelectedItem;
            }
        }
    }
}
