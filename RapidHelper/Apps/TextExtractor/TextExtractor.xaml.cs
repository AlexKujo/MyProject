using HtmlAgilityPack;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RapidHelper.Apps.TextExtractor
{
    /// <summary>
    /// Логика взаимодействия для ExtractHtmlTextWindow.xaml
    /// </summary>
    public partial class TextExtractor : Window
    {
        private HtmlDocument htmlDoc;

        public TextExtractor()
        {
            InitializeComponent();
            htmlDoc = new HtmlDocument();
            ExtractedListBox.ItemsSource = new List<string> { "Перетащите HTML-файл сюда" };
        }

        private void ExtractedContentBox_Drop(object sender, DragEventArgs e)
        {
            ExtractedListBox.ItemsSource = null;

            if (ExtractedListBox.Items != null)
                ExtractedListBox.Items.Clear();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filesPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                ExtractText(filesPaths[0]);
            }
        }

        private void ExtractText(string filePath)
        {
            HtmlDocument htmDoc = new HtmlDocument();
            htmDoc.Load(filePath, Encoding.UTF8);

            var titleParaNodes = htmDoc.DocumentNode.SelectNodes("//div[(contains(@class, 'title') and not(parent::div[@class='figure'])) or contains(@class, 'para p')]");
            var reqSupportNodes = htmDoc.DocumentNode.SelectNodes("//div[@class='name' or @class='partNumber']");

            // Заголовок "ИНСТРУМЕНТЫ"
            AddHeader("ИНСТРУМЕНТЫ");
            AddNodes(reqSupportNodes, "name");

            // Пустая строка
            AddEmptyLine();

            // Заголовок "ШАГИ"
            AddHeader("ШАГИ");
            AddNodes(titleParaNodes, "title");
        }

        private void AddNodes(HtmlNodeCollection nodes, string firstNode)
        {
            foreach (var node in nodes)
            {
                if (node.Attributes["class"].Value == firstNode)
                {
                    if (node.Attributes["class"].Value == "title" || node.Attributes["class"].Value == "name")
                    {
                        var toolsTitleItem = new ListBoxItem
                        {
                            Content = node.InnerText.Trim(),
                            FontWeight = FontWeights.SemiBold,
                            Margin = new Thickness(0, 3, 5, 0)
                        };

                        ExtractedListBox.Items.Add(toolsTitleItem);
                    }
                    else
                        AddTextNode(node);
                }
                else
                {
                    AddTextNode(node);
                }
            }
        }

        private void AddHeader(string title)
        {
            var toolsTitleItem = new ListBoxItem { Content = title, FontWeight = FontWeights.DemiBold, FontSize = 14, Background = Brushes.WhiteSmoke };
            ExtractedListBox.Items.Add(toolsTitleItem);
        }

        private void AddTextNode(HtmlNode node) => ExtractedListBox.Items.Add(node.InnerText.Trim());

        private void AddEmptyLine() => ExtractedListBox.Items.Add(string.Empty);
            
        private void ExtractedContentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (ExtractedListBox.SelectedItem != null)
                {
                    CopyText();
                }
            }
        }

        private void ContextMenu_Copy_Click(object sender, RoutedEventArgs e) => CopyText();

        private void CopyText()
        {
            var selectedItemText = ExtractedListBox.SelectedItem.ToString();
            Clipboard.SetText(selectedItemText);
        }
    }
}
