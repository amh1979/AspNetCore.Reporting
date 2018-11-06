using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupInstance : BaseInstance
	{
		private string m_uniqueName;

		private bool m_documentMapLabelEvaluated;

		private string m_documentMapLabel;

		private GroupExpressionValueCollection m_groupExpressions;

		private Group m_owner;

		private int m_recursiveLevel = -1;

		private bool m_pageNameEvaluated;

		private string m_pageName;

		public string UniqueName
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					if (this.m_owner.CurrentRenderGroupIndex >= 0)
					{
						if (this.m_owner.IsDetailGroup)
						{
							return this.m_owner.TableDetailMember.DetailInstanceUniqueName;
						}
						return this.m_owner.CurrentShimRenderGroup.UniqueName;
					}
					return string.Empty;
				}
				if (this.m_uniqueName == null)
				{
					this.m_uniqueName = InstancePathItem.GenerateUniqueNameString(this.m_owner.MemberDefinition.ID, this.m_owner.MemberDefinition.InstancePath);
				}
				return this.m_uniqueName;
			}
		}

		public string DocumentMapLabel
		{
			get
			{
				if (!this.m_documentMapLabelEvaluated)
				{
					this.m_documentMapLabelEvaluated = true;
					if (this.m_owner.IsOldSnapshot)
					{
						if (!this.m_owner.IsDetailGroup && this.m_owner.CurrentRenderGroupIndex >= 0)
						{
							this.m_documentMapLabel = this.m_owner.CurrentShimRenderGroup.Label;
						}
					}
					else if (this.m_owner.MemberDefinition.Grouping != null && this.m_owner.MemberDefinition.Grouping.GroupLabel != null)
					{
						this.m_documentMapLabel = this.m_owner.MemberDefinition.Grouping.EvaluateGroupingLabelExpression(this.ReportScopeInstance, this.m_owner.OwnerDataRegion.RenderingContext.OdpContext);
					}
				}
				return this.m_documentMapLabel;
			}
		}

		public GroupExpressionValueCollection GroupExpressions
		{
			get
			{
				if (this.m_groupExpressions == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (!this.m_owner.IsDetailGroup && this.m_owner.OwnerDataRegion != null && this.m_owner.CurrentRenderGroupIndex >= 0 && this.m_owner.OwnerDataRegion.DataRegionType == DataRegion.Type.Chart)
						{
							ChartHeadingInstanceInfo instanceInfo = ((AspNetCore.ReportingServices.ReportRendering.ChartMember)this.m_owner.CurrentShimRenderGroup).InstanceInfo;
							if (instanceInfo != null)
							{
								if (this.m_groupExpressions == null)
								{
									this.m_groupExpressions = new GroupExpressionValueCollection();
								}
								this.m_groupExpressions.UpdateValues(instanceInfo.GroupExpressionValue);
							}
						}
					}
					else if (!this.m_owner.IsDetailGroup && this.m_owner.OwnerDataRegion != null && this.m_owner.MemberDefinition.CurrentMemberIndex >= 0)
					{
						object[] groupInstanceExpressionValues = this.m_owner.MemberDefinition.Grouping.GetGroupInstanceExpressionValues(this.ReportScopeInstance, this.m_owner.OwnerDataRegion.RenderingContext.OdpContext);
						if (this.m_groupExpressions == null)
						{
							this.m_groupExpressions = new GroupExpressionValueCollection();
						}
						this.m_groupExpressions.UpdateValues(groupInstanceExpressionValues);
					}
				}
				return this.m_groupExpressions;
			}
		}

		public int RecursiveLevel
		{
			get
			{
				if (this.m_recursiveLevel < 0 && !this.m_owner.IsOldSnapshot)
				{
					this.m_recursiveLevel = this.m_owner.MemberDefinition.Grouping.GetRecursiveLevel(this.ReportScopeInstance, this.m_owner.OwnerDataRegion.RenderingContext.OdpContext);
				}
				return this.m_recursiveLevel;
			}
		}

		public string PageName
		{
			get
			{
				if (!this.m_pageNameEvaluated)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						this.m_pageName = null;
					}
					else
					{
						this.m_pageNameEvaluated = true;
						AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = this.m_owner.MemberDefinition.Grouping;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo pageName = grouping.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								this.m_pageName = grouping.EvaluatePageName(this.ReportScopeInstance, this.m_owner.OwnerDataRegion.RenderingContext.OdpContext);
							}
							else
							{
								this.m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return this.m_pageName;
			}
		}

		internal GroupInstance(Group owner)
			: base(null)
		{
			this.m_owner = owner;
		}

		internal GroupInstance(Group owner, IReportScope reportScope)
			: base(reportScope)
		{
			this.m_owner = owner;
		}

		protected override void ResetInstanceCache()
		{
			this.m_uniqueName = null;
			this.m_documentMapLabelEvaluated = false;
			this.m_documentMapLabel = null;
			this.m_groupExpressions = null;
			this.m_recursiveLevel = -1;
			this.m_pageNameEvaluated = false;
			this.m_pageName = null;
			if (!this.m_owner.IsOldSnapshot && this.m_owner.MemberDefinition.Grouping != null)
			{
				this.m_owner.MemberDefinition.Grouping.ResetReportItemsWithHideDuplicates();
			}
		}
	}
}
