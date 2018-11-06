using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RelationshipRestartContext : RestartContext
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] m_expressions;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult[] m_values;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_idcDataSet;

		private SortDirection[] m_sortDirections;

		public RelationshipRestartContext(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] expressions, AspNetCore.ReportingServices.RdlExpressions.VariantResult[] values, SortDirection[] sortDirections, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet idcDataSet)
			: base(RestartMode.Query)
		{
			this.m_expressions = expressions;
			this.m_values = values;
			this.m_idcDataSet = idcDataSet;
			this.m_sortDirections = sortDirections;
			this.NormalizeValues(this.m_values);
		}

		public override RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField[] recordFields)
		{
			for (int i = 0; i < this.m_expressions.Length; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.m_expressions[i];
				object value = this.m_values[i].Value;
				AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field = recordFields[expressionInfo.FieldIndex];
				bool isSortedAscending = this.m_sortDirections[i] == SortDirection.Ascending;
				RowSkippingControlFlag rowSkippingControlFlag = base.CompareFieldWithScopeValueAndStopOnInequality(odpContext, field, value, isSortedAscending, ObjectType.DataSet, this.m_idcDataSet.Name, "Relationship.QueryRestart");
				if (rowSkippingControlFlag != 0)
				{
					return rowSkippingControlFlag;
				}
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		private void NormalizeValues(AspNetCore.ReportingServices.RdlExpressions.VariantResult[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i].Value is DBNull)
				{
					values[i].Value = null;
				}
			}
		}

		public override List<ScopeValueFieldName> GetScopeValueFieldNameCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = new List<ScopeValueFieldName>();
			for (int i = 0; i < this.m_expressions.Length; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = dataSet.Fields[this.m_expressions[i].FieldIndex];
				string dataField = field.DataField;
				list.Add(new ScopeValueFieldName(dataField, this.m_values[i].Value));
			}
			return list;
		}

		public override void TraceStartAtRecoveryMessage()
		{
		}
	}
}
