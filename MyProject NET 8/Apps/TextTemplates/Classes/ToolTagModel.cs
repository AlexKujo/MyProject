using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject_NET_8.TextTemplates
{
    internal class ToolTagModel
    {
        public string Name { get; private set; }

        public string IdentNumber { get; private set; }

        public string TagStructure { get; private set; }

        public ToolTagModel(string name, string identNumber)
        {
            Name = name;
            IdentNumber = identNumber;
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
