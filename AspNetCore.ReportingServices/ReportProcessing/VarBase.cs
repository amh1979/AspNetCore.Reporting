using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal abstract class VarBase : DataAggregate
	{
		private DataTypeCode m_expressionType;

		protected uint m_currentCount;

		protected DataTypeCode m_sumOfXType;

		protected object m_sumOfX;

		protected object m_sumOfXSquared;

		internal override void Init()
		{
			this.m_currentCount = 0u;
			this.m_sumOfXType = DataTypeCode.Null;
			this.m_sumOfX = null;
			this.m_sumOfXSquared = null;
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
				object obj2 = DataAggregate.Square(typeCode, obj);
				if (this.m_sumOfX == null)
				{
					this.m_sumOfXType = typeCode;
					this.m_sumOfX = obj;
					this.m_sumOfXSquared = obj2;
				}
				else
				{
					this.m_sumOfX = DataAggregate.Add(this.m_sumOfXType, this.m_sumOfX, typeCode, obj);
					this.m_sumOfXSquared = DataAggregate.Add(this.m_sumOfXType, this.m_sumOfXSquared, typeCode, obj2);
				}
				this.m_currentCount += 1u;
			}
		}
	}
}
