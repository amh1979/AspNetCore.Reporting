using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Resources;

namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class BorderTypeRegistry : IServiceProvider
	{
		private ResourceManager resourceManager;

		internal Hashtable registeredBorderTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdBorderTypes = new Hashtable(StringComparer.OrdinalIgnoreCase);

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

		private BorderTypeRegistry()
		{
		}

		public BorderTypeRegistry(IServiceContainer container)
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
			if (serviceType == typeof(BorderTypeRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionBorderTypeRegistryUnsupportedType(serviceType.ToString()));
		}

		public void Register(string name, Type borderType)
		{
			if (this.registeredBorderTypes.Contains(name))
			{
				if (this.registeredBorderTypes[name].GetType() != borderType)
				{
					throw new ArgumentException(SR.ExceptionBorderTypeNameIsNotUnique(name));
				}
			}
			else
			{
				bool flag = false;
				Type[] interfaces = borderType.GetInterfaces();
				Type[] array = interfaces;
				foreach (Type type in array)
				{
					if (type == typeof(IBorderType))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(SR.ExceptionBorderTypeHasNoInterface);
				}
				this.registeredBorderTypes[name] = borderType;
			}
		}

		public IBorderType GetBorderType(string name)
		{
			if (!this.registeredBorderTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionBorderTypeUnknown(name));
			}
			if (!this.createdBorderTypes.Contains(name))
			{
				this.createdBorderTypes[name] = ((Type)this.registeredBorderTypes[name]).Assembly.CreateInstance(((Type)this.registeredBorderTypes[name]).ToString());
			}
			return (IBorderType)this.createdBorderTypes[name];
		}
	}
}
