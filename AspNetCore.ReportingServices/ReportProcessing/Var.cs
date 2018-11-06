using AspNetCore.ReportingServices.Diagnostics.Utilities;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class Var : VarBase
	{
		internal override object Result()
		{
			if (1 == base.m_currentCount)
			{
				return null;
			}
			switch (base.m_sumOfXType)
			{
			case DataTypeCode.Null:
				return null;
			case DataTypeCode.Double:
				return ((double)base.m_currentCount * (double)base.m_sumOfXSquared - (double)base.m_sumOfX * (double)base.m_sumOfX) / (double)(base.m_currentCount * (base.m_currentCount - 1));
			case DataTypeCode.Decimal:
				return ((decimal)base.m_currentCount * (decimal)base.m_sumOfXSquared - (decimal)base.m_sumOfX * (decimal)base.m_sumOfX) / (decimal)(base.m_currentCount * (base.m_currentCount - 1));
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
