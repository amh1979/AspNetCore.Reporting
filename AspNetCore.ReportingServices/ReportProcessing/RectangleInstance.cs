using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RectangleInstance : ReportItemInstance, IShowHideContainer, IIndexInto, IPageItem
	{
		private ReportItemColInstance m_reportItemColInstance;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

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

		internal RectangleInstance(ReportProcessing.ProcessingContext pc, Rectangle reportItemDef, int index)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new RectangleInstanceInfo(pc, reportItemDef, this, index);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			this.m_reportItemColInstance = new ReportItemColInstance(pc, reportItemDef.ReportItems);
		}

		internal RectangleInstance()
		{
		}

		internal override int GetDocumentMapUniqueName()
		{
			int linkToChild = ((Rectangle)base.m_reportItemDef).LinkToChild;
			if (linkToChild >= 0)
			{
				return this.m_reportItemColInstance.GetReportItemUniqueName(linkToChild);
			}
			return base.m_uniqueName;
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			return ((IIndexInto)this.m_reportItemColInstance).GetChildAt(index, out nonCompNames);
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			return ((ISearchByUniqueName)this.m_reportItemColInstance).Find(targetUniqueName, ref nonCompNames, chunkManager);
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadRectangleInstanceInfo((Rectangle)base.m_reportItemDef);
		}
	}
}
