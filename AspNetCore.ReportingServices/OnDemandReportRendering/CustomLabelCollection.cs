using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomLabelCollection : GaugePanelObjectCollectionBase<CustomLabel>
	{
		private GaugePanel m_gaugePanel;

		private GaugeScale m_gaugeScale;

		public CustomLabel this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel customLabel = this.m_gaugeScale.GaugeScaleDef.CustomLabels[i];
					if (string.CompareOrdinal(name, customLabel.Name) == 0)
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
				return this.m_gaugeScale.GaugeScaleDef.CustomLabels.Count;
			}
		}

		internal CustomLabelCollection(GaugeScale gaugeScale, GaugePanel gaugePanel)
		{
			this.m_gaugeScale = gaugeScale;
			this.m_gaugePanel = gaugePanel;
		}

		protected override CustomLabel CreateGaugePanelObject(int index)
		{
			return new CustomLabel(this.m_gaugeScale.GaugeScaleDef.CustomLabels[index], this.m_gaugePanel);
		}
	}
}
