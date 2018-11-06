namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLSubReportProps : RPLItemProps
	{
		private string m_language;

		public string Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				this.m_language = value;
			}
		}

		internal RPLSubReportProps()
		{
		}
	}
}
