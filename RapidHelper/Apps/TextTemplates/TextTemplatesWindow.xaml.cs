
using RapidHelper.Apps.TextTemplates;
using RapidHelper.Apps.TextTemplates.Classes;
using RapidHelper.Lib;
using RapidHelper.TextTemplates;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RapidHelper.Apps.TextTemplates
{
    /// <summary>
    /// Логика взаимодействия для TextTemplatesWindow.xaml
    /// </summary>
    public partial class TextTemplatesWindow : Window
    {
        private ObservableCollection<MechThread> _threads;
        private ObservableCollection<Tool> _tools;
        private ObservableCollection<Tool> _selectedTools;
        private ObservableCollection<Supply> _selectedSupplies;
        private ObservableCollection<Supply> _supplies;

        private string _jsonFilePath;

        private Template _template;

        public TextTemplatesWindow()
        {
            ResourceManager.InitializeResources();

            InitializeComponent();
            InitializeData();
            InitializeEvents();
        }

        private void InitializeData()
        {
            _jsonFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

            _threads = JsonLoader.Load<MechThread>(_jsonFilePath, "Threads");
            _tools = JsonLoader.Load<Tool>(_jsonFilePath, "Tools");
            _supplies = JsonLoader.Load<Supply>(_jsonFilePath, "Supplies");

            _selectedTools = new ObservableCollection<Tool>();
            _selectedSupplies = new ObservableCollection<Supply>();

            _template = new Template();

            ToolsManager.SortToolsList(_tools);

            ComboBoxWrenchSize.ItemsSource = ToolsManager.GetWrenchSizes(_tools);

            ComboBoxInputThread.ItemsSource = _threads;
            ComboBoxInputThread.DisplayMemberPath = "Designation";

            ListBoxTools.ItemsSource = _tools;
            ListBoxSupplies.ItemsSource = _supplies;
            ListBoxToolsExport.ItemsSource = _selectedTools;
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
            ComboBoxToolType.SelectedItem = null;
            ComboBoxWrenchSize.SelectedItem = null;

            if (ComboBoxInputThread.SelectedItem == null)
            {
                ComboBoxToolType.IsEnabled = true;
                ComboBoxWrenchSize.IsEnabled = true;

                MomentFilterCheckBox.IsEnabled = false;
                WrenchSizeFilterCheckBox.IsEnabled = false;
            }
            else
            {
                ComboBoxToolType.IsEnabled = false;
                ComboBoxWrenchSize.IsEnabled = false;

                MomentFilterCheckBox.IsEnabled = true;
                WrenchSizeFilterCheckBox.IsEnabled = true;
            }

            if (MomentFilterCheckBox.IsChecked == true || WrenchSizeFilterCheckBox.IsChecked == true)
            {
                UpdateToolsList();
            }

            UpdateTemplateText(sender, e);
        }

        private void ImpactPointsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = char.IsDigit(e.Text, 0) == false;
        }

        private void ListBox_SelectionChanged<T>(ListBox listBox, ObservableCollection<T> selectedItems, SelectionChangedEventArgs e) where T : class
        {
            foreach (var item in e.AddedItems)
            {
                if (item is T castedItem && selectedItems.Contains(castedItem) == false)
                {
                    selectedItems.Add(castedItem);
                }
            }

            foreach (var item in e.RemovedItems)
            {
                if (item is T castedItem && selectedItems.Contains(castedItem))
                {
                    listBox.SelectedItems.Add(castedItem);
                }
            }

            UpdateTemplateText(null, null); // Обновляем текст шаблона
        }

        private void ListBoxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox_SelectionChanged((ListBox)sender, _selectedTools, e);
        }

        private void ListBoxSupplies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox_SelectionChanged((ListBox)sender, _selectedSupplies, e);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabTools.IsSelected)
            {
                ListBoxToolsExport.ItemsSource = _selectedTools;
                ToolsExportTitle.Content = "Вспомогательное оборудование";

                ComboBoxToolType.IsEnabled = true;
                ComboBoxWrenchSize.IsEnabled = true;
            }
            else if (TabSupplies.IsSelected)
            {
                ListBoxToolsExport.ItemsSource = _selectedSupplies;
                ToolsExportTitle.Content = "Расходные материалы";

                ComboBoxToolType.IsEnabled = false;
                ComboBoxWrenchSize.IsEnabled = false;
                MomentFilterCheckBox.IsEnabled = false;
                WrenchSizeFilterCheckBox.IsEnabled = false;
            }
        }

        private void UpdateToolsList()
        {
            // Получаем выбранные значения
            var selectedType = (ComboBoxToolType.SelectedItem as ComboBoxItem)?.Tag as string;
            float? wrenchSize = ComboBoxWrenchSize.SelectedItem as float?;
            bool isWrenchSizeFilterEnabled = (bool)WrenchSizeFilterCheckBox.IsChecked;
            bool isMomentFilterEnabled = (bool)MomentFilterCheckBox.IsChecked;
            MechThread selectedThread = ComboBoxInputThread.SelectedItem as MechThread;

            // Применяем фильтрацию с помощью ToolFilter
            var filteredTools = _tools.AsEnumerable();

            if (selectedThread != null && (isWrenchSizeFilterEnabled || isMomentFilterEnabled))
            {
                filteredTools = ToolsManager.FilterTools(filteredTools, selectedThread, isWrenchSizeFilterEnabled, isMomentFilterEnabled);
            }
            else
            {
                filteredTools = ToolsManager.FilterTools(filteredTools, selectedType, wrenchSize);
            }

            // Обновляем ListBox
            ListBoxTools.ItemsSource = filteredTools.ToList();
        }

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

        private void ButtonResetAll_Click(object sender, RoutedEventArgs e)
        {
            ImpactPointsTextBox.Text = string.Empty;

            InstallationCheckBox.IsChecked = false;
            RemoveCheckBox.IsChecked = false;
            WrenchSizeFilterCheckBox.IsChecked = false;

            ComboBoxInputThread.SelectedItem = null;
            ComboBoxToolType.SelectedItem = null;
            ComboBoxWrenchSize.SelectedItem = null;
        }

        private void ButtonResetSelection_Click(object sender, RoutedEventArgs e)
        {
            _selectedTools.Clear();
            _selectedSupplies.Clear();
            ListBoxTools.SelectedItems.Clear();
            ListBoxSupplies.SelectedItems.Clear();
        }

        private void ContextMenu_CopyTool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is ITaggable item)
            {
                ClipboardHelper.Copy(item, CopyNotificationLabel); // Вызываем метод для копирования одного инструмента
            }
        }

        private void ButtonCopyTags_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxToolsExport.ItemsSource is IEnumerable<ITaggable> items)
            {
                ClipboardHelper.CopyAll(items, CopyNotificationLabel);
            }            
        }
    }
}
