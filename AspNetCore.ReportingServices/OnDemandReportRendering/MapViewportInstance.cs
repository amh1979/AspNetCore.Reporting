using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapViewportInstance : MapSubItemInstance
	{
		private MapViewport m_defObject;

		private MapCoordinateSystem? m_mapCoordinateSystem;

		private MapProjection? m_mapProjection;

		private double? m_projectionCenterX;

		private double? m_projectionCenterY;

		private double? m_maximumZoom;

		private double? m_minimumZoom;

		private ReportSize m_contentMargin;

		private bool? m_gridUnderContent;

		private double? m_simplificationResolution;

		public MapCoordinateSystem MapCoordinateSystem
		{
			get
			{
				if (!this.m_mapCoordinateSystem.HasValue)
				{
					this.m_mapCoordinateSystem = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateMapCoordinateSystem(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_mapCoordinateSystem.Value;
			}
		}

		public MapProjection MapProjection
		{
			get
			{
				if (!this.m_mapProjection.HasValue)
				{
					this.m_mapProjection = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateMapProjection(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_mapProjection.Value;
			}
		}

		public double ProjectionCenterX
		{
			get
			{
				if (!this.m_projectionCenterX.HasValue)
				{
					this.m_projectionCenterX = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateProjectionCenterX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_projectionCenterX.Value;
			}
		}

		public double ProjectionCenterY
		{
			get
			{
				if (!this.m_projectionCenterY.HasValue)
				{
					this.m_projectionCenterY = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateProjectionCenterY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_projectionCenterY.Value;
			}
		}

		public double MaximumZoom
		{
			get
			{
				if (!this.m_maximumZoom.HasValue)
				{
					this.m_maximumZoom = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateMaximumZoom(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_maximumZoom.Value;
			}
		}

		public double MinimumZoom
		{
			get
			{
				if (!this.m_minimumZoom.HasValue)
				{
					this.m_minimumZoom = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateMinimumZoom(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_minimumZoom.Value;
			}
		}

		public double SimplificationResolution
		{
			get
			{
				if (!this.m_simplificationResolution.HasValue)
				{
					this.m_simplificationResolution = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateSimplificationResolution(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_simplificationResolution.Value;
			}
		}

		public ReportSize ContentMargin
		{
			get
			{
				if (this.m_contentMargin == null)
				{
					this.m_contentMargin = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateContentMargin(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_contentMargin;
			}
		}

		public bool GridUnderContent
		{
			get
			{
				if (!this.m_gridUnderContent.HasValue)
				{
					this.m_gridUnderContent = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)this.m_defObject.MapSubItemDef).EvaluateGridUnderContent(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_gridUnderContent.Value;
			}
		}

		internal MapViewportInstance(MapViewport defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_mapCoordinateSystem = null;
			this.m_mapProjection = null;
			this.m_projectionCenterX = null;
			this.m_projectionCenterY = null;
			this.m_maximumZoom = null;
			this.m_minimumZoom = null;
			this.m_contentMargin = null;
			this.m_gridUnderContent = null;
			this.m_simplificationResolution = null;
		}
	}
}
