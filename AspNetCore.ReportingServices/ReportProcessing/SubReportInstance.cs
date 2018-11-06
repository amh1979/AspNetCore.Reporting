using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SubReportInstance : ReportItemInstance, IPageItem
	{
		private ReportInstance m_reportInstance;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportInstance ReportInstance
		{
			get
			{
				return this.m_reportInstance;
			}
			set
			{
				this.m_reportInstance = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		internal SubReportInstance(ReportProcessing.ProcessingContext pc, SubReport reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new SubReportInstanceInfo(pc, reportItemDef, this, index);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
		}

		internal SubReportInstance()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (this.m_reportInstance == null)
			{
				return null;
			}
			return ((ISearchByUniqueName)this.m_reportInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadSubReportInstanceInfo((SubReport)base.m_reportItemDef);
		}
	}
}
