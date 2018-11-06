using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TcPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_TblWidth _tcW;

		private CT_DecimalNumber _gridSpan;

		private CT_OnOff _noWrap;

		private CT_OnOff _tcFitText;

		private CT_OnOff _hideMark;

		public CT_TblWidth TcW
		{
			get
			{
				return this._tcW;
			}
			set
			{
				this._tcW = value;
			}
		}

		public CT_DecimalNumber GridSpan
		{
			get
			{
				return this._gridSpan;
			}
			set
			{
				this._gridSpan = value;
			}
		}

		public CT_OnOff NoWrap
		{
			get
			{
				return this._noWrap;
			}
			set
			{
				this._noWrap = value;
			}
		}

		public CT_OnOff TcFitText
		{
			get
			{
				return this._tcFitText;
			}
			set
			{
				this._tcFitText = value;
			}
		}

		public CT_OnOff HideMark
		{
			get
			{
				return this._hideMark;
			}
			set
			{
				this._hideMark = value;
			}
		}

		public static string TcWElementName
		{
			get
			{
				return "tcW";
			}
		}

		public static string GridSpanElementName
		{
			get
			{
				return "gridSpan";
			}
		}

		public static string NoWrapElementName
		{
			get
			{
				return "noWrap";
			}
		}

		public static string TcFitTextElementName
		{
			get
			{
				return "tcFitText";
			}
		}

		public static string HideMarkElementName
		{
			get
			{
				return "hideMark";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_tcW(s);
			this.Write_gridSpan(s);
			this.Write_noWrap(s);
			this.Write_tcFitText(s);
			this.Write_hideMark(s);
		}

		public void Write_tcW(TextWriter s)
		{
			if (this._tcW != null)
			{
				this._tcW.Write(s, "tcW");
			}
		}

		public void Write_gridSpan(TextWriter s)
		{
			if (this._gridSpan != null)
			{
				this._gridSpan.Write(s, "gridSpan");
			}
		}

		public void Write_noWrap(TextWriter s)
		{
			if (this._noWrap != null)
			{
				this._noWrap.Write(s, "noWrap");
			}
		}

		public void Write_tcFitText(TextWriter s)
		{
			if (this._tcFitText != null)
			{
				this._tcFitText.Write(s, "tcFitText");
			}
		}

		public void Write_hideMark(TextWriter s)
		{
			if (this._hideMark != null)
			{
				this._hideMark.Write(s, "hideMark");
			}
		}
	}
}
