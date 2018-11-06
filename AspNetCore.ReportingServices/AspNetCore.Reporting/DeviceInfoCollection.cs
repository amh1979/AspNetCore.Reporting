using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace AspNetCore.Reporting
{
	[Serializable]
	[ComVisible(false)]
	internal sealed class DeviceInfoCollection : KeyedCollection<string, DeviceInfo>
	{
		[NonSerialized]
		private DeviceInfoNameBlackList m_deviceInfoNamesBlackList = new DeviceInfoNameBlackList();



		internal DeviceInfoNameBlackList DeviceInfoNameBlackList
		{
			get
			{
				return this.m_deviceInfoNamesBlackList;
			}
			set
			{
				this.m_deviceInfoNamesBlackList = value;
			}
		}

		

		internal DeviceInfoCollection()
			: base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
		{
		}

		public void Add(string name, string value)
		{
			base.Add(new DeviceInfo(name, value));
		}

		protected override void ClearItems()
		{

			base.ClearItems();
		}

		protected override string GetKeyForItem(DeviceInfo item)
		{
			return item.Name;
		}

		protected override void InsertItem(int index, DeviceInfo item)
		{
			this.ValidateKey(item.Name);
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, DeviceInfo item)
		{
			this.ValidateKey(item.Name);
			base.SetItem(index, item);
		}



		private bool ValidateKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.m_deviceInfoNamesBlackList.Contains(key))
			{
				throw new ArgumentException(this.m_deviceInfoNamesBlackList.GetExceptionText(key));
			}
			return true;
		}
	}
}
