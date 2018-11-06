using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingInstance
	{
		private CustomReportItemHeadingInstanceList m_subHeadingInstances;

		[Reference]
		private CustomReportItemHeading m_headingDef;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private DataValueInstanceList m_customPropertyInstances;

		private string m_label;

		private VariantList m_groupExpressionValues;

		[NonSerialized]
		private int m_recursiveLevel = -1;

		internal CustomReportItemHeadingInstanceList SubHeadingInstances
		{
			get
			{
				return this.m_subHeadingInstances;
			}
			set
			{
				this.m_subHeadingInstances = value;
			}
		}

		internal CustomReportItemHeading HeadingDefinition
		{
			get
			{
				return this.m_headingDef;
			}
			set
			{
				this.m_headingDef = value;
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

		internal string Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal VariantList GroupExpressionValues
		{
			get
			{
				return this.m_groupExpressionValues;
			}
			set
			{
				this.m_groupExpressionValues = value;
			}
		}

		internal int RecursiveLevel
		{
			get
			{
				return this.m_recursiveLevel;
			}
		}

		internal CustomReportItemHeadingInstance(ReportProcessing.ProcessingContext pc, int headingCellIndex, CustomReportItemHeading headingDef, VariantList groupExpressionValues, int recursiveLevel)
		{
			if (headingDef.InnerHeadings != null)
			{
				this.m_subHeadingInstances = new CustomReportItemHeadingInstanceList();
			}
			this.m_headingDef = headingDef;
			this.m_headingCellIndex = headingCellIndex;
			if (groupExpressionValues != null)
			{
				this.m_groupExpressionValues = new VariantList(groupExpressionValues.Count);
				for (int i = 0; i < groupExpressionValues.Count; i++)
				{
					if (((ArrayList)groupExpressionValues)[i] == null || DBNull.Value == ((ArrayList)groupExpressionValues)[i])
					{
						this.m_groupExpressionValues.Add(null);
					}
					else
					{
						this.m_groupExpressionValues.Add(((ArrayList)groupExpressionValues)[i]);
					}
				}
			}
			if (headingDef.Grouping != null && headingDef.Grouping.GroupLabel != null)
			{
				this.m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(headingDef.Grouping, headingDef.DataRegionDef.ObjectType, headingDef.DataRegionDef.Name));
			}
			if (headingDef.CustomProperties != null)
			{
				this.m_customPropertyInstances = headingDef.CustomProperties.EvaluateExpressions(headingDef.DataRegionDef.ObjectType, headingDef.DataRegionDef.Name, "DataGrouping.", pc);
			}
			this.m_recursiveLevel = recursiveLevel;
		}

		internal CustomReportItemHeadingInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.SubHeadingInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingDefinition, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeading));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
