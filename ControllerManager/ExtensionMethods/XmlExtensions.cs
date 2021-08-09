using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen;

namespace ControllerManager.ExtensionMethods
{
    public static class XmlExtensions
    {
		public static XmlNode AppendNewEmptyNode(this XmlNode xmlNode, string nodeName)
		{
			XmlDocument xmlDocument = (XmlDocument)((xmlNode.OwnerDocument == null) ? xmlNode : xmlNode.OwnerDocument);
			XmlNode newNode = xmlDocument.CreateElement(nodeName);
			xmlNode.AppendChild(newNode);
			return xmlNode;
		}

		public static XmlNode CreateNewNode(this XmlNode xmlNode, string nodeName)
		{
			XmlDocument xmlDocument = (XmlDocument)((xmlNode.OwnerDocument == null) ? xmlNode : xmlNode.OwnerDocument);
			XmlNode newNode = xmlDocument.CreateElement(nodeName);
			return newNode;
		}
	}
}
