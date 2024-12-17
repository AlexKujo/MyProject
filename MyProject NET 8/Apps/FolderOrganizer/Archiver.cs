using System.IO;
using System.IO.Compression;
using System.Windows;

namespace MyProject_NET_8.Apps.FolderOrganizer
{
    internal class Archiver
    {
        private List<string> _directories;
        private List<string> _newArchives;
        private ConsoleController _consoleController;

        public Archiver(List<string> directories)
        {
            _directories = directories;
            _newArchives = new List<string>();
            _consoleController = new ConsoleController();
            _consoleController.AllocateConsole();
        }

        public void CreateArchives()
        {
            Console.Clear();
            _consoleController.Show();

            try
            {
                foreach (var directory in _directories)
                {
                    // Обрабатываем каждую папку
                    ProcessDirectory(directory);
                }

                if (_newArchives.Count > 0)
                {
                    var messageBoxResult = System.Windows.MessageBox.Show(
                        $"Созданы архивы:\n{string.Join("\n", _newArchives)}",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    Console.WriteLine("Процесс завершён.");

                    if (messageBoxResult == MessageBoxResult.OK)
                    {
                        _consoleController.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Произошла ошибка: {ex.Message}", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                _consoleController.Hide();
            }
        }

        private void RunConsole() { }

        private void ProcessDirectory(string mainFolderDirectory)
        {
            // Получаем имя текущей папки
            string folderName = Path.GetFileName(mainFolderDirectory);

            string[] subDirectories = Directory.GetDirectories(mainFolderDirectory);
            // Определяем подкаталог для обработки

            if (subDirectories.Length > 0)
            {
                string subFolder = subDirectories[0];

                if (TryGetHtmFileName(subFolder, out string htmFileName) == true)
                {
                    Console.WriteLine($"\nСоздание архива для папки: {folderName}");
                    CreateArchive(Path.GetFullPath(subFolder), htmFileName);
                }
            }
            else
            {
                Console.WriteLine($"Ошибка. Папка \"{folderName}\" не содержит подкаталогов.\n");
                System.Windows.MessageBox.Show($"В папке не найдено подкаталогов.", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private void CreateArchive(string subFolderPath, string htmFileName)        //сделать отображение процесса в консоли
        {
            try
            {
                string parentFolderPath = Directory.GetParent(subFolderPath).ToString();
                string archivePath = Path.Combine(parentFolderPath, $"{htmFileName}.vmb");

                if (File.Exists(archivePath))
                {
                    if (IsRewrite(archivePath, htmFileName) == false)
                        return;
                }

                // Создание архива
                Console.WriteLine($"Создание архива: {archivePath}");
                ZipFile.CreateFromDirectory(subFolderPath, archivePath);
                Console.WriteLine($"Архив создан: {archivePath}");

                _newArchives.Add(archivePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка:{ex.Message}");
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private bool TryGetHtmFileName(string directory, out string htmFileName)
        {
            var files = Directory.GetFiles(directory, "*.htm");

            if (files.Length == 1)
            {
                htmFileName = Path.GetFileNameWithoutExtension(files[0]);
                return true;
            }
            else
            {
                Console.WriteLine($"В папке \"{directory}\" не найдено .htm файла для получения имени архива.\n");
                System.Windows.MessageBox.Show($"Ошибка. Не найдено .htm файла для получения имени архива.", "Внимание", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                htmFileName = string.Empty;
                return false;
            }
        }

        private bool IsRewrite(string archivePath, string htmFileName)
        {
            // Если файл существует, запрашиваем подтверждение замены
            var result = System.Windows.MessageBox.Show
                ($"{archivePath}\n\nФайл {htmFileName}.vmb уже существует. Заменить?",
                "Подтверждение замены",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Console.WriteLine($"Файл {archivePath}.vmb будет заменён");
                // Если пользователь выбрал "Да", удаляем старый файл
                File.Delete(archivePath);
                return true;
            }
            else
            {
                Console.WriteLine("Пропуск каталога. Переход к следующему.");
                return false;
            }
        }
    }
}
