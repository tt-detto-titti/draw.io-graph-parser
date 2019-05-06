using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    class Text : GraphElement
    {
        public GraphElement Parent { get; private set; }

        public Text(XmlNode node, GraphElement parent) : base(node)
        {
            Parent = parent;
            //Parent.SetWeight(GetAttributeValue("value"));
        }
    }
}
