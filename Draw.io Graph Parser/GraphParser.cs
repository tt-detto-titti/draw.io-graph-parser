using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public class GraphParser
    {
        public XmlDocument Graph { get; private set; }

        public List<Vertex> Vertices { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Text> Texts { get; private set; }

        public GraphParser(string xmlFilePath)
        {
            Graph = new XmlDocument();
            Graph.Load(xmlFilePath);

            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
            Texts = new List<Text>();

            List<CommonElement> elmnts = new List<CommonElement>();
            foreach (XmlNode node in Graph.GetElementsByTagName("root").Item(0))
            {
                XmlNode style = node.Attributes.GetNamedItem("style");
                if (style != null) elmnts.Add(new CommonElement(node));
            }
            LoadVertices(elmnts);
            LoadEdges(elmnts);
            LoadTexts(elmnts);
        }

        public Vertex GetVertexById(string id)
        {
            return Vertices.Find(v => v.Id == id);
        }

        public Edge GetEdgeById(string id)
        {
            return Edges.Find(e => e.Id == id);
        }

        private void LoadVertices(List<CommonElement> elements)
        {           
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                CommonElement elmn = elements[i];

                if (elmn.GetStylePropertyValue("ellipse") != null)
                {
                    Vertices.Add(new Vertex(elmn.Node, elmn.StyleProperties));
                    elements.RemoveAt(i);
                }
            }
        }

        private void LoadEdges(List<CommonElement> elements)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                CommonElement elmn = elements[i];

                if (elmn.GetAttributeInnerText("edge") != null)
                {
                    Edges.Add(new Edge(
                        elmn.Node,
                        elmn.StyleProperties,
                        GetVertexById(elmn.GetAttributeInnerText("source")),
                        GetVertexById(elmn.GetAttributeInnerText("target"))));
                    elements.RemoveAt(i);
                }
            }
        }

        private void LoadTexts(List<CommonElement> elements)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                CommonElement elmn = elements[i];

                if (elmn.GetStylePropertyValue("text") != null)
                {
                    Texts.Add(new Text(
                        elmn.Node,
                        elmn.StyleProperties,
                        GetEdgeById(elmn.GetAttributeInnerText("parent"))));
                    elements.RemoveAt(i);
                }
            }
        }

        private void CreateLinks()
        {
            foreach (Edge edge in Edges)
            {
                edge.Source.AddNeighbour(
                    new NavigableNeighbour(
                        edge.Target,
                        edge.Value != "" ? Double.Parse(edge.Value) : 0));
                if (edge.IsBidirectional)
                    edge.Target.AddNeighbour(
                    new NavigableNeighbour(
                        edge.Source,
                        edge.Value != "" ? Double.Parse(edge.Value) : 0));
            }
        }

        private class CommonElement : GraphElement
        {
            public CommonElement(XmlNode node) : base(node) { }
        }
    }
}
