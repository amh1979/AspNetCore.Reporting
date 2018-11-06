using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScaleCollection : GaugePanelObjectCollectionBase<LinearScale>
	{
		private GaugePanel m_gaugePanel;

		private LinearGauge m_linearGauge;

		public LinearScale this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale linearScale = this.m_linearGauge.LinearGaugeDef.GaugeScales[i];
					if (string.CompareOrdinal(name, linearScale.Name) == 0)
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
				return this.m_linearGauge.LinearGaugeDef.GaugeScales.Count;
			}
		}

		internal LinearScaleCollection(LinearGauge linearGauge, GaugePanel gaugePanel)
		{
			this.m_linearGauge = linearGauge;
			this.m_gaugePanel = gaugePanel;
		}

		protected override LinearScale CreateGaugePanelObject(int index)
		{
			return new LinearScale(this.m_linearGauge.LinearGaugeDef.GaugeScales[index], this.m_gaugePanel);
		}
	}
}
