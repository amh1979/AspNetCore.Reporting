using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScaleCollection : GaugePanelObjectCollectionBase<RadialScale>
	{
		private GaugePanel m_gaugePanel;

		private RadialGauge m_radialGauge;

		public RadialScale this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale radialScale = this.m_radialGauge.RadialGaugeDef.GaugeScales[i];
					if (string.CompareOrdinal(name, radialScale.Name) == 0)
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
				return this.m_radialGauge.RadialGaugeDef.GaugeScales.Count;
			}
		}

		internal RadialScaleCollection(RadialGauge radialGauge, GaugePanel gaugePanel)
		{
			this.m_radialGauge = radialGauge;
			this.m_gaugePanel = gaugePanel;
		}

		protected override RadialScale CreateGaugePanelObject(int index)
		{
			return new RadialScale(this.m_radialGauge.RadialGaugeDef.GaugeScales[index], this.m_gaugePanel);
		}
	}
}
