using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[TypeConverter(typeof(ReportSizeConverter))]
	internal struct ReportSize : IComparable, IXmlSerializable, IFormattable, IShouldSerialize
	{
		internal const double CentimetersPerInch = 2.54;

		internal const double MillimetersPerInch = 25.4;

		internal const double PicasPerInch = 6.0;

		internal const double PointsPerInch = 72.0;

		internal const int DefaultDecimalDigits = 5;

		private static float m_dotsPerInch;

		private static readonly ReportSize m_empty;

		private static string m_serializationFormat;

		private static int m_serializedDecimalDigits;

		private static SizeTypes m_defaultType;

		private SizeTypes m_type;

		private double m_value;

		public static SizeTypes DefaultType
		{
			get
			{
				return ReportSize.m_defaultType;
			}
			set
			{
				ReportSize.m_defaultType = value;
			}
		}

		public static int SerializedDecimalDigits
		{
			get
			{
				return ReportSize.m_serializedDecimalDigits;
			}
			set
			{
				if (value > 0 && value <= 99)
				{
					ReportSize.m_serializedDecimalDigits = value;
					ReportSize.m_serializationFormat = "{0:0." + new string('#', value) + "}{1}";
					return;
				}
				throw new ArgumentException("SerializedDecimalDigits");
			}
		}

		public static float DotsPerInch
		{
			get
			{
				if (ReportSize.m_dotsPerInch == 0.0)
				{
					using (Bitmap image = new Bitmap(1, 1))
					{
						using (Graphics graphics = Graphics.FromImage(image))
						{
							ReportSize.m_dotsPerInch = graphics.DpiX;
						}
					}
				}
				return ReportSize.m_dotsPerInch;
			}
		}

		public static ReportSize Empty
		{
			get
			{
				return ReportSize.m_empty;
			}
		}

		public SizeTypes Type
		{
			get
			{
				if (this.m_type == SizeTypes.Invalid)
				{
					return ReportSize.DefaultType;
				}
				return this.m_type;
			}
		}

		public double Value
		{
			get
			{
				return this.m_value;
			}
		}

		public double SerializedValue
		{
			get
			{
				return Math.Round(this.m_value, ReportSize.m_serializedDecimalDigits);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_type == SizeTypes.Invalid;
			}
		}

		static ReportSize()
		{
			ReportSize.m_empty = default(ReportSize);
			ReportSize.m_defaultType = SizeTypes.Inch;
			ReportSize.SerializedDecimalDigits = 5;
		}

		public ReportSize(double value, SizeTypes type)
		{
			this.m_value = value;
			this.m_type = type;
		}

		public ReportSize(string value)
		{
			this = new ReportSize(value, CultureInfo.CurrentCulture);
		}

		public ReportSize(double value)
		{
			this.m_value = value;
			this.m_type = ReportSize.DefaultType;
		}

		public ReportSize(string value, IFormatProvider provider)
		{
			this = new ReportSize(value, provider, ReportSize.DefaultType);
		}

		public ReportSize(string value, IFormatProvider provider, SizeTypes defaultType)
		{
			this.m_value = 0.0;
			this.m_type = SizeTypes.Invalid;
			if (!string.IsNullOrEmpty(value))
			{
				this.Init(value, provider, defaultType);
			}
		}

		private void Init(string value, IFormatProvider provider, SizeTypes defaultType)
		{
			if (provider == null)
			{
				provider = CultureInfo.CurrentCulture;
			}
			string text = value.Trim();
			int length = text.Length;
			NumberFormatInfo numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
			if (numberFormatInfo == null)
			{
				numberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
			}
			int num = -1;
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (!char.IsDigit(c) && c != numberFormatInfo.NegativeSign[0] && c != numberFormatInfo.NumberDecimalSeparator[0] && c != numberFormatInfo.NumberGroupSeparator[0])
				{
					break;
				}
				num = i;
			}
			if (num == -1)
			{
				throw new FormatException(SRErrors.UnitParseNoDigits(value));
			}
			if (num < length - 1)
			{
				try
				{
					this.m_type = ReportSize.GetTypeFromString(text.Substring(num + 1).Trim().ToLowerInvariant());
				}
				catch (ArgumentException ex)
				{
					throw new FormatException(ex.Message);
				}
				goto IL_00f1;
			}
			if (defaultType != 0)
			{
				this.m_type = defaultType;
				goto IL_00f1;
			}
			throw new FormatException(SRErrors.UnitParseNoUnit(value));
			IL_00f1:
			string text2 = text.Substring(0, num + 1);
			try
			{
				this.m_value = double.Parse(text2, provider);
			}
			catch
			{
				throw new FormatException(SRErrors.UnitParseNumericPart(value, text2, ((Enum)(object)this.m_type).ToString("G")));
			}
		}

		public static ReportSize Parse(string s, IFormatProvider provider)
		{
			return new ReportSize(s, provider);
		}

		public override int GetHashCode()
		{
			return this.m_type.GetHashCode() << 2 ^ this.m_value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is ReportSize)
			{
				ReportSize reportSize = (ReportSize)obj;
				if (reportSize.Value == this.Value && reportSize.m_type == this.m_type)
				{
					return true;
				}
				return false;
			}
			return false;
		}

		public static bool operator ==(ReportSize left, ReportSize right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ReportSize left, ReportSize right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(ReportSize left, ReportSize right)
		{
			return left.ToMillimeters() < right.ToMillimeters();
		}

		public static bool operator >(ReportSize left, ReportSize right)
		{
			return left.ToMillimeters() > right.ToMillimeters();
		}

		public static ReportSize operator +(ReportSize size1, ReportSize size2)
		{
			if (size1.IsEmpty)
			{
				size1 = new ReportSize(0.0);
			}
			size1.SetPixels(size1.ToPixels() + size2.ToPixels());
			return size1;
		}

		public static ReportSize operator -(ReportSize size1, ReportSize size2)
		{
			if (size1.IsEmpty)
			{
				size1 = new ReportSize(0.0);
			}
			size1.SetPixels(size1.ToPixels() - size2.ToPixels());
			return size1;
		}

		private static string GetStringFromType(SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Point:
				return "pt";
			case SizeTypes.Pica:
				return "pc";
			case SizeTypes.Inch:
				return "in";
			case SizeTypes.Mm:
				return "mm";
			case SizeTypes.Cm:
				return "cm";
			default:
				return string.Empty;
			}
		}

		internal static SizeTypes GetTypeFromString(string value)
		{
			if (value != null && value.Length > 0)
			{
				if (value.Equals("pt"))
				{
					return SizeTypes.Point;
				}
				if (value.Equals("pc"))
				{
					return SizeTypes.Pica;
				}
				if (value.Equals("in"))
				{
					return SizeTypes.Inch;
				}
				if (value.Equals("mm"))
				{
					return SizeTypes.Mm;
				}
				if (value.Equals("cm"))
				{
					return SizeTypes.Cm;
				}
				throw new ArgumentException(SRErrors.InvalidUnitType(value));
			}
			return ReportSize.DefaultType;
		}

		public int ToIntPixels()
		{
			return Convert.ToInt32(this.ConvertToPixels(this.m_value, this.m_type));
		}

		public double ToPixels()
		{
			return this.ConvertToPixels(this.m_value, this.m_type);
		}

		public void SetPixels(double pixels)
		{
			this.m_value = ReportSize.ConvertToUnits(pixels, this.m_type);
		}

		public static ReportSize FromPixels(double pixels, SizeTypes type)
		{
			return new ReportSize(ReportSize.ConvertToUnits(pixels, type), type);
		}

		public double ToMillimeters()
		{
			return this.ConvertToMillimeters(this.m_value, this.m_type);
		}

		public double ToCentimeters()
		{
			return 0.1 * this.ConvertToMillimeters(this.m_value, this.m_type);
		}

		public double ToInches()
		{
			return this.ConvertToMillimeters(this.m_value, this.m_type) / 25.4;
		}

		public double ToPoints()
		{
			if (this.m_type == SizeTypes.Point)
			{
				return this.m_value;
			}
			return this.ToInches() * 72.0;
		}

		public override string ToString()
		{
			return this.ToString(null, CultureInfo.CurrentCulture);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			if (this.IsEmpty)
			{
				return string.Empty;
			}
			return string.Format(provider, ReportSize.m_serializationFormat, this.SerializedValue, ReportSize.GetStringFromType(this.m_type));
		}

		internal ReportSize ChangeType(SizeTypes type)
		{
			if (type == this.m_type)
			{
				return this;
			}
			return new ReportSize(ReportSize.ConvertToUnits(this.ConvertToPixels(this.m_value, this.m_type), type), type);
		}

		internal double ConvertToPixels(double value, SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Cm:
				value *= (double)ReportSize.DotsPerInch / 2.54;
				break;
			case SizeTypes.Inch:
				value *= (double)ReportSize.DotsPerInch;
				break;
			case SizeTypes.Mm:
				value *= (double)ReportSize.DotsPerInch / 25.4;
				break;
			case SizeTypes.Pica:
				value *= (double)ReportSize.DotsPerInch / 6.0;
				break;
			case SizeTypes.Point:
				value *= (double)ReportSize.DotsPerInch / 72.0;
				break;
			}
			return value;
		}

		internal double ConvertToMillimeters(double value, SizeTypes type)
		{
			switch (type)
			{
			case SizeTypes.Cm:
				value *= 10.0;
				break;
			case SizeTypes.Inch:
				value *= 25.4;
				break;
			case SizeTypes.Pica:
				value *= 4.2333333333333334;
				break;
			case SizeTypes.Point:
				value *= 0.35277777777777775;
				break;
			}
			return value;
		}

		internal static double ConvertToUnits(double pixels, SizeTypes type)
		{
			double num = pixels;
			switch (type)
			{
			case SizeTypes.Cm:
				num /= (double)ReportSize.DotsPerInch / 2.54;
				break;
			case SizeTypes.Inch:
				num /= (double)ReportSize.DotsPerInch;
				break;
			case SizeTypes.Mm:
				num /= (double)ReportSize.DotsPerInch / 25.4;
				break;
			case SizeTypes.Pica:
				num /= (double)ReportSize.DotsPerInch / 6.0;
				break;
			case SizeTypes.Point:
				num /= (double)ReportSize.DotsPerInch / 72.0;
				break;
			}
			return num;
		}

		int IComparable.CompareTo(object value)
		{
			if (!(value is ReportSize))
			{
				throw new ArgumentException("value is not a RdlSize");
			}
			double num = this.ToMillimeters();
			double num2 = ((ReportSize)value).ToMillimeters();
			if (!(num < num2))
			{
				if (!(num > num2))
				{
					return 0;
				}
				return 1;
			}
			return -1;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string value = reader.ReadString();
			this.Init(value, CultureInfo.InvariantCulture, SizeTypes.Invalid);
			reader.Skip();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			string text = this.ToString(null, CultureInfo.InvariantCulture);
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
