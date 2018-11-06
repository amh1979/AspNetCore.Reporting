using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_GraphicalObjectData : OoxmlComplexType, IOoxmlComplexType
	{
		private string _uri_attr;

		private CT_Picture _pic;

		private List<XmlElement> _any;

		public string Uri_Attr
		{
			get
			{
				return this._uri_attr;
			}
			set
			{
				this._uri_attr = value;
			}
		}

		public CT_Picture Pic
		{
			get
			{
				return this._pic;
			}
			set
			{
				this._pic = value;
			}
		}

		public List<XmlElement> Any
		{
			get
			{
				return this._any;
			}
			set
			{
				this._any = value;
			}
		}

		public static string PicElementName
		{
			get
			{
				return "pic";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._pic = new CT_Picture();
		}

		protected override void InitCollections()
		{
			this._any = new List<XmlElement>();
		}

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "a", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</a:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" uri=\"");
			OoxmlComplexType.WriteData(s, this._uri_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_pic(s);
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter xmlWriter = XmlWriter.Create(s, xmlWriterSettings);
			foreach (XmlElement item in this._any)
			{
				item.WriteTo(xmlWriter);
			}
			xmlWriter.Flush();
		}

		public void Write_pic(TextWriter s)
		{
			if (this._pic != null)
			{
				this._pic.Write(s, "pic");
			}
		}
	}
}
