using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyProject_NET_8.TextTemplates;
using MyProject_NET_8.TextTemplates.AddTool;
using MyProject_NET_8.TextTemplates.EditToolsList;
using System.Xml.Linq;

namespace MyProject_NET_8
{
    /// <summary>
    /// Логика взаимодействия для TextTemplatesWindow.xaml
    /// </summary>
    public partial class TextTemplatesWindow : Window
    {
        private ObservableCollection<MechThread> _threads;
        private ObservableCollection<Tool> _tools;
        private ObservableCollection<Tool> _selectedTools;
        private ObservableCollection<ToolTagModel> _toolsTagExport;

        private string _jsonFilePath;

        private ToolFilter _toolFilter;

        private Template _template;

        public TextTemplatesWindow()
        {
            InitializeComponent();
            InitializeData();
            InitializeEvents();
        }

        private void InitializeData()
        {
            _jsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            _selectedTools = new ObservableCollection<Tool>();
            _template = new Template();
            _threads = LoadJson<MechThread>("Threads");
            _tools = LoadJson<Tool>("Tools");
            _toolFilter = new ToolFilter();
            _toolsTagExport = new ObservableCollection<ToolTagModel>();

            SortToolsList(_tools);

            var sizes = _tools.Where(tool => tool.WrenchSize > 0)
                     .Select(tool => tool.WrenchSize)
                     .Distinct()
                     .OrderBy(size => size)
                     .ToList();
            ComboBoxWrenchSize.ItemsSource = sizes;

            ComboBoxInputThread.ItemsSource = _threads;
            ComboBoxInputThread.DisplayMemberPath = "Designation";

            ListBoxTools.ItemsSource = _tools;
            ListBoxTools.DisplayMemberPath = "Name";

            ListBoxToolsExport.ItemsSource = _toolsTagExport;
        }

        private void InitializeEvents()
        {
            ComboBoxInputThread.SelectionChanged += UpdateTemplateText;

            InstallationCheckBox.Checked += UpdateTemplateText;
            InstallationCheckBox.Unchecked += UpdateTemplateText;

            RemoveCheckBox.Checked += UpdateTemplateText;
            RemoveCheckBox.Unchecked += UpdateTemplateText;

            ImpactPointsTextBox.TextChanged += UpdateTemplateText;
        }

        private ObservableCollection<T> LoadJson<T>(string jsonName)
        {
            string jsonPath = System.IO.Path.Combine(_jsonFilePath, jsonName + ".json");

            string jsonString = File.ReadAllText(jsonPath);

            return JsonSerializer.Deserialize<ObservableCollection<T>>(jsonString);
        }

        private void SortToolsList(ObservableCollection<Tool> tools)
        {
            // Создаем отсортированный список
            var sortedList = tools.OrderBy(tool => tool.Name)
                                  .ThenBy(tool => tool.WrenchSize)
                                  .ToList();

            // Очищаем коллекцию и добавляем отсортированные элементы
            tools.Clear();
            foreach (var tool in sortedList)
            {
                tools.Add(tool);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;

            if (currentCheckBox == RemoveCheckBox)
            {
                InstallationCheckBox.IsChecked = false;
            }
            else if (currentCheckBox == InstallationCheckBox)
            {
                RemoveCheckBox.IsChecked = false;
            }

            UpdateToolsList();
            UpdateTemplateText(sender, e);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateToolsList();
            UpdateTemplateText(sender, e);
        }

        private void ComboBoxWrenchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateToolsList();
        }

        private void ComboBoxWrenchSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateToolsList();
        }

