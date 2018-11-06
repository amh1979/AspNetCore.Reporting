namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLGaugePanel : RPLItem
	{
		internal RPLGaugePanel()
		{
			base.m_rplElementProps = new RPLGaugePanelProps();
			base.m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLGaugePanel(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
