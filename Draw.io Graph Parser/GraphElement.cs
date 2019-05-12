using System;
using System.Collections.Generic;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    /// <summary>Class <c>GraphElement</c> is the basic class from which all the elements in a graph must derive.</summary>
    public abstract class GraphElement
    {
        /// <value>Property <c>Node</c> represents the <see cref="XmlNode"/> the graph element comes from.</value>
        public XmlNode Node { get; protected set; }
        /// <value>Property <c>Id</c> represents the univocal identifier for the graph element.</value>
        public string Id { get; protected set; }
        /// <summary>Instance variable <c>vl</c> represents the value of the graph element.</summary>
        /// <remarks>For <see cref="Vertex"/>-s, the value represents the human-readable id of the vertex; for <see cref="Edge"/>-s, it represents the cost; for <see cref="Text"/>, it represents the text itself.</remarks>
        private string vl = "";
        /// <value>Property <c>Value</c> represents the value of the graph element.</value>
        /// <remarks>For a <see cref="Vertex"/>, the value represents the human-readable id of the vertex; for an <see cref="Edge"/>, it represents the cost; for a <see cref="Text"/>, it represents the text itself.</remarks>
        public string Value
        {
            get
            {
                return vl;
            }
            set
            {
                vl = value ?? "";
                SetAttributeInnerText("value", vl);
            }
        }
        /// <value>Property <c>StyleProperties</c> represents the <see cref="List{T}"/> containing all the properties concerning the style of the graph element (such as the type).</value>
        public List<KeyValuePair<string, string>> StyleProperties { get; protected set; }

        /// <summary>This constructor initializes the new <c>GraphElement</c> from a <see cref="XmlNode"/> and from previously calculated <see cref="StyleProperties"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the graph element.</param>
        /// <param name="styleProperties">The <see cref="List{T}"/> containing all the properties concerning the style of the graph element (such as the type).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>This constructor is useful when you extract the <see cref="StyleProperties"/> from the <see cref="XmlNode"/> using <see cref="LoadStyleProperties(XmlNode)"/> in order to choose the subclass of the graph element, and you don't want to re-extract them when initializing the object.</remarks>
        /// <seealso cref="LoadStyleProperties(XmlNode)"/>
        public GraphElement(XmlNode node, List<KeyValuePair<string, string>> styleProperties)
        {
            Node = node ?? throw new ArgumentNullException("node");
            Id = GetAttributeInnerText("id") ?? throw new ArgumentNullException("id");
            Value = GetAttributeInnerText("value");
            StyleProperties = styleProperties ?? throw new ArgumentNullException("styleProperties");
        }

        /// <summary>This constructor initializes the new <c>GraphElement</c> from a <see cref="XmlNode"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> that represents the graph element.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GraphElement(XmlNode node) : this(node, LoadStyleProperties(node)) { }

        /// <summary>This method extracts a <see cref="List{KeyValuePair}"/> of style properties from a <see cref="XmlNode"/>.</summary>
        /// <param name="node">The <see cref="XmlNode"/> from which the properties are extracted.</param>
        /// <returns>The <see cref="List{KeyValuePair}"/> of properties.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="XmlException"></exception>        
        public static List<KeyValuePair<string, string>> LoadStyleProperties(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

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

        /// <summary>This method returns the value of <paramref name="property" /> style property.</summary>
        /// <param name="property">The name of the style property whose value you want to get.</param>
        /// <returns>The value of the style property, <c>null</c> if the property doesn't exist.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetStylePropertyValue(string property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);
            return prpt.HasValue ? prpt.Value.Value : null;
        }

        /// <summary>This method sets the <paramref name="value" /> of <paramref name="property" /> style property.</summary>
        /// <param name="property">The name of the style property whose value you want to set.</param>
        /// <param name="value">The value you want to set.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>If the property doesn't exist, it's created.</remarks>
        public void SetStylePropertyValue(string property, string value)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);

            if (prpt.HasValue) StyleProperties.Remove(prpt.Value);

            prpt = new KeyValuePair<string, string>(property, value ?? "");
            StyleProperties.Add(prpt.Value);
            SetAttributeInnerText("style", StylePropertiesToString());
        }

        /// <summary>This methods returns the inner text of <paramref name="attribute" /> <see cref="XmlAttribute"/>.</summary>
        /// <param name="attribute">The name of the <see cref="XmlAttribute"/> whose inner text you want to get.</param>
        /// <returns>The inner text of the <see cref="XmlAttribute"/>, <c>null</c> if the <see cref="XmlAttribute"/> doesn't exist.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetAttributeInnerText(string attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            XmlNode attr = Node.Attributes.GetNamedItem(attribute);
            return attr != null ? attr.InnerText : null;
        }

        /// <summary>This method sets the <paramref name="innerText" /> of <paramref name="attribute" /> <see cref="XmlAttribute"/>.</summary>
        /// <param name="attribute">The name of the <see cref="XmlAttribute"/> whose inner text you want to set.</param>
        /// <param name="innerText">The inner text to set.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>If the <see cref="XmlAttribute"/> doesn't exist, it's created.</remarks>
        public void SetAttributeInnerText(string attribute, string innerText)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            XmlNode attr = Node.Attributes.GetNamedItem(attribute);

            if (attr != null)
                attr.InnerText = innerText;
            else
            {
                attr = Node.OwnerDocument.CreateAttribute(attribute);
                attr.InnerText = innerText ?? "";
                Node.Attributes.SetNamedItem(attr);
            }
        }

        /// <summary>This method converts <see cref="StyleProperties"/> to string.</summary>
        /// <returns><see cref="StyleProperties"/> in a string format.</returns>
        /// <remarks>It's used to update the style <see cref="XmlAttribute"/> in a node when one of its properties changes.</remarks>
        /// <example>
        /// The following properties:
        /// <list type="table"><listHeader><term>Key</term><term>Value</term></listHeader><item><description>ellipse</description><description></description></item><item><description>fillColor</description><description>‎#FFD700</description></item></list>
        /// will result: "ellipse;fillColor=#FFD700;". It's important to notice that since the ellipse property doesn't have a value, in the string the <c>=</c> sign is omitted.
        /// </example>
        private string StylePropertiesToString()
        {
            string prpts = "";

            foreach (KeyValuePair<string, string> prpt in StyleProperties)
            {
                prpts += prpt.Key;
                prpts += prpt.Value != "" ? "=" : "";
                prpts += prpt.Value + ";";
            }

            return prpts;
        }
    }
}
