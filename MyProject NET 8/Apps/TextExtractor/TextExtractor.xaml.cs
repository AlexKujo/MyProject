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
using System.Windows.Shapes;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace MyProject_NET_8.Apps.TextExtractor
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
        }

        private void ExtractText(string filePath)
        {
            HtmlDocument htmDoc = new HtmlDocument();
            htmDoc.Load(filePath, Encoding.UTF8);

            var titleParaNodes = htmDoc.DocumentNode.SelectNodes("//div[(contains(@class, 'title') and not(parent::div[@class='figure'])) or contains(@class, 'para p')]");
            var reqSupportNodes = htmDoc.DocumentNode.SelectNodes("//div[@class='name' or @class='partNumber']");

            ExtractedContentBox.Items.Add("ИНСТРУМЕНТЫ");
            AddNodes(reqSupportNodes, "name");

            AddEmptyLine();

            ExtractedContentBox.Items.Add("ШАГИ");
            AddNodes(titleParaNodes, "title");
        }

        private void AddNodes(HtmlNodeCollection nodes, string firstNode)
        {
            foreach (var node in nodes)
            {
                if (node.Attributes["class"].Value == firstNode)
                {
                    AddEmptyLine();

                    if (node.Attributes["class"].Value == "title")
                        ExtractedContentBox.Items.Add(node.InnerText.Trim().ToUpper());
                    else
                        AddTextNode(node);
                }
                else
                {
                    AddTextNode(node);
                }
            }
        }

        private void AddTextNode(HtmlNode node) => ExtractedContentBox.Items.Add(node.InnerText.Trim());

        private void AddEmptyLine() => ExtractedContentBox.Items.Add(string.Empty);

        private void ExtractedContentBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filesPaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                ExtractText(filesPaths[0]);
            }
        }
            
        private void ExtractedContentBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (ExtractedContentBox.SelectedItem != null)
                {
                    var selectedItemText = ExtractedContentBox.SelectedItem.ToString();
                    Clipboard.SetText(selectedItemText);

                    MessageBox.Show("Текст скопирован в буфер обмена.");
                }
            }
        }
    }
}
