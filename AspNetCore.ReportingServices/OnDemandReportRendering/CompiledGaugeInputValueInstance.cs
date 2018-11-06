namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CompiledGaugeInputValueInstance
	{
		private object m_value;

		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal CompiledGaugeInputValueInstance(object value)
		{
			this.m_value = value;
		}
	}
}
