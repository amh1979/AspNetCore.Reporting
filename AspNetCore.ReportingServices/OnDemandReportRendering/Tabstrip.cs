using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Tabstrip : Navigation
	{
		private NavigationItem m_navigationItem;

		private Slider m_slider;

		public NavigationItem NavigationItem
		{
			get
			{
				if (this.m_navigationItem == null && this.RIFTabstrip.NavigationItem != null)
				{
					this.m_navigationItem = new NavigationItem(this.RIFTabstrip.NavigationItem);
				}
				return this.m_navigationItem;
			}
		}

		public Slider Slider
		{
			get
			{
				if (this.m_slider == null && this.RIFTabstrip.Slider != null)
				{
					this.m_slider = new Slider(this.RIFTabstrip.Slider);
				}
				return this.m_slider;
			}
		}

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Tabstrip RIFTabstrip
		{
			get
			{
				return base.m_navigation as AspNetCore.ReportingServices.ReportIntermediateFormat.Tabstrip;
			}
		}

		internal Tabstrip(AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}
