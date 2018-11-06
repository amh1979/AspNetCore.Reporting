using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class IntAutoFitConverter : Int32Converter
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
			arrayList.Add(0);
			return new StandardValuesCollection(arrayList);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			int num = (int)value;
			if (destinationType == typeof(string) && num == 0)
			{
				return "AutoFit";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string strA = (string)value;
				if (string.Compare(strA, "AUTOFIT", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return 0;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
