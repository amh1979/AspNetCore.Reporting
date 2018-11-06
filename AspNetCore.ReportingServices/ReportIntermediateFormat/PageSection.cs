using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class PageSection : ReportItem, IPersistable
	{
		private bool m_printOnFirstPage;

		private bool m_printOnLastPage;

		private bool m_printBetweenSections;

		private ReportItemCollection m_reportItems;

		private bool m_postProcessEvaluate;

		[NonSerialized]
		private bool m_isHeader;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = PageSection.GetDeclaration();

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				if (!this.m_isHeader)
				{
					return AspNetCore.ReportingServices.ReportProcessing.ObjectType.PageFooter;
				}
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.PageHeader;
			}
		}

		internal bool IsHeader
		{
			get
			{
				return this.m_isHeader;
			}
			set
			{
				this.m_isHeader = value;
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

		internal bool PrintBetweenSections
		{
			get
			{
				return this.m_printBetweenSections;
			}
			set
			{
				this.m_printBetweenSections = value;
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

		internal bool UpgradedSnapshotPostProcessEvaluate
		{
			get
			{
				return this.m_postProcessEvaluate;
			}
		}

		internal PageSection(bool isHeader, int id, int idForReportItems, ReportSection reportSection)
			: base(id, reportSection)
		{
			this.m_reportItems = new ReportItemCollection(idForReportItems, true);
			this.m_isHeader = isHeader;
		}

		internal PageSection(ReportItem parent)
			: base(parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection;
			context.ObjectType = this.ObjectType;
			context.ObjectName = null;
			context.ExprHostBuilder.PageSectionStart();
			base.Initialize(context);
			this.m_reportItems.Initialize(context);
			base.ExprHostID = context.ExprHostBuilder.PageSectionEnd();
			return false;
		}

		protected override void DataRendererInitialize(InitializationContext context)
		{
		}

		[SkipMemberStaticValidation(MemberName.PostProcessEvaluate)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PrintOnFirstPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PrintOnLastPage, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ReportItems, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemCollection));
			list.Add(new ReadOnlyMemberInfo(MemberName.PostProcessEvaluate, Token.Boolean));
			list.Add(new MemberInfo(MemberName.PrintBetweenSections, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PageSection.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PrintOnFirstPage:
					writer.Write(this.m_printOnFirstPage);
					break;
				case MemberName.PrintOnLastPage:
					writer.Write(this.m_printOnLastPage);
					break;
				case MemberName.ReportItems:
					writer.Write(this.m_reportItems);
					break;
				case MemberName.PrintBetweenSections:
					writer.Write(this.m_printBetweenSections);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(PageSection.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PrintOnFirstPage:
					this.m_printOnFirstPage = reader.ReadBoolean();
					break;
				case MemberName.PrintOnLastPage:
					this.m_printOnLastPage = reader.ReadBoolean();
					break;
				case MemberName.ReportItems:
					this.m_reportItems = (ReportItemCollection)reader.ReadRIFObject();
					break;
				case MemberName.PostProcessEvaluate:
					this.m_postProcessEvaluate = reader.ReadBoolean();
					break;
				case MemberName.PrintBetweenSections:
					this.m_printBetweenSections = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			if (base.m_name == null)
			{
				if (this.IsHeader)
				{
					base.m_name = "PageHeader";
				}
				else
				{
					base.m_name = "PageFooter";
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageSection;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.PageSectionHostsRemotable[base.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (base.m_styleClass != null)
				{
					base.m_styleClass.SetStyleExprHost(this.m_exprHost);
				}
			}
		}
	}
}
