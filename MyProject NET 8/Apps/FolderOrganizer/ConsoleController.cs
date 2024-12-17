using System.Runtime.InteropServices;

namespace MyProject_NET_8.Apps.FolderOrganizer
{
    internal class ConsoleController
    {
        // Константы для управления окном
        private const int SW_HIDE = 0;           // Скрыть окно
        private const int SW_SHOW = 5;           // Показать окно
        private const int SW_MINIMIZE = 6;       // Свернуть окно
        private const int SW_RESTORE = 9;        // Развернуть окно

        private IntPtr _consoleWindow;
        private bool _isConsoleAllocated;

        public ConsoleController()
        {
            // После создания консоли получаем дескриптор окна
            _consoleWindow = IntPtr.Zero;
            _isConsoleAllocated = false;
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Метод для выделения консоли
        public void AllocateConsole()
        {
            if (!_isConsoleAllocated)
            {
                AllocConsole();  // Создаем консольное окно
                _consoleWindow = FindWindow(null, Console.Title);  // Получаем дескриптор окна консоли
                _isConsoleAllocated = true;
            }
        }

        // Метод для освобождения консоли
        public void FreeConsoleWindow()
        {
            if (_isConsoleAllocated)
            {
                FreeConsole();  // Закрываем консольное окно
                _isConsoleAllocated = false;
                _consoleWindow = IntPtr.Zero;  // Сбрасываем дескриптор окна
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public void Show() => ShowWindow(_consoleWindow, SW_SHOW);

        public void Hide() => ShowWindow(_consoleWindow,SW_HIDE);

        public void Minimize() => ShowWindow(_consoleWindow,SW_MINIMIZE);

        public void Restore() => ShowWindow(_consoleWindow,SW_RESTORE);
    }
}
