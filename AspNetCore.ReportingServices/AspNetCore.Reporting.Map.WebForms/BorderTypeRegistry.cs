using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class BorderTypeRegistry : IServiceProvider
	{
		internal Hashtable registeredBorderTypes = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);

		private Hashtable createdBorderTypes = new Hashtable(new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture), StringComparer.OrdinalIgnoreCase);

		private IServiceContainer serviceContainer;

		private BorderTypeRegistry()
		{
		}

		public BorderTypeRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("Valid Service Container object must be provided");
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
			throw new ArgumentException("3D border registry does not provide service of type: " + serviceType.ToString());
		}

		public void Register(string name, Type borderType)
		{
			if (this.registeredBorderTypes.Contains(name))
			{
				if (this.registeredBorderTypes[name].GetType() != borderType)
				{
					throw new ArgumentException("Border type with name \"" + name + "\" is already registered.");
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
					throw new ArgumentException("Border type must implement the IBorderType interface.");
				}
				this.registeredBorderTypes[name] = borderType;
			}
		}

		public IBorderType GetBorderType(string name)
		{
			if (!this.registeredBorderTypes.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionUnknownBorderType(name));
			}
			if (!this.createdBorderTypes.Contains(name))
			{
				this.createdBorderTypes[name] = ((Type)this.registeredBorderTypes[name]).Assembly.CreateInstance(((Type)this.registeredBorderTypes[name]).ToString());
			}
			return (IBorderType)this.createdBorderTypes[name];
		}
	}
}
