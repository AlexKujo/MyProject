using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject_NET_8.Apps.TextTemplates.Classes
{
    class Supply : ITaggable
    {
        public Supply(string name, string identNumber)
        {
            Name = name;
            IdentNumber = identNumber;
        }

        public string Name { get; private set; }

        public string IdentNumber { get; private set; }

        public XElement RootElement { get; private set; } = new XElement("reqSupplies");

        public XElement GroupDescription { get; private set; } = new XElement("supplyDescr");

        public string GetTagStructure()
        {
            var supplyDescr = new XElement("supplyDescr",
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
            return supplyDescr.ToString(SaveOptions.None);
        }

        public string GetCopyMessage() => "Расходный материал скопирован в буфер обмена";
    }
}
