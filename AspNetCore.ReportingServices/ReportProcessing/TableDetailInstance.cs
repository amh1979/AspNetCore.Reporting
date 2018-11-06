using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetailInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private TableRowInstance[] m_detailRowInstances;

		[NonSerialized]
		[Reference]
		private TableDetail m_tableDetailDef;

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

		internal TableDetail TableDetailDef
		{
			get
			{
				return this.m_tableDetailDef;
			}
			set
			{
				this.m_tableDetailDef = value;
			}
		}

		internal TableRowInstance[] DetailRowInstances
		{
			get
			{
				return this.m_detailRowInstances;
			}
			set
			{
				this.m_detailRowInstances = value;
			}
		}

		internal TableDetailInstance(ReportProcessing.ProcessingContext pc, TableDetail tableDetailDef, Table tableDef)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			base.m_instanceInfo = new TableDetailInstanceInfo(pc, tableDetailDef, this, tableDef);
			pc.Pagination.EnterIgnoreHeight(tableDetailDef.StartHidden);
			this.m_tableDetailDef = tableDetailDef;
			if (tableDetailDef.DetailRows != null)
			{
				IndexedExprHost visibilityHiddenExprHost = (tableDetailDef.ExprHost != null) ? tableDetailDef.ExprHost.TableRowVisibilityHiddenExpressions : null;
				this.m_detailRowInstances = new TableRowInstance[tableDetailDef.DetailRows.Count];
				for (int i = 0; i < this.m_detailRowInstances.Length; i++)
				{
					this.m_detailRowInstances[i] = new TableRowInstance(pc, tableDetailDef.DetailRows[i], tableDef, visibilityHiddenExprHost);
				}
			}
		}

		internal TableDetailInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			if (this.m_detailRowInstances != null)
			{
				int num = this.m_detailRowInstances.Length;
				for (int i = 0; i < num; i++)
				{
					obj = ((ISearchByUniqueName)this.m_detailRowInstances[i]).Find(targetUniqueName, ref nonCompNames, chunkManager);
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
			context.BeginProcessContainer(this.m_uniqueName, this.m_tableDetailDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(this.m_uniqueName, this.m_tableDetailDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DetailRowInstances, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableDetailInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadTableDetailInstanceInfo();
			}
			return (TableDetailInstanceInfo)base.m_instanceInfo;
		}
	}
}
