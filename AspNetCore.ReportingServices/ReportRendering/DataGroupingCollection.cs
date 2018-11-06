using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataGroupingCollection
	{
		private CustomReportItem m_owner;

		private CustomReportItemHeadingList m_headingDef;

		private CustomReportItemHeadingInstanceList m_headingInstances;

		private DataMemberCollection[] m_collections;

		private DataMemberCollection m_firstCollection;

		private DataMember m_parent;

		public DataMemberCollection this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					DataMemberCollection dataMemberCollection = null;
					if (index == 0)
					{
						dataMemberCollection = this.m_firstCollection;
					}
					else if (this.m_collections != null)
					{
						dataMemberCollection = this.m_collections[index - 1];
					}
					if (dataMemberCollection == null)
					{
						bool headingDefIsStaticSubtotal = index > 0 && this.m_headingDef[index].Static && !this.m_headingDef[index].Subtotal && this.m_headingDef[index - 1].Subtotal;
						CustomReportItemHeadingInstanceList customReportItemHeadingInstanceList = null;
						if (this.m_headingDef[index].Static && this.m_headingInstances != null && this.m_headingInstances.Count > index)
						{
							customReportItemHeadingInstanceList = new CustomReportItemHeadingInstanceList(1);
							customReportItemHeadingInstanceList.Add(this.m_headingInstances[index]);
						}
						else
						{
							customReportItemHeadingInstanceList = this.m_headingInstances;
						}
						dataMemberCollection = new DataMemberCollection(this.m_owner, this.m_parent, this.m_headingDef[index], headingDefIsStaticSubtotal, customReportItemHeadingInstanceList);
						if (this.m_owner.UseCache)
						{
							if (index == 0)
							{
								this.m_firstCollection = dataMemberCollection;
							}
							else
							{
								if (this.m_collections == null)
								{
									this.m_collections = new DataMemberCollection[this.Count - 1];
								}
								this.m_collections[index - 1] = dataMemberCollection;
							}
						}
					}
					return dataMemberCollection;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_headingDef.Count;
			}
		}

		internal DataGroupingCollection(CustomReportItem owner, DataMember parent, CustomReportItemHeadingList headingDef, CustomReportItemHeadingInstanceList headingInstances)
		{
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_headingInstances = headingInstances;
			this.m_headingDef = headingDef;
		}
	}
}
