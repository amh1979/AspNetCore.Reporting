using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class SeriesYValueTypeConverter : EnumConverter
	{
		public SeriesYValueTypeConverter(Type type)
			: base(type)
		{
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			StandardValuesCollection standardValues = base.GetStandardValues(context);
			foreach (object item in standardValues)
			{
				if (item.ToString() != "String")
				{
					arrayList.Add(item);
				}
			}
			return new StandardValuesCollection(arrayList);
		}
	}
}
