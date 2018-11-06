using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartMember : DataRegionMember
	{
		protected ChartMemberCollection m_children;

		protected ChartMemberInstance m_instance;

		protected ReportStringProperty m_label;

		protected ChartSeries m_chartSeries;

		public ChartMember Parent
		{
			get
			{
				return base.m_parent as ChartMember;
			}
		}

		public abstract ReportStringProperty Label
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
		}

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract ChartMemberCollection Children
		{
			get;
		}

		public abstract bool IsCategory
		{
			get;
		}

		public abstract int SeriesSpan
		{
			get;
		}

		public abstract int CategorySpan
		{
			get;
		}

		public abstract bool IsTotal
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.ChartMember MemberDefinition
		{
			get;
		}

		internal override ReportHierarchyNode DataRegionMemberDefinition
		{
			get
			{
				return this.MemberDefinition;
			}
		}

		internal Chart OwnerChart
		{
			get
			{
				return base.m_owner as Chart;
			}
		}

		public abstract ChartMemberInstance Instance
		{
			get;
		}

		internal override IDataRegionMemberCollection SubMembers
		{
			get
			{
				return this.m_children;
			}
		}

		private ChartSeries ChartSeries
		{
			get
			{
				if (!this.IsCategory && this.Children == null)
				{
					if (this.m_chartSeries == null)
					{
						this.m_chartSeries = ((ReportElementCollectionBase<ChartSeries>)((Chart)base.m_owner).ChartData.SeriesCollection)[this.MemberCellIndex];
					}
					return this.m_chartSeries;
				}
				return null;
			}
		}

		internal ChartMember(IDefinitionPath parentDefinitionPath, Chart owner, ChartMember parent, int parentCollectionIndex)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex)
		{
		}

		internal override bool GetIsColumn()
		{
			return this.IsCategory;
		}

		internal override void SetNewContext(bool fromMoveNext)
		{
			base.SetNewContext(fromMoveNext);
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			ChartSeries chartSeries = this.ChartSeries;
			if (chartSeries != null)
			{
				chartSeries.SetNewContext();
			}
		}
	}
}
