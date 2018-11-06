using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class ChartTypeConverter : StringConverter
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
			ChartTypeRegistry chartTypeRegistry = null;
			ArrayList arrayList = new ArrayList();
			if (context != null && context.Instance != null)
			{
				IServiceContainer serviceContainer = null;
				if (context.Instance is Series && ((Series)context.Instance).serviceContainer != null)
				{
					serviceContainer = ((Series)context.Instance).serviceContainer;
				}
				if (serviceContainer == null && context.Instance is Array)
				{
					Array array = (Array)context.Instance;
					if (array.Length > 0 && array.GetValue(0) is Series)
					{
						serviceContainer = ((Series)array.GetValue(0)).serviceContainer;
					}
				}
				if (serviceContainer != null)
				{
					chartTypeRegistry = (ChartTypeRegistry)serviceContainer.GetService(typeof(ChartTypeRegistry));
					if (chartTypeRegistry != null)
					{
						foreach (object key in chartTypeRegistry.registeredChartTypes.Keys)
						{
							if (key is string)
							{
								arrayList.Add(key);
							}
						}
						goto IL_0106;
					}
					throw new InvalidOperationException(SR.ExceptionEditorChartTypeRegistryServiceInaccessible);
				}
			}
			goto IL_0106;
			IL_0106:
			arrayList.Sort();
			return new StandardValuesCollection(arrayList);
		}
	}
}
