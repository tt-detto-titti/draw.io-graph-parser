using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public struct NavigableNeighbour
    {
        public Vertex Neighbour;
        public double Cost;

        public NavigableNeighbour(Vertex Neighbour, double Cost)
        {
            this.Neighbour = Neighbour;
            this.Cost = Cost;
        }
    }

    public class Vertex : GraphElement
    {
        public List<NavigableNeighbour> Neighbours { get; private set; }

        public Vertex(XmlNode node, List<KeyValuePair<string, string>> styleProperties) : base(node, styleProperties)
        {
            Neighbours = new List<NavigableNeighbour>();
        }

        public Vertex(XmlNode node) : this(node, LoadStyleProperties(node)) { }

        public void AddNeighbour(NavigableNeighbour neighbour)
        {
            Neighbours.Add(neighbour);
        }
    }
}
