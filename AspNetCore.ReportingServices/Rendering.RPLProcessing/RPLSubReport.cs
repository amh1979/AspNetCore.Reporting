namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLSubReport : RPLContainer
	{
		internal RPLSubReport()
		{
			base.m_rplElementProps = new RPLSubReportProps();
			base.m_rplElementProps.Definition = new RPLSubReportPropsDef();
		}

		internal RPLSubReport(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}

		internal RPLSubReport(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