        private void ComboBoxToolType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateToolsList();
        }

        private void ComboBoxInputThread_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTemplateText(sender, e);
            UpdateToolsList();
        }

        private void ImpactPointsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = char.IsDigit(e.Text, 0) == false;
        }

        private void ListBoxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Tool tool in e.AddedItems)
            {
                if (tool != null)
                {
                    _selectedTools.Add(tool);
                }
            }

            foreach (Tool tool in e.RemovedItems)
            {
                if (tool != null)
                {
                    _selectedTools.Remove(tool);
                }
            }

            UpdateExportList();
            UpdateTemplateText(null, null); // Обновляем текст шаблона
        }

        private void UpdateExportList()
        {
            _toolsTagExport.Clear();

            foreach (Tool tool in _selectedTools)
            {
                _toolsTagExport.Add(new ToolTagModel(tool.Name, tool.IdentNumber));
            }
        }

        private void UpdateToolsList()
        {
            // Получаем выбранные значения
            var selectedType = (ComboBoxToolType.SelectedItem as ComboBoxItem)?.Tag as string;
            float? wrenchSize = ComboBoxWrenchSize.SelectedItem as float?;
            float? threadWrenchSize = GetThreadWrenchSize();
            bool isThreadFilterEnabled = ThreadFilterCheckBox.IsChecked == true;

            // Применяем фильтрацию с помощью ToolFilter
            var filteredTools = _tools.AsEnumerable();
            filteredTools = _toolFilter.FilterTools(filteredTools, selectedType, wrenchSize, threadWrenchSize, isThreadFilterEnabled);

            // Обновляем ListBox
            ListBoxTools.ItemsSource = filteredTools.ToList();
        }

        private float? GetThreadWrenchSize() => (ComboBoxInputThread.SelectedItem as MechThread)?.WrenchSize;

        private void UpdateTemplateText(object sender, RoutedEventArgs e)
        {
            var selectedThread = (MechThread)ComboBoxInputThread.SelectedItem;
            var selectedToolsNames = _selectedTools.Select(tool => tool.Name).ToList();

            _template.ImpactPoints = ImpactPointsTextBox.Text;

            if (selectedThread != null)
            {
                _template.MinMomentKgSFM = selectedThread.MinValue;
                _template.MaxMomentKgSFM = selectedThread.MaxValue;
            }

            if (InstallationCheckBox.IsChecked == true)
            {
                _template.Tools = selectedToolsNames;
                TextBoxTemplate.Text = _template.GenerateInstallationText();
            }
            else if (RemoveCheckBox.IsChecked == true)
            {
                _template.Tools = selectedToolsNames;
                TextBoxTemplate.Text = _template.GenerateRemoveText();
            }
            else
            {
                TextBoxTemplate.Text = string.Empty;
            }
        }

        private void TextBoxTemplate_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(TextBoxTemplate.Text);
            NoticeHelper.ShowNotification(CopyNotificationLabel, "Текст скопирован в буфер обмена!");
        }

        private void ButtonEditToolsList_Click(object sender, RoutedEventArgs e)
        {
            WindowEditToolsList windowEditToolsList = new WindowEditToolsList(_tools);
            windowEditToolsList.ShowDialog();
            SortToolsList(_tools);
        }

        private void ButtonResetAll_Click(object sender, RoutedEventArgs e)
        {
            ImpactPointsTextBox.Text = string.Empty;

            InstallationCheckBox.IsChecked = false;
            RemoveCheckBox.IsChecked = false;
            ThreadFilterCheckBox.IsChecked = false;

            ComboBoxInputThread.SelectedItem = null;
            ComboBoxToolType.SelectedItem = null;
            ComboBoxWrenchSize.SelectedItem = null;

            ListBoxTools.SelectedItems.Clear();

            _toolsTagExport.Clear();
            _selectedTools.Clear();
        }

        private void CopyToolsToClipboard(IEnumerable<ToolTagModel> tools)
        {
            if (_toolsTagExport == null || _toolsTagExport.Count == 0)
            {
                MessageBox.Show("Список инструментов пуст.");
                return;
            }

            // Создание корневого элемента
            var reqSupportEquips = new XElement("reqSupportEquips");

            // Создание группы описаний инструментов
            var supportEquipDescrGroup = new XElement("supportEquipDescrGroup");

            // Добавление каждого инструмента в XML
            foreach (var tool in tools)
            {
                // Получаем XML строку для каждого инструмента
                var tagStructure = tool.GetTagStructure();
                // Добавляем XML строку как элемент
                var toolElement = XElement.Parse(tagStructure);
                supportEquipDescrGroup.Add(toolElement);
            }

            // Добавление группы описаний в корневой элемент
            reqSupportEquips.Add(supportEquipDescrGroup);

            // Формирование финального XML
            var document = new XDocument(reqSupportEquips);
            var xmlString = document.ToString();

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(CopyNotificationLabel, "Группа инструментов скопированы в буфер обмена.");
        }

        private void CopyToolsToClipboard(ToolTagModel tool)
        {

            // Получаем XML строку для инструмента
            var tagStructure = tool.GetTagStructure();
            var toolElement = XElement.Parse(tagStructure);

            // Формирование финального XML только для одного инструмента
            var document = new XDocument(toolElement);
            var xmlString = document.ToString();

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(CopyNotificationLabel, "Инструмент скопирован в буфер обмена.");
        }

        private void ContextMenu_CopyTool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is ToolTagModel tool)
            {
                CopyToolsToClipboard(tool); // Вызываем метод для копирования одного инструмента
            }
        }
        private void ButtonCopyTags_Click(object sender, RoutedEventArgs e)
        {
            CopyToolsToClipboard(_toolsTagExport);
        }
    }
}
