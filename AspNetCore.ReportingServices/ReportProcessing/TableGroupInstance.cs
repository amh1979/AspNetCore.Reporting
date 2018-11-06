using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private TableRowInstance[] m_headerRowInstances;

		private TableRowInstance[] m_footerRowInstances;

		private TableGroupInstanceList m_subGroupInstances;

		private TableDetailInstanceList m_tableDetailInstances;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		[Reference]
		private TableGroup m_tableGroupDef;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		internal int UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		internal TableGroup TableGroupDef
		{
			get
			{
				return this.m_tableGroupDef;
			}
			set
			{
				this.m_tableGroupDef = value;
			}
		}

		internal TableRowInstance[] HeaderRowInstances
		{
			get
			{
				return this.m_headerRowInstances;
			}
			set
			{
				this.m_headerRowInstances = value;
			}
		}

		internal TableRowInstance[] FooterRowInstances
		{
			get
			{
				return this.m_footerRowInstances;
			}
			set
			{
				this.m_footerRowInstances = value;
			}
		}

		internal TableGroupInstanceList SubGroupInstances
		{
			get
			{
				return this.m_subGroupInstances;
			}
			set
			{
				this.m_subGroupInstances = value;
			}
		}

		internal TableDetailInstanceList TableDetailInstances
		{
			get
			{
				return this.m_tableDetailInstances;
			}
			set
			{
				this.m_tableDetailInstances = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return this.m_renderingPages;
			}
			set
			{
				this.m_renderingPages = value;
			}
		}

		internal int NumberOfChildrenOnThisPage
		{
			get
			{
				return this.m_numberOfChildrenOnThisPage;
			}
			set
			{
				this.m_numberOfChildrenOnThisPage = value;
			}
		}

		internal TableGroupInstance(ReportProcessing.ProcessingContext pc, TableGroup tableGroupDef)
		{
			Table table = (Table)tableGroupDef.DataRegionDef;
			this.m_uniqueName = pc.CreateUniqueName();
			base.m_instanceInfo = new TableGroupInstanceInfo(pc, tableGroupDef, this);
			pc.Pagination.EnterIgnoreHeight(tableGroupDef.StartHidden);
			this.m_tableGroupDef = tableGroupDef;
			IndexedExprHost visibilityHiddenExprHost = (tableGroupDef.ExprHost != null) ? tableGroupDef.ExprHost.TableRowVisibilityHiddenExpressions : null;
			this.m_renderingPages = new RenderingPagesRangesList();
			if (tableGroupDef.HeaderRows != null)
			{
				this.m_headerRowInstances = new TableRowInstance[tableGroupDef.HeaderRows.Count];
				for (int i = 0; i < this.m_headerRowInstances.Length; i++)
				{
					this.m_headerRowInstances[i] = new TableRowInstance(pc, tableGroupDef.HeaderRows[i], table, visibilityHiddenExprHost);
				}
			}
			if (tableGroupDef.FooterRows != null)
			{
				this.m_footerRowInstances = new TableRowInstance[tableGroupDef.FooterRows.Count];
				for (int j = 0; j < this.m_footerRowInstances.Length; j++)
				{
					this.m_footerRowInstances[j] = new TableRowInstance(pc, tableGroupDef.FooterRows[j], table, visibilityHiddenExprHost);
				}
			}
			if (tableGroupDef.SubGroup != null)
			{
				this.m_subGroupInstances = new TableGroupInstanceList();
			}
			else if (table.TableDetail != null)
			{
				this.m_tableDetailInstances = new TableDetailInstanceList();
			}
		}

		internal TableGroupInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			if (this.m_headerRowInstances != null)
			{
				int num = this.m_headerRowInstances.Length;
				for (int i = 0; i < num; i++)
				{
					obj = ((ISearchByUniqueName)this.m_headerRowInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (this.m_subGroupInstances != null)
			{
				int count = this.m_subGroupInstances.Count;
				for (int j = 0; j < count; j++)
				{
					obj = ((ISearchByUniqueName)this.m_subGroupInstances[j]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			else if (this.m_tableDetailInstances != null)
			{
				int count2 = this.m_tableDetailInstances.Count;
				for (int k = 0; k < count2; k++)
				{
					obj = ((ISearchByUniqueName)this.m_tableDetailInstances[k]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			if (this.m_footerRowInstances != null)
			{
				int num2 = this.m_footerRowInstances.Length;
				for (int l = 0; l < num2; l++)
				{
					obj = ((ISearchByUniqueName)this.m_footerRowInstances[l]).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
			}
			return null;
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(this.m_uniqueName, this.m_tableGroupDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(this.m_uniqueName, this.m_tableGroupDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRowInstances, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRowInstances, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			memberInfoList.Add(new MemberInfo(MemberName.SubGroupInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableGroupInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailStartUniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableDetailInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableDetailInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableGroupInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadTableGroupInstanceInfo();
			}
			return (TableGroupInstanceInfo)base.m_instanceInfo;
		}
	}
}
