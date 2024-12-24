using MyProject_NET_8.Apps.Encoder;
using MyProject_NET_8.Apps.FolderOrganizer;
using MyProject_NET_8.Apps.ProcedureReverse;
using MyProject_NET_8.Apps.TextExtractor;
using MyProject_NET_8.Apps.TextTemplates;
using MyProject_NET_8;
using System.Windows;

namespace MyProject_NET_8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowVersion()
        {
            VersionLabel.Content = VersionInfo.GetVersion();
        }

        private void EncoderButton_Click(object sender, RoutedEventArgs e)
        {
            ICN_Encoder changeEncoding = new ICN_Encoder();
            changeEncoding.Show();
        }

        private void ExtractHtmlText_Click(object sender, RoutedEventArgs e)
        {
            TextExtractor extractHtmlTextWindow = new TextExtractor();
            extractHtmlTextWindow.Show();
        }

        private void TextPresetsButton_Click(object sender, RoutedEventArgs e)
        {
            TextTemplatesWindow textTemplatesWindow = new TextTemplatesWindow();
            textTemplatesWindow.Show();
        }

        private void ProcedureReverserButton_Click(object sender, RoutedEventArgs e)
        {
            ProcedureReverser procedureReverser = new ProcedureReverser();
            procedureReverser.Show();      
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderOrganizer folderOrganizer = new FolderOrganizer();
            folderOrganizer.Show();
        }
    }
}