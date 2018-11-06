using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSection : ReportItem
	{
		private bool m_printOnFirstPage;

		private bool m_printOnLastPage;

		private ReportItemCollection m_reportItems;

		private bool m_postProcessEvaluate;

		[NonSerialized]
		private bool m_isHeader;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		internal override ObjectType ObjectType
		{
			get
			{
				if (!this.m_isHeader)
				{
					return ObjectType.PageFooter;
				}
				return ObjectType.PageHeader;
			}
		}

		internal bool PrintOnFirstPage
		{
			get
			{
				return this.m_printOnFirstPage;
			}
			set
			{
				this.m_printOnFirstPage = value;
			}
		}

		internal bool PrintOnLastPage
		{
			get
			{
				return this.m_printOnLastPage;
			}
			set
			{
				this.m_printOnLastPage = value;
			}
		}

		internal ReportItemCollection ReportItems
		{
			get
			{
				return this.m_reportItems;
			}
			set
			{
				this.m_reportItems = value;
			}
		}

		internal bool PostProcessEvaluate
		{
			get
			{
				return this.m_postProcessEvaluate;
			}
			set
			{
				this.m_postProcessEvaluate = value;
			}
		}

		internal PageSection(bool isHeader, int id, int idForReportItems, Report report)
			: base(id, report)
		{
			this.m_reportItems = new ReportItemCollection(idForReportItems, true);
			this.m_isHeader = isHeader;
		}

		internal PageSection(bool isHeader, ReportItem parent)
			: base(parent)
		{
			this.m_isHeader = isHeader;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= LocationFlags.InPageSection;
			context.ObjectType = this.ObjectType;
			context.ObjectName = null;
			context.ExprHostBuilder.PageSectionStart();
			base.Initialize(context);
			this.m_reportItems.Initialize(context, true);
			base.ExprHostID = context.ExprHostBuilder.PageSectionEnd();
			return false;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.PageSectionHostsRemotable[base.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (base.m_styleClass != null)
				{
					base.m_styleClass.SetStyleExprHost(this.m_exprHost);
				}
			}
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PrintOnFirstPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PrintOnLastPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemCollection));
			memberInfoList.Add(new MemberInfo(MemberName.PostProcessEvaluate, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
