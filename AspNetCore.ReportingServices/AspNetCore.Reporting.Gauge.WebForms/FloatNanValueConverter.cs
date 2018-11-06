using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class FloatNanValueConverter : SingleConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(float.NaN);
			return new StandardValuesCollection(arrayList);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			float f = (float)value;
			if (destinationType == typeof(string) && float.IsNaN(f))
			{
				return "Not set";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string strA = (string)value;
				if (string.Compare(strA, "Not set", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return float.NaN;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
