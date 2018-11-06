using System;
using System.Collections.Generic;

namespace AspNetCore.Reporting
{
	internal sealed class DeviceInfoNameBlackList
	{
		private Dictionary<string, string> m_blackList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public void Add(string deviceInfoName)
		{
			this.Add(deviceInfoName, null);
		}

		public void Add(string deviceInfoName, string deviceInfoExceptionText)
		{
			if (deviceInfoName == null)
			{
				throw new ArgumentNullException("deviceInfoName");
			}
			if (this.m_blackList.ContainsKey(deviceInfoName))
			{
				throw new ArgumentException("DeviceInfo Name already exists", "deviceInfoName");
			}
			this.m_blackList.Add(deviceInfoName, deviceInfoExceptionText);
		}

		public bool Contains(string deviceInfoName)
		{
			if (deviceInfoName == null)
			{
				return false;
			}
			return this.m_blackList.ContainsKey(deviceInfoName);
		}

		public string GetExceptionText(string deviceInfoName)
		{
			string text = null;
			if (this.m_blackList.TryGetValue(deviceInfoName, out text))
			{
				if (text != null)
				{
					return text;
				}
				return CommonStrings.DeviceInfoInternal(deviceInfoName);
			}
			return null;
		}
	}
}
