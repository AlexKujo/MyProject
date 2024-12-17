using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MyProject_NET_8.Apps.ProcedureReverse
{
    /// <summary>
    /// Логика взаимодействия для ProcedureReverserWindow.xaml
    /// </summary>
    public partial class ProcedureReverser : Window
    {
        private string _inputXmlText;
        private string _outputXmlText;
        private string _outputEditedText;

        private XDocument _outputDocument;

        public ProcedureReverser()
        {
            InitializeComponent();
        }

        private void DisplayProcedure(XDocument document, ListBox listBox)
        {
            // Очистка ListBox перед добавлением новых элементов
            listBox.Items.Clear();

            var steps = document.Descendants("proceduralStep");

            foreach (var step in steps)
            {
                string title = step.Element("title")?.Value.Trim();
                var paragraphs = step.Elements("para").Select(p => p.Value.Trim()).ToList();

                // Создание объекта ProceduralStep
                ProceduralStep proceduralStep = new ProceduralStep(title, paragraphs);

                // Добавление объекта в ListBox
                listBox.Items.Add(proceduralStep);
            }
        }

        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputProcedure.Items.IsEmpty)
            {
                MessageBox.Show("Нет шагов для инвертирования");
                return;
            }

            // Исходный XML в виде строки
            string inputProcedure = _inputXmlText;

            // Загружаем XML
            XDocument document = XDocument.Parse(inputProcedure);

            // Получаем все шаги proceduralStep
            var steps = document.Descendants("proceduralStep").ToList();

            // Инвертируем шаги
            steps.Reverse();

            // Инвертируем параграфы в каждом шаге
            InvertElements(steps, "para");

            // Создаем новый документ для инвертированных шагов
            _outputDocument = new XDocument(new XElement("mainProcedure", steps));

            // Отображение шагов в ListBox
            DisplayProcedure(_outputDocument, OutputProcedure);

            // Сохранение инвертированного XML в переменную
            _outputDocument = CreateXml(steps);
            _outputXmlText = _outputDocument.ToString();
        }

        private void RemoveEmpty(XElement step, string tagName)
        {
            step.Elements(tagName)
                .Where(para => string.IsNullOrWhiteSpace(para.Value))
                .Remove();
        }

        private void InvertElements(List<XElement> elements, string tagName)
        {
            // Инвертируем параграфы в каждом шаге
            foreach (var step in elements)
            {
                RemoveEmpty(step, tagName);

                var paragraphs = step.Elements(tagName).Reverse().ToList();
                step.Elements(tagName).Remove();
                step.Add(paragraphs);
            }
        }

        private XDocument EditXml(XDocument document)
        {
            Cleaner cleaner = new Cleaner();

            cleaner.RemoveBlocks(document);
            cleaner.ReplaceWords(document);
            cleaner.SetNumeration(document);

            return document;
        }

        private void EditCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            XDocument editedDocument = new XDocument(_outputDocument);

            EditXml(editedDocument);
            _outputEditedText = editedDocument.ToString();

            DisplayProcedure(editedDocument, OutputProcedure);
        }

        private void EditCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DisplayProcedure(_outputDocument, OutputProcedure);
        }

        private XDocument CreateXml(List<XElement> steps)
        {
            // Создание нового документа для инвертированного XML
            XDocument outputDocument = new XDocument(new XElement("mainProcedure"));

            foreach (var step in steps)
            {
                string title = step.Element("title")?.Value.Trim();
                var paragraphs = step.Elements("para").Select(p => p.Value.Trim()).ToList();

                // Добавление инвертированного шага в новый документ с инвертированными параграфами
                XElement newStep = new XElement("proceduralStep",
                    new XElement("title", title),
                    paragraphs.Select(p => new XElement("para", p))
                );

                outputDocument.Root.Add(newStep);
            }

            return outputDocument;
        }

        private void PasteFromClipboard()
        {
            // Получение текста из буфера обмена
            _inputXmlText = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(_inputXmlText))
            {
                MessageBox.Show("Буфер обмена пуст или не содержит текста.");
                return;
            }

            try
            {
                // Парсинг XML-текста
                XDocument document = XDocument.Parse(_inputXmlText);

                // Отображение шагов в ListBox
                DisplayProcedure(document, InputProcedure);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при парсинге XML: " + ex.Message);
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            PasteFromClipboard();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (OutputProcedure.Items.IsEmpty)
            {
                MessageBox.Show("Нечего копировать");
                return;
            }

            if (EditCheckBox.IsChecked == true)
            {
                Clipboard.SetText(_outputEditedText);
                NoticeHelper.ShowNotification(Label,"Скопировано!");
            }
            else
            {
                Clipboard.SetText(_outputXmlText);
                NoticeHelper.ShowNotification(Label,"Скопировано!");
            }
        }
    }

    public class ProceduralStep
    {
        public string Title { get; set; }
        public List<string> Paragraphs { get; set; }

        public ProceduralStep(string title, List<string> paragraphs)
        {
            Title = title;
            Paragraphs = paragraphs;
        }
    }
}
