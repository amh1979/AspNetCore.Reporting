namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapViewInstance : BaseInstance
	{
		private MapView m_defObject;

		private double? m_zoom;

		public double Zoom
		{
			get
			{
				if (!this.m_zoom.HasValue)
				{
					this.m_zoom = this.m_defObject.MapViewDef.EvaluateZoom(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_zoom.Value;
			}
		}

		internal MapViewInstance(MapView defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_zoom = null;
		}
	}
}
