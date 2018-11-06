using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStyleModel : IStyleModel, ICloneable, IDeepCloneable<XMLStyleModel>
	{
		private Style _interface;

		private GlobalStyle _globalInterface;

		private readonly StyleManager _manager;

		private readonly CT_Xf _xf;

		private bool _hasBeenModified;

		private uint _index;

		private string _numberformat;

		private XMLFontModel _font;

		private XMLFillModel _fill;

		private XMLBorderModel _border;

		public virtual Style Interface
		{
			get
			{
				if (this._interface == null)
				{
					this._interface = new Style(this);
				}
				return this._interface;
			}
		}

		public GlobalStyle GlobalInterface
		{
			get
			{
				if (this._globalInterface == null)
				{
					this._globalInterface = new GlobalStyle(this);
				}
				return this._globalInterface;
			}
		}

		public CT_Xf Data
		{
			get
			{
				return this._xf;
			}
		}

		public XMLFontModel Font
		{
			get
			{
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public string NumberFormat
		{
			get
			{
				return this._numberformat;
			}
			set
			{
				this._xf.ApplyNumberFormat_Attr = (value == null);
				this._numberformat = value;
				this._hasBeenModified = true;
			}
		}

		public IBorderModel BorderModel
		{
			get
			{
				return this._border;
			}
		}

		private XMLFillModel Fill
		{
			get
			{
				return this._fill;
			}
			set
			{
				this._fill = value;
			}
		}

		public ColorModel BackgroundColor
		{
			get
			{
				if (this.Fill != null)
				{
					return this.Fill.Color;
				}
				return null;
			}
			set
			{
				this.Fill.Color = (XMLColorModel)value;
				this._hasBeenModified = true;
			}
		}

		private CT_CellAlignment Alignment
		{
			get
			{
				if (this._xf.Alignment == null)
				{
					this._xf.Alignment = new CT_CellAlignment();
				}
				this._xf.ApplyAlignment_Attr = true;
				return this._xf.Alignment;
			}
		}

		public ST_VerticalAlignment VerticalAlignment
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.Vertical_Attr_Is_Specified)
				{
					return this.Alignment.Vertical_Attr;
				}
				return null;
			}
			set
			{
				if (value == (ST_VerticalAlignment)null)
				{
					this.Alignment.Vertical_Attr_Is_Specified = false;
				}
				else
				{
					this.Alignment.Vertical_Attr = value;
				}
				this._hasBeenModified = true;
			}
		}

		public ST_HorizontalAlignment HorizontalAlignment
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.Horizontal_Attr_Is_Specified)
				{
					return this.Alignment.Horizontal_Attr;
				}
				return null;
			}
			set
			{
				if (value == (ST_HorizontalAlignment)null)
				{
					this.Alignment.Horizontal_Attr_Is_Specified = false;
				}
				else
				{
					this.Alignment.Horizontal_Attr = value;
				}
				this._hasBeenModified = true;
			}
		}

		public uint? TextDirection
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					return this.Alignment.ReadingOrder_Attr;
				}
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					this.Alignment.ReadingOrder_Attr_Is_Specified = false;
				}
				else
				{
					this.Alignment.ReadingOrder_Attr = value.Value;
				}
				this._hasBeenModified = true;
			}
		}

		public bool? WrapText
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.WrapText_Attr_Is_Specified)
				{
					return this.Alignment.WrapText_Attr;
				}
				return null;
			}
			set
			{
				if (!value.HasValue)
				{
					this.Alignment.WrapText_Attr_Is_Specified = false;
				}
				else
				{
					this.Alignment.WrapText_Attr = value.Value;
					this._hasBeenModified = true;
				}
			}
		}

		public int? IndentLevel
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.WrapText_Attr_Is_Specified)
				{
					return (int)this.Alignment.Indent_Attr;
				}
				return null;
			}
			set
			{
				if (!(value < 0) && !(value > 255))
				{
					if (!value.HasValue)
					{
						this.Alignment.Indent_Attr_Is_Specified = false;
					}
					else
					{
						this.Alignment.Indent_Attr = (uint)value.Value;
						this._hasBeenModified = true;
					}
					return;
				}
				throw new FatalException();
			}
		}

		public int? Orientation
		{
			get
			{
				if (this._xf.Alignment != null && this._xf.Alignment.TextRotation_Attr_Is_Specified)
				{
					return (int)this.Alignment.TextRotation_Attr;
				}
				return null;
			}
			set
			{
				if (!(value < 0) && (!(value > 180) || !(value < 254)) && !(value > 255))
				{
					if (!value.HasValue)
					{
						this.Alignment.TextRotation_Attr_Is_Specified = false;
					}
					else
					{
						this.Alignment.TextRotation_Attr = (uint)value.Value;
						this._hasBeenModified = true;
					}
					return;
				}
				throw new FatalException();
			}
		}

		public bool HasBeenModified
		{
			get
			{
				if (!this._hasBeenModified && !this.Font.HasBeenModified)
				{
					return this.BorderModel.HasBeenModified;
				}
				return true;
			}
		}

		public uint Index
		{
			get
			{
				return this._index;
			}
			set
			{
				this._index = value;
			}
		}

		public XMLStyleModel(StyleManager manager)
			: this(new CT_Xf(), manager)
		{
		}

		public XMLStyleModel(CT_Xf xf, StyleManager manager)
			: this(xf, manager, true)
		{
		}

		public XMLStyleModel(CT_Xf xf, StyleManager manager, bool setVerticalAlignment)
		{
			this._xf = xf;
			this._manager = manager;
			this._font = (((bool)this._xf.ApplyFont_Attr) ? this._manager.GetFont(this._xf.FontId_Attr) : new XMLFontModel(this._manager.Palette));
			this._border = (((bool)this._xf.ApplyBorder_Attr) ? this._manager.GetBorder(this._xf.BorderId_Attr) : new XMLBorderModel(this._manager.Palette));
			this._fill = (((bool)this._xf.ApplyFill_Attr) ? this._manager.GetFill(this._xf.FillId_Attr).DeepClone() : new XMLFillModel(this._manager.Palette));
			if (setVerticalAlignment)
			{
				this.Alignment.Vertical_Attr = ST_VerticalAlignment.top;
				this.Alignment.WrapText_Attr = true;
			}
		}

		public IStyleModel cloneStyle(bool cellStyle)
		{
			if (cellStyle)
			{
				return (XMLStyleModel)this.Clone();
			}
			throw new FatalException();
		}

		private void Copy(IStyleModel srcStyle)
		{
			XMLStyleModel xMLStyleModel = (XMLStyleModel)srcStyle;
			this._hasBeenModified = xMLStyleModel._hasBeenModified;
			this._xf.ApplyAlignment_Attr = xMLStyleModel._xf.ApplyAlignment_Attr;
			this._xf.ApplyBorder_Attr = xMLStyleModel._xf.ApplyBorder_Attr;
			this._xf.ApplyFill_Attr = xMLStyleModel._xf.ApplyFill_Attr;
			this._xf.ApplyFont_Attr = xMLStyleModel._xf.ApplyFont_Attr;
			this._xf.ApplyNumberFormat_Attr = xMLStyleModel._xf.ApplyNumberFormat_Attr;
			this._xf.ApplyProtection_Attr = xMLStyleModel._xf.ApplyProtection_Attr;
			this._xf.NumFmtId_Attr = xMLStyleModel._xf.NumFmtId_Attr;
			if (xMLStyleModel._xf.BorderId_Attr_Is_Specified)
			{
				this._xf.BorderId_Attr = xMLStyleModel._xf.BorderId_Attr;
			}
			if (xMLStyleModel._xf.FillId_Attr_Is_Specified)
			{
				this._xf.FillId_Attr = xMLStyleModel._xf.FillId_Attr;
			}
			if (xMLStyleModel._xf.FontId_Attr_Is_Specified)
			{
				this._xf.FontId_Attr = xMLStyleModel._xf.FontId_Attr;
			}
			if (xMLStyleModel._xf.Alignment != null)
			{
				if (xMLStyleModel._xf.Alignment.Vertical_Attr_Is_Specified)
				{
					this.Alignment.Vertical_Attr = xMLStyleModel._xf.Alignment.Vertical_Attr;
				}
				if (xMLStyleModel.Alignment.Horizontal_Attr_Is_Specified)
				{
					this.Alignment.Horizontal_Attr = xMLStyleModel._xf.Alignment.Horizontal_Attr;
				}
				if (xMLStyleModel.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					this.Alignment.ReadingOrder_Attr = xMLStyleModel._xf.Alignment.ReadingOrder_Attr;
				}
				if (xMLStyleModel.Alignment.WrapText_Attr_Is_Specified)
				{
					this.Alignment.WrapText_Attr = xMLStyleModel._xf.Alignment.WrapText_Attr;
				}
				if (xMLStyleModel.Alignment.ShrinkToFit_Attr_Is_Specified)
				{
					this.Alignment.ShrinkToFit_Attr = xMLStyleModel._xf.Alignment.ShrinkToFit_Attr;
				}
				if (xMLStyleModel.Alignment.Indent_Attr_Is_Specified)
				{
					this.Alignment.Indent_Attr = xMLStyleModel._xf.Alignment.Indent_Attr;
				}
				if (xMLStyleModel.Alignment.JustifyLastLine_Attr_Is_Specified)
				{
					this.Alignment.JustifyLastLine_Attr = xMLStyleModel._xf.Alignment.JustifyLastLine_Attr;
				}
				if (xMLStyleModel.Alignment.TextRotation_Attr_Is_Specified)
				{
					this.Alignment.TextRotation_Attr = xMLStyleModel._xf.Alignment.TextRotation_Attr;
				}
			}
			if (xMLStyleModel._numberformat != null)
			{
				this._numberformat = xMLStyleModel._numberformat;
			}
			if (xMLStyleModel._font != null)
			{
				this.Font = (XMLFontModel)xMLStyleModel._font.Clone();
			}
			if (xMLStyleModel._fill != null)
			{
				this.Fill = xMLStyleModel._fill.DeepClone();
			}
			if (xMLStyleModel._border != null)
			{
				this._border = xMLStyleModel._border.DeepClone();
			}
		}

		public XMLStyleModel DeepClone()
		{
			XMLStyleModel xMLStyleModel = new XMLStyleModel(this._manager);
			xMLStyleModel.Copy(this);
			return xMLStyleModel;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is XMLStyleModel))
			{
				return false;
			}
			XMLStyleModel xMLStyleModel = (XMLStyleModel)obj;
			if (this._hasBeenModified != xMLStyleModel._hasBeenModified)
			{
				return false;
			}
			if (this._xf.Alignment != null)
			{
				if (xMLStyleModel._xf.Alignment == null)
				{
					return false;
				}
				if (this._xf.Alignment.Horizontal_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Horizontal_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.Horizontal_Attr != xMLStyleModel._xf.Alignment.Horizontal_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.Indent_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Indent_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.Indent_Attr != xMLStyleModel._xf.Alignment.Indent_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.JustifyLastLine_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.JustifyLastLine_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(this._xf.Alignment.JustifyLastLine_Attr != xMLStyleModel._xf.Alignment.JustifyLastLine_Attr))
					{
						return false;
					}
				}
				if (this._xf.Alignment.ReadingOrder_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.ReadingOrder_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.ReadingOrder_Attr != xMLStyleModel._xf.Alignment.ReadingOrder_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.RelativeIndent_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.RelativeIndent_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.RelativeIndent_Attr != xMLStyleModel._xf.Alignment.RelativeIndent_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.ShrinkToFit_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.ShrinkToFit_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(this._xf.Alignment.ShrinkToFit_Attr != xMLStyleModel._xf.Alignment.ShrinkToFit_Attr))
					{
						return false;
					}
				}
				if (this._xf.Alignment.TextRotation_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.TextRotation_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.TextRotation_Attr != xMLStyleModel._xf.Alignment.TextRotation_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.Vertical_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.Vertical_Attr_Is_Specified)
					{
						return false;
					}
					if (this._xf.Alignment.Vertical_Attr != xMLStyleModel._xf.Alignment.Vertical_Attr)
					{
						return false;
					}
				}
				if (this._xf.Alignment.WrapText_Attr_Is_Specified)
				{
					if (!xMLStyleModel._xf.Alignment.WrapText_Attr_Is_Specified)
					{
						return false;
					}
					if ((bool)(this._xf.Alignment.WrapText_Attr != xMLStyleModel._xf.Alignment.WrapText_Attr))
					{
						return false;
					}
				}
			}
			else if (xMLStyleModel._xf.Alignment != null)
			{
				return false;
			}
			if ((bool)(this._xf.ApplyAlignment_Attr != xMLStyleModel._xf.ApplyAlignment_Attr))
			{
				return false;
			}
			if ((bool)(this._xf.ApplyBorder_Attr != xMLStyleModel._xf.ApplyBorder_Attr))
			{
				return false;
			}
			if ((bool)(this._xf.ApplyFill_Attr != xMLStyleModel._xf.ApplyFill_Attr))
			{
				return false;
			}
			if ((bool)(this._xf.ApplyFont_Attr != xMLStyleModel._xf.ApplyFont_Attr))
			{
				return false;
			}
			if ((bool)(this._xf.ApplyNumberFormat_Attr != xMLStyleModel._xf.ApplyNumberFormat_Attr))
			{
				return false;
			}
			if ((bool)(this._xf.ApplyProtection_Attr != xMLStyleModel._xf.ApplyProtection_Attr))
			{
				return false;
			}
			if (this._xf.XfId_Attr != xMLStyleModel._xf.XfId_Attr)
			{
				return false;
			}
			if ((bool)(this._xf.QuotePrefix_Attr != xMLStyleModel._xf.QuotePrefix_Attr))
			{
				return false;
			}
			if (this._border != null && !this._border.Equals(xMLStyleModel._border))
			{
				return false;
			}
			if (this._border == null && xMLStyleModel._border != null)
			{
				return false;
			}
			if (this._fill != null && !this._fill.Equals(xMLStyleModel._fill))
			{
				return false;
			}
			if (this._fill == null && xMLStyleModel._fill != null)
			{
				return false;
			}
			if (this._font != null && !this._font.Equals(xMLStyleModel._font))
			{
				return false;
			}
			if (this._font == null && xMLStyleModel._font != null)
			{
				return false;
			}
			if (this._numberformat == null && xMLStyleModel._numberformat == null && (bool)this._xf.ApplyNumberFormat_Attr && ((bool)(!xMLStyleModel._xf.ApplyNumberFormat_Attr) || this._xf.NumFmtId_Attr != xMLStyleModel._xf.NumFmtId_Attr))
			{
				return false;
			}
			if (this._numberformat != null && !this._numberformat.Equals(xMLStyleModel._numberformat))
			{
				return false;
			}
			if (this._numberformat == null && xMLStyleModel._numberformat != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			num ^= (((bool)this._xf.ApplyAlignment_Attr) ? 1 : 0);
			num ^= (((bool)this._xf.ApplyBorder_Attr) ? 2 : 0);
			num ^= (((bool)this._xf.ApplyFill_Attr) ? 4 : 0);
			num ^= (((bool)this._xf.ApplyFont_Attr) ? 8 : 0);
			num ^= (((bool)this._xf.ApplyNumberFormat_Attr) ? 16 : 0);
			num ^= (((bool)this._xf.ApplyProtection_Attr) ? 32 : 0);
			if (this._xf.Alignment != null)
			{
				num ^= (this._xf.Alignment.Horizontal_Attr_Is_Specified ? this._xf.Alignment.Horizontal_Attr.GetHashCode() : 0);
				num ^= (this._xf.Alignment.Indent_Attr_Is_Specified ? this._xf.Alignment.Indent_Attr.GetHashCode() : 0);
				num ^= ((this._xf.Alignment.JustifyLastLine_Attr_Is_Specified && (bool)this._xf.Alignment.JustifyLastLine_Attr) ? 256 : 0);
				num ^= (this._xf.Alignment.ReadingOrder_Attr_Is_Specified ? this._xf.Alignment.ReadingOrder_Attr.GetHashCode() : 0);
				num ^= (this._xf.Alignment.RelativeIndent_Attr_Is_Specified ? this._xf.Alignment.RelativeIndent_Attr : 0);
				num ^= ((this._xf.Alignment.ShrinkToFit_Attr_Is_Specified && (bool)this._xf.Alignment.ShrinkToFit_Attr) ? 512 : 0);
				num ^= (this._xf.Alignment.TextRotation_Attr_Is_Specified ? this._xf.Alignment.TextRotation_Attr.GetHashCode() : 0);
				num ^= (this._xf.Alignment.Vertical_Attr_Is_Specified ? this._xf.Alignment.Vertical_Attr.GetHashCode() : 0);
				num ^= ((this._xf.Alignment.WrapText_Attr_Is_Specified && (bool)this._xf.Alignment.WrapText_Attr) ? 1024 : 0);
			}
			if (this._border != null)
			{
				num ^= this._border.GetHashCode();
			}
			if (this._fill != null)
			{
				num ^= this._fill.GetHashCode();
			}
			if (this._font != null)
			{
				num ^= this._font.GetHashCode();
			}
			if (this._numberformat != null)
			{
				num ^= this._numberformat.GetHashCode();
			}
			return num;
		}

		public object Clone()
		{
			return this.DeepClone();
		}

		public void Cleanup()
		{
			if (this._numberformat != null)
			{
				this._xf.ApplyNumberFormat_Attr = true;
				this._xf.NumFmtId_Attr = this._manager.AddNumberFormat(this._numberformat);
			}
			if (this._font != null)
			{
				this._xf.ApplyFont_Attr = true;
				this._xf.FontId_Attr = this._manager.AddFont(this._font);
			}
			if (this._fill != null)
			{
				this._xf.ApplyFill_Attr = true;
				this._xf.FillId_Attr = this._manager.AddFill(this._fill);
			}
			if (this._border != null)
			{
				this._xf.ApplyBorder_Attr = true;
				this._xf.BorderId_Attr = this._manager.AddBorder(this._border);
			}
		}
	}
}
