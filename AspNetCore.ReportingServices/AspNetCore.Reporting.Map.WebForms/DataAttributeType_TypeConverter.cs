using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DataAttributeType_TypeConverter : TypeConverter
	{
		private static Type[] types;

		private StandardValuesCollection values;

		static DataAttributeType_TypeConverter()
		{
			Type[] array = DataAttributeType_TypeConverter.types = new Type[7]
			{
				typeof(string),
				typeof(int),
				typeof(double),
				typeof(decimal),
				typeof(bool),
				typeof(DateTime),
				typeof(TimeSpan)
			};
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value != null && value.GetType() == typeof(string))
			{
				for (int i = 0; i < DataAttributeType_TypeConverter.types.Length; i++)
				{
					if (DataAttributeType_TypeConverter.types[i].ToString().Equals(value))
					{
						return DataAttributeType_TypeConverter.types[i];
					}
				}
				return typeof(string);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return string.Empty;
				}
				value.ToString();
			}
			if (value != null && destinationType == typeof(InstanceDescriptor))
			{
				object obj = value;
				if (value is string)
				{
					for (int i = 0; i < DataAttributeType_TypeConverter.types.Length; i++)
					{
						if (DataAttributeType_TypeConverter.types[i].ToString().Equals(value))
						{
							obj = DataAttributeType_TypeConverter.types[i];
						}
					}
				}
				if (value is Type || value is string)
				{
					Type[] array = new Type[1]
					{
						typeof(string)
					};
					MethodInfo method = typeof(Type).GetMethod("GetType", array);
					if (method != null)
					{
						object[] arguments = new object[1]
						{
							((Type)obj).AssemblyQualifiedName
						};
						return new InstanceDescriptor(method, arguments);
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.values == null)
			{
				object[] destinationArray;
				if (DataAttributeType_TypeConverter.types != null)
				{
					destinationArray = new object[DataAttributeType_TypeConverter.types.Length];
					Array.Copy(DataAttributeType_TypeConverter.types, destinationArray, DataAttributeType_TypeConverter.types.Length);
				}
				else
				{
					destinationArray = null;
				}
				this.values = new StandardValuesCollection(destinationArray);
			}
			return this.values;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
