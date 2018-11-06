using System;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class AnchorPointValueConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
				{
					return "NotSet";
				}
				if (value is DataPoint)
				{
					DataPoint dataPoint = (DataPoint)value;
					if (dataPoint.series != null)
					{
						int num = dataPoint.series.Points.IndexOf(dataPoint) + 1;
						return dataPoint.series.Name + " - " + SR.DescriptionTypePoint + num.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
