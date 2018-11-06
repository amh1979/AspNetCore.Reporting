using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ScopeIDContext : RestartContext
	{
		private readonly ScopeID m_scopeID;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_memberDef;

		private readonly InternalStreamingOdpDynamicMemberLogic m_memberLogic;

		internal ScopeID ScopeID
		{
			get
			{
				return this.m_scopeID;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		internal InternalStreamingOdpDynamicMemberLogic MemberLogic
		{
			get
			{
				return this.m_memberLogic;
			}
		}

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> Expressions
		{
			get
			{
				if (this.m_memberDef.Sorting != null && this.m_memberDef.Sorting.NaturalSort)
				{
					return this.m_memberDef.Sorting.SortExpressions;
				}
				return this.m_memberDef.Grouping.GroupExpressions;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return this.m_memberDef.Sorting.SortDirections;
			}
		}

		internal ScopeIDContext(ScopeID scopeID, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, InternalStreamingOdpDynamicMemberLogic memberLogic, RestartMode restartMode)
			: base(restartMode)
		{
			this.m_scopeID = scopeID;
			this.m_memberDef = memberDef;
			this.m_memberLogic = memberLogic;
		}

		public bool RomBasedRestart()
		{
			return this.m_memberLogic.RomBasedRestart(this.m_scopeID);
		}

		public override List<ScopeValueFieldName> GetScopeValueFieldNameCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = new List<ScopeValueFieldName>();
			int num = 0;
			foreach (ScopeValue item in this.m_scopeID.QueryRestartPosition)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.Expressions[num];
				AspNetCore.ReportingServices.ReportIntermediateFormat.Field field = dataSet.Fields[expressionInfo.FieldIndex];
				string dataField = field.DataField;
				list.Add(new ScopeValueFieldName(dataField, item.Value));
				num++;
			}
			return list;
		}

		public override RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField[] recordFields)
		{
			int num = 0;
			foreach (ScopeValue item in this.m_scopeID.QueryRestartPosition)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.Expressions[num];
				AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field = recordFields[expressionInfo.FieldIndex];
				RowSkippingControlFlag rowSkippingControlFlag = base.CompareFieldWithScopeValueAndStopOnInequality(odpContext, field, item.Value, this.SortDirections[num], ObjectType.DataSet, this.m_memberDef.DataScopeInfo.DataSet.Name, "ScopeID.QueryRestart");
				if (rowSkippingControlFlag != 0)
				{
					return rowSkippingControlFlag;
				}
				num++;
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		public override void TraceStartAtRecoveryMessage()
		{
			Global.Tracer.Trace(TraceLevel.Warning, "START AT Recovery Mode: Target row grouping {0} did not match with ScopeID = {1}.", this.m_memberDef.Grouping.Name.MarkAsModelInfo(), this.m_scopeID.ToString());
		}
	}
}
