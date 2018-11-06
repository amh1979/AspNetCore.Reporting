using AspNetCore.Reporting.Chart.WebForms.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class SeriesNameConverter : StringConverter
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
			DataManager dataManager = null;
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				MethodInfo method = context.Instance.GetType().GetMethod("GetService");
				if (method != null)
				{
					object[] parameters = new object[1]
					{
						typeof(DataManager)
					};
					dataManager = (DataManager)method.Invoke(context.Instance, parameters);
				}
				if (dataManager != null)
				{
					foreach (Series item in dataManager.Series)
					{
						arrayList.Add(item.Name);
					}
					goto IL_00c2;
				}
				throw new InvalidOperationException(SR.ExceptionEditorChartTypeRegistryServiceInObjectInaccessible(context.Instance.GetType().ToString()));
			}
			goto IL_00c2;
			IL_00c2:
			return new StandardValuesCollection(arrayList);
		}
	}
}
