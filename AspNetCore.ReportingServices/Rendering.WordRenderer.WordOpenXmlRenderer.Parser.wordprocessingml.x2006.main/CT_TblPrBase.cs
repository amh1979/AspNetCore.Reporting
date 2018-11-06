using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _tblStyle;

		private CT_OnOff _bidiVisual;

		private CT_DecimalNumber _tblStyleRowBandSize;

		private CT_DecimalNumber _tblStyleColBandSize;

		private CT_TblWidth _tblW;

		private CT_TblWidth _tblCellSpacing;

		private CT_TblWidth _tblInd;

		private CT_TblCellMar _tblCellMar;

		private CT_String _tblCaption;

		private CT_String _tblDescription;

		public CT_String TblStyle
		{
			get
			{
				return this._tblStyle;
			}
			set
			{
				this._tblStyle = value;
			}
		}

		public CT_OnOff BidiVisual
		{
			get
			{
				return this._bidiVisual;
			}
			set
			{
				this._bidiVisual = value;
			}
		}

		public CT_DecimalNumber TblStyleRowBandSize
		{
			get
			{
				return this._tblStyleRowBandSize;
			}
			set
			{
				this._tblStyleRowBandSize = value;
			}
		}

		public CT_DecimalNumber TblStyleColBandSize
		{
			get
			{
				return this._tblStyleColBandSize;
			}
			set
			{
				this._tblStyleColBandSize = value;
			}
		}

		public CT_TblWidth TblW
		{
			get
			{
				return this._tblW;
			}
			set
			{
				this._tblW = value;
			}
		}

		public CT_TblWidth TblCellSpacing
		{
			get
			{
				return this._tblCellSpacing;
			}
			set
			{
				this._tblCellSpacing = value;
			}
		}

		public CT_TblWidth TblInd
		{
			get
			{
				return this._tblInd;
			}
			set
			{
				this._tblInd = value;
			}
		}

		public CT_TblCellMar TblCellMar
		{
			get
			{
				return this._tblCellMar;
			}
			set
			{
				this._tblCellMar = value;
			}
		}

		public CT_String TblCaption
		{
			get
			{
				return this._tblCaption;
			}
			set
			{
				this._tblCaption = value;
			}
		}

		public CT_String TblDescription
		{
			get
			{
				return this._tblDescription;
			}
			set
			{
				this._tblDescription = value;
			}
		}

		public static string TblStyleElementName
		{
			get
			{
				return "tblStyle";
			}
		}

		public static string BidiVisualElementName
		{
			get
			{
				return "bidiVisual";
			}
		}

		public static string TblStyleRowBandSizeElementName
		{
			get
			{
				return "tblStyleRowBandSize";
			}
		}

		public static string TblStyleColBandSizeElementName
		{
			get
			{
				return "tblStyleColBandSize";
			}
		}

		public static string TblWElementName
		{
			get
			{
				return "tblW";
			}
		}

		public static string TblCellSpacingElementName
		{
			get
			{
				return "tblCellSpacing";
			}
		}

		public static string TblIndElementName
		{
			get
			{
				return "tblInd";
			}
		}

		public static string TblCellMarElementName
		{
			get
			{
				return "tblCellMar";
			}
		}

		public static string TblCaptionElementName
		{
			get
			{
				return "tblCaption";
			}
		}

		public static string TblDescriptionElementName
		{
			get
			{
				return "tblDescription";
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
			this.Write_tblStyle(s);
			this.Write_bidiVisual(s);
			this.Write_tblStyleRowBandSize(s);
			this.Write_tblStyleColBandSize(s);
			this.Write_tblW(s);
			this.Write_tblCellSpacing(s);
			this.Write_tblInd(s);
			this.Write_tblCellMar(s);
			this.Write_tblCaption(s);
			this.Write_tblDescription(s);
		}

		public void Write_tblStyle(TextWriter s)
		{
			if (this._tblStyle != null)
			{
				this._tblStyle.Write(s, "tblStyle");
			}
		}

		public void Write_bidiVisual(TextWriter s)
		{
			if (this._bidiVisual != null)
			{
				this._bidiVisual.Write(s, "bidiVisual");
			}
		}

		public void Write_tblStyleRowBandSize(TextWriter s)
		{
			if (this._tblStyleRowBandSize != null)
			{
				this._tblStyleRowBandSize.Write(s, "tblStyleRowBandSize");
			}
		}

		public void Write_tblStyleColBandSize(TextWriter s)
		{
			if (this._tblStyleColBandSize != null)
			{
				this._tblStyleColBandSize.Write(s, "tblStyleColBandSize");
			}
		}

		public void Write_tblW(TextWriter s)
		{
			if (this._tblW != null)
			{
				this._tblW.Write(s, "tblW");
			}
		}

		public void Write_tblCellSpacing(TextWriter s)
		{
			if (this._tblCellSpacing != null)
			{
				this._tblCellSpacing.Write(s, "tblCellSpacing");
			}
		}

		public void Write_tblInd(TextWriter s)
		{
			if (this._tblInd != null)
			{
				this._tblInd.Write(s, "tblInd");
			}
		}

		public void Write_tblCellMar(TextWriter s)
		{
			if (this._tblCellMar != null)
			{
				this._tblCellMar.Write(s, "tblCellMar");
			}
		}

		public void Write_tblCaption(TextWriter s)
		{
			if (this._tblCaption != null)
			{
				this._tblCaption.Write(s, "tblCaption");
			}
		}

		public void Write_tblDescription(TextWriter s)
		{
			if (this._tblDescription != null)
			{
				this._tblDescription.Write(s, "tblDescription");
			}
		}
	}
}
