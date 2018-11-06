using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_SheetFormatPr : OoxmlComplexType
	{
		private uint _baseColWidth_attr;

		private double _defaultRowHeight_attr;

		private OoxmlBool _customHeight_attr;

		private OoxmlBool _zeroHeight_attr;

		private OoxmlBool _thickTop_attr;

		private OoxmlBool _thickBottom_attr;

		private byte _outlineLevelRow_attr;

		private byte _outlineLevelCol_attr;

		private double _defaultColWidth_attr;

		private bool _defaultColWidth_attr_is_specified;

		public uint BaseColWidth_Attr
		{
			get
			{
				return this._baseColWidth_attr;
			}
			set
			{
				this._baseColWidth_attr = value;
			}
		}

		public double DefaultRowHeight_Attr
		{
			get
			{
				return this._defaultRowHeight_attr;
			}
			set
			{
				this._defaultRowHeight_attr = value;
			}
		}

		public OoxmlBool CustomHeight_Attr
		{
			get
			{
				return this._customHeight_attr;
			}
			set
			{
				this._customHeight_attr = value;
			}
		}

		public OoxmlBool ZeroHeight_Attr
		{
			get
			{
				return this._zeroHeight_attr;
			}
			set
			{
				this._zeroHeight_attr = value;
			}
		}

		public OoxmlBool ThickTop_Attr
		{
			get
			{
				return this._thickTop_attr;
			}
			set
			{
				this._thickTop_attr = value;
			}
		}

		public OoxmlBool ThickBottom_Attr
		{
			get
			{
				return this._thickBottom_attr;
			}
			set
			{
				this._thickBottom_attr = value;
			}
		}

		public byte OutlineLevelRow_Attr
		{
			get
			{
				return this._outlineLevelRow_attr;
			}
			set
			{
				this._outlineLevelRow_attr = value;
			}
		}

		public byte OutlineLevelCol_Attr
		{
			get
			{
				return this._outlineLevelCol_attr;
			}
			set
			{
				this._outlineLevelCol_attr = value;
			}
		}

		public double DefaultColWidth_Attr
		{
			get
			{
				return this._defaultColWidth_attr;
			}
			set
			{
				this._defaultColWidth_attr = value;
				this._defaultColWidth_attr_is_specified = true;
			}
		}

		public bool DefaultColWidth_Attr_Is_Specified
		{
			get
			{
				return this._defaultColWidth_attr_is_specified;
			}
			set
			{
				this._defaultColWidth_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			this._baseColWidth_attr = Convert.ToUInt32("8", CultureInfo.InvariantCulture);
			this._customHeight_attr = OoxmlBool.OoxmlFalse;
			this._zeroHeight_attr = OoxmlBool.OoxmlFalse;
			this._thickTop_attr = OoxmlBool.OoxmlFalse;
			this._thickBottom_attr = OoxmlBool.OoxmlFalse;
			this._outlineLevelRow_attr = Convert.ToByte("0", CultureInfo.InvariantCulture);
			this._outlineLevelCol_attr = Convert.ToByte("0", CultureInfo.InvariantCulture);
			this._defaultColWidth_attr_is_specified = false;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" defaultRowHeight=\"");
			OoxmlComplexType.WriteData(s, this._defaultRowHeight_attr);
			s.Write("\"");
			if (this._baseColWidth_attr != Convert.ToUInt32("8", CultureInfo.InvariantCulture))
			{
				s.Write(" baseColWidth=\"");
				OoxmlComplexType.WriteData(s, this._baseColWidth_attr);
				s.Write("\"");
			}
			if ((bool)(this._customHeight_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" customHeight=\"");
				OoxmlComplexType.WriteData(s, this._customHeight_attr);
				s.Write("\"");
			}
			if ((bool)(this._zeroHeight_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" zeroHeight=\"");
				OoxmlComplexType.WriteData(s, this._zeroHeight_attr);
				s.Write("\"");
			}
			if ((bool)(this._thickTop_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickTop=\"");
				OoxmlComplexType.WriteData(s, this._thickTop_attr);
				s.Write("\"");
			}
			if ((bool)(this._thickBottom_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickBottom=\"");
				OoxmlComplexType.WriteData(s, this._thickBottom_attr);
				s.Write("\"");
			}
			if (this._outlineLevelRow_attr != Convert.ToByte("0", CultureInfo.InvariantCulture))
			{
				s.Write(" outlineLevelRow=\"");
				OoxmlComplexType.WriteData(s, this._outlineLevelRow_attr);
				s.Write("\"");
			}
			if (this._outlineLevelCol_attr != Convert.ToByte("0", CultureInfo.InvariantCulture))
			{
				s.Write(" outlineLevelCol=\"");
				OoxmlComplexType.WriteData(s, this._outlineLevelCol_attr);
				s.Write("\"");
			}
			if (this._defaultColWidth_attr_is_specified)
			{
				s.Write(" defaultColWidth=\"");
				OoxmlComplexType.WriteData(s, this._defaultColWidth_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
