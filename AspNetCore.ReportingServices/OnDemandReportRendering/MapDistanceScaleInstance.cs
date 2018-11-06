using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDistanceScaleInstance : MapDockableSubItemInstance
	{
		private MapDistanceScale m_defObject;

		private ReportColor m_scaleColor;

		private ReportColor m_scaleBorderColor;

		public ReportColor ScaleColor
		{
			get
			{
				if (this.m_scaleColor == null)
				{
					this.m_scaleColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale)this.m_defObject.MapDockableSubItemDef).EvaluateScaleColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_scaleColor;
			}
		}

		public ReportColor ScaleBorderColor
		{
			get
			{
				if (this.m_scaleBorderColor == null)
				{
					this.m_scaleBorderColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDistanceScale)this.m_defObject.MapDockableSubItemDef).EvaluateScaleBorderColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_scaleBorderColor;
			}
		}

		internal MapDistanceScaleInstance(MapDistanceScale defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_scaleColor = null;
			this.m_scaleBorderColor = null;
		}
	}
}
