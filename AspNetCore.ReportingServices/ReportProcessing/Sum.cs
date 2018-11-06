using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class Sum : DataAggregate
	{
		private DataTypeCode m_expressionType;

		protected DataTypeCode m_currentTotalType;

		protected object m_currentTotal;

		internal override void Init()
		{
			this.m_currentTotalType = DataTypeCode.Null;
			this.m_currentTotal = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			object obj = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataTypeUtility.IsNumeric(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfNonNumericData, Severity.Warning);
					throw new InvalidOperationException();
				}
				if (this.m_expressionType == DataTypeCode.Null)
				{
					this.m_expressionType = typeCode;
				}
				else if (typeCode != this.m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
					throw new InvalidOperationException();
				}
				DataAggregate.ConvertToDoubleOrDecimal(typeCode, obj, out typeCode, out obj);
				if (this.m_currentTotal == null)
				{
					this.m_currentTotalType = typeCode;
					this.m_currentTotal = obj;
				}
				else
				{
					this.m_currentTotal = DataAggregate.Add(this.m_currentTotalType, this.m_currentTotal, typeCode, obj);
				}
			}
		}

		internal override object Result()
		{
			return this.m_currentTotal;
		}
	}
}
