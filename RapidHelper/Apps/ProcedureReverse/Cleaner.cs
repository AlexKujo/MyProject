using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RapidHelper.Apps.ProcedureReverse
{
    internal class Cleaner
    {
        private Dictionary<string, string> _replacements;
        private char[] _punctuations;
        private string _removeKeyword;

        public Cleaner()
        {
            _replacements = new Dictionary<string, string>
            {
                {"снять","установить"},
                {"отсоединить","присоединить"},
                {"отвернуть","завернуть"}
            };

            _punctuations = new char[] { ',', '.' };

            _removeKeyword = "Очистить";
        }
        public XDocument ReplaceWords(XDocument document)
        {
            // Проходим по всем элементам документа
            foreach (var element in document.Descendants())
            {
                if (!element.HasElements && !string.IsNullOrWhiteSpace(element.Value))
                {
                    // Элемент не имеет вложенных элементов, работаем с его текстом
                    element.Value = ReplaceInText(element.Value);
                }
            }

            return document;
        }

        public XDocument RemoveBlocks(XDocument document)
        {
            // Проходим по всем элементам документа
            var elementsToRemove = document.Descendants("para")
                .Where(element => element.Value.StartsWith(_removeKeyword, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var element in elementsToRemove)
            {
                element.Remove();
            }

            return document;
        }

        public XDocument SetNumeration(XDocument document)
        {
            int currentImageNumber = 1; // Начальный номер рисунка
            IEnumerable<XElement> paras = document.Descendants("para");

            // Паттерн для поиска упоминаний рисунков
            string replacement;
            string pattern = @"см\.\s*рисунок\s+\d+";
            // Формат замены

            foreach (var para in paras)
            {
                replacement = $"см. рисунок {currentImageNumber}";

                // Замена упоминаний рисунков на текущий номер
                para.Value = Regex.Replace(para.Value, pattern, replacement, RegexOptions.IgnoreCase);

                // Увеличиваем номер для следующего параграфа
                currentImageNumber++;
            }

            return document;
        }

        private bool IsCapitalized(string word) => char.IsUpper(word[0]);

        private string CapitalizeFirstLetter(string word) => char.ToUpper(word[0]) + word.Substring(1);

        private string ReplaceInText(string text)
        {
            var inputText = new StringBuilder(text);

            // Заменяем слово без знаков препинания
            foreach (var pair in _replacements)
            {
                ReplaceWithCase(inputText, pair.Key, pair.Value);

                foreach (var mark in _punctuations)
                {
                    ReplaceWithCase(inputText, pair.Key + mark, pair.Value + mark);
                }
            }

            return inputText.ToString();
        }

        private void ReplaceWithCase(StringBuilder text, string target, string replacement)
        {
            // Заменяем слово как есть
            text.Replace(target, replacement);

            // Заменяем слово с заглавной буквы
            text.Replace(CapitalizeFirstLetter(target), CapitalizeFirstLetter(replacement));
        }


    }
}
