using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorStateCollection : GaugePanelObjectCollectionBase<IndicatorState>
	{
		private StateIndicator m_stateIndicator;

		private GaugePanel m_gaugePanel;

		public IndicatorState this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState indicatorState = this.m_stateIndicator.StateIndicatorDef.IndicatorStates[i];
					if (string.CompareOrdinal(name, indicatorState.Name) == 0)
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
				if (this.m_stateIndicator.StateIndicatorDef.IndicatorStates != null)
				{
					return this.m_stateIndicator.StateIndicatorDef.IndicatorStates.Count;
				}
				return 0;
			}
		}

		internal IndicatorStateCollection(StateIndicator stateIndicator, GaugePanel gaugePanel)
		{
			this.m_stateIndicator = stateIndicator;
			this.m_gaugePanel = gaugePanel;
		}

		protected override IndicatorState CreateGaugePanelObject(int index)
		{
			return new IndicatorState(this.m_stateIndicator.StateIndicatorDef.IndicatorStates[index], this.m_gaugePanel);
		}
	}
}
