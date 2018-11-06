namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLLine : RPLItem
	{
		internal RPLLine()
		{
			base.m_rplElementProps = new RPLLineProps();
			base.m_rplElementProps.Definition = new RPLLinePropsDef();
		}

		internal RPLLine(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
