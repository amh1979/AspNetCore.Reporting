using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGaugeCollection : GaugePanelObjectCollectionBase<RadialGauge>
	{
		private GaugePanel m_gaugePanel;

		public RadialGauge this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge radialGauge = this.m_gaugePanel.GaugePanelDef.RadialGauges[i];
					if (string.CompareOrdinal(name, radialGauge.Name) == 0)
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
				if (this.m_gaugePanel.GaugePanelDef.RadialGauges != null)
				{
					return this.m_gaugePanel.GaugePanelDef.RadialGauges.Count;
				}
				return 0;
			}
		}

		internal RadialGaugeCollection(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		protected override RadialGauge CreateGaugePanelObject(int index)
		{
			return new RadialGauge(this.m_gaugePanel.GaugePanelDef.RadialGauges[index], this.m_gaugePanel);
		}
	}
}
