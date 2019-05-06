using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public class Edge : GraphElement
    {
        public Vertex Source { get; private set; }
        public Vertex Target { get; private set; }
        public double Cost { get; private set; }
        public bool IsBidirectional { get; private set; }

        public Edge(XmlNode node) : base(node)
        {
            try
            {
                Source = GraphParser.GetVertexById(GetAttributeInnerText("source"));
                Target = GraphParser.GetVertexById(GetAttributeInnerText("target"));

                if ((IsArrowPresent("startArrow") && IsArrowPresent("endArrow")) ||
                    (!IsArrowPresent("startArrow") && !IsArrowPresent("endArrow")))
                    IsBidirectional = true;
                else                
                    IsBidirectional = false;

                if (IsArrowPresent("startArrow"))
                {
                    Vertex swap = Source;
                    Source = Target;
                    Target = swap;
                }
            }
            catch (XmlException e)
            {
                throw new XmlException(e.Message);
            }
        }

        private bool IsArrowPresent(string arrow)
        {
            KeyValuePair<string, string>? arrw = StyleProperties.Find(x => x.Key == arrow);

            if (arrow == "endArrow")
            {
                if (!arrw.HasValue)
                    return true;
                else if (arrw.Value.Value != "none")
                    return true;
                else
                    return false;
            }
            else if (arrw.Value.Value != "none")
                return true;

            return false;
        }
    }
}
