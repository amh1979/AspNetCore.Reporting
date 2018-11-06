namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLChart : RPLItem
	{
		internal RPLChart()
		{
			base.m_rplElementProps = new RPLChartProps();
			base.m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLChart(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
