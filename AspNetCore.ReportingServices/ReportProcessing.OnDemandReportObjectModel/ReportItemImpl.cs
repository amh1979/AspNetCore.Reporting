using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class ReportItemImpl : AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.ReportItem
	{
		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem m_item;

		internal AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		internal IErrorContext m_iErrorContext;

		internal IScope m_scope;

		internal string Name
		{
			get
			{
				return this.m_item.Name;
			}
		}

		internal IScope Scope
		{
			set
			{
				this.m_scope = value;
			}
		}

		internal ReportItemImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem itemDef, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != itemDef, "(null != itemDef)");
			Global.Tracer.Assert(null != reportRT, "(null != reportRT)");
			Global.Tracer.Assert(null != iErrorContext, "(null != iErrorContext)");
			this.m_item = itemDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = iErrorContext;
		}

		internal abstract void Reset();

		internal abstract void Reset(AspNetCore.ReportingServices.RdlExpressions.VariantResult aResult);
	}
}
