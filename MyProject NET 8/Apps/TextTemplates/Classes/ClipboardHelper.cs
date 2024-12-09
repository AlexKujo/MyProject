using MyProject_NET_8.TextTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyProject_NET_8.Apps.TextTemplates.Classes
{
    static class ClipboardHelper
    {
        public static void CopyAll(IEnumerable<Tool> tools, Label notificationLabel)
        {
            if (tools == null || tools.Any() == false)
            {
                MessageBox.Show("Список инструментов пуст.");
                return;
            }

            // Создание корневого элемента
            var reqSupportEquips = new XElement("reqSupportEquips");

            // Создание группы описаний инструментов
            var supportEquipDescrGroup = new XElement("supportEquipDescrGroup");

            // Добавление каждого инструмента в XML
            foreach (var tool in tools)
            {
                // Получаем XML строку для каждого инструмента
                var tagStructure = tool.GetTagStructure();
                // Добавляем XML строку как элемент
                var toolElement = XElement.Parse(tagStructure);
                supportEquipDescrGroup.Add(toolElement);
            }

            // Добавление группы описаний в корневой элемент
            reqSupportEquips.Add(supportEquipDescrGroup);

            // Формирование финального XML
            var document = new XDocument(reqSupportEquips);
            var xmlString = document.ToString();

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(notificationLabel, "Группа инструментов скопированы в буфер обмена.");
        }

        public static void CopyAll<T>(IEnumerable<T> items, Label notificationLabel, string labelMessage) where T : ITaggable
        {
            if (items == null || items.Any() == false)
            {
                MessageBox.Show("Список пуст.");
                return;
            }

            var firstElement = items.First();
            // Создание корневого элемента
            var RootElement = firstElement.RootElement;
            // Создание группы описаний инструментов
            var GroupDescription = firstElement.GroupDescription;

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
            var xmlString = document.ToString();

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(notificationLabel, labelMessage);
        }

        public static void Copy(ITaggable taggable, Label notificationLabel)
        {
            // Получаем XML строку для инструмента
            var tagStructure = taggable.GetTagStructure();
            var taggableElement = XElement.Parse(tagStructure);

            // Формирование финального XML только для одного инструмента
            var document = new XDocument(taggableElement);
            var xmlString = document.ToString();

            // Копируем результат в буфер обмена
            Clipboard.SetText(xmlString);

            NoticeHelper.ShowNotification(notificationLabel, taggable.GetCopyMessage());
        }
    }
}
