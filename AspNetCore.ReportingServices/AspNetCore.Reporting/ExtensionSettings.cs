
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AspNetCore.Reporting
{
	internal sealed class ExtensionSettings
	{
		private readonly string m_extensionName;

		private readonly NameValueCollection m_extensionParameters;

		public NameValueCollection Settings
		{
			get
			{
				return this.m_extensionParameters;
			}
		}

		public ExtensionSettings(string name, NameValueCollection extensionParameters)
		{
			this.m_extensionName = name;
			this.m_extensionParameters = extensionParameters;
		}

	}
}
