using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject_NET_8.Apps.TextTemplates.Classes
{
    interface ITaggable
    {
        public XElement RootElement { get; }

        public XElement GroupDescription { get; }

        public string GetTagStructure();

        public string GetCopyMessage();
    }
}
