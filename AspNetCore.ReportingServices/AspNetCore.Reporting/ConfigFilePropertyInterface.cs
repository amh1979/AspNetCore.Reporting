using System;
using System.Configuration;

namespace AspNetCore.Reporting
{
	internal sealed class ConfigFilePropertyInterface<InterfaceType> where InterfaceType : class
	{
		private string m_propertyName;

		private string m_interfaceTypeName;

		private bool m_propertyLoaded;

		private Type m_propertyType;

		public ConfigFilePropertyInterface(string propertyName, string interfaceTypeName)
		{
			this.m_propertyName = propertyName;
			this.m_interfaceTypeName = interfaceTypeName;
		}

		public InterfaceType GetInstance()
		{
			this.EnsurePropertyLoaded();
			if (this.m_propertyType == null)
			{
				return null;
			}
			return (InterfaceType)Activator.CreateInstance(this.m_propertyType);
		}

		private void EnsurePropertyLoaded()
		{
            /*
			if (!this.m_propertyLoaded && this.m_propertyName != null)
			{
				string text = ConfigurationManager.AppSettings[this.m_propertyName];
				if (!string.IsNullOrEmpty(text))
				{
					Type type = Type.GetType(text);
					if (type == null)
					{
						throw new Exception($" InvalidConfigFileTypeException:{text}");
					}
					if (!typeof(InterfaceType).IsAssignableFrom(type))
					{
						throw new Exception($" InvalidConfigFileTypeException:{text},{this.m_interfaceTypeName}");
					}
					this.m_propertyType = type;
				}
				this.m_propertyLoaded = true;
			}
            */
		}
	}
}
