using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLFontModel : IFontModel, ICloneable, IDeepCloneable<XMLFontModel>
	{
		private readonly CT_Font _font;

		private readonly Font _interface;

		private readonly XMLPaletteModel _palette;

		private bool _hasBeenModified;

		public bool HasBeenModified
		{
			get
			{
				return this._hasBeenModified;
			}
		}

		public Font Interface
		{
			get
			{
				return this._interface;
			}
		}

		public CT_Font Data
		{
			get
			{
				return this._font;
			}
		}

		public bool Bold
		{
			get
			{
				if (this._font.B == null)
				{
					this._font.B = new CT_BooleanProperty();
					this._font.B.Val_Attr = false;
				}
				return this._font.B.Val_Attr;
			}
			set
			{
				if (this._font.B == null)
				{
					this._font.B = new CT_BooleanProperty();
				}
				this._font.B.Val_Attr = value;
				this._hasBeenModified = true;
			}
		}

		public bool Italic
		{
			get
			{
				if (this._font.I == null)
				{
					this._font.I = new CT_BooleanProperty();
					this._font.I.Val_Attr = false;
				}
				return this._font.I.Val_Attr;
			}
			set
			{
				if (this._font.I == null)
				{
					this._font.I = new CT_BooleanProperty();
				}
				this._font.I.Val_Attr = value;
				this._hasBeenModified = true;
			}
		}

		public bool Strikethrough
		{
			get
			{
				if (this._font.Strike == null)
				{
					this._font.Strike = new CT_BooleanProperty();
					this._font.Strike.Val_Attr = false;
				}
				return this._font.Strike.Val_Attr;
			}
			set
			{
				if (this._font.Strike == null)
				{
					this._font.Strike = new CT_BooleanProperty();
				}
				this._font.Strike.Val_Attr = value;
				this._hasBeenModified = true;
			}
		}

		public string Name
		{
			get
			{
				if (this._font.Name == null)
				{
					this._font.Name = new CT_FontName();
					this._font.Name.Val_Attr = "Calibri";
				}
				return this._font.Name.Val_Attr;
			}
			set
			{
				if (this._font.Name == null)
				{
					this._font.Name = new CT_FontName();
				}
				this._font.Name.Val_Attr = ExcelGeneratorStringUtil.Truncate(value, 31);
				this._hasBeenModified = true;
			}
		}

		public double Size
		{
			get
			{
				if (this._font.Sz == null)
				{
					this._font.Sz = new CT_FontSize();
					this._font.Sz.Val_Attr = 11.0;
				}
				return this._font.Sz.Val_Attr;
			}
			set
			{
				if (!(value < 1.0) && !(value > 409.55))
				{
					if (this._font.Sz == null)
					{
						this._font.Sz = new CT_FontSize();
					}
					this._font.Sz.Val_Attr = value;
					this._hasBeenModified = true;
					return;
				}
				throw new FatalException();
			}
		}

		public ST_UnderlineValues Underline
		{
			get
			{
				if (this._font.U == null)
				{
					return null;
				}
				return this._font.U.Val_Attr;
			}
			set
			{
				if (value == (ST_UnderlineValues)null)
				{
					this._font.U = null;
				}
				else
				{
					if (this._font.U == null)
					{
						this._font.U = new CT_UnderlineProperty();
					}
					this._font.U.Val_Attr = value;
				}
				this._hasBeenModified = true;
			}
		}

		public ST_VerticalAlignRun ScriptStyle
		{
			get
			{
				if (this._font.VertAlign == null)
				{
					return null;
				}
				return this._font.VertAlign.Val_Attr;
			}
			set
			{
				if (value == (ST_VerticalAlignRun)null)
				{
					this._font.VertAlign = null;
				}
				else
				{
					if (this._font.VertAlign == null)
					{
						this._font.VertAlign = new CT_VerticalAlignFontProperty();
					}
					this._font.VertAlign.Val_Attr = value;
				}
				this._hasBeenModified = true;
			}
		}

		public ColorModel Color
		{
			get
			{
				if (this._font.Color == null)
				{
					this._font.Color = new CT_Color();
					this._font.Color.Theme_Attr = 1u;
				}
				return this._palette.GetColorFromCT(this._font.Color);
			}
			set
			{
				this._font.Color = ((XMLColorModel)value).Data;
				this._hasBeenModified = true;
			}
		}

		public XMLFontModel(CT_Font font, XMLPaletteModel palette)
		{
			this._font = font;
			this._interface = new Font(this);
			this._palette = palette;
		}

		public XMLFontModel(XMLPaletteModel palette)
			: this(new CT_Font(), palette)
		{
		}

		public void SetFont(IFontModel font)
		{
			this.copy((XMLFontModel)font);
		}

		public void copy(XMLFontModel srcFont)
		{
			CT_Font font = srcFont._font;
			this._hasBeenModified = srcFont.HasBeenModified;
			if (font.B != null)
			{
				this.Bold = srcFont.Bold;
			}
			if (font.I != null)
			{
				this.Italic = srcFont.Italic;
			}
			if (font.Strike != null)
			{
				this.Strikethrough = srcFont.Strikethrough;
			}
			if (font.Name != null)
			{
				this.Name = srcFont.Name;
			}
			if (font.Sz != null)
			{
				this.Size = srcFont.Size;
			}
			if (font.U != null)
			{
				this.Underline = srcFont.Underline;
			}
			if (font.VertAlign != null)
			{
				this.ScriptStyle = srcFont.ScriptStyle;
			}
			if (font.Family != null)
			{
				this._font.Family = font.Family;
			}
			if (font.Scheme != null)
			{
				this._font.Scheme = font.Scheme;
			}
			if (font.Color != null)
			{
				this.Color = ((XMLColorModel)srcFont.Color).Clone();
			}
		}

		public object Clone()
		{
			return this.DeepClone();
		}

		public XMLFontModel DeepClone()
		{
			XMLFontModel xMLFontModel = new XMLFontModel(this._palette);
			xMLFontModel.copy(this);
			return xMLFontModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLFontModel))
			{
				return false;
			}
			XMLFontModel xMLFontModel = (XMLFontModel)o;
			if (this._hasBeenModified != xMLFontModel._hasBeenModified)
			{
				return false;
			}
			if (this._font.B == null && xMLFontModel._font.B == null)
			{
				goto IL_004c;
			}
			if (this.Bold == xMLFontModel.Bold)
			{
				goto IL_004c;
			}
			goto IL_00cc;
			IL_016f:
			int num = ((this._font.Color == null && xMLFontModel._font.Color == null) || this.Color.Equals(xMLFontModel.Color)) ? 1 : 0;
			goto IL_01a0;
			IL_0074:
			if (this._font.Strike == null && xMLFontModel._font.Strike == null)
			{
				goto IL_009c;
			}
			if (this.Strikethrough == xMLFontModel.Strikethrough)
			{
				goto IL_009c;
			}
			goto IL_00cc;
			IL_009c:
			int num2 = ((this._font.U == null && xMLFontModel._font.U == null) || this.Underline == xMLFontModel.Underline) ? 1 : 0;
			goto IL_00cd;
			IL_00cd:
			bool flag = (byte)num2 != 0;
			if (this._font.Family == xMLFontModel._font.Family)
			{
				if (this._font.Name == null && xMLFontModel._font.Name == null)
				{
					goto IL_0113;
				}
				if (this.Name == xMLFontModel.Name)
				{
					goto IL_0113;
				}
			}
			int num3 = 0;
			goto IL_0141;
			IL_0113:
			num3 = (((this._font.Sz == null && xMLFontModel._font.Sz == null) || this.Size == xMLFontModel.Size) ? 1 : 0);
			goto IL_0141;
			IL_0141:
			bool flag2 = (byte)num3 != 0;
			if (this._font.VertAlign == null && xMLFontModel._font.VertAlign == null)
			{
				goto IL_016f;
			}
			if (this.ScriptStyle == xMLFontModel.ScriptStyle)
			{
				goto IL_016f;
			}
			num = 0;
			goto IL_01a0;
			IL_004c:
			if (this._font.I == null && xMLFontModel._font.I == null)
			{
				goto IL_0074;
			}
			if (this.Italic == xMLFontModel.Italic)
			{
				goto IL_0074;
			}
			goto IL_00cc;
			IL_00cc:
			num2 = 0;
			goto IL_00cd;
			IL_01a0:
			bool flag3 = (byte)num != 0;
			return flag & flag2 & flag3;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this._font.B != null && (bool)this._font.B.Val_Attr)
			{
				num |= 0x10;
			}
			if (this._font.I != null && (bool)this._font.I.Val_Attr)
			{
				num |= 0x20;
			}
			if (this._font.Strike != null && (bool)this._font.Strike.Val_Attr)
			{
				num |= 0x40;
			}
			if (this._font.Family != null)
			{
				num ^= this._font.Family.Val_Attr.GetHashCode();
			}
			num ^= ((this._font.Name != null) ? this._font.Name.Val_Attr.GetHashCode() : "Calibri".GetHashCode());
			num ^= ((this._font.Sz != null) ? this._font.Sz.Val_Attr.GetHashCode() : 11.0.GetHashCode());
			num ^= ((this._font.U != null) ? this._font.U.Val_Attr.GetHashCode() : XMLConstants.Font.Default.Underline.GetHashCode());
			num ^= ((this._font.VertAlign != null) ? this._font.VertAlign.GetHashCode() : XMLConstants.Font.Default.VerticalAlignment.GetHashCode());
			CT_Color cT_Color = new CT_Color();
			cT_Color.Theme_Attr = 1u;
			return num ^ ((this._font.Color != null) ? this._palette.GetColorFromCT(this._font.Color).GetHashCode() : this._palette.GetColorFromCT(cT_Color).GetHashCode());
		}
	}
}
