namespace AspNetCore.ReportingServices.Interfaces
{
	internal class ValidValue
	{
		private string m_value;

		private string m_label;

		public string Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		public string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}
	}
}
