using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class AxisCrossingValueConverter : AxisMinMaxValueConverter
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
			arrayList.Add(double.NaN);
			arrayList.Add(-1.7976931348623157E+308);
			arrayList.Add(1.7976931348623157E+308);
			return new StandardValuesCollection(arrayList);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			double num = (double)value;
			if (destinationType == typeof(string))
			{
				if (double.IsNaN(num))
				{
					return "Auto";
				}
				if (num == -1.7976931348623157E+308)
				{
					return "Min";
				}
				if (num == 1.7976931348623157E+308)
				{
					return "Max";
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string strA = (string)value;
				if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return double.NaN;
				}
				if (string.Compare(strA, "Min", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return -1.7976931348623157E+308;
				}
				if (string.Compare(strA, "Max", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return 1.7976931348623157E+308;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
