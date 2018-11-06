using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal abstract class OoxmlComplexType
	{
		protected OoxmlComplexType()
		{
			this.InitAttributes();
			this.InitElements();
			this.InitCollections();
		}

		public abstract void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces);

		public abstract void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces);

		public abstract void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root);

		public abstract void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces);

		public abstract void WriteAttributes(TextWriter s);

		public abstract void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces);

		protected abstract void InitAttributes();

		protected abstract void InitElements();

		protected abstract void InitCollections();

		protected static void WriteXmlPrefix(TextWriter s, Dictionary<string, string> namespaces, string tagNamespace)
		{
			if (namespaces[tagNamespace] != "")
			{
				s.Write(namespaces[tagNamespace]);
				s.Write(":");
			}
		}

		protected static void WriteData(TextWriter s, object data)
		{
			if (data != null)
			{
				if (data is DateTime)
				{
					s.Write(((DateTime)data).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
				}
				else if (data is bool)
				{
					s.Write(((bool)data) ? 1 : 0);
				}
				else if (data is float)
				{
					s.Write(((float)data).ToString(CultureInfo.InvariantCulture));
				}
				else if (data is double)
				{
					s.Write(((double)data).ToString(CultureInfo.InvariantCulture));
				}
				else if (data is decimal)
				{
					s.Write(((decimal)data).ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					string text = SecurityElement.Escape(data.ToString());
					if (text != null)
					{
						text = text.Replace("\0", "");
					}
					s.Write(text);
				}
			}
		}

		protected static void WriteRawTag(TextWriter s, int depth, Dictionary<string, string> namespaces, string tagname, string tagNamespace, object data)
		{
			OoxmlComplexType.WriteRawTag(s, depth, namespaces, tagname, false, tagNamespace, data);
		}

		protected static void WriteRawTag(TextWriter s, int depth, Dictionary<string, string> namespaces, string tagname, bool preserveWhitespace, string tagNamespace, object data)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, tagNamespace);
			s.Write(tagname);
			if (preserveWhitespace)
			{
				s.Write(" xml:space=\"preserve\"");
			}
			s.Write(">");
			OoxmlComplexType.WriteData(s, data);
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, tagNamespace);
			s.Write(tagname);
			s.Write(">");
		}
	}
}
