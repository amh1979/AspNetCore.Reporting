namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ExtendedProperty
	{
		private string m_name;

		private object m_value;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ExtendedProperty(string name, object value)
		{
			this.m_name = name;
			this.m_value = value;
		}

		internal void UpdateValue(object value)
		{
			this.m_value = value;
		}
	}
}
