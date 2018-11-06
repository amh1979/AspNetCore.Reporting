using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_PageSetup : OoxmlComplexType
	{
		private uint _paperSize_attr;

		private uint _scale_attr;

		private uint _firstPageNumber_attr;

		private uint _fitToWidth_attr;

		private uint _fitToHeight_attr;

		private ST_PageOrder _pageOrder_attr;

		private ST_Orientation _orientation_attr;

		private OoxmlBool _usePrinterDefaults_attr;

		private OoxmlBool _blackAndWhite_attr;

		private OoxmlBool _draft_attr;

		private ST_CellComments _cellComments_attr;

		private OoxmlBool _useFirstPageNumber_attr;

		private ST_PrintError _errors_attr;

		private uint _horizontalDpi_attr;

		private uint _verticalDpi_attr;

		private uint _copies_attr;

		private string _paperHeight_attr;

		private bool _paperHeight_attr_is_specified;

		private string _paperWidth_attr;

		private bool _paperWidth_attr_is_specified;

		private string _id_attr;

		private bool _id_attr_is_specified;

		public uint PaperSize_Attr
		{
			get
			{
				return this._paperSize_attr;
			}
			set
			{
				this._paperSize_attr = value;
			}
		}

		public uint Scale_Attr
		{
			get
			{
				return this._scale_attr;
			}
			set
			{
				this._scale_attr = value;
			}
		}

		public uint FirstPageNumber_Attr
		{
			get
			{
				return this._firstPageNumber_attr;
			}
			set
			{
				this._firstPageNumber_attr = value;
			}
		}

		public uint FitToWidth_Attr
		{
			get
			{
				return this._fitToWidth_attr;
			}
			set
			{
				this._fitToWidth_attr = value;
			}
		}

		public uint FitToHeight_Attr
		{
			get
			{
				return this._fitToHeight_attr;
			}
			set
			{
				this._fitToHeight_attr = value;
			}
		}

		public ST_PageOrder PageOrder_Attr
		{
			get
			{
				return this._pageOrder_attr;
			}
			set
			{
				this._pageOrder_attr = value;
			}
		}

		public ST_Orientation Orientation_Attr
		{
			get
			{
				return this._orientation_attr;
			}
			set
			{
				this._orientation_attr = value;
			}
		}

		public OoxmlBool UsePrinterDefaults_Attr
		{
			get
			{
				return this._usePrinterDefaults_attr;
			}
			set
			{
				this._usePrinterDefaults_attr = value;
			}
		}

		public OoxmlBool BlackAndWhite_Attr
		{
			get
			{
				return this._blackAndWhite_attr;
			}
			set
			{
				this._blackAndWhite_attr = value;
			}
		}

		public OoxmlBool Draft_Attr
		{
			get
			{
				return this._draft_attr;
			}
			set
			{
				this._draft_attr = value;
			}
		}

		public ST_CellComments CellComments_Attr
		{
			get
			{
				return this._cellComments_attr;
			}
			set
			{
				this._cellComments_attr = value;
			}
		}

		public OoxmlBool UseFirstPageNumber_Attr
		{
			get
			{
				return this._useFirstPageNumber_attr;
			}
			set
			{
				this._useFirstPageNumber_attr = value;
			}
		}

		public ST_PrintError Errors_Attr
		{
			get
			{
				return this._errors_attr;
			}
			set
			{
				this._errors_attr = value;
			}
		}

		public uint HorizontalDpi_Attr
		{
			get
			{
				return this._horizontalDpi_attr;
			}
			set
			{
				this._horizontalDpi_attr = value;
			}
		}

		public uint VerticalDpi_Attr
		{
			get
			{
				return this._verticalDpi_attr;
			}
			set
			{
				this._verticalDpi_attr = value;
			}
		}

		public uint Copies_Attr
		{
			get
			{
				return this._copies_attr;
			}
			set
			{
				this._copies_attr = value;
			}
		}

		public string PaperHeight_Attr
		{
			get
			{
				return this._paperHeight_attr;
			}
			set
			{
				this._paperHeight_attr = value;
				this._paperHeight_attr_is_specified = (value != null);
			}
		}

		public string PaperWidth_Attr
		{
			get
			{
				return this._paperWidth_attr;
			}
			set
			{
				this._paperWidth_attr = value;
				this._paperWidth_attr_is_specified = (value != null);
			}
		}

		public string Id_Attr
		{
			get
			{
				return this._id_attr;
			}
			set
			{
				this._id_attr = value;
				this._id_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._paperSize_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			this._scale_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			this._firstPageNumber_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			this._fitToWidth_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			this._fitToHeight_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			this._pageOrder_attr = ST_PageOrder.downThenOver;
			this._orientation_attr = ST_Orientation._default;
			this._usePrinterDefaults_attr = OoxmlBool.OoxmlTrue;
			this._blackAndWhite_attr = OoxmlBool.OoxmlFalse;
			this._draft_attr = OoxmlBool.OoxmlFalse;
			this._cellComments_attr = ST_CellComments.none;
			this._useFirstPageNumber_attr = OoxmlBool.OoxmlFalse;
			this._errors_attr = ST_PrintError.displayed;
			this._horizontalDpi_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			this._verticalDpi_attr = Convert.ToUInt32("600", CultureInfo.InvariantCulture);
			this._copies_attr = Convert.ToUInt32("1", CultureInfo.InvariantCulture);
			this._paperHeight_attr_is_specified = false;
			this._paperWidth_attr_is_specified = false;
			this._id_attr_is_specified = false;
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
			if (this._paperSize_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" paperSize=\"");
				OoxmlComplexType.WriteData(s, this._paperSize_attr);
				s.Write("\"");
			}
			if (this._scale_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" scale=\"");
				OoxmlComplexType.WriteData(s, this._scale_attr);
				s.Write("\"");
			}
			if (this._firstPageNumber_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" firstPageNumber=\"");
				OoxmlComplexType.WriteData(s, this._firstPageNumber_attr);
				s.Write("\"");
			}
			if (this._fitToWidth_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" fitToWidth=\"");
				OoxmlComplexType.WriteData(s, this._fitToWidth_attr);
				s.Write("\"");
			}
			if (this._fitToHeight_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" fitToHeight=\"");
				OoxmlComplexType.WriteData(s, this._fitToHeight_attr);
				s.Write("\"");
			}
			if (this._pageOrder_attr != ST_PageOrder.downThenOver)
			{
				s.Write(" pageOrder=\"");
				OoxmlComplexType.WriteData(s, this._pageOrder_attr);
				s.Write("\"");
			}
			if (this._orientation_attr != ST_Orientation._default)
			{
				s.Write(" orientation=\"");
				OoxmlComplexType.WriteData(s, this._orientation_attr);
				s.Write("\"");
			}
			if ((bool)(this._usePrinterDefaults_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" usePrinterDefaults=\"");
				OoxmlComplexType.WriteData(s, this._usePrinterDefaults_attr);
				s.Write("\"");
			}
			if ((bool)(this._blackAndWhite_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" blackAndWhite=\"");
				OoxmlComplexType.WriteData(s, this._blackAndWhite_attr);
				s.Write("\"");
			}
			if ((bool)(this._draft_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" draft=\"");
				OoxmlComplexType.WriteData(s, this._draft_attr);
				s.Write("\"");
			}
			if (this._cellComments_attr != ST_CellComments.none)
			{
				s.Write(" cellComments=\"");
				OoxmlComplexType.WriteData(s, this._cellComments_attr);
				s.Write("\"");
			}
			if ((bool)(this._useFirstPageNumber_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" useFirstPageNumber=\"");
				OoxmlComplexType.WriteData(s, this._useFirstPageNumber_attr);
				s.Write("\"");
			}
			if (this._errors_attr != ST_PrintError.displayed)
			{
				s.Write(" errors=\"");
				OoxmlComplexType.WriteData(s, this._errors_attr);
				s.Write("\"");
			}
			if (this._horizontalDpi_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" horizontalDpi=\"");
				OoxmlComplexType.WriteData(s, this._horizontalDpi_attr);
				s.Write("\"");
			}
			if (this._verticalDpi_attr != Convert.ToUInt32("600", CultureInfo.InvariantCulture))
			{
				s.Write(" verticalDpi=\"");
				OoxmlComplexType.WriteData(s, this._verticalDpi_attr);
				s.Write("\"");
			}
			if (this._copies_attr != Convert.ToUInt32("1", CultureInfo.InvariantCulture))
			{
				s.Write(" copies=\"");
				OoxmlComplexType.WriteData(s, this._copies_attr);
				s.Write("\"");
			}
			if (this._paperHeight_attr_is_specified)
			{
				s.Write(" paperHeight=\"");
				OoxmlComplexType.WriteData(s, this._paperHeight_attr);
				s.Write("\"");
			}
			if (this._paperWidth_attr_is_specified)
			{
				s.Write(" paperWidth=\"");
				OoxmlComplexType.WriteData(s, this._paperWidth_attr);
				s.Write("\"");
			}
			if (this._id_attr_is_specified)
			{
				s.Write(" r:id=\"");
				OoxmlComplexType.WriteData(s, this._id_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
