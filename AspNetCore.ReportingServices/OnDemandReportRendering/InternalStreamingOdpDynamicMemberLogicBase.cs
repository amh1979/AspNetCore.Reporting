using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class InternalStreamingOdpDynamicMemberLogicBase : InternalDynamicMemberLogic
	{
		protected readonly DataRegionMember m_memberDef;

		protected readonly OnDemandProcessingContext m_odpContext;

		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping m_grouping;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting m_sorting;

		private readonly List<ScopeIDType> m_memberGroupAndSortExpressionFlag;

		private ScopeID m_scopeID;

		private ScopeID m_lastScopeID;

		protected InternalStreamingOdpDynamicMemberLogicBase(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
		{
			this.m_memberDef = memberDef;
			this.m_sorting = this.m_memberDef.DataRegionMemberDefinition.Sorting;
			this.m_grouping = this.m_memberDef.DataRegionMemberDefinition.Grouping;
			this.m_memberGroupAndSortExpressionFlag = this.m_memberDef.DataRegionMemberDefinition.MemberGroupAndSortExpressionFlag;
			this.m_odpContext = odpContext;
		}

		public override void ResetContext()
		{
			base.m_isNewContext = true;
			base.m_currentContext = -1;
			this.m_scopeID = null;
			this.m_memberDef.DataRegionMemberDefinition.InstanceCount = -1;
			this.m_memberDef.DataRegionMemberDefinition.InstancePathItem.ResetContext();
			IRIFReportDataScope iRIFReportDataScope = (IRIFReportDataScope)this.m_memberDef.ReportScope.RIFReportScope;
			iRIFReportDataScope.ClearStreamingScopeInstanceBinding();
		}

		protected bool MoveNextCore(System.Action actionOnNextInstance)
		{
			IRIFReportDataScope iRIFReportDataScope = (IRIFReportDataScope)this.m_memberDef.ReportScope.RIFReportScope;
			if (iRIFReportDataScope.IsBoundToStreamingScopeInstance)
			{
				this.m_odpContext.BindNextMemberInstance(this.m_memberDef.DataRegionMemberDefinition, this.m_memberDef.ReportScopeInstance, base.m_currentContext + 1);
			}
			else
			{
				this.m_odpContext.SetupContext(this.m_memberDef.DataRegionMemberDefinition, this.m_memberDef.ReportScopeInstance, -1);
			}
			if (iRIFReportDataScope.CurrentStreamingScopeInstance.Value().IsNoRows)
			{
				return false;
			}
			if (actionOnNextInstance != null)
			{
				actionOnNextInstance();
			}
			base.m_isNewContext = true;
			base.m_currentContext++;
			this.m_memberDef.DataRegionMemberDefinition.InstancePathItem.MoveNext();
			this.m_memberDef.SetNewContext(true);
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			this.ResetContext();
			if (index < 0)
			{
				return true;
			}
			int i;
			for (i = -1; i < index; i++)
			{
				if (!this.MoveNext())
				{
					break;
				}
			}
			return i == index;
		}

		internal override ScopeID GetScopeID()
		{
			if (this.m_grouping.IsDetail)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsDetailGroupsNotSupportedInStreamingMode, "GetScopeID");
			}
			if (this.m_scopeID != (ScopeID)null)
			{
				return this.m_scopeID;
			}
			this.m_odpContext.SetupContext(this.m_memberDef.DataRegionMemberDefinition, this.m_memberDef.ReportScopeInstance);
			IRIFReportDataScope iRIFReportDataScope = (IRIFReportDataScope)this.m_memberDef.ReportScope.RIFReportScope;
			if (iRIFReportDataScope.IsBoundToStreamingScopeInstance && !iRIFReportDataScope.CurrentStreamingScopeInstance.Value().IsNoRows)
			{
				List<ScopeValue> list = null;
				IOnDemandMemberInstance onDemandMemberInstance = (IOnDemandMemberInstance)iRIFReportDataScope.CurrentStreamingScopeInstance.Value();
				list = this.EvaluateSortAndGroupExpressionValues(onDemandMemberInstance.GroupExprValues);
				this.m_scopeID = new ScopeID((list == null) ? null : list.ToArray());
			}
			return this.m_scopeID;
		}

		internal override ScopeID GetLastScopeID()
		{
			return this.m_lastScopeID;
		}

		private List<ScopeValue> EvaluateSortAndGroupExpressionValues(List<object> groupExpressionValues)
		{
			if (this.m_memberGroupAndSortExpressionFlag == null)
			{
				return null;
			}
			return this.AddSortAndGroupExpressionValues(groupExpressionValues);
		}

		private List<ScopeValue> AddSortAndGroupExpressionValues(List<object> groupExpValues)
		{
			List<ScopeValue> list = new List<ScopeValue>(this.m_memberGroupAndSortExpressionFlag.Count);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.m_memberGroupAndSortExpressionFlag.Count; i++)
			{
				ScopeValue scopeValue = null;
				switch (this.m_memberGroupAndSortExpressionFlag[i])
				{
				case ScopeIDType.SortValues:
					scopeValue = this.CreateScopeValueFromSortExpression(num, this.m_memberGroupAndSortExpressionFlag[i]);
					num++;
					break;
				case ScopeIDType.GroupValues:
					scopeValue = new ScopeValue(this.NormalizeValue(groupExpValues[num2]), this.m_memberGroupAndSortExpressionFlag[i]);
					num2++;
					break;
				case ScopeIDType.SortGroup:
					scopeValue = ((groupExpValues.Count != 1) ? this.CreateScopeValueFromSortExpression(num, this.m_memberGroupAndSortExpressionFlag[i]) : new ScopeValue(this.NormalizeValue(groupExpValues[num2]), this.m_memberGroupAndSortExpressionFlag[i]));
					num2++;
					num++;
					break;
				}
				if (scopeValue != (ScopeValue)null)
				{
					list.Add(scopeValue);
				}
			}
			return list;
		}

		private ScopeValue CreateScopeValueFromSortExpression(int sortCursor, ScopeIDType scopeIdType)
		{
			RuntimeExpressionInfo runtimeExpression = new RuntimeExpressionInfo(this.m_sorting.SortExpressions, this.m_sorting.ExprHost, this.m_sorting.SortDirections, sortCursor);
			object value = this.m_odpContext.ReportRuntime.EvaluateRuntimeExpression(runtimeExpression, ObjectType.Grouping, this.m_memberDef.DataRegionMemberDefinition.Grouping.Name, "Sort");
			return new ScopeValue(this.NormalizeValue(value), scopeIdType);
		}

		private object NormalizeValue(object value)
		{
			if (value is DBNull)
			{
				return null;
			}
			return value;
		}

		protected void ResetScopeID()
		{
			this.m_scopeID = null;
		}
	}
}
