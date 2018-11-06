using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_CalcPr : OoxmlComplexType
	{
		private uint _calcId_attr;

		private ST_CalcMode _calcMode_attr;

		private OoxmlBool _fullCalcOnLoad_attr;

		private ST_RefMode _refMode_attr;

		private OoxmlBool _iterate_attr;

		private uint _iterateCount_attr;

		private double _iterateDelta_attr;

		private OoxmlBool _fullPrecision_attr;

		private OoxmlBool _calcCompleted_attr;

		private OoxmlBool _calcOnSave_attr;

		private OoxmlBool _concurrentCalc_attr;

		private uint _concurrentManualCount_attr;

		private bool _concurrentManualCount_attr_is_specified;

		private OoxmlBool _forceFullCalc_attr;

		private bool _forceFullCalc_attr_is_specified;

		public uint CalcId_Attr
		{
			get
			{
				return this._calcId_attr;
			}
			set
			{
				this._calcId_attr = value;
			}
		}

		public ST_CalcMode CalcMode_Attr
		{
			get
			{
				return this._calcMode_attr;
			}
			set
			{
				this._calcMode_attr = value;
			}
		}

		public OoxmlBool FullCalcOnLoad_Attr
		{
			get
			{
				return this._fullCalcOnLoad_attr;
			}
			set
			{
				this._fullCalcOnLoad_attr = value;
			}
		}

		public ST_RefMode RefMode_Attr
		{
			get
			{
				return this._refMode_attr;
			}
			set
			{
				this._refMode_attr = value;
			}
		}

		public OoxmlBool Iterate_Attr
		{
			get
			{
				return this._iterate_attr;
			}
			set
			{
				this._iterate_attr = value;
			}
		}

		public uint IterateCount_Attr
		{
			get
			{
				return this._iterateCount_attr;
			}
			set
			{
				this._iterateCount_attr = value;
			}
		}

		public double IterateDelta_Attr
		{
			get
			{
				return this._iterateDelta_attr;
			}
			set
			{
				this._iterateDelta_attr = value;
			}
		}

		public OoxmlBool FullPrecision_Attr
		{
			get
			{
				return this._fullPrecision_attr;
			}
			set
			{
				this._fullPrecision_attr = value;
			}
		}

		public OoxmlBool CalcCompleted_Attr
		{
			get
			{
				return this._calcCompleted_attr;
			}
			set
			{
				this._calcCompleted_attr = value;
			}
		}

		public OoxmlBool CalcOnSave_Attr
		{
			get
			{
				return this._calcOnSave_attr;
			}
			set
			{
				this._calcOnSave_attr = value;
			}
		}

		public OoxmlBool ConcurrentCalc_Attr
		{
			get
			{
				return this._concurrentCalc_attr;
			}
			set
			{
				this._concurrentCalc_attr = value;
			}
		}

		public uint ConcurrentManualCount_Attr
		{
			get
			{
				return this._concurrentManualCount_attr;
			}
			set
			{
				this._concurrentManualCount_attr = value;
				this._concurrentManualCount_attr_is_specified = true;
			}
		}

		public bool ConcurrentManualCount_Attr_Is_Specified
		{
			get
			{
				return this._concurrentManualCount_attr_is_specified;
			}
			set
			{
				this._concurrentManualCount_attr_is_specified = value;
			}
		}

		public OoxmlBool ForceFullCalc_Attr
		{
			get
			{
				return this._forceFullCalc_attr;
			}
			set
			{
				this._forceFullCalc_attr = value;
				this._forceFullCalc_attr_is_specified = true;
			}
		}

		public bool ForceFullCalc_Attr_Is_Specified
		{
			get
			{
				return this._forceFullCalc_attr_is_specified;
			}
			set
			{
				this._forceFullCalc_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			this._calcMode_attr = ST_CalcMode.auto;
			this._fullCalcOnLoad_attr = OoxmlBool.OoxmlFalse;
			this._refMode_attr = ST_RefMode.A1;
			this._iterate_attr = OoxmlBool.OoxmlFalse;
			this._iterateCount_attr = Convert.ToUInt32("100", CultureInfo.InvariantCulture);
			this._iterateDelta_attr = Convert.ToDouble("0.001", CultureInfo.InvariantCulture);
			this._fullPrecision_attr = OoxmlBool.OoxmlTrue;
			this._calcCompleted_attr = OoxmlBool.OoxmlTrue;
			this._calcOnSave_attr = OoxmlBool.OoxmlTrue;
			this._concurrentCalc_attr = OoxmlBool.OoxmlTrue;
			this._concurrentManualCount_attr_is_specified = false;
			this._forceFullCalc_attr_is_specified = false;
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
			s.Write(" calcId=\"");
			OoxmlComplexType.WriteData(s, this._calcId_attr);
			s.Write("\"");
			if (this._calcMode_attr != ST_CalcMode.auto)
			{
				s.Write(" calcMode=\"");
				OoxmlComplexType.WriteData(s, this._calcMode_attr);
				s.Write("\"");
			}
			if ((bool)(this._fullCalcOnLoad_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" fullCalcOnLoad=\"");
				OoxmlComplexType.WriteData(s, this._fullCalcOnLoad_attr);
				s.Write("\"");
			}
			if (this._refMode_attr != ST_RefMode.A1)
			{
				s.Write(" refMode=\"");
				OoxmlComplexType.WriteData(s, this._refMode_attr);
				s.Write("\"");
			}
			if ((bool)(this._iterate_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" iterate=\"");
				OoxmlComplexType.WriteData(s, this._iterate_attr);
				s.Write("\"");
			}
			if (this._iterateCount_attr != Convert.ToUInt32("100", CultureInfo.InvariantCulture))
			{
				s.Write(" iterateCount=\"");
				OoxmlComplexType.WriteData(s, this._iterateCount_attr);
				s.Write("\"");
			}
			if (this._iterateDelta_attr != Convert.ToDouble("0.001", CultureInfo.InvariantCulture))
			{
				s.Write(" iterateDelta=\"");
				OoxmlComplexType.WriteData(s, this._iterateDelta_attr);
				s.Write("\"");
			}
			if ((bool)(this._fullPrecision_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" fullPrecision=\"");
				OoxmlComplexType.WriteData(s, this._fullPrecision_attr);
				s.Write("\"");
			}
			if ((bool)(this._calcCompleted_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" calcCompleted=\"");
				OoxmlComplexType.WriteData(s, this._calcCompleted_attr);
				s.Write("\"");
			}
			if ((bool)(this._calcOnSave_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" calcOnSave=\"");
				OoxmlComplexType.WriteData(s, this._calcOnSave_attr);
				s.Write("\"");
			}
			if ((bool)(this._concurrentCalc_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" concurrentCalc=\"");
				OoxmlComplexType.WriteData(s, this._concurrentCalc_attr);
				s.Write("\"");
			}
			if (this._concurrentManualCount_attr_is_specified)
			{
				s.Write(" concurrentManualCount=\"");
				OoxmlComplexType.WriteData(s, this._concurrentManualCount_attr);
				s.Write("\"");
			}
			if (this._forceFullCalc_attr_is_specified)
			{
				s.Write(" forceFullCalc=\"");
				OoxmlComplexType.WriteData(s, this._forceFullCalc_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
