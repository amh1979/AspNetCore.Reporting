using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRangeCollection : GaugePanelObjectCollectionBase<ScaleRange>
	{
		private GaugePanel m_gaugePanel;

		private GaugeScale m_gaugeScale;

		public ScaleRange this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ScaleRange scaleRange = this.m_gaugeScale.GaugeScaleDef.ScaleRanges[i];
					if (string.CompareOrdinal(name, scaleRange.Name) == 0)
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
				return this.m_gaugeScale.GaugeScaleDef.ScaleRanges.Count;
			}
		}

		internal ScaleRangeCollection(GaugeScale gaugeScale, GaugePanel gaugePanel)
		{
			this.m_gaugeScale = gaugeScale;
			this.m_gaugePanel = gaugePanel;
		}

		protected override ScaleRange CreateGaugePanelObject(int index)
		{
			return new ScaleRange(this.m_gaugeScale.GaugeScaleDef.ScaleRanges[index], this.m_gaugePanel);
		}
	}
}
