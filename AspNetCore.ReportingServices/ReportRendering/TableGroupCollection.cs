using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableGroupCollection
	{
		private Table m_owner;

		private AspNetCore.ReportingServices.ReportProcessing.TableGroup m_groupDef;

		private TableGroupInstanceList m_groupInstances;

		private TableGroup[] m_groups;

		private TableGroup m_firstGroup;

		private TableGroup m_parent;

		public TableGroup this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					TableGroup tableGroup = null;
					if (index == 0)
					{
						tableGroup = this.m_firstGroup;
					}
					else if (this.m_groups != null)
					{
						tableGroup = this.m_groups[index - 1];
					}
					if (tableGroup == null)
					{
						TableGroupInstance groupInstance = null;
						if (this.m_groupInstances != null && index < this.m_groupInstances.Count)
						{
							groupInstance = this.m_groupInstances[index];
						}
						else
						{
							Global.Tracer.Assert(this.m_groupInstances == null || 0 == this.m_groupInstances.Count);
						}
						tableGroup = new TableGroup(this.m_owner, this.m_parent, this.m_groupDef, groupInstance);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (index == 0)
							{
								this.m_firstGroup = tableGroup;
							}
							else
							{
								if (this.m_groups == null)
								{
									this.m_groups = new TableGroup[this.m_groupInstances.Count - 1];
								}
								this.m_groups[index - 1] = tableGroup;
							}
						}
					}
					return tableGroup;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_groupInstances != null && this.m_groupInstances.Count != 0)
				{
					return this.m_groupInstances.Count;
				}
				return 1;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.TableGroup GroupDefinition
		{
			get
			{
				return this.m_groupDef;
			}
		}

		internal TableGroupCollection(Table owner, TableGroup parent, AspNetCore.ReportingServices.ReportProcessing.TableGroup groupDef, TableGroupInstanceList groupInstances)
		{
			this.m_owner = owner;
			this.m_parent = parent;
			this.m_groupInstances = groupInstances;
			this.m_groupDef = groupDef;
		}
	}
}
