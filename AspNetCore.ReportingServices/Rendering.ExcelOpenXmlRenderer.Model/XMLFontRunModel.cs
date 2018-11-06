using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.sharedTypes;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal class XMLFontRunModel : IFontModel, ICloneable
	{
		private readonly Font _interface;

		private readonly List<CT_RPrElt> _props;

		private readonly XMLPaletteModel _palette;

		public Font Interface
		{
			get
			{
				return this._interface;
			}
		}

		public bool Bold
		{
			set
			{
				if (this._props[0].B == null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.B = new CT_BooleanProperty();
						prop.B.Val_Attr = value;
					}
				}
				else
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.B.Val_Attr = value;
					}
				}
			}
		}

		public bool Italic
		{
			set
			{
				if (this._props[0].I == null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.I = new CT_BooleanProperty();
						prop.I.Val_Attr = value;
					}
				}
				else
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.I.Val_Attr = value;
					}
				}
			}
		}

		public bool Strikethrough
		{
			set
			{
				if (this._props[0].Strike == null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.Strike = new CT_BooleanProperty();
						prop.Strike.Val_Attr = value;
					}
				}
				else
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.Strike.Val_Attr = value;
					}
				}
			}
		}

		public string Name
		{
			set
			{
				if (value == null)
				{
					throw new FatalException();
				}
				string val_Attr = ExcelGeneratorStringUtil.Truncate(value, 31);
				if (this._props[0].RFont == null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.RFont = new CT_FontName();
						prop.RFont.Val_Attr = val_Attr;
					}
				}
				else
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.RFont.Val_Attr = val_Attr;
					}
				}
			}
		}

		public double Size
		{
			set
			{
				if (!(value < 1.0) && !(value > 409.55))
				{
					if (this._props[0].Sz == null)
					{
						foreach (CT_RPrElt prop in this._props)
						{
							prop.Sz = new CT_FontSize();
							prop.Sz.Val_Attr = value;
						}
					}
					return;
				}
				throw new FatalException();
			}
		}

		public ST_UnderlineValues Underline
		{
			set
			{
				if (value == (ST_UnderlineValues)null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.U = null;
					}
				}
				else if (this._props[0].U == null)
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.U = new CT_UnderlineProperty();
						prop2.U.Val_Attr = value;
					}
				}
				else
				{
					foreach (CT_RPrElt prop3 in this._props)
					{
						prop3.U.Val_Attr = value;
					}
				}
			}
		}

		public ST_VerticalAlignRun ScriptStyle
		{
			set
			{
				if (value == (ST_VerticalAlignRun)null)
				{
					foreach (CT_RPrElt prop in this._props)
					{
						prop.VertAlign = null;
					}
				}
				else if (this._props[0].VertAlign == null)
				{
					foreach (CT_RPrElt prop2 in this._props)
					{
						prop2.VertAlign = new CT_VerticalAlignFontProperty();
						prop2.VertAlign.Val_Attr = value;
					}
				}
				else
				{
					foreach (CT_RPrElt prop3 in this._props)
					{
						prop3.VertAlign.Val_Attr = value;
					}
				}
			}
		}

		public int Charset
		{
			set
			{
				if (value >= 0 && value <= 255)
				{
					if (this._props[0].Charset == null)
					{
						foreach (CT_RPrElt prop in this._props)
						{
							prop.Charset = new CT_IntProperty();
							prop.Charset.Val_Attr = value;
						}
					}
					else
					{
						foreach (CT_RPrElt prop2 in this._props)
						{
							prop2.Charset.Val_Attr = value;
						}
					}
					return;
				}
				throw new FatalException();
			}
		}

		public ColorModel Color
		{
			set
			{
				if (value == null)
				{
					throw new FatalException();
				}
				if (!(value is XMLColorModel))
				{
					throw new FatalException();
				}
				foreach (CT_RPrElt prop in this._props)
				{
					prop.Color = ((XMLColorModel)value).Clone().Data;
				}
			}
		}

		public XMLFontRunModel(XMLPaletteModel palette)
		{
			this._interface = new Font(this);
			this._props = new List<CT_RPrElt>();
			this._palette = palette;
		}

		public object Clone()
		{
			throw new FatalException();
		}

		public void Add(CT_RPrElt prop)
		{
			this._props.Add(prop);
		}

		public void SetFont(IFontModel font)
		{
			this.SetFont((XMLFontModel)font);
		}

		public void SetFont(XMLFontModel font)
		{
			XMLFontModel xMLFontModel = new XMLFontModel(this._palette);
			xMLFontModel.copy(font);
			this.Bold = xMLFontModel.Bold;
			this.Italic = xMLFontModel.Italic;
			this.Strikethrough = xMLFontModel.Strikethrough;
			this.Name = xMLFontModel.Name;
			this.Size = xMLFontModel.Size;
			this.Underline = xMLFontModel.Underline;
			this.ScriptStyle = xMLFontModel.ScriptStyle;
			this.Color = xMLFontModel.Color;
		}
	}
}
