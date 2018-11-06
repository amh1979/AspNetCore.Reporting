namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLBody : RPLContainer
	{
		internal RPLBody()
		{
			base.m_rplElementProps = new RPLItemProps();
			base.m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLBody(RPLItemProps props)
			: base(props)
		{
		}

		internal RPLBody(long startOffset, RPLContext context, RPLItemMeasurement[] children)
			: base(startOffset, context, children)
		{
		}
	}
}
