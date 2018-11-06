using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointerCollection : GaugePanelObjectCollectionBase<RadialPointer>
	{
		private GaugePanel m_gaugePanel;

		private RadialScale m_radialScale;

		public RadialPointer this[string name]
		{
			get
			{
				for (int i = 0; i < this.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer radialPointer = this.m_radialScale.RadialScaleDef.GaugePointers[i];
					if (string.CompareOrdinal(name, radialPointer.Name) == 0)
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
				return this.m_radialScale.RadialScaleDef.GaugePointers.Count;
			}
		}

		internal RadialPointerCollection(RadialScale radialScale, GaugePanel gaugePanel)
		{
			this.m_radialScale = radialScale;
			this.m_gaugePanel = gaugePanel;
		}

		protected override RadialPointer CreateGaugePanelObject(int index)
		{
			return new RadialPointer(this.m_radialScale.RadialScaleDef.GaugePointers[index], this.m_gaugePanel);
		}
	}
}
