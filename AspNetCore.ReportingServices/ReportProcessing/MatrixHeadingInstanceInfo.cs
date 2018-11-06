using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class MatrixHeadingInstanceInfo : InstanceInfo
	{
		private NonComputedUniqueNames m_contentUniqueNames;

		private bool m_startHidden;

		private int m_headingCellIndex;

		private int m_headingSpan = 1;

		private object m_groupExpressionValue;

		private string m_label;

		private DataValueInstanceList m_customPropertyInstances;

		internal NonComputedUniqueNames ContentUniqueNames
		{
			get
			{
				return this.m_contentUniqueNames;
			}
			set
			{
				this.m_contentUniqueNames = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
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

		internal MatrixHeadingInstanceInfo(ReportProcessing.ProcessingContext pc, int headingCellIndex, MatrixHeading matrixHeadingDef, MatrixHeadingInstance owner, bool isSubtotal, int reportItemDefIndex, VariantList groupExpressionValues, out NonComputedUniqueNames nonComputedUniqueNames)
		{
			ReportItemCollection reportItems;
			if (isSubtotal)
			{
				reportItems = matrixHeadingDef.Subtotal.ReportItems;
			}
			else
			{
				reportItems = matrixHeadingDef.ReportItems;
				if (matrixHeadingDef.OwcGroupExpression)
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
			}
			if (0 < reportItems.Count && !reportItems.IsReportItemComputed(reportItemDefIndex))
			{
				this.m_contentUniqueNames = NonComputedUniqueNames.CreateNonComputedUniqueNames(pc, reportItems[reportItemDefIndex]);
			}
			nonComputedUniqueNames = this.m_contentUniqueNames;
			this.m_headingCellIndex = headingCellIndex;
			if (!isSubtotal && pc.ShowHideType != 0)
			{
				this.m_startHidden = pc.ProcessReceiver(owner.UniqueName, matrixHeadingDef.Visibility, matrixHeadingDef.ExprHost, matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name);
			}
			if (matrixHeadingDef.Grouping != null && matrixHeadingDef.Grouping.GroupLabel != null)
			{
				this.m_label = pc.NavigationInfo.RegisterLabel(pc.ReportRuntime.EvaluateGroupingLabelExpression(matrixHeadingDef.Grouping, matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name));
			}
			if (matrixHeadingDef.Grouping != null && matrixHeadingDef.Grouping.CustomProperties != null)
			{
				this.m_customPropertyInstances = matrixHeadingDef.Grouping.CustomProperties.EvaluateExpressions(matrixHeadingDef.DataRegionDef.ObjectType, matrixHeadingDef.DataRegionDef.Name, matrixHeadingDef.Grouping.Name + ".", pc);
			}
			matrixHeadingDef.StartHidden = this.m_startHidden;
		}

		internal MatrixHeadingInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ContentUniqueNames, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingCellIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeadingSpan, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.GroupExpressionValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
