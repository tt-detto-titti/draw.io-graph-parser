using System;
using System.Collections.Generic;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>Class <c>Edge</c> represents the connecting edge between two vertices in a graph.</summary>
    /// <seealso cref="GraphElement"/>
    public class Edge : GraphElement
    {
        /// <value>Property <c>Source</c> represents the <see cref="Vertex"/> from which the edge starts.</value>
        public Vertex Source { get; private set; }
        /// <value>Property <c>Target</c> represents the <see cref="Vertex"/> on which the edge ends.</value>
        public Vertex Target { get; private set; }
        /// <value>Property <c>IsBidirectional</c> indicates whether this edge can be navigated in both directions.</value>
        public bool IsBidirectional { get; private set; }

        /// <summary>This constructor initializes the new <c>Edge</c> from a <see cref="XmlNode"/>, from previously calculated <see cref="GraphElement.StyleProperties"/>, from the source <see cref="Vertex"/> and from the target <see cref="Vertex"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the edge.</param>
        /// <param name="styleProperties">The <see cref="List{T}"/> containing all the properties concerning the style of the edge.</param>
        /// <param name="source">The <see cref="Vertex"/> object that represents the node from which the edge starts.</param>
        /// <param name="target">The <see cref="Vertex"/> object that represents the node on which the edge ends.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>This constructor is useful when you extract the <see cref="GraphElement.StyleProperties"/> from the <see cref="XmlNode"/> using <see cref="GraphElement.LoadStyleProperties(XmlNode)"/>, in order to choose the subclass of the graph element, and you don't want to re-extract them when initializing the object.</remarks>
        public Edge(XmlNode node, List<KeyValuePair<string, string>> styleProperties, Vertex source, Vertex target) : base(node, styleProperties)
        {
            Source = source ?? throw new ArgumentNullException("source");
            Target = target ?? throw new ArgumentNullException("target");

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

        /// <summary>This constructor initializes the new <c>Edge</c> from a <see cref="XmlNode"/>, from the source <see cref="Vertex"/> and from the target <see cref="Vertex"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the edge.</param>
        /// <param name="source">The <see cref="Vertex"/> object that represents the node from which the edge starts.</param>
        /// <param name="target">The <see cref="Vertex"/> object that represents the node on which the edge ends.</param>
        public Edge(XmlNode node, Vertex source, Vertex target) : this(node, LoadStyleProperties(node), source, target) { }

        /// <summary>Determines whether the specified arrow is present.</summary>
        /// <param name="arrow">The name of the arrow.</param>
        /// <returns><c>true</c> if the specified arrow is present; otherwise, <c>false</c>.</returns>
        /// <remarks>The possible names for the arrow are: <em>endArrow</em> and <em>startArrow</em>.</remarks>
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
