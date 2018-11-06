using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Slider
	{
		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Slider m_slider;

		public bool Hidden
		{
			get
			{
				return this.m_slider.Hidden;
			}
		}

		public LabelData LabelData
		{
			get
			{
				if (this.m_slider.LabelData == null)
				{
					return null;
				}
				return new LabelData(this.m_slider.LabelData);
			}
		}

		internal Slider(AspNetCore.ReportingServices.ReportIntermediateFormat.Slider slider)
		{
			this.m_slider = slider;
		}
	}
}
