using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListContentInstance : InstanceInfoOwner, ISearchByUniqueName, IShowHideContainer
	{
		private int m_uniqueName;

		private ReportItemColInstance m_reportItemColInstance;

		[NonSerialized]
		[Reference]
		private List m_listDef;

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

		internal List ListDef
		{
			get
			{
				return this.m_listDef;
			}
			set
			{
				this.m_listDef = value;
			}
		}

		internal ReportItemColInstance ReportItemColInstance
		{
			get
			{
				return this.m_reportItemColInstance;
			}
			set
			{
				this.m_reportItemColInstance = value;
			}
		}

		internal ListContentInstance(ReportProcessing.ProcessingContext pc, List listDef)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			this.m_listDef = listDef;
			this.m_reportItemColInstance = new ReportItemColInstance(pc, listDef.ReportItems);
			base.m_instanceInfo = new ListContentInstanceInfo(pc, this, listDef);
			pc.Pagination.EnterIgnoreHeight(listDef.StartHidden);
		}

		internal ListContentInstance()
		{
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)this.m_reportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(this.m_uniqueName, this.m_listDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(this.m_uniqueName, this.m_listDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}

		internal ListContentInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadListContentInstanceInfo();
			}
			return (ListContentInstanceInfo)base.m_instanceInfo;
		}
	}
}
