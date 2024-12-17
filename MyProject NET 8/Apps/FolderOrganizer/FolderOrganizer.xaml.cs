using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MyProject_NET_8.Apps.FolderOrganizer
{
    /// <summary>
    /// Interaction logic for FolderOrganizer.xaml
    /// </summary>
    public partial class FolderOrganizer : Window
    {
        private DispatcherTimer? _clipboardTimer;
        private string _lastClipboardText;

        public FolderOrganizer()
        {
            InitializeComponent();

            TargetDirectory = string.Empty;
            NewFolders = new List<string>();
            _lastClipboardText = string.Empty;

            //SetUpTimer();
        }

        public string TargetDirectory { get; set; }

        public List<string> ArchiveFolders { get; set; }

        public List<string> NewFolders { get; set; }

        private void SetUpTimer()
        {
            // Инициализация таймера
            _clipboardTimer = new DispatcherTimer();
            _clipboardTimer.Interval = TimeSpan.FromSeconds(1); // Проверяем каждые 1 секунду
            _clipboardTimer.Tick += UpdateClipboardText;
            _clipboardTimer.Start();
        }

        private void UpdateClipboardText(object sender, EventArgs e)
        {
            try
            {
                // Получение текста из буфера обмена
                string clipboardText = System.Windows.Clipboard.GetText();

                // Проверка, изменился ли текст
                if (clipboardText != _lastClipboardText)
                {
                    _lastClipboardText = clipboardText; // Обновляем последнее значение

                    if (string.IsNullOrEmpty(clipboardText))
                    {
                        ClipboardTextBox.Text = "Буфер обмена пуст.";
                    }
                    else
                    {
                        ClipboardTextBox.Text = clipboardText; // Обновляем текст в TextBlock
                    }
                }
            }
            catch (Exception ex)
            {
                ClipboardTextBox.Text = $"Ошибка: {ex.Message}";
            }
        }

        private bool IsRightDirectory()
        {
            if (string.IsNullOrEmpty(TargetDirectory))
            {
                MessageBox.Show("Директория пуста","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return false;
            }
            else if (Directory.Exists(TargetDirectory) == false)
            {
                MessageBox.Show("Такой директории не существует","Ошибка",MessageBoxButton.OK, MessageBoxImage.Error);
                PathTextBox.Text = string.Empty;
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CreateFoldersButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsRightDirectory() == false)
                return;

            HandleClipboardData();
        }

        private bool TrySelectDirectory(bool allowMultiSelect)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Multiselect = allowMultiSelect,
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (allowMultiSelect)
                {
                    ArchiveFolders = dialog.FileNames.ToList();
                }
                else
                {
                    TargetDirectory = dialog.FileName;
                    PathTextBox.Text = TargetDirectory;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private string[] GetClipboardData()
        {
            try
            {
                string clipboardText = System.Windows.Clipboard.GetText();

                // Разделение текста на строки
                string[] lines = clipboardText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                return lines;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка");
            }

            return null;
        }

        private bool IsUserConfirm(Dictionary<string, string> foldersToCreate)
        {
            string message = "Будут созданы следующие каталоги:\n";
            message += "\n" + string.Join("\n", foldersToCreate.Select(name => $"{name.Key}_{name.Value}"));

            if (MessageBoxResult.Yes == System.Windows.MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Asterisk))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsCorrectInput(string[] lines)
        {
            try
            {
                foreach (string line in lines)
                {
                    string[] parts = line.Split();
                    bool isNumber = int.TryParse(parts[0].Trim(), out int result);

                    if (isNumber == false || parts.Length < 3)
                    {
                        MessageBox.Show("Неподходящий формат ввода", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return false;
                    }
                }           
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка");
            }

            return true;
        }

        private void HandleClipboardData()
        {
            try
            {
                string[] lines = GetClipboardData();

                if (IsCorrectInput(lines) == false)
                    return;

                var foldersToCreate = new Dictionary<string, string>();

                foreach (string line in lines)
                {
                    // Разделение строки по табуляции
                    var parts = line.Split('\t');

                    if (parts.Length >= 3) // Убедитесь, что есть номер и название карты
                    {
                        string techCardNumber = parts[0].Trim(); // Номер техкарты
                        string techCardName = parts[2].Trim(); // Название техкарты

                        foldersToCreate[techCardNumber] = techCardName;
                    }
                }

                if (IsUserConfirm(foldersToCreate) == false)
                    return;

                foreach (var folderName in foldersToCreate)
                {
                    string techCardNumber = folderName.Key;
                    string techCardName = folderName.Value;

                    CreateFolder(techCardNumber, techCardName);
                }

                MessageBox.Show("Каталоги успешно созданы", "Успех",MessageBoxButton.OK,MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void CreateFolder(string techCardNumber, string techCardName)
        {
            // Формирование имени папки
            string folderName = ($"{techCardNumber}_{techCardName}");
            string subFolderName = techCardName;
            string folderPath = Path.Combine(TargetDirectory, folderName);

            // Создание папки
            Directory.CreateDirectory(folderPath);

            // Создание подкаталога с таким же именем
            Directory.CreateDirectory(Path.Combine(folderPath, subFolderName));
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            TrySelectDirectory(false);
        }

        private void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrySelectDirectory(true))
            {
                Archiver archiver = new Archiver(ArchiveFolders);

                archiver.CreateArchives();
            }
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e) => TargetDirectory = PathTextBox.Text;
    }
}