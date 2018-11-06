using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(FieldConverter))]
	internal class Field : NamedElement
	{
		private Type type;

		private bool uniqueIdentifier;

		private bool isTemporary;

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeField_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (this.FieldHasData())
				{
					throw new ArgumentException(SR.ExceptionCannotRenameField);
				}
				base.Name = value;
				this.InvalidateViewport();
			}
		}

		[TypeConverter(typeof(DataAttributeType_TypeConverter))]
		[SRDescription("DescriptionAttributeField_Type")]
		[SRCategory("CategoryAttribute_Data")]
		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = Field.ConvertToSupportedType(value);
			}
		}

		[SRDescription("DescriptionAttributeField_UniqueIdentifier")]
		[DefaultValue(false)]
		[SRCategory("CategoryAttribute_Data")]
		public bool UniqueIdentifier
		{
			get
			{
				return this.uniqueIdentifier;
			}
			set
			{
				this.uniqueIdentifier = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override object Tag
		{
			get
			{
				return base.Tag;
			}
			set
			{
				base.Tag = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		public bool IsTemporary
		{
			get
			{
				return this.isTemporary;
			}
			set
			{
				this.isTemporary = value;
			}
		}

		public Field()
			: this(null)
		{
		}

		internal Field(CommonElements common)
			: base(common)
		{
			this.type = typeof(string);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public bool IsNumeric()
		{
			if (this.Type != typeof(int) && this.Type != typeof(double) && this.Type != typeof(decimal) && this.Type != typeof(DateTime) && this.Type != typeof(TimeSpan))
			{
				return false;
			}
			return true;
		}

		public string GetKeyword()
		{
			string text = this.Name;
			if (base.common == null || base.common.MapCore.UppercaseFieldKeywords)
			{
				text = text.ToUpper(CultureInfo.InvariantCulture);
			}
			text = text.Replace(' ', '_');
			return "#" + text;
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.ParentElement;
		}

		private static bool IsValid(Type type)
		{
			if (type != typeof(string) && type != typeof(int) && type != typeof(double) && type != typeof(decimal) && type != typeof(bool) && type != typeof(DateTime) && type != typeof(TimeSpan))
			{
				return false;
			}
			return true;
		}

		internal string FormatValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (this.Type == typeof(string))
			{
				return XmlConvert.EncodeName(Convert.ToString(value, CultureInfo.InvariantCulture));
			}
			if (this.Type == typeof(int))
			{
				return XmlConvert.ToString(Convert.ToInt32(value, CultureInfo.InvariantCulture));
			}
			if (this.Type == typeof(double))
			{
				return XmlConvert.ToString(Convert.ToDouble(value, CultureInfo.InvariantCulture));
			}
			if (this.Type == typeof(decimal))
			{
				return XmlConvert.ToString(Convert.ToDecimal(value, CultureInfo.InvariantCulture));
			}
			if (this.Type == typeof(bool))
			{
				return XmlConvert.ToString(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
			}
			if (this.Type == typeof(DateTime))
			{
				return XmlConvert.ToString(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
			}
			return XmlConvert.ToString((TimeSpan)value);
		}

		internal void ParseValue(string fieldValue, Hashtable fields)
		{
			if (this.Type == typeof(string))
			{
				fields[this.Name] = XmlConvert.DecodeName(fieldValue);
			}
			else if (this.Type == typeof(int))
			{
				fields[this.Name] = XmlConvert.ToInt32(fieldValue);
			}
			else if (this.Type == typeof(double))
			{
				fields[this.Name] = XmlConvert.ToDouble(fieldValue);
			}
			else if (this.Type == typeof(decimal))
			{
				fields[this.Name] = XmlConvert.ToDecimal(fieldValue);
			}
			else if (this.Type == typeof(bool))
			{
				fields[this.Name] = XmlConvert.ToBoolean(fieldValue);
			}
			else if (this.Type == typeof(DateTime))
			{
				fields[this.Name] = XmlConvert.ToDateTime(fieldValue);
			}
			else
			{
				fields[this.Name] = XmlConvert.ToTimeSpan(fieldValue);
			}
		}

		internal void SetValue(object value, Hashtable fields)
		{
			if (this.Type == typeof(string))
			{
				fields[this.Name] = Convert.ToString(value, CultureInfo.InvariantCulture);
			}
			else if (this.Type == typeof(int))
			{
				fields[this.Name] = Convert.ToInt32(value, CultureInfo.InvariantCulture);
			}
			else if (this.Type == typeof(double))
			{
				fields[this.Name] = Convert.ToDouble(value, CultureInfo.InvariantCulture);
			}
			else if (this.Type == typeof(decimal))
			{
				fields[this.Name] = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
			}
			else if (this.Type == typeof(bool))
			{
				fields[this.Name] = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			}
			else if (this.Type == typeof(DateTime))
			{
				fields[this.Name] = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
			}
			else
			{
				fields[this.Name] = (TimeSpan)value;
			}
		}

		internal object Parse(string stringValue)
		{
			try
			{
				if (this.Type == typeof(string))
				{
					return stringValue;
				}
				if (string.IsNullOrEmpty(stringValue))
				{
					return null;
				}
				if (this.Type == typeof(int))
				{
					return int.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (this.Type == typeof(double))
				{
					return double.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (this.Type == typeof(decimal))
				{
					return decimal.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				if (this.Type == typeof(bool))
				{
					return bool.Parse(stringValue);
				}
				if (this.Type == typeof(DateTime))
				{
					return DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
				}
				return TimeSpan.Parse(stringValue);
			}
			catch
			{
				return null;
			}
		}

		internal static object ConvertToSupportedValue(object value)
		{
			Type type = value.GetType();
			Type type2 = Field.ConvertToSupportedType(type);
			if (type == type2)
			{
				return value;
			}
			return Convert.ChangeType(value, type2, CultureInfo.InvariantCulture);
		}

		internal static Type ConvertToSupportedType(Type valueType)
		{
			if (Field.IsValid(valueType))
			{
				return valueType;
			}
			if (valueType == typeof(char))
			{
				return typeof(string);
			}
			if (valueType == typeof(float))
			{
				return typeof(double);
			}
			if (valueType != typeof(byte) && valueType != typeof(sbyte) && valueType != typeof(short) && valueType != typeof(ushort))
			{
				if (valueType != typeof(uint) && valueType != typeof(long) && valueType != typeof(ulong) && valueType != typeof(ushort))
				{
					return typeof(string);
				}
				return typeof(decimal);
			}
			return typeof(int);
		}

		internal double ConvertToDouble(object fieldValue)
		{
			if (fieldValue == null)
			{
				return double.NaN;
			}
			if (this.Type == typeof(int))
			{
				return (double)(int)fieldValue;
			}
			if (this.Type == typeof(double))
			{
				return (double)fieldValue;
			}
			if (this.Type == typeof(decimal))
			{
				return (double)(decimal)fieldValue;
			}
			if (this.Type == typeof(DateTime))
			{
				TimeSpan timeSpan = new TimeSpan(((DateTime)fieldValue).Ticks);
				return timeSpan.TotalMilliseconds;
			}
			if (this.Type == typeof(TimeSpan))
			{
				return ((TimeSpan)fieldValue).TotalMilliseconds;
			}
			throw new Exception(fieldValue.ToString() + " is not a supported numeric type.");
		}

		internal static string ToStringInvariant(object fieldValue)
		{
			if (fieldValue == null)
			{
				return string.Empty;
			}
			if (fieldValue is IConvertible)
			{
				return ((string)((IConvertible)fieldValue).ToType(typeof(string), CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
			}
			return fieldValue.ToString();
		}

		internal bool FieldHasData()
		{
			MapCore mapCore = this.GetMapCore();
			if (mapCore != null && mapCore.IsDesignMode())
			{
				if (this.Collection == mapCore.GroupFields)
				{
					foreach (Group group in mapCore.Groups)
					{
						if (group[this.Name] != null)
						{
							return true;
						}
					}
				}
				else if (this.Collection == mapCore.ShapeFields)
				{
					foreach (Shape shape in mapCore.Shapes)
					{
						if (shape[this.Name] != null)
						{
							return true;
						}
					}
				}
				else if (this.Collection == mapCore.PathFields)
				{
					foreach (Path path in mapCore.Paths)
					{
						if (path[this.Name] != null)
						{
							return true;
						}
					}
				}
				else if (this.Collection == mapCore.SymbolFields)
				{
					foreach (Symbol symbol in mapCore.Symbols)
					{
						if (symbol[this.Name] != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
