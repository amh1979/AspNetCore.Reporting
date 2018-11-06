using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ParentSourceConverter : StringConverter
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
			if (context != null && context.Instance != null && context.Instance is NamedElement)
			{
				NamedElement namedElement = (NamedElement)context.Instance;
			}
			arrayList.Add("(none)");
			return new StandardValuesCollection(arrayList);
		}
	}
}
