namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapLayerInstance : BaseInstance
	{
		private MapLayer m_defObject;

		private MapVisibilityMode? m_visibilityMode;

		private double? m_minimumZoom;

		private double? m_maximumZoom;

		private double? m_transparency;

		public MapVisibilityMode VisibilityMode
		{
			get
			{
				if (!this.m_visibilityMode.HasValue)
				{
					this.m_visibilityMode = this.m_defObject.MapLayerDef.EvaluateVisibilityMode(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_visibilityMode.Value;
			}
		}

		public double MinimumZoom
		{
			get
			{
				if (!this.m_minimumZoom.HasValue)
				{
					this.m_minimumZoom = this.m_defObject.MapLayerDef.EvaluateMinimumZoom(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_minimumZoom.Value;
			}
		}

		public double MaximumZoom
		{
			get
			{
				if (!this.m_maximumZoom.HasValue)
				{
					this.m_maximumZoom = this.m_defObject.MapLayerDef.EvaluateMaximumZoom(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_maximumZoom.Value;
			}
		}

		public double Transparency
		{
			get
			{
				if (!this.m_transparency.HasValue)
				{
					this.m_transparency = this.m_defObject.MapLayerDef.EvaluateTransparency(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_transparency.Value;
			}
		}

		internal MapLayerInstance(MapLayer defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_visibilityMode = null;
			this.m_minimumZoom = null;
			this.m_maximumZoom = null;
			this.m_transparency = null;
		}
	}
}
