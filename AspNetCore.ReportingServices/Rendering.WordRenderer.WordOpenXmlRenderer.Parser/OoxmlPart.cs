using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser
{
	internal abstract class OoxmlPart
	{
		public static string XmlDeclaration
		{
			get
			{
				return "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?>";
			}
		}

		public abstract OoxmlComplexType Root
		{
			get;
		}

		public abstract string Tag
		{
			get;
		}

		public abstract Dictionary<string, string> Namespaces
		{
			get;
		}

		protected OoxmlPart()
		{
		}

		protected OoxmlPart(XmlDocument xml)
		{
		}

		public void Write(TextWriter s)
		{
			s.Write(OoxmlPart.XmlDeclaration);
			this.Root.WriteAsRoot(s, this.Tag, this.Namespaces);
		}
	}
}
