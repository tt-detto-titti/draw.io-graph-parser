using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Draw.io_Graph_Parser
{
    public abstract class GraphElement
    {
        public static XmlDocument Graph { get; private set; }
        public XmlNode Node { get; protected set; }
        public string Id { get; protected set; }
        public List<KeyValuePair<string, string>> StyleProperties { get; protected set; }       

        public GraphElement(XmlNode node)
        {            
            try
            {
                Node = node;
                Id = GetAttributeInnerText("id");
                StyleProperties = new List<KeyValuePair<string, string>>();

                foreach (string property in GetAttributeInnerText("style").Split(';'))
                    if (property != "")
                    {
                        string[] info = property.Split('=');
                        StyleProperties.Add(new KeyValuePair<string, string>(
                            info[0],
                            info.Length > 1 ? info[1] : null));
                    }
            }
            catch (XmlException e)
            {
                throw new XmlException(e.Message);
            }
        }

        public string GetStylePropertyValue(string property)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);

            if (prpt == null)
                throw new XmlException("The property {0} doesn't exist in the style attribute.");

            return prpt.Value.Value;
        }

        public void SetStylePropertyValue(string property, string value)
        {
            KeyValuePair<string, string>? prpt = StyleProperties.Find(x => x.Key == property);

            prpt = new KeyValuePair<string, string>(property, value);
            SetAttributeInnerText("style", StylePropertiesToString());
        }

        private string StylePropertiesToString()
        {
            string s = "";

            foreach (KeyValuePair<string, string> prpt in StyleProperties)
                s += prpt.Key + "=" + prpt.Value + ";";

            return s;
        }

        public string GetAttributeInnerText(string attribute)
        {
            XmlNode attr = Node.Attributes.GetNamedItem(attribute);

            if (attr == null)
                throw new XmlException(string.Format("The attribute {0} doesn't exist in the node.", attribute));

            return attr.InnerText;
        }

        public void SetAttributeInnerText(string attribute, string innerText)
        {
            XmlNode attr = Node.Attributes.GetNamedItem(attribute);

            if (attr == null)
            {
                attr = Graph.CreateAttribute(attribute);
                attr.InnerText = innerText;
                Node.Attributes.SetNamedItem(attr);
            }
            else
                attr.InnerText = innerText;
        }
    }
}
