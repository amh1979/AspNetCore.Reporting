namespace AspNetCore.Reporting
{
	internal sealed class ValidValue
	{
		private string m_label;

		private string m_value;

		public string Label
		{
			get
			{
				return this.m_label;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ValidValue(string label, string value)
		{
			this.m_label = label;
			this.m_value = value;
		}
	}
}
