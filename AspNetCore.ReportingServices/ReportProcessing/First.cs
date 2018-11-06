namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class First : DataAggregate
	{
		private object m_value;

		private bool m_updated;

		internal override void Init()
		{
			this.m_value = null;
			this.m_updated = false;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			if (!this.m_updated)
			{
				this.m_value = expressions[0];
				this.m_updated = true;
			}
		}

		internal override object Result()
		{
			return this.m_value;
		}
	}
}
