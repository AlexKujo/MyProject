using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject_NET_8.TextTemplates
{
    public class Tool
    {
        public string Name { get; private set; }

        public string IdentNumber { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Category { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? WrenchSize { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Length { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SocketSize { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxMoment { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinMoment { get; private set; }

        [JsonConstructor]

        public Tool(string name, string identNumber, float? wrenchSize = null, int? maxMoment = null, int? minMoment = null, string length = null, string socketSize = null, string type = null, string category = null)
        {
            Name = name;
            IdentNumber = identNumber;
            Category = category;
            Type = type;
            WrenchSize = wrenchSize;
            Length = length;
            SocketSize = socketSize;
            MaxMoment = maxMoment;
            MinMoment = minMoment;
        }

        public string GetTagStructure()
        {
            var supportEquipDescr = new XElement("supportEquipDescr",
                new XElement("name", Name),
                new XElement("identNumber",
                    new XElement("manufacturerCode"),
                    new XElement("partAndSerialNumber",
                        new XElement("partNumber", IdentNumber)
                    )
                ),
                new XElement("reqQuantity", "1", new XAttribute("unitOfMeasure", "EA"))
            );

            // Возвращаем строку XML с отступами и переносами строк
            return supportEquipDescr.ToString(SaveOptions.None);
        }
    }
}
