namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Previous : DataAggregate
	{
		private object m_value;

		private object m_previous;

		internal override void Init()
		{
			this.m_value = null;
			this.m_previous = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			this.m_previous = this.m_value;
			this.m_value = expressions[0];
		}

		internal override object Result()
		{
			return this.m_previous;
		}
	}
}
