using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject_NET_8.TextTemplates
{
    internal class ToolFilter
    {
        public IEnumerable<Tool> FilterTools(IEnumerable<Tool> tools, string selectedType = null, float? wrenchSize = null, float? threadWrenchSize = null, bool isThreadFilterEnabled = false)
        {
            Func<Tool, bool> typePredicate = tool => string.IsNullOrEmpty(selectedType) || tool.Type == selectedType || tool.Category == selectedType;

            Func<Tool, bool> sizePredicate = tool => !wrenchSize.HasValue || tool.WrenchSize == wrenchSize.Value;

            Func<Tool, bool> threadSizePredicate = tool => !isThreadFilterEnabled || !threadWrenchSize.HasValue || tool.WrenchSize == threadWrenchSize.Value;

            return tools.Where(tool => typePredicate(tool) && sizePredicate(tool) && threadSizePredicate(tool));
        }
    }
}
