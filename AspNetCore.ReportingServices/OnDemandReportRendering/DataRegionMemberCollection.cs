namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionMemberCollection<T> : ReportElementCollectionBase<T>, IDefinitionPath, IDataRegionMemberCollection
	{
		protected DataRegionMember[] m_children;

		protected IDefinitionPath m_parentDefinitionPath;

		protected ReportItem m_owner;

		public abstract string DefinitionPath
		{
			get;
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_parentDefinitionPath;
			}
		}

		internal DataRegionMemberCollection(IDefinitionPath parentDefinitionPath, ReportItem owner)
		{
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_owner = owner;
		}

		void IDataRegionMemberCollection.SetNewContext()
		{
			if (this.m_children != null)
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (this.m_children[i] != null)
					{
						this.m_children[i].SetNewContext(false);
					}
				}
			}
		}
	}
}
