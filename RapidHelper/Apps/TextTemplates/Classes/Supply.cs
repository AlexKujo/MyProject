using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RapidHelper.Apps.TextTemplates.Classes
{
    class Supply : ITaggable
    {
        public Supply(string name, string identNumber)
        {
            Name = name;
            IdentNumber = identNumber;

            RootElement = "reqSupplies";
            GroupDescription = "supplyDescrGroup";
        }

        public string Name { get; private set; }

        public string IdentNumber { get; private set; }

        public string RootElement { get; private set; }

        public string GroupDescription { get; private set; }

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

        public string GetCopySingleMessage() => "Расходный материал скопирован в буфер обмена";

        public string GetCopyListMessage() => "Список расходных материалов скопирован";
    }
}
