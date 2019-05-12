using System;
using System.Collections.Generic;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>Class <c>Text</c> represents some text in the graph, usually the cost of an edge.</summary>
    /// <seealso cref="GraphElement"/>
    /// <seealso cref="Edge"/>
    public class Text : GraphElement
    {
        /// <value>Property <c>Parent</c> represents the <see cref="GraphElement"/> to which this text is associated.</value>
        public GraphElement Parent { get; private set; }

        /// <summary>This constructor initializes the new <c>Text</c> from a <see cref="XmlNode"/>, from previously calculated <see cref="GraphElement.StyleProperties"/> and from the <see cref="GraphElement"/> to which this text is associated.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the text.</param>
        /// <param name="styleProperties">The <see cref="List{T}"/> containing all the properties concerning the style of the text.</param>
        /// <param name="parent">The <see cref="GraphElement"/> that contains this text.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>This constructor is useful when you extract the <see cref="GraphElement.StyleProperties"/> from the <see cref="XmlNode"/> using <see cref="GraphElement.LoadStyleProperties(XmlNode)"/>, in order to choose the subclass of the graph element, and you don't want to re-extract them when initializing the object.</remarks>
        public Text(XmlNode node, List<KeyValuePair<string, string>> styleProperties, GraphElement parent) : base(node, styleProperties)
        {
            Parent = parent ?? throw new ArgumentNullException("parent");
            Parent.Value = Value;
        }

        /// <summary>This constructor initializes the new <c>Text</c> from a <see cref="XmlNode"/> and from the <see cref="GraphElement"/> to which this text is associated.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the text.</param>
        /// <param name="parent">The <see cref="GraphElement"/> that contains this text.</param>
        public Text(XmlNode node, GraphElement parent) : this(node, LoadStyleProperties(node), parent) { }
    }
}
