using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Min : DataAggregate
	{
		private DataTypeCode m_expressionType;

		private object m_currentMin;

		private CompareInfo m_compareInfo;

		private CompareOptions m_compareOptions;

		internal Min(CompareInfo compareInfo, CompareOptions compareOptions)
		{
			this.m_currentMin = null;
			this.m_compareInfo = compareInfo;
			this.m_compareOptions = compareOptions;
		}

		internal override void Init()
		{
			this.m_currentMin = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != expressions);
			Global.Tracer.Assert(1 == expressions.Length);
			object obj = expressions[0];
			DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataAggregate.IsVariant(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
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
				if (this.m_currentMin == null)
				{
					this.m_currentMin = obj;
				}
				else
				{
					try
					{
						int num = ReportProcessing.CompareTo(this.m_currentMin, obj, this.m_compareInfo, this.m_compareOptions);
						if (num > 0)
						{
							this.m_currentMin = obj;
						}
					}
					catch
					{
						iErrorContext.Register(ProcessingErrorCode.rsMinMaxOfNonSortableData, Severity.Warning);
					}
				}
			}
		}

		internal override object Result()
		{
			return this.m_currentMin;
		}
	}
}
