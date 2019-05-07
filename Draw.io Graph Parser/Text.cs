using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public class Text : GraphElement
    {
        public GraphElement Parent { get; private set; }

        public Text(XmlNode node, List<KeyValuePair<string, string>> styleProperties, GraphElement parent) : base(node, styleProperties)
        {
            Parent = parent;

            if (Parent is Edge)
                ((Edge)Parent).Value = Value;
        }

        public Text(XmlNode node, GraphElement parent) : this(node, LoadStyleProperties(node), parent) { }
    }
}
