using System;
using System.Collections.Generic;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>Class <c>GraphParser</c> takes a graph in XML format made with https://www.draw.io/ and parses it.</summary>
    /// <remarks>The XML file must be uncompressed.</remarks>
    /// <seealso cref="XmlDocument"/>
    public abstract class GraphParser
    {
        /// <value>The property <c>Graph</c> represents the whole graph in XML format.</value>
        public XmlDocument Graph { get; private set; }
        /// <value>The property <c>Vertices</c> represents the <see cref="List{T}"/> containing all the vertices of the graph.</value>
        public List<Vertex> Vertices { get; private set; }
        /// <value>The property <c>Edges</c> represents the <see cref="List{T}"/> containing all the edges of the graph.</value>
        public List<Edge> Edges { get; private set; }
        /// <value>The property <c>Texts</c> represents the <see cref="List{T}"/> containing all the texts of the graph. Usually they are the cost of the edges.</value>
        public List<Text> Texts { get; private set; }

        /// <summary>This constructor initializes the new <c>GraphParser</c> from a <see cref="XmlDocument"/>.</summary>
        /// <param name="xmlFilePath">The path of the XML file.</param>
        /// <remarks>The XML file can be made with https://www.draw.io/. When exporting it, it's fundamental to uncheck the <em>Compressed</em> option.</remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public GraphParser(string xmlFilePath)
        {
            if (xmlFilePath == null)
                throw new ArgumentNullException("xmlFilePath");

            try
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
                CreateLinks();
            }
            catch (Exception e)
            {
                throw new XmlException("Unable to parse the graph: " + e.Message);
            }
        }

        /// <summary>Gets the <see cref="Vertex"/> in <see cref="Vertices"/> with the specified id.</summary>
        /// <param name="id">The id of the <see cref="Vertex"/>.</param>
        /// <returns>The <see cref="Vertex"/>, <c>null</c> if it can't be found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Vertex GetVertexById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return Vertices.Find(v => v.Id == id);
        }

        /// <summary>Gets the <see cref="Edge"/> in <see cref="Edges"/> with the specified id.</summary>
        /// <param name="id">The id of the <see cref="Edge"/>.</param>
        /// <returns>The <see cref="Edge"/>, <c>null</c> if it can't be found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Edge GetEdgeById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return Edges.Find(e => e.Id == id);
        }

        /// <summary>Loads all the <see cref="Vertex"/>-s from the <see cref="List{T}"/> of <see cref="CommonElement"/>-s.</summary>
        /// <param name="elements">The <see cref="List{T}"/> of <see cref="CommonElement"/>-s from which all the <see cref="Vertex"/>-s are taken.</param>
        /// <remarks>When a <see cref="CommonElement"/> is used, it is also removed from the list.</remarks>
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

        /// <summary>Loads all the <see cref="Edge"/>-s from the <see cref="List{T}"/> of <see cref="CommonElement"/>-s.</summary>
        /// <param name="elements">The <see cref="List{T}"/> of <see cref="CommonElement"/>-s from which all the <see cref="Edge"/>-s are taken.</param>
        /// <remarks>When a <see cref="CommonElement"/> is used, it is also removed from the list.</remarks>
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

        /// <summary>Loads all the <see cref="Text"/>-s from the <see cref="List{T}"/> of <see cref="CommonElement"/>-s.</summary>
        /// <param name="elements">The <see cref="List{T}"/> of <see cref="CommonElement"/>-s from which all the <see cref="Text"/>-s are taken.</param>
        /// <remarks>When a <see cref="CommonElement"/> is used, it is also removed from the list.</remarks>
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

        /// <summary>Lets all the <see cref="Vertex"/>-s know who are their <see cref="NavigableNeighbor"/>-s.</summary>
        private void CreateLinks()
        {
            foreach (Edge edge in Edges)
            {
                edge.Source.Neighbors.Add(
                    new NavigableNeighbor(
                        edge.Target,
                        edge.Value != "" ? Double.Parse(edge.Value) : 0));
                if (edge.IsBidirectional)
                    edge.Target.Neighbors.Add(
                    new NavigableNeighbor(
                        edge.Source,
                        edge.Value != "" ? Double.Parse(edge.Value) : 0));
            }
        }

        /// <summary>Class <c>CommonElement</c> represents an element in a graph which type hasn't been decided yet.</summary>
        /// <seealso cref="GraphElement"/>
        private class CommonElement : GraphElement
        {
            public CommonElement(XmlNode node) : base(node) { }
        }
    }
}
