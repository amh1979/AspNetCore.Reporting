namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LiteralInfo
	{
		private readonly object m_value;

		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		public LiteralInfo(object value)
		{
			this.m_value = value;
		}
	}
}
