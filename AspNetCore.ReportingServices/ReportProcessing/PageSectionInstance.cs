using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSectionInstance : ReportItemInstance, IIndexInto
	{
		private int m_pageNumber;

		private ReportItemColInstance m_reportItemColInstance;

		internal int PageNumber
		{
			get
			{
				return this.m_pageNumber;
			}
			set
			{
				this.m_pageNumber = value;
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

		internal PageSectionInstance(ReportProcessing.ProcessingContext pc, int pageNumber, PageSection reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new PageSectionInstanceInfo(pc, reportItemDef, this);
			this.m_pageNumber = pageNumber;
			this.m_reportItemColInstance = new ReportItemColInstance(pc, reportItemDef.ReportItems);
		}

		internal PageSectionInstance()
		{
		}

		object IIndexInto.GetChildAt(int index, out NonComputedUniqueNames nonCompNames)
		{
			return ((IIndexInto)this.m_reportItemColInstance).GetChildAt(index, out nonCompNames);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadPageSectionInstanceInfo((PageSection)base.m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PageNumber, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItemColInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}
	}
}
