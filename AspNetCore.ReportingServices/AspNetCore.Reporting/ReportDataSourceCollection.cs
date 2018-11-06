using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace AspNetCore.Reporting
{
	[Serializable]
	[ComVisible(false)]
	internal sealed class ReportDataSourceCollection : SyncList<ReportDataSource>, ISerializable
	{
		private EventHandler m_onChangeEventHandler;

		public ReportDataSource this[string name]
		{
			get
			{
				foreach (ReportDataSource item in this)
				{
					if (string.Compare(name, item.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}

		internal event EventHandler Change;

		internal ReportDataSourceCollection(object syncObject)
			: base(syncObject)
		{
			this.m_onChangeEventHandler = this.OnChange;
		}

		internal ReportDataSourceCollection(SerializationInfo info, StreamingContext context)
			: this(new object())
		{
			int @int = info.GetInt32("Count");
			for (int i = 0; i < @int; i++)
			{
				string str = i.ToString(CultureInfo.InvariantCulture);
				base.Add(new ReportDataSource
				{
					Name = info.GetString("Name" + str),
					DataSourceId = info.GetString("ID" + str)
				});
			}
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Count", base.Count);
			for (int i = 0; i < base.Count; i++)
			{
				string str = i.ToString(CultureInfo.InvariantCulture);
				ReportDataSource reportDataSource = base[i];
				info.AddValue("Name" + str, reportDataSource.Name);
				info.AddValue("ID" + str, reportDataSource.DataSourceId);
			}
		}

		protected override void ClearItems()
		{
			foreach (ReportDataSource item in this)
			{
				this.UnregisterItem(item);
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, ReportDataSource item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			base.InsertItem(index, item);
			this.RegisterItem(item);
		}

		protected override void RemoveItem(int index)
		{
			this.UnregisterItem(base[index]);
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, ReportDataSource item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.UnregisterItem(base[index]);
			base.SetItem(index, item);
			this.RegisterItem(item);
		}

		private void RegisterItem(ReportDataSource item)
		{
			item.Changed += this.m_onChangeEventHandler;
			this.OnChange();
		}

		private void UnregisterItem(ReportDataSource item)
		{
			item.Changed -= this.m_onChangeEventHandler;
			this.OnChange();
		}

		private void OnChange()
		{
			if (this.Change != null)
			{
				this.Change(this, EventArgs.Empty);
			}
		}

		private void OnChange(object sender, EventArgs e)
		{
			this.OnChange();
		}
	}
}
