using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartMember : ChartMember
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember m_memberDef;

		private IReportScope m_reportScope;

		private bool m_customPropertyCollectionReady;

		private string m_uniqueName;

		internal override string UniqueName
		{
			get
			{
				if (this.m_uniqueName == null)
				{
					this.m_uniqueName = this.m_memberDef.UniqueName;
				}
				return this.m_uniqueName;
			}
		}

		public override string ID
		{
			get
			{
				return this.m_memberDef.RenderingModelID;
			}
		}

		public override ReportStringProperty Label
		{
			get
			{
				if (base.m_label == null)
				{
					base.m_label = new ReportStringProperty(this.m_memberDef.Label);
				}
				return base.m_label;
			}
		}

		public override string DataElementName
		{
			get
			{
				return this.m_memberDef.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_memberDef.DataElementOutput;
			}
		}

		public override ChartMemberCollection Children
		{
			get
			{
				ChartMemberList chartMembers = this.m_memberDef.ChartMembers;
				if (chartMembers == null)
				{
					return null;
				}
				if (base.m_children == null)
				{
					base.m_children = new InternalChartMemberCollection(this, base.OwnerChart, this, chartMembers);
				}
				return base.m_children;
			}
		}

		public override bool IsStatic
		{
			get
			{
				if (this.m_memberDef.Grouping == null)
				{
					return true;
				}
				return false;
			}
		}

		public override bool IsCategory
		{
			get
			{
				return this.m_memberDef.IsColumn;
			}
		}

		public override int SeriesSpan
		{
			get
			{
				return this.m_memberDef.RowSpan;
			}
		}

		public override int CategorySpan
		{
			get
			{
				return this.m_memberDef.ColSpan;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_memberDef.MemberCellIndex;
			}
		}

		public override bool IsTotal
		{
			get
			{
				return this.m_memberDef.IsAutoSubtotal;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition
		{
			get
			{
				return this.m_memberDef;
			}
		}

		internal override IReportScope ReportScope
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope;
				}
				return this;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope.RIFReportScope;
				}
				return this.MemberDefinition;
			}
		}

		internal override IReportScopeInstance ReportScopeInstance
		{
			get
			{
				if (this.IsStatic)
				{
					return this.m_reportScope.ReportScopeInstance;
				}
				return (IReportScopeInstance)this.Instance;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customPropertyCollection == null)
				{
					string objectName = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerChart.Name;
					base.m_customPropertyCollection = new CustomPropertyCollection(this.ReportScope.ReportScopeInstance, base.OwnerChart.RenderingContext, null, this.m_memberDef, ObjectType.Chart, objectName);
					this.m_customPropertyCollectionReady = true;
				}
				else if (!this.m_customPropertyCollectionReady)
				{
					string objectName2 = (this.m_memberDef.Grouping != null) ? this.m_memberDef.Grouping.Name : base.OwnerChart.Name;
					base.m_customPropertyCollection.UpdateCustomProperties(this.ReportScope.ReportScopeInstance, this.m_memberDef, base.OwnerChart.RenderingContext.OdpContext, ObjectType.Chart, objectName2);
					this.m_customPropertyCollectionReady = true;
				}
				return base.m_customPropertyCollection;
			}
		}

		public override ChartMemberInstance Instance
		{
			get
			{
				if (base.OwnerChart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new ChartMemberInstance(base.OwnerChart, this);
					}
					else
					{
						ChartDynamicMemberInstance instance = new ChartDynamicMemberInstance(base.OwnerChart, this, this.BuildOdpMemberLogic(base.OwnerChart.RenderingContext.OdpContext));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal InternalChartMember(IReportScope reportScope, IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember memberDef, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
			this.m_memberDef = memberDef;
			if (this.m_memberDef.IsStatic)
			{
				this.m_reportScope = reportScope;
			}
			base.m_group = new Group(owner, this.m_memberDef, this);
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			if (!fromMoveNext && base.m_instance != null && !this.IsStatic)
			{
				((IDynamicInstance)base.m_instance).ResetContext();
			}
			base.SetNewContext(fromMoveNext);
			this.m_customPropertyCollectionReady = false;
			this.m_uniqueName = null;
		}
	}
}
