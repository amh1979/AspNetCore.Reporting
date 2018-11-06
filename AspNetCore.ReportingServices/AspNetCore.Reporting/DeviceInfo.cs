using System;
using System.Runtime.InteropServices;

namespace AspNetCore.Reporting
{
	[Serializable]
	[ComVisible(false)]
	internal sealed class DeviceInfo
	{
		private string m_name;

		private string m_value;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		public DeviceInfo(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.m_name = name;
			this.m_value = value;
		}
	}
}
