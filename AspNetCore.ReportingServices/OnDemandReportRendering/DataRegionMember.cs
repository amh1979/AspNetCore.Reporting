using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionMember : IDefinitionPath, IReportScope
	{
		protected IDefinitionPath m_parentDefinitionPath;

		protected int m_parentCollectionIndex;

		protected string m_definitionPath;

		protected ReportItem m_owner;

		protected Group m_group;

		protected DataRegionMember m_parent;

		protected CustomPropertyCollection m_customPropertyCollection;

		internal abstract string UniqueName
		{
			get;
		}

		public abstract string ID
		{
			get;
		}

		public string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(this.m_parentDefinitionPath, this.m_parentCollectionIndex);
				}
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_parentDefinitionPath;
			}
		}

		public Group Group
		{
			get
			{
				return this.m_group;
			}
		}

		public abstract bool IsStatic
		{
			get;
		}

		public virtual CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_customPropertyCollection == null)
				{
					this.m_customPropertyCollection = new CustomPropertyCollection();
				}
				return this.m_customPropertyCollection;
			}
		}

		public abstract int MemberCellIndex
		{
			get;
		}

		internal abstract IReportScope ReportScope
		{
			get;
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return this.ReportScopeInstance;
			}
		}

		internal abstract IReportScopeInstance ReportScopeInstance
		{
			get;
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return this.RIFReportScope;
			}
		}

		internal abstract IRIFReportScope RIFReportScope
		{
			get;
		}

		internal IDataRegion OwnerDataRegion
		{
			get
			{
				return (IDataRegion)this.m_owner;
			}
		}

		internal abstract IDataRegionMemberCollection SubMembers
		{
			get;
		}

		internal abstract ReportHierarchyNode DataRegionMemberDefinition
		{
			get;
		}

		internal DataRegionMember(IDefinitionPath parentDefinitionPath, ReportItem owner, DataRegionMember parent, int parentCollectionIndex)
		{
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_parentCollectionIndex = parentCollectionIndex;
		}

		internal abstract bool GetIsColumn();

		internal virtual void ResetContext()
		{
			if (this.m_group != null)
			{
				this.m_group.SetNewContext();
			}
		}

		internal virtual void SetNewContext(bool fromMoveNext)
		{
			if (this.m_group != null)
			{
				this.m_group.SetNewContext();
			}
			if (this.IsStatic || this.SubMembers == null || fromMoveNext)
			{
				if (this.SubMembers != null)
				{
					this.SubMembers.SetNewContext();
				}
				else
				{
					this.SetCellsNewContext();
				}
			}
			if (!fromMoveNext && this.DataRegionMemberDefinition != null)
			{
				this.DataRegionMemberDefinition.ClearStreamingScopeInstanceBinding();
			}
		}

		private void SetCellsNewContext()
		{
			if (this.OwnerDataRegion.HasDataCells)
			{
				IDataRegionRowCollection rowCollection = this.OwnerDataRegion.RowCollection;
				if (this.GetIsColumn())
				{
					for (int i = 0; i < rowCollection.Count; i++)
					{
						IDataRegionRow ifExists = rowCollection.GetIfExists(i);
						if (ifExists != null)
						{
							IDataRegionCell ifExists2 = ifExists.GetIfExists(this.MemberCellIndex);
							if (ifExists2 != null)
							{
								ifExists2.SetNewContext();
							}
						}
					}
				}
				else
				{
					IDataRegionRow ifExists3 = rowCollection.GetIfExists(this.MemberCellIndex);
					if (ifExists3 != null)
					{
						for (int j = 0; j < ifExists3.Count; j++)
						{
							IDataRegionCell ifExists4 = ifExists3.GetIfExists(j);
							if (ifExists4 != null)
							{
								ifExists4.SetNewContext();
							}
						}
					}
				}
			}
		}

		internal virtual InternalDynamicMemberLogic BuildOdpMemberLogic(OnDemandProcessingContext odpContext)
		{
			if (odpContext.StreamingMode)
			{
				return new InternalStreamingOdpDynamicMemberLogic(this, odpContext);
			}
			return new InternalFullOdpDynamicMemberLogic(this, odpContext);
		}
	}
}
