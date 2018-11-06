using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class ChartTypeRegistry : IServiceProvider
	{
		private ResourceManager resourceManager;

		internal Hashtable registeredChartTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdChartTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private IServiceContainer serviceContainer;

		public ResourceManager ResourceManager
		{
			get
			{
				if (this.resourceManager == null)
				{
					this.resourceManager = new ResourceManager("AspNetCore.Reporting.Chart.WebForms.Design", Assembly.GetExecutingAssembly());
				}
				return this.resourceManager;
			}
		}

		private ChartTypeRegistry()
		{
		}

		public ChartTypeRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartTypeRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartTypeRegistryUnsupportedType(serviceType.ToString()));
		}

		public void Register(string name, Type chartType)
		{
			if (this.registeredChartTypes.Contains(name))
			{
				if (this.registeredChartTypes[name].GetType() != chartType)
				{
					throw new ArgumentException(SR.ExceptionChartTypeNameIsNotUnique(name));
				}
			}
			else
			{
				bool flag = false;
				Type[] interfaces = chartType.GetInterfaces();
				Type[] array = interfaces;
				foreach (Type type in array)
				{
					if (type == typeof(IChartType))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(SR.ExceptionChartTypeHasNoInterface);
				}
				this.registeredChartTypes[name] = chartType;
			}
		}

		public IChartType GetChartType(SeriesChartType chartType)
		{
			return this.GetChartType(Series.GetChartTypeName(chartType));
		}

		public IChartType GetChartType(string name)
		{
			if (!this.registeredChartTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionChartTypeUnknown(name));
			}
			if (!this.createdChartTypes.Contains(name))
			{
				this.createdChartTypes[name] = ((Type)this.registeredChartTypes[name]).Assembly.CreateInstance(((Type)this.registeredChartTypes[name]).ToString());
			}
			return (IChartType)this.createdChartTypes[name];
		}
	}
}
