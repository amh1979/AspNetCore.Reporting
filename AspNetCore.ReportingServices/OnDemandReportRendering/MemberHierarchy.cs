namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MemberHierarchy<T> : IDefinitionPath
	{
		protected DataRegionMemberCollection<T> m_members;

		protected bool m_isColumn;

		protected ReportItem m_owner;

		protected string m_definitionPath;

		public string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = DefinitionPathConstants.GetTablixHierarchyDefinitionPath(this.m_owner, this.m_isColumn);
				}
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_owner;
			}
		}

		internal MemberHierarchy(ReportItem owner, bool isColumn)
		{
			this.m_owner = owner;
			this.m_isColumn = isColumn;
		}

		internal void SetNewContext()
		{
			if (this.m_members != null)
			{
				((IDataRegionMemberCollection)this.m_members).SetNewContext();
			}
		}

		internal abstract void ResetContext();
	}
}
