using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public abstract class GraphElement
    {
        public XmlNode Node { get; protected set; }
        public string Id { get; protected set; }
        private string vl = "";
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
        public List<KeyValuePair<string, string>> StyleProperties { get; protected set; }

        public GraphElement(XmlNode node, List<KeyValuePair<string, string>> styleProperties)
        {
            Node = node;
            Id = GetAttributeInnerText("id");
            Value = GetAttributeInnerText("value");
            StyleProperties = styleProperties;
        }

        public GraphElement(XmlNode node) : this(node, LoadStyleProperties(node)) { }

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

        public string GetStylePropertyValue(string property)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);
            return prpt.HasValue ? prpt.Value.Value : null;
        }

        public void SetStylePropertyValue(string property, string value)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);

            if (prpt.HasValue) StyleProperties.Remove(prpt.Value);

            prpt = new KeyValuePair<string, string>(property, value);
            StyleProperties.Add(prpt.Value);
            SetAttributeInnerText("style", StylePropertiesToString());
        }

        public string GetAttributeInnerText(string attribute)
        {
            XmlNode attr = Node.Attributes.GetNamedItem(attribute);
            return attr != null ? attr.InnerText : null;
        }

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

        private string StylePropertiesToString()
        {
            string prpts = "";

            foreach (KeyValuePair<string, string> prpt in StyleProperties)
                prpts += prpt.Key + "=" + prpt.Value + ";";

            return prpts;
        }        
    }
}
