using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>
    /// Class <c>GraphElement</c> is the basic class all the elements in a graph must derive from.
    /// </summary>
    public abstract class GraphElement
    {
        /// <value>
        /// Property <c>Node</c> represents the <see cref="XmlNode"/> the graph element comes from.
        /// </value>
        public XmlNode Node { get; protected set; }
        /// <value>
        /// Property <c>Id</c> represents the univocal identificator for the graph element.
        /// </value>
        public string Id { get; protected set; }
        /// <summary>
        /// Istance variable <c>vl</c> represents the value of the graph element.
        /// </summary>
        /// <remarks>
        /// For <see cref="Vertex">Vertices</see>, the value represents the human-readable id of the vertex; for <see cref="Edge"/>-s, it represents the cost; for <see cref="Text"/>-s, it represents the text itself.
        /// </remarks>
        private string vl = "";
        /// <value>
        /// Property <c>Value</c> represents the value of the graph element.
        /// </value>
        /// <remarks>
        /// For <see cref="Vertex"/>-s, the value represents the human-readable id of the vertex; for <see cref="Edge"/>-s, it represents the cost; for <see cref="Text"/>-s, it represents the text itself.
        /// </remarks>
        public string Value
        {
            get
            {
                return vl;
            }
            set
            {
                vl = value;
                SetAttributeInnerText("value", vl);
            }
        }
        /// <value>
        /// Property <c>StyleProperties</c> represents the <see cref="List{KeyValuePair}"/> containing all the properties concerning the style of the graph element (such as the type).
        /// </value>
        public List<KeyValuePair<string, string>> StyleProperties { get; protected set; }

        /// <summary>
        /// This constructor initializes the new <c>GraphElement</c> from a <see cref="XmlNode"/> and pre-calculated <see cref="StyleProperties"/>.
        /// </summary>
        /// <remarks>
        /// This constructor is useful when you extract the <see cref="StyleProperties"/> from the <see cref="XmlNode"/> using <see cref="LoadStyleProperties(XmlNode)"/>, in order to choose the subclass of the graph element, and you don't want to re-extract them when initializing the object.
        /// </remarks>
        /// <seealso cref="LoadStyleProperties(XmlNode)"/>
        /// <param name="node">The <see cref="XmlNode"/> that represents the graph element.</param>
        /// <param name="styleProperties">The <see cref="List{T}"/> containing all the properties concerning the style of the graph element (such as the type).</param>
        public GraphElement(XmlNode node, List<KeyValuePair<string, string>> styleProperties)
        {
            Node = node;
            Id = GetAttributeInnerText("id");
            Value = GetAttributeInnerText("value");
            StyleProperties = styleProperties;
        }

        /// <summary>
        /// This constructor initializes the new <c>GraphElement</c> from a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the graph element.</param>
        public GraphElement(XmlNode node) : this(node, LoadStyleProperties(node)) { }

        /// <summary>
        /// This method extracts a <see cref="List{KeyValuePair}"/> of style properties from a <see cref="XmlNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/> from which the properties are extracted.</param>
        /// <returns>The <see cref="List{KeyValuePair}"/> of properties.</returns>
        /// <exception cref="XmlException">
        /// Thrown when the <see cref="XmlNode"/> doesn't contain a style <see cref="XmlAttribute"/>.
        /// </exception>
        public static List<KeyValuePair<string, string>> LoadStyleProperties(XmlNode node)
        {
            XmlNode attr = node.Attributes.GetNamedItem("style");

            if (attr == null)
                throw new XmlException("The node doesn't contain a style attribute.");

            List<KeyValuePair<string, string>> prpts = new List<KeyValuePair<string, string>>();
            foreach (string property in attr.InnerText.Split(';'))
                if (property != "")
                {
                    string[] info = property.Split('=');
                    prpts.Add(new KeyValuePair<string, string>(
                        info[0],
                        info.Length > 1 ? info[1] : ""));
                }
            
            return prpts;
        }

        /// <summary>
        /// This method returns the value of <paramref name="property"/> style property.
        /// </summary>
        /// <param name="property">The name of the style property whose value you want to get.</param>
        /// <returns>The value of the style property, <c>null</c> if the property doesn't exist.</returns>
        public string GetStylePropertyValue(string property)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);
            return prpt.HasValue ? prpt.Value.Value : null;
        }

        /// <summary>
        /// This method sets the <paramref name="value"/> of <paramref name="property"/> style property.
        /// </summary>
        /// <param name="property">The name of the style property whose value you want to set.</param>
        /// <param name="value">The value you want to set.</param>
        public void SetStylePropertyValue(string property, string value)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);

            if (prpt.HasValue) StyleProperties.Remove(prpt.Value);

            prpt = new KeyValuePair<string, string>(property, value);
            StyleProperties.Add(prpt.Value);
            SetAttributeInnerText("style", StylePropertiesToString());
        }

        /// <summary>
        /// This methods returns the inner text of <paramref name="attribute"/> <see cref="XmlAttribute"/>.
        /// </summary>
        /// <param name="attribute">The name of the <see cref="XmlAttribute"/> whose inner text you want to get.</param>
        /// <returns>The inner text of the <see cref="XmlAttribute"/>, <c>null</c> if the <see cref="XmlAttribute"/> doesn't exist.</returns>
        public string GetAttributeInnerText(string attribute)
        {
            XmlNode attr = Node.Attributes.GetNamedItem(attribute);
            return attr != null ? attr.InnerText : null;
        }

        /// <summary>
        /// This method sets the <paramref name="innerText"/> of <paramref name="attribute"/> <see cref="XmlAttribute"/>.
        /// </summary>
        /// <param name="attribute">The name of the <see cref="XmlAttribute"/> whose inner text you want to set.</param>
        /// <param name="innerText">The inner text to set.</param>
        public void SetAttributeInnerText(string attribute, string innerText)
        {
            XmlNode attr = Node.Attributes.GetNamedItem(attribute);

            if (attr != null)
                attr.InnerText = innerText;
            else
            {
                attr = Node.OwnerDocument.CreateAttribute(attribute);
                attr.InnerText = innerText;
                Node.Attributes.SetNamedItem(attr);
            }
        }

        /// <summary>
        /// This method converts the <see cref="List{KeyValuePair}"/> of style properties to a string.
        /// </summary>
        /// <remarks>
        /// It's used to update the style <see cref="XmlAttribute"/> in a node when one of its properties changes.
        /// </remarks>
        /// <returns></returns>
        private string StylePropertiesToString()
        {
            string prpts = "";

            foreach (KeyValuePair<string, string> prpt in StyleProperties)
                prpts += prpt.Key + "=" + prpt.Value + ";";
            
            return prpts;
        }
    }
}
