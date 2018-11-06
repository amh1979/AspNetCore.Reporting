namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert
{
	internal sealed class ArgCount
	{
		private short m_count;

		internal short Count
		{
			get
			{
				return this.m_count;
			}
			set
			{
				this.m_count = value;
			}
		}
	}
}
