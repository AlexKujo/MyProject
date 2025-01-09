using RapidHelper.TextTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace RapidHelper.Apps.TextTemplates.Classes
{
    static class ClipboardHelper
    {
        public static void CopyAll<T>(IEnumerable<T> items, Label notificationLabel) where T : ITaggable
        {
            if (items == null || items.Any() == false)
            {
                MessageBox.Show("Список пуст.");
                return;
            }

            var firstElement = items.First();
            // Создание корневого элемента
            var RootElement = new XElement(firstElement.RootElement);
            // Создание группы описаний инструментов
            var GroupDescription = new XElement(firstElement.GroupDescription);

            // Добавление каждого инструмента в XML
            foreach (var item in items)
            {
                // Получаем XML строку для каждого инструмента
                var tagStructure = item.GetTagStructure();
                // Добавляем XML строку как элемент
                var element = XElement.Parse(tagStructure);
                GroupDescription.Add(element);
            }

            // Добавление группы описаний в корневой элемент
            RootElement.Add(GroupDescription);

            // Формирование финального XML
            var document = new XDocument(RootElement);
            var xmlString = document.ToString(SaveOptions.None);

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(notificationLabel, items.First().GetCopyListMessage());
        }

        public static void Copy(ITaggable taggable, Label notificationLabel)
        {
            // Получаем XML строку для инструмента
            var tagStructure = taggable.GetTagStructure();
            var taggableElement = XElement.Parse(tagStructure);

            // Формирование финального XML только для одного инструмента
            var document = new XDocument(taggableElement);
            var xmlString = document.ToString(SaveOptions.None);

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(notificationLabel, taggable.GetCopySingleMessage());
        }
    }
}
