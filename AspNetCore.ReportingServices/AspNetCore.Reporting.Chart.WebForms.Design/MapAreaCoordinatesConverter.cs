using System;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class MapAreaCoordinatesConverter : ArrayConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] array = ((string)value).Split(',');
				float[] array2 = new float[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = float.Parse(array[i], CultureInfo.CurrentCulture);
				}
				return array2;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				float[] array = (float[])value;
				string text = "";
				float[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					text = text + array2[i].ToString(CultureInfo.CurrentCulture) + ",";
				}
				return text.TrimEnd(',');
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
