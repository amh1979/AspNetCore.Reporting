using System;
using System.Collections;
using System.ComponentModel;
using System.Data;


namespace AspNetCore.Reporting
{
	internal sealed class ReportDataSource
	{
		private string m_dataSourceID = "";

		private string m_dataMember;

		private string m_name = "";

		private object m_value;

		[DefaultValue("")]
		//[WebBrowsable(true)]
		[NotifyParentProperty(true)]
		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
				this.OnChanged();
			}
		}

		//[WebBrowsable(true)]
		[NotifyParentProperty(true)]
		public string DataSourceId
		{
			get
			{
				return this.m_dataSourceID;
			}
			set
			{
				this.m_dataSourceID = value;
				this.OnChanged();
			}
		}

		[NotifyParentProperty(true)]
		//[WebBrowsable(true)]
		public string DataMember
		{
			get
			{
				return this.m_dataMember;
			}
			set
			{
				this.m_dataMember = value;
			}
		}

		public object Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				if (value != null && !(value is DataTable) && !(value is AspNetCore.Reporting.IDataSource) && !(value is IEnumerable))
				{
					throw new ArgumentException("Errors.BadReportDataSourceType");
				}
				this.m_value = value;
				this.OnChanged();
			}
		}

		internal event EventHandler Changed;

		public ReportDataSource()
		{
		}

		public ReportDataSource(string name)
		{
			this.Name = name;
		}

		public ReportDataSource(string name, object dataSourceValue)
			: this(name)
		{
			this.Value = dataSourceValue;
		}

		public ReportDataSource(string name, DataTable dataSourceValue)
			: this(name)
		{
			this.Value = dataSourceValue;
		}

		public ReportDataSource(string name, string dataSourceId)
			: this(name)
		{
			this.DataSourceId = dataSourceId;
		}

		public ReportDataSource(string name, IEnumerable dataSourceValue)
			: this(name)
		{
			this.Value = dataSourceValue;
		}

		internal void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this, null);
			}
		}

		internal void SetValueWithoutChange(object dsValue)
		{
			this.m_value = dsValue;
		}
	}
}
