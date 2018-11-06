using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeImageCollection : GaugePanelObjectCollectionBase<GaugeImage>
	{
		private GaugePanel m_gaugePanel;

		public GaugeImage this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeImage gaugeImage = this.m_gaugePanel.GaugePanelDef.GaugeImages[i];
					if (string.CompareOrdinal(name, gaugeImage.Name) == 0)
					{
						return base[i];
					}
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsNotInCollection, name);
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_gaugePanel.GaugePanelDef.GaugeImages != null)
				{
					return this.m_gaugePanel.GaugePanelDef.GaugeImages.Count;
				}
				return 0;
			}
		}

		internal GaugeImageCollection(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		protected override GaugeImage CreateGaugePanelObject(int index)
		{
			return new GaugeImage(this.m_gaugePanel.GaugePanelDef.GaugeImages[index], this.m_gaugePanel);
		}
	}
}
