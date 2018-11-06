namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLMap : RPLItem
	{
		internal RPLMap()
		{
			base.m_rplElementProps = new RPLMapProps();
			base.m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLMap(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
