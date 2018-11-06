using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PlayAxis : Navigation
	{
		public Slider Slider
		{
			get
			{
				if (this.RIFPlayAxis.Slider == null)
				{
					return null;
				}
				return new Slider(this.RIFPlayAxis.Slider);
			}
		}

		public DockingOption DockingOption
		{
			get
			{
				return this.RIFPlayAxis.DockingOption;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PlayAxis RIFPlayAxis
		{
			get
			{
				return base.m_navigation as AspNetCore.ReportingServices.ReportIntermediateFormat.PlayAxis;
			}
		}

		internal PlayAxis(AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}
