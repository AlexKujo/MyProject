using RapidHelper.Apps.Encoder;
using RapidHelper.Apps.FolderOrganizer;
using RapidHelper.Apps.ProcedureReverse;
using RapidHelper.Apps.TextExtractor;
using RapidHelper.Apps.TextTemplates;
using RapidHelper;
using System.Windows;

namespace RapidHelper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowManager _windowManager;

        public MainWindow()
        {
            InitializeComponent();
            ShowVersion();

            _windowManager = new WindowManager(this);
        }

        private void ShowVersion()
        {
            VersionBox.Text = VersionInfo.GetVersion();
        }

        private void EncoderButton_Click(object sender, RoutedEventArgs e)
        {
            ICN_Encoder changeEncoding = new ICN_Encoder();
            _windowManager.OpenChildWindow(changeEncoding);
        }

        private void ExtractHtmlText_Click(object sender, RoutedEventArgs e)
        {
            TextExtractor extractHtmlTextWindow = new TextExtractor();
            _windowManager.OpenChildWindow(extractHtmlTextWindow);
        }

        private void TextPresetsButton_Click(object sender, RoutedEventArgs e)
        {
            TextTemplatesWindow textTemplatesWindow = new TextTemplatesWindow();
            _windowManager.OpenChildWindow(textTemplatesWindow);
        }

        private void ProcedureReverserButton_Click(object sender, RoutedEventArgs e)
        {
            ProcedureReverser procedureReverser = new ProcedureReverser();
            _windowManager.OpenChildWindow(procedureReverser);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderOrganizer folderOrganizer = new FolderOrganizer();
            _windowManager.OpenChildWindow(folderOrganizer);
        }
    }

    public class WindowManager
    {
        private Window _mainWindow;

        public WindowManager(Window mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void OpenChildWindow(Window childWindow)
        {
            _mainWindow.Visibility = Visibility.Hidden;
            childWindow.Closed += (s, args) => _mainWindow.Visibility = Visibility.Visible;
            childWindow.Show();
        }
    }
}