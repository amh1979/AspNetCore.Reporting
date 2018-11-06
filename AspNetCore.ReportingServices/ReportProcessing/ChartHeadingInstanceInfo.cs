using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ChartHeadingInstanceInfo : InstanceInfo
	{
		private object m_headingLabel;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private object m_groupExpressionValue;

		private int m_staticGroupingIndex = -1;

		private DataValueInstanceList m_customPropertyInstances;

		internal object HeadingLabel
		{
			get
			{
				return this.m_headingLabel;
			}
			set
			{
				this.m_headingLabel = value;
			}
		}

		internal int HeadingCellIndex
		{
			get
			{
				return this.m_headingCellIndex;
			}
			set
			{
				this.m_headingCellIndex = value;
			}
		}

		internal int HeadingSpan
		{
			get
			{
				return this.m_headingSpan;
			}
			set
			{
				this.m_headingSpan = value;
			}
		}

		internal object GroupExpressionValue
		{
			get
			{
				return this.m_groupExpressionValue;
			}
			set
			{
				this.m_groupExpressionValue = value;
			}
		}

		internal int StaticGroupingIndex
		{
			get
			{
				return this.m_staticGroupingIndex;
			}
			set
			{
				this.m_staticGroupingIndex = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return this.m_customPropertyInstances;
			}
			set
			{
				this.m_customPropertyInstances = value;
			}
		}

		internal ChartHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, ChartHeading chartHeadingDef, int labelIndex, VariantList groupExpressionValues)
		{
			this.m_headingCellIndex = headingCellIndex;
			if (chartHeadingDef.ChartGroupExpression)
			{
				if (groupExpressionValues == null || DBNull.Value == ((ArrayList)groupExpressionValues)[0])
				{
					this.m_groupExpressionValue = null;
				}
				else
				{
					this.m_groupExpressionValue = ((ArrayList)groupExpressionValues)[0];
				}
			}
			if (chartHeadingDef.Labels != null)
			{
				ExpressionInfo expressionInfo = chartHeadingDef.Labels[labelIndex];
				if (expressionInfo != null)
				{
					if (chartHeadingDef.Grouping != null)
					{
						this.m_headingLabel = pc.ReportRuntime.EvaluateChartDynamicHeadingLabelExpression(chartHeadingDef, expressionInfo, chartHeadingDef.DataRegionDef.Name);
					}
					else
					{
						this.m_headingLabel = pc.ReportRuntime.EvaluateChartStaticHeadingLabelExpression(chartHeadingDef, expressionInfo, chartHeadingDef.DataRegionDef.Name);
					}
				}
			}
			if (chartHeadingDef.Grouping == null)
			{
				this.m_staticGroupingIndex = labelIndex;
			}
			else if (chartHeadingDef.Grouping.CustomProperties != null)
			{
				this.m_customPropertyInstances = chartHeadingDef.Grouping.CustomProperties.EvaluateExpressions(chartHeadingDef.DataRegionDef.ObjectType, chartHeadingDef.DataRegionDef.Name, chartHeadingDef.Grouping.Name + ".", pc);
			}
		}

		internal ChartHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeadingLabel, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.StaticGroupingIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
