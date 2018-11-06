using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RestartContext
	{
		private readonly RestartMode m_restartMode;

		public RestartMode RestartMode
		{
			get
			{
				return this.m_restartMode;
			}
		}

		public bool IsRowLevelRestart
		{
			get
			{
				return this.m_restartMode != RestartMode.Rom;
			}
		}

		public RestartContext(RestartMode mode)
		{
			this.m_restartMode = mode;
		}

		public abstract RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField[] recordFields);

		public RowSkippingControlFlag CompareFieldWithScopeValueAndStopOnInequality(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field, object scopeValue, bool isSortedAscending, ObjectType objectType, string objectName, string propertyName)
		{
			if (field == null)
			{
				throw new ReportProcessingException(ErrorCode.rsMissingFieldInStartAt);
			}
			int num = odpContext.CompareAndStopOnError(field.FieldValue, scopeValue, objectType, objectName, propertyName, false);
			if (num < 0)
			{
				if (!isSortedAscending)
				{
					return RowSkippingControlFlag.Stop;
				}
				return RowSkippingControlFlag.Skip;
			}
			if (num > 0)
			{
				if (!isSortedAscending)
				{
					return RowSkippingControlFlag.Skip;
				}
				return RowSkippingControlFlag.Stop;
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		public abstract List<ScopeValueFieldName> GetScopeValueFieldNameCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet);

		public abstract void TraceStartAtRecoveryMessage();
	}
}
