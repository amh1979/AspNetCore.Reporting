namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLHeaderFooter : RPLContainer
	{
		internal RPLHeaderFooter()
		{
			base.m_rplElementProps = new RPLItemProps();
			base.m_rplElementProps.Definition = new RPLHeaderFooterPropsDef();
		}

		internal RPLHeaderFooter(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}
	}
}
