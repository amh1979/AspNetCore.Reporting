using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicatorCollection : GaugePanelObjectCollectionBase<NumericIndicator>
	{
		private GaugePanel m_gaugePanel;

		public NumericIndicator this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator numericIndicator = this.m_gaugePanel.GaugePanelDef.NumericIndicators[i];
					if (string.CompareOrdinal(name, numericIndicator.Name) == 0)
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
				if (this.m_gaugePanel.GaugePanelDef.NumericIndicators != null)
				{
					return this.m_gaugePanel.GaugePanelDef.NumericIndicators.Count;
				}
				return 0;
			}
		}

		internal NumericIndicatorCollection(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		protected override NumericIndicator CreateGaugePanelObject(int index)
		{
			return new NumericIndicator(this.m_gaugePanel.GaugePanelDef.NumericIndicators[index], this.m_gaugePanel);
		}
	}
}
