namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Last : DataAggregate
	{
		private object m_value;

		internal override void Init()
		{
			this.m_value = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			this.m_value = expressions[0];
		}

		internal override object Result()
		{
			return this.m_value;
		}
	}
}
