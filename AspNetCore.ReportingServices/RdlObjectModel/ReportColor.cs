using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[TypeConverter(typeof(ReportColorConverter))]
	internal struct ReportColor : IXmlSerializable, IFormattable, IShouldSerialize
	{
		private Color m_color;

		private static readonly ReportColor m_empty = default(ReportColor);

		public static ReportColor Empty
		{
			get
			{
				return ReportColor.m_empty;
			}
		}

		public Color Color
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return Color.Empty == this.m_color;
			}
		}

		public ReportColor(Color color)
		{
			this.m_color = color;
		}

		public ReportColor(string value)
		{
			this.m_color = Color.Empty;
			this.Init(value);
		}

		private void Init(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.Color = ReportColor.RdlStringToColor(value);
			}
		}

		internal static Color RdlStringToColor(string value)
		{
			if (value[0] == '#')
			{
				return ReportColor.RgbStringToColor(value);
			}
			Color result = ReportColor.FromName(value);
            /*
            if (!result.IsKnownColor)
			{
				throw new ArgumentException(SRErrors.InvalidColor(value));
			}
            */
 
            return result;
		}

		private static Color RgbStringToColor(string value)
		{
			byte alpha = 255;
			if (value == "#00ffffff")
			{
				return Color.Transparent;
			}
			if (value == "#00000000")
			{
				return Color.Empty;
			}
			bool flag = true;
			if (value.Length != 7 && value.Length != 9)
			{
				goto IL_004d;
			}
			if (value[0] != '#')
			{
				goto IL_004d;
			}
			string text = "abcdefABCDEF";
			int num = 1;
			while (num < value.Length)
			{
				if (char.IsDigit(value[num]) || -1 != text.IndexOf(value[num]))
				{
					num++;
					continue;
				}
				flag = false;
				break;
			}
			goto IL_0094;
			IL_0094:
			if (flag)
			{
				int num2 = 1;
				if (value.Length == 9)
				{
					alpha = Convert.ToByte(value.Substring(num2, 2), 16);
					num2 += 2;
				}
				byte red = Convert.ToByte(value.Substring(num2, 2), 16);
				byte green = Convert.ToByte(value.Substring(num2 + 2, 2), 16);
				byte blue = Convert.ToByte(value.Substring(num2 + 4, 2), 16);
				return Color.FromArgb(alpha, red, green, blue);
			}
			throw new ArgumentException(SRErrors.InvalidColor(value));
			IL_004d:
			flag = false;
			goto IL_0094;
		}

		public static string ColorToRdlString(Color c)
		{
			if (c.IsEmpty)
			{
				return "";
			}
			if (c == Color.Transparent)
			{
				return "#00ffffff";
			}
            /*
            if (c.IsNamedColor && !c.IsSystemColor)
            {
                return ReportColor.ToName(c);
            }
            */
            if (c.IsNamedColor)
            {
                return ReportColor.ToName(c);
            }
            if (c.A == 255)
			{
				return StringUtil.FormatInvariant("#{0:x6}", c.ToArgb() & 0xFFFFFF);
			}
			return StringUtil.FormatInvariant("#{0:x8}", c.ToArgb());
		}

		public static ReportColor Parse(string s, IFormatProvider provider)
		{
			return new ReportColor(s);
		}

		public void SetEmpty()
		{
			this.m_color = Color.Empty;
		}

		internal static Color FromName(string name)
		{
			if (string.Equals(name, "LightGrey", StringComparison.OrdinalIgnoreCase))
			{
				name = "LightGray";
			}
			return Color.FromName(name);
		}

		internal static string ToName(Color color)
		{
			string text = color.Name;
			if (text == "LightGray")
			{
				text = "LightGrey";
			}
			return text;
		}

		public override string ToString()
		{
			return ReportColor.ColorToRdlString(this.Color);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.ToString();
		}

		public override int GetHashCode()
		{
			return this.m_color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is ReportColor)
			{
				return this == (ReportColor)obj;
			}
			return false;
		}

		public static bool operator ==(ReportColor left, ReportColor right)
		{
			return left.Color == right.Color;
		}

		public static bool operator !=(ReportColor left, ReportColor right)
		{
			return left.Color != right.Color;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string text = reader.ReadString();
			this.Init(text.Trim());
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			string text = this.ToString();
			writer.WriteString(text);
		}

		bool IShouldSerialize.ShouldSerializeThis()
		{
			return !this.IsEmpty;
		}

		SerializationMethod IShouldSerialize.ShouldSerializeProperty(string name)
		{
			return SerializationMethod.Auto;
		}
	}
}
