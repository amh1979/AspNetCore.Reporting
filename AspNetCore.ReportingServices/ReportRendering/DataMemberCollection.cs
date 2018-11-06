using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataMemberCollection
	{
		private CustomReportItem m_owner;

		private CustomReportItemHeading m_headingDef;

		private CustomReportItemHeadingInstanceList m_headingInstances;

		private DataMember[] m_members;

		private DataMember m_firstMember;

		private DataMember m_parent;

		private bool m_isSubtotal;

		public DataMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					DataMember dataMember = null;
					if (index == 0)
					{
						dataMember = this.m_firstMember;
					}
					else if (this.m_members != null)
					{
						dataMember = this.m_members[index - 1];
					}
					if (dataMember == null)
					{
						CustomReportItemHeadingInstance headingInstance = null;
						if (this.m_headingInstances != null && index < this.m_headingInstances.Count)
						{
							headingInstance = this.m_headingInstances[index];
						}
						dataMember = new DataMember(this.m_owner, this.m_parent, this.m_headingDef, headingInstance, this.m_isSubtotal, index);
						if (this.m_owner.UseCache)
						{
							if (index == 0)
							{
								this.m_firstMember = dataMember;
							}
							else
							{
								if (this.m_members == null)
								{
									this.m_members = new DataMember[this.Count - 1];
								}
								this.m_members[index - 1] = dataMember;
							}
						}
					}
					return dataMember;
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
				return 1;
			}
		}

		internal DataMemberCollection(CustomReportItem owner, DataMember parent, CustomReportItemHeading headingDef, bool headingDefIsStaticSubtotal, CustomReportItemHeadingInstanceList headingInstances)
		{
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_headingInstances = headingInstances;
			this.m_isSubtotal = headingDefIsStaticSubtotal;
			this.m_headingDef = headingDef;
		}
	}
}
