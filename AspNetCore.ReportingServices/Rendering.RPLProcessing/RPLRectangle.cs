namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLRectangle : RPLContainer
	{
		internal RPLRectangle()
		{
			base.m_rplElementProps = new RPLItemProps();
			base.m_rplElementProps.Definition = new RPLRectanglePropsDef();
		}

		internal RPLRectangle(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}

		internal RPLRectangle(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
