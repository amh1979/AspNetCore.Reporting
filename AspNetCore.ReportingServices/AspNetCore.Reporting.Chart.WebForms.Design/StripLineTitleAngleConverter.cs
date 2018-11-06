using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class StripLineTitleAngleConverter : Int32Converter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(0);
			arrayList.Add(90);
			arrayList.Add(180);
			arrayList.Add(270);
			return new StandardValuesCollection(arrayList);
		}
	}
}
