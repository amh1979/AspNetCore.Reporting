using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Coverflow : Navigation
	{
		private NavigationItem m_navigationItem;

		private Slider m_slider;

		public NavigationItem NavigationItem
		{
			get
			{
				if (this.m_navigationItem == null && this.RIFCoverflow.NavigationItem != null)
				{
					this.m_navigationItem = new NavigationItem(this.RIFCoverflow.NavigationItem);
				}
				return this.m_navigationItem;
			}
		}

		public Slider Slider
		{
			get
			{
				if (this.m_slider == null && this.RIFCoverflow.Slider != null)
				{
					this.m_slider = new Slider(this.RIFCoverflow.Slider);
				}
				return this.m_slider;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Coverflow RIFCoverflow
		{
			get
			{
				return base.m_navigation as AspNetCore.ReportingServices.ReportIntermediateFormat.Coverflow;
			}
		}

		internal Coverflow(AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}
