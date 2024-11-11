using System;
using System.IO;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace MyProject_NET_8.TextTemplates.AddTool
{
    /// <summary>
    /// Логика взаимодействия для WindowAddTool.xaml
    /// </summary>
    public partial class WindowAddTool : Window
    {
        private ObservableCollection<Tool> _tools;

        public WindowAddTool(ObservableCollection<Tool> tools)
        {
            InitializeComponent();
            _tools = tools;
        }

        private void InputTool_Click(object sender, RoutedEventArgs e)
        {
            var newTool = new Tool(NameTextBox.Text, IdentNumberTextBox.Text, category: "other");

            _tools.Add(newTool);

            MessageBox.Show("Инструмент успешно добавлен!");
            this.Close();
        }
    }
}
