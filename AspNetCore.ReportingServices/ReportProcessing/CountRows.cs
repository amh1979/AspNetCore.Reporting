namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class CountRows : DataAggregate
	{
		private int m_currentTotal;

		internal override void Init()
		{
			this.m_currentTotal = 0;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			this.m_currentTotal++;
		}

		internal override object Result()
		{
			return this.m_currentTotal;
		}
	}
}
