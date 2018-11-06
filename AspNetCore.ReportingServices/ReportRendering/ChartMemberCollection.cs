using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ChartMemberCollection
	{
		private Chart m_owner;

		private ChartHeading m_headingDef;

		private ChartHeadingInstanceList m_headingInstances;

		private ChartMember[] m_members;

		private ChartMember m_firstMember;

		private ChartMember m_parent;

		public ChartMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					ChartMember chartMember = null;
					if (index == 0)
					{
						chartMember = this.m_firstMember;
					}
					else if (this.m_members != null)
					{
						chartMember = this.m_members[index - 1];
					}
					if (chartMember == null)
					{
						ChartHeadingInstance headingInstance = null;
						if (this.m_headingInstances != null && index < this.m_headingInstances.Count)
						{
							headingInstance = this.m_headingInstances[index];
						}
						chartMember = new ChartMember(this.m_owner, this.m_parent, this.m_headingDef, headingInstance, index);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (index == 0)
							{
								this.m_firstMember = chartMember;
							}
							else
							{
								if (this.m_members == null)
								{
									this.m_members = new ChartMember[this.Count - 1];
								}
								this.m_members[index - 1] = chartMember;
							}
						}
					}
					return chartMember;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_headingInstances != null && this.m_headingInstances.Count != 0)
				{
					return this.m_headingInstances.Count;
				}
				if (this.m_headingDef.Grouping == null && this.m_headingDef.Labels != null)
				{
					return this.m_headingDef.Labels.Count;
				}
				return 1;
			}
		}

		internal ChartMemberCollection(Chart owner, ChartMember parent, ChartHeading headingDef, ChartHeadingInstanceList headingInstances)
		{
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_headingInstances = headingInstances;
			this.m_headingDef = headingDef;
		}
	}
}
