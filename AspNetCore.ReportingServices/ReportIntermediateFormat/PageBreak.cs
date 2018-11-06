using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class PageBreak : IPersistable
	{
		private PageBreakLocation m_pageBreakLocation;

		private ExpressionInfo m_disabled;

		private ExpressionInfo m_resetPageNumber;

		[NonSerialized]
		private PageBreakExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = PageBreak.GetDeclaration();

		internal PageBreakLocation BreakLocation
		{
			get
			{
				return this.m_pageBreakLocation;
			}
			set
			{
				this.m_pageBreakLocation = value;
			}
		}

		internal ExpressionInfo ResetPageNumber
		{
			get
			{
				return this.m_resetPageNumber;
			}
			set
			{
				this.m_resetPageNumber = value;
			}
		}

		internal ExpressionInfo Disabled
		{
			get
			{
				return this.m_disabled;
			}
			set
			{
				this.m_disabled = value;
			}
		}

		internal PageBreakExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
			set
			{
				this.m_exprHost = value;
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			PageBreak pageBreak = (PageBreak)base.MemberwiseClone();
			if (this.m_disabled != null)
			{
				pageBreak.m_disabled = (ExpressionInfo)this.m_disabled.PublishClone(context);
			}
			if (this.m_resetPageNumber != null)
			{
				pageBreak.m_resetPageNumber = (ExpressionInfo)this.m_resetPageNumber.PublishClone(context);
			}
			return pageBreak;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PageBreakStart();
			if (this.m_disabled != null)
			{
				this.m_disabled.Initialize("Disabled", context);
				context.ExprHostBuilder.Disabled(this.m_disabled);
			}
			if (this.m_resetPageNumber != null)
			{
				this.m_resetPageNumber.Initialize("ResetPageNumber", context);
				context.ExprHostBuilder.ResetPageNumber(this.m_resetPageNumber);
			}
			context.ExprHostBuilder.PageBreakEnd();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(PageBreak.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageBreakLocation:
					writer.WriteEnum((int)this.m_pageBreakLocation);
					break;
				case MemberName.Disabled:
					writer.Write(this.m_disabled);
					break;
				case MemberName.ResetPageNumber:
					writer.Write(this.m_resetPageNumber);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(PageBreak.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageBreakLocation:
					this.m_pageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.Disabled:
					this.m_disabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResetPageNumber:
					this.m_resetPageNumber = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "No references to resolve");
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.Disabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResetPageNumber, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal void SetExprHost(PageBreakExprHost pageBreakExpressionHost, ObjectModelImpl reportObjectModel)
		{
			if (pageBreakExpressionHost != null)
			{
				this.m_exprHost = pageBreakExpressionHost;
				Global.Tracer.Assert(this.m_exprHost != null && reportObjectModel != null);
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal bool EvaluateDisabled(IReportScopeInstance romInstance, OnDemandProcessingContext context, IPageBreakOwner pageBreakOwner)
		{
			context.SetupContext(pageBreakOwner.InstancePath, romInstance);
			return context.ReportRuntime.EvaluatePageBreakDisabledExpression(this, this.m_disabled, pageBreakOwner.ObjectType, pageBreakOwner.ObjectName);
		}

		internal bool EvaluateResetPageNumber(IReportScopeInstance romInstance, OnDemandProcessingContext context, IPageBreakOwner pageBreakOwner)
		{
			context.SetupContext(pageBreakOwner.InstancePath, romInstance);
			return context.ReportRuntime.EvaluatePageBreakResetPageNumberExpression(this, this.m_resetPageNumber, pageBreakOwner.ObjectType, pageBreakOwner.ObjectName);
		}
	}
}
