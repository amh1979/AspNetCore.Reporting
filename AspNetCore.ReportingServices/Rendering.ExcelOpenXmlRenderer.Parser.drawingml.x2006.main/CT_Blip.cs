using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Blip : OoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			alphaBiLevel,
			alphaCeiling,
			alphaFloor,
			alphaInv,
			alphaMod,
			alphaModFix,
			alphaRepl,
			biLevel,
			blur,
			clrChange,
			clrRepl,
			duotone,
			fillOverlay,
			grayscl,
			hsl,
			lum,
			tint
		}

		private string _embed_attr;

		private string _link_attr;

		private ST_BlipCompression _cstate_attr;

		private ChoiceBucket_0 _choice_0;

		public string Embed_Attr
		{
			get
			{
				return this._embed_attr;
			}
			set
			{
				this._embed_attr = value;
			}
		}

		public string Link_Attr
		{
			get
			{
				return this._link_attr;
			}
			set
			{
				this._link_attr = value;
			}
		}

		public ST_BlipCompression Cstate_Attr
		{
			get
			{
				return this._cstate_attr;
			}
			set
			{
				this._cstate_attr = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return this._choice_0;
			}
			set
			{
				this._choice_0 = value;
			}
		}

		protected override void InitAttributes()
		{
			this._embed_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._link_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._cstate_attr = ST_BlipCompression.none;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, true);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, false);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			this.WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (this._embed_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" r:embed=\"");
				OoxmlComplexType.WriteData(s, this._embed_attr);
				s.Write("\"");
			}
			if (this._link_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" r:link=\"");
				OoxmlComplexType.WriteData(s, this._link_attr);
				s.Write("\"");
			}
			if (this._cstate_attr != ST_BlipCompression.none)
			{
				s.Write(" cstate=\"");
				OoxmlComplexType.WriteData(s, this._cstate_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
