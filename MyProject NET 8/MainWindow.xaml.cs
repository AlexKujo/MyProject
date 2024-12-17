using MyProject_NET_8;
using MyProject_NET_8.Apps.TextExtractor;
using MyProject_NET_8.Apps.Encoder;
using MyProject_NET_8.Apps.ProcedureReverse;
using System;
using System.Collections.Generic;
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
using MyProject_NET_8.Apps.TextTemplates;

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
    }
}