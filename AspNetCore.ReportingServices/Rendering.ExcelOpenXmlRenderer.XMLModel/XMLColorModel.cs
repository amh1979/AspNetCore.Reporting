using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLColorModel : ColorModel
	{
		private const string Default = "FF000000";

		private readonly CT_Color _color;

		private readonly XMLPaletteModel _palette;

		public CT_Color Data
		{
			get
			{
				return this._color;
			}
		}

		public int Red
		{
			get
			{
				return this.GetValueAtByte(1);
			}
			set
			{
				this.SetValueAtByte(1, (byte)value);
			}
		}

		public int Green
		{
			get
			{
				return this.GetValueAtByte(2);
			}
			set
			{
				this.SetValueAtByte(2, (byte)value);
			}
		}

		public int Blue
		{
			get
			{
				return this.GetValueAtByte(3);
			}
			set
			{
				this.SetValueAtByte(3, (byte)value);
			}
		}

		public XMLColorModel(CT_Color color, XMLPaletteModel palette)
		{
			this._palette = palette;
			if (color == null)
			{
				this._color = new CT_Color();
				this._color.Rgb_Attr = "FF000000";
			}
			else
			{
				this._color = color;
			}
		}

		public XMLColorModel(int red, int green, int blue)
		{
			this._color = new CT_Color();
			this._color.Rgb_Attr = "FF000000";
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
		}

		public XMLColorModel(string argb)
		{
			this._color = new CT_Color();
			this._color.Rgb_Attr = argb;
		}

		private int GetValueAtByte(int index)
		{
			return int.Parse(this._color.Rgb_Attr.Substring(2 * index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}

		private void SetValueAtByte(int index, byte value)
		{
			this._color.Rgb_Attr = this._color.Rgb_Attr.Substring(0, index * 2) + value.ToString("X", CultureInfo.InvariantCulture).PadLeft(2, '0') + this._color.Rgb_Attr.Substring(index * 2 + 2);
		}

		public override int getRed()
		{
			return this.Red;
		}

		public override int getBlue()
		{
			return this.Blue;
		}

		public override int getGreen()
		{
			return this.Green;
		}

		public XMLColorModel Clone()
		{
			CT_Color cT_Color = new CT_Color();
			if (this._color.Rgb_Attr != null)
			{
				cT_Color.Rgb_Attr = this._color.Rgb_Attr;
			}
			if (this._color.Indexed_Attr_Is_Specified)
			{
				cT_Color.Indexed_Attr = this._color.Indexed_Attr;
			}
			if (this._color.Theme_Attr_Is_Specified)
			{
				cT_Color.Theme_Attr = this._color.Theme_Attr;
			}
			cT_Color.Tint_Attr = this._color.Tint_Attr;
			cT_Color.Auto_Attr = this._color.Auto_Attr;
			return new XMLColorModel(cT_Color, this._palette);
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLColorModel))
			{
				return false;
			}
			XMLColorModel xMLColorModel = (XMLColorModel)o;
			if (this._color.Rgb_Attr != null && xMLColorModel._color.Rgb_Attr != null)
			{
				return this._color.Rgb_Attr == xMLColorModel._color.Rgb_Attr;
			}
			if (this._color.Rgb_Attr == null && xMLColorModel._color.Rgb_Attr == null)
			{
				if (this._color.Indexed_Attr_Is_Specified ^ xMLColorModel._color.Indexed_Attr_Is_Specified)
				{
					return false;
				}
				if (this._color.Indexed_Attr_Is_Specified && this._color.Indexed_Attr != xMLColorModel._color.Indexed_Attr)
				{
					return false;
				}
				if (this._color.Theme_Attr_Is_Specified ^ xMLColorModel._color.Theme_Attr_Is_Specified)
				{
					return false;
				}
				if (this._color.Theme_Attr_Is_Specified && this._color.Theme_Attr != xMLColorModel._color.Theme_Attr)
				{
					return false;
				}
				if (this._color.Tint_Attr == xMLColorModel._color.Tint_Attr)
				{
					return this._color.Auto_Attr == xMLColorModel._color.Auto_Attr;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this._color.Rgb_Attr != null)
			{
				return this._color.Rgb_Attr.GetHashCode();
			}
			int hashCode = this._color.Auto_Attr.GetHashCode();
			hashCode = (hashCode * 397 ^ this._color.Tint_Attr.GetHashCode());
			hashCode = (hashCode * 397 ^ this._color.Indexed_Attr.GetHashCode());
			return hashCode * 397 ^ this._color.Theme_Attr.GetHashCode();
		}
	}
}
