using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class StDev : Var
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
				return Math.Sqrt((double)base.Result());
			case DataTypeCode.Decimal:
				return Math.Sqrt(Convert.ToDouble((decimal)base.Result()));
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}
	}
}
