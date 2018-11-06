using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicatorCollection : GaugePanelObjectCollectionBase<StateIndicator>
	{
		private GaugePanel m_gaugePanel;

		public StateIndicator this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator stateIndicator = this.m_gaugePanel.GaugePanelDef.StateIndicators[i];
					if (string.CompareOrdinal(name, stateIndicator.Name) == 0)
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
				if (this.m_gaugePanel.GaugePanelDef.StateIndicators != null)
				{
					return this.m_gaugePanel.GaugePanelDef.StateIndicators.Count;
				}
				return 0;
			}
		}

		internal StateIndicatorCollection(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		protected override StateIndicator CreateGaugePanelObject(int index)
		{
			return new StateIndicator(this.m_gaugePanel.GaugePanelDef.StateIndicators[index], this.m_gaugePanel);
		}
	}
}
