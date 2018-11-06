using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace AspNetCore.Reporting.Chart.WebForms.Formulas
{
	internal class FormulaRegistry : IServiceProvider
	{
		internal Hashtable registeredModules = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private Hashtable createdModules = new Hashtable(StringComparer.OrdinalIgnoreCase);

		private ArrayList modulesNames = new ArrayList();

		private IServiceContainer serviceContainer;

		public int Count
		{
			get
			{
				return this.modulesNames.Count;
			}
		}

		private FormulaRegistry()
		{
		}

		public FormulaRegistry(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
		}

		public void Register(string name, Type moduleType)
		{
			if (this.registeredModules.Contains(name))
			{
				if (this.registeredModules[name].GetType() != moduleType)
				{
					throw new ArgumentException(SR.ExceptionFormulaModuleNameIsNotUnique(name));
				}
			}
			else
			{
				this.modulesNames.Add(name);
				bool flag = false;
				Type[] interfaces = moduleType.GetInterfaces();
				Type[] array = interfaces;
				foreach (Type type in array)
				{
					if (type == typeof(IFormula))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(SR.ExceptionFormulaModuleHasNoInterface);
				}
				this.registeredModules[name] = moduleType;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(FormulaRegistry))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionFormulaModuleRegistryUnsupportedType(serviceType.ToString()));
		}

		public IFormula GetFormulaModule(string name)
		{
			if (!this.registeredModules.Contains(name))
			{
				throw new ArgumentException(SR.ExceptionFormulaModuleNameUnknown(name));
			}
			if (!this.createdModules.Contains(name))
			{
				this.createdModules[name] = ((Type)this.registeredModules[name]).Assembly.CreateInstance(((Type)this.registeredModules[name]).ToString());
			}
			return (IFormula)this.createdModules[name];
		}

		public string GetModuleName(int index)
		{
			return (string)this.modulesNames[index];
		}
	}
}
