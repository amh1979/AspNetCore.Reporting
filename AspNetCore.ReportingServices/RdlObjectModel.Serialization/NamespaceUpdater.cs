using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal sealed class NamespaceUpdater
	{
		private sealed class ExtensionNamespaceCounter
		{
			public ExtensionNamespace ExtensionNamespace
			{
				get;
				private set;
			}

			public int Count
			{
				get;
				set;
			}

			public ExtensionNamespaceCounter(ExtensionNamespace extensionNamespace)
			{
				this.ExtensionNamespace = extensionNamespace;
				this.Count = 0;
			}
		}

		public string[] Update(XmlWriter writer, string xml, ISerializerHost host = null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			NamespaceUpdater namespaceUpdater = new NamespaceUpdater();
			string[] result = namespaceUpdater.Update(xmlDocument, host);
			xmlDocument.Save(writer);
			return result;
		}

		public string[] Update(XmlDocument document, ISerializerHost host = null)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			Dictionary<string, ExtensionNamespaceCounter> dictionary = (host == null) ? new Dictionary<string, ExtensionNamespaceCounter>() : host.GetExtensionNamespaces().ToDictionary((ExtensionNamespace v) => v.Namespace, (ExtensionNamespace v) => new ExtensionNamespaceCounter(v));
			XmlElement documentElement = document.DocumentElement;
			if (documentElement == null)
			{
				throw new InvalidOperationException("Document contains no elements");
			}
			this.AddMustUnderstandNamespaces(dictionary, documentElement);
			this.AddXmlnsNamespaces(dictionary, documentElement);
			this.UpdateLocalNames(documentElement, dictionary);
			ExtensionNamespace[] array = (from v in dictionary
			where v.Value.Count > 0
			select v.Value.ExtensionNamespace).ToArray();
			NamespaceUpdater.UpdateRootNamespaces(array, documentElement);
			string[] array2 = (from v in array
			where v.MustUnderstand
			select v.LocalName).ToArray();
			string text = string.Join(" ", array2);
			if (text.Any())
			{
				documentElement.SetAttribute("MustUnderstand", text);
			}
			else
			{
				documentElement.RemoveAttribute("MustUnderstand");
			}
			return array2;
		}

		private void AddXmlnsNamespaces(Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary, XmlElement rootElem)
		{
			foreach (XmlAttribute item in rootElem.Attributes.Cast<XmlAttribute>().Where(delegate(XmlAttribute a)
			{
				if (a.Prefix == "xmlns")
				{
					return !xmlnsDictionary.ContainsKey(a.Value);
				}
				return false;
			}))
			{
				ExtensionNamespace extensionNamespace = new ExtensionNamespace(item.LocalName, item.Value, false);
				xmlnsDictionary.Add(item.Value, new ExtensionNamespaceCounter(extensionNamespace));
			}
		}

		private static void UpdateRootNamespaces(IEnumerable<ExtensionNamespace> namespaces, XmlElement rootElem)
		{
			int num = rootElem.Attributes.Count;
			while (num-- > 0)
			{
				XmlAttribute xmlAttribute = rootElem.Attributes[num];
				if (xmlAttribute.Prefix == "xmlns")
				{
					rootElem.Attributes.RemoveAt(num);
				}
			}
			foreach (ExtensionNamespace @namespace in namespaces)
			{
				rootElem.SetAttribute(string.Format("xmlns:{0}", @namespace.LocalName), @namespace.Namespace);
			}
		}

		private void AddMustUnderstandNamespaces(Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary, XmlElement rootElem)
		{
			XmlNode namedItem = rootElem.Attributes.GetNamedItem("MustUnderstand");
			if (namedItem != null && !string.IsNullOrEmpty(namedItem.Value))
			{
				string[] array = namedItem.Value.Split();
				string[] array2 = array;
				foreach (string text in array2)
				{
					string namespaceOfPrefix = rootElem.GetNamespaceOfPrefix(text);
					if (!string.IsNullOrEmpty(namespaceOfPrefix) && !xmlnsDictionary.ContainsKey(namespaceOfPrefix))
					{
						xmlnsDictionary.Add(namespaceOfPrefix, new ExtensionNamespaceCounter(new ExtensionNamespace(text, namespaceOfPrefix, true)));
					}
				}
			}
		}

		private void UpdateLocalNames(XmlElement xmlElement, Dictionary<string, ExtensionNamespaceCounter> xmlnsDictionary)
		{
			Stack<XmlNode> stack = new Stack<XmlNode>(new XmlElement[1]
			{
				xmlElement
			});
			while (stack.Count != 0)
			{
				XmlNode xmlNode = stack.Pop();
				ExtensionNamespaceCounter extensionNamespaceCounter = default(ExtensionNamespaceCounter);
				if (xmlnsDictionary.TryGetValue(xmlNode.NamespaceURI, out extensionNamespaceCounter))
				{
					xmlNode.Prefix = extensionNamespaceCounter.ExtensionNamespace.LocalName;
					XmlElement xmlElement2 = xmlNode as XmlElement;
					if (xmlElement2 != null)
					{
						xmlElement2.RemoveAttribute("xmlns");
					}
					extensionNamespaceCounter.Count++;
				}
				IEnumerable<XmlNode> first = (xmlNode.Attributes == null) ? new XmlNode[0] : xmlNode.Attributes.Cast<XmlNode>();
				IEnumerable<XmlNode> second = xmlNode.ChildNodes.Cast<XmlNode>();
				foreach (XmlNode item in first.Concat(second))
				{
					stack.Push(item);
				}
			}
		}
	}
}
