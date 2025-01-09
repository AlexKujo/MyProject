using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RapidHelper.Apps.TextTemplates.Classes
{
    public interface ITaggable
    {
        public string RootElement { get; }

        public string GroupDescription { get; }

        public string GetTagStructure();

        public string GetCopySingleMessage();

        public string GetCopyListMessage();

    }
}
