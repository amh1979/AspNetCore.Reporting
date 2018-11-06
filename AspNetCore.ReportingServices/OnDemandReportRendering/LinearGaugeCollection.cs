using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGaugeCollection : GaugePanelObjectCollectionBase<LinearGauge>
	{
		private GaugePanel m_gaugePanel;

		public LinearGauge this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge linearGauge = this.m_gaugePanel.GaugePanelDef.LinearGauges[i];
					if (string.CompareOrdinal(name, linearGauge.Name) == 0)
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
				if (this.m_gaugePanel.GaugePanelDef.LinearGauges != null)
				{
					return this.m_gaugePanel.GaugePanelDef.LinearGauges.Count;
				}
				return 0;
			}
		}

		internal LinearGaugeCollection(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		protected override LinearGauge CreateGaugePanelObject(int index)
		{
			return new LinearGauge(this.m_gaugePanel.GaugePanelDef.LinearGauges[index], this.m_gaugePanel);
		}
	}
}
