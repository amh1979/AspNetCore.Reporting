namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Count : DataAggregate
	{
		private int m_currentTotal;

		internal override void Init()
		{
			this.m_currentTotal = 0;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			object o = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(o);
			if (!DataAggregate.IsNull(typeCode))
			{
				this.m_currentTotal++;
			}
		}

		internal override object Result()
		{
			return this.m_currentTotal;
		}
	}
}
