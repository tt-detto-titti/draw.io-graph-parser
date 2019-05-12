using System;
using System.Collections.Generic;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>
    /// Struct <c>NavigableNeighbor</c> contains information about a neighbor of the vertex:
    /// <list type="bullet"><item><description>The <see cref="Vertex"/> object</description></item><item><description>The cost to reach the vertex</description></item></list>
    /// </summary>
    public struct NavigableNeighbor
    {

        /// <summary>It represents the <see cref="Vertex"/> object of the neighbor.</summary>
        public Vertex Neighbor;
        /// <summary>It represents the connecting <see cref="Edge"/> between the two <see cref="Vertex"/>-s.</summary>
        public Edge ConnectingEdge;
        /// <summary>It represents the cost of the <see cref="Edge"/> to reach the vertex.</summary>
        public double Cost;

        /// <summary>This constructor initializes the new <c>NavigableNeighbor</c> from the <see cref="Vertex"/> object of the neighbor and from the cost to reach it.</summary>
        /// <param name="neighbor">The <see cref="Vertex"/> object of the neighbor.</param>
        /// <param name="connectingEdge">The connecting <see cref="Edge"/> to the neighbor.</param>
        /// <param name="cost">The cost to reach the neighbor.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NavigableNeighbor(Vertex neighbor, Edge connectingEdge, double cost)
        {
            Neighbor = neighbor ?? throw new ArgumentNullException("neighbor");
            ConnectingEdge = connectingEdge ?? throw new ArgumentNullException("connectingEdge");
            Cost = cost;
        }
    }

    /// <summary>Class <c>Vertex</c> represents a node in a graph.</summary>
    /// <seealso cref="GraphElement"/>
    public class Vertex : GraphElement
    {
        /// <value>Property <c>NavigableNeighbors</c> represents the <see cref="List{T}"/> containing all the vertices, at a distance of one edge, that can be reached from this vertex.</value>
        public List<NavigableNeighbor> Neighbors { get; private set; }

        /// <summary>This constructor initializes the new <c>Vertex</c> from a <see cref="XmlNode"/> and from previously calculated <see cref="GraphElement.StyleProperties"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the vertex.</param>
        /// <param name="styleProperties">The <see cref="List{T}"/> containing all the properties concerning the style of the vertex.</param>
        /// <remarks>This constructor is useful when you extract the <see cref="GraphElement.StyleProperties"/> from the <see cref="XmlNode"/> using <see cref="GraphElement.LoadStyleProperties(XmlNode)"/>, in order to choose the subclass of the graph element, and you don't want to re-extract them when initializing the object.</remarks>
        public Vertex(XmlNode node, List<KeyValuePair<string, string>> styleProperties) : base(node, styleProperties)
        {
            Neighbors = new List<NavigableNeighbor>();
        }

        /// <summary>This constructor initializes the new <c>Vertex</c> from a <see cref="XmlNode"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the vertex.</param>
        public Vertex(XmlNode node) : this(node, LoadStyleProperties(node)) { }
    }
}
