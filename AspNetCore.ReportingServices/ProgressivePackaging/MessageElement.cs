namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class MessageElement
	{
		private string m_name;

		private object m_value;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
		}

		internal object Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal MessageElement(string name, object value)
		{
			this.m_name = name;
			this.m_value = value;
		}
	}
}
