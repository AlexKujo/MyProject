using RapidHelper.TextTemplates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidHelper.Apps.TextTemplates.Classes
{
    static class ToolsManager
    {
        public static void SortToolsList(ObservableCollection<Tool> tools)
        {
            // Создаем отсортированный список
            var sortedList = tools.OrderBy(tool => tool.Name)
                                  .ThenBy(tool => tool.WrenchSize)
                                  .ToList();

            // Очищаем коллекцию и добавляем отсортированные элементы
            tools.Clear();

            foreach (var tool in sortedList)
            {
                tools.Add(tool);
            }
        }

        private static IEnumerable<Tool> FilterByType(IEnumerable<Tool> tools, string selectedType) => tools.Where(tool => tool.Type == selectedType);

        private static IEnumerable<Tool> FilterByWrenchSize(IEnumerable<Tool> tools, float? wrenchSize) => tools.Where(tool => tool.WrenchSize == wrenchSize);

        private static IEnumerable<Tool> FilterIncludeThread(IEnumerable<Tool> tools, float? threadWrenchSize) => tools.Where(tool => tool.WrenchSize == threadWrenchSize);

        private static IEnumerable<Tool> FilterIncludeMoment(IEnumerable<Tool> tools, float minThreadMoment, float maxThreadMoment) => tools.Where(tool => tool.MinMoment <= maxThreadMoment && tool.MaxMoment >= minThreadMoment);

        public static IEnumerable<Tool> FilterTools(IEnumerable<Tool> tools, string? selectedType = null, float? wrenchSize = null)
        {
            if (string.IsNullOrEmpty(selectedType) == false)
            {
                tools = FilterByType(tools, selectedType);
            }

            if (wrenchSize.HasValue)
            {
                tools = FilterByWrenchSize(tools, wrenchSize);
            }

            return tools;
        }

        public static IEnumerable<Tool> FilterTools(IEnumerable<Tool> tools, MechThread thread, bool isWrenchSizeFilterEnabled, bool isMomentFilterEnabled)
        {
            var filteredTools = tools;
            var threadWrenchSize = thread?.WrenchSize;
            float minThreadMoment = Converter.ConvertMomentToHm(thread.MinValue);
            float maxThreadMoment = Converter.ConvertMomentToHm(thread.MaxValue);

            IEnumerable<Tool> threadFilteredTools = null;
            IEnumerable<Tool> momentFilteredTools = null;


            if (isWrenchSizeFilterEnabled)
            {
                threadFilteredTools = FilterIncludeThread(tools, threadWrenchSize);
                filteredTools = threadFilteredTools;
            }

            if (isMomentFilterEnabled)
            {
                momentFilteredTools = FilterIncludeMoment(tools, minThreadMoment, maxThreadMoment);
                filteredTools = momentFilteredTools;
            }

            if (isWrenchSizeFilterEnabled && isMomentFilterEnabled)
            {
                filteredTools = momentFilteredTools.Union(threadFilteredTools);
            }

            return filteredTools;
        }

        public static List<float?> GetWrenchSizes(ObservableCollection<Tool> toolsList)
        {
            List<float?> sizes = toolsList.Where(tool => tool.WrenchSize > 0)
                     .Select(tool => tool.WrenchSize)
                     .Distinct()
                     .OrderBy(size => size)
                     .ToList();

            return sizes;
        }
    }
}
