namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class Aggregate : DataAggregate
	{
		private object m_value;

		internal override void Init()
		{
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
