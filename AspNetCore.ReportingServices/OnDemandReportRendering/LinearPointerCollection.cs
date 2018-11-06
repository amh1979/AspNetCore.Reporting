using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointerCollection : GaugePanelObjectCollectionBase<LinearPointer>
	{
		private GaugePanel m_gaugePanel;

		private LinearScale m_linearScale;

		public LinearPointer this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer linearPointer = this.m_linearScale.LinearScaleDef.GaugePointers[i];
					if (string.CompareOrdinal(name, linearPointer.Name) == 0)
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
				return this.m_linearScale.LinearScaleDef.GaugePointers.Count;
			}
		}

		internal LinearPointerCollection(LinearScale linearScale, GaugePanel gaugePanel)
		{
			this.m_linearScale = linearScale;
			this.m_gaugePanel = gaugePanel;
		}

		protected override LinearPointer CreateGaugePanelObject(int index)
		{
			return new LinearPointer(this.m_linearScale.LinearScaleDef.GaugePointers[index], this.m_gaugePanel);
		}
	}
}
