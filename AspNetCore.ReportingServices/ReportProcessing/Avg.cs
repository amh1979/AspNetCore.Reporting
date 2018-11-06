using AspNetCore.ReportingServices.Diagnostics.Utilities;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Avg : Sum
	{
		private uint m_currentCount;

		internal override void Init()
		{
			base.Init();
			this.m_currentCount = 0u;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			object o = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(o);
			if (!DataAggregate.IsNull(typeCode))
			{
				base.Update(expressions, iErrorContext);
				this.m_currentCount += 1u;
			}
		}

		internal override object Result()
		{
			switch (base.m_currentTotalType)
			{
			case DataTypeCode.Null:
				return null;
			case DataTypeCode.Double:
				return (double)base.m_currentTotal / (double)this.m_currentCount;
			case DataTypeCode.Decimal:
				return (decimal)base.m_currentTotal / (decimal)this.m_currentCount;
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
