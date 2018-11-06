namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class ReportItemImpl : ReportItem
	{
		internal AspNetCore.ReportingServices.ReportProcessing.ReportItem m_item;

		internal ReportRuntime m_reportRT;

		internal IErrorContext m_iErrorContext;

		internal ReportProcessing.IScope m_scope;

		internal string Name
		{
			get
			{
				return this.m_item.Name;
			}
		}

		internal ReportProcessing.IScope Scope
		{
			set
			{
				this.m_scope = value;
			}
		}

		internal ReportItemImpl(AspNetCore.ReportingServices.ReportProcessing.ReportItem itemDef, ReportRuntime reportRT, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(null != itemDef, "(null != itemDef)");
			Global.Tracer.Assert(null != reportRT, "(null != reportRT)");
			Global.Tracer.Assert(null != iErrorContext, "(null != iErrorContext)");
			this.m_item = itemDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = iErrorContext;
		}
	}
}
