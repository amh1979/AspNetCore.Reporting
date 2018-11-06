using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableRowInstance : InstanceInfoOwner, IShowHideContainer, ISearchByUniqueName
	{
		private int m_uniqueName;

		private ReportItemColInstance m_tableRowReportItemColInstance;

		[NonSerialized]
		[Reference]
		private TableRow m_tableRowDef;

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

		internal ReportItemColInstance TableRowReportItemColInstance
		{
			get
			{
				return this.m_tableRowReportItemColInstance;
			}
			set
			{
				this.m_tableRowReportItemColInstance = value;
			}
		}

		internal TableRow TableRowDef
		{
			get
			{
				return this.m_tableRowDef;
			}
			set
			{
				this.m_tableRowDef = value;
			}
		}

		internal TableRowInstance(ReportProcessing.ProcessingContext pc, TableRow rowDef, Table tableDef, IndexedExprHost visibilityHiddenExprHost)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			base.m_instanceInfo = new TableRowInstanceInfo(pc, rowDef, this, tableDef, visibilityHiddenExprHost);
			this.m_tableRowDef = rowDef;
			this.m_tableRowReportItemColInstance = new ReportItemColInstance(pc, rowDef.ReportItems);
		}

		internal TableRowInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)this.m_tableRowReportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(this.m_uniqueName, this.m_tableRowDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(this.m_uniqueName, this.m_tableRowDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.TableRowReportItemColInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal TableRowInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(null != chunkManager);
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadTableRowInstanceInfo();
			}
			return (TableRowInstanceInfo)base.m_instanceInfo;
		}
	}
}
