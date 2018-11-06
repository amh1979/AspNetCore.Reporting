using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapViewport : MapSubItem
	{
		private ReportEnumProperty<MapCoordinateSystem> m_mapCoordinateSystem;

		private ReportEnumProperty<MapProjection> m_mapProjection;

		private ReportDoubleProperty m_projectionCenterX;

		private ReportDoubleProperty m_projectionCenterY;

		private MapLimits m_mapLimits;

		private MapView m_mapView;

		private ReportDoubleProperty m_maximumZoom;

		private ReportDoubleProperty m_minimumZoom;

		private ReportSizeProperty m_contentMargin;

		private MapGridLines m_mapMeridians;

		private MapGridLines m_mapParallels;

		private ReportBoolProperty m_gridUnderContent;

		private ReportDoubleProperty m_simplificationResolution;

		public ReportEnumProperty<MapCoordinateSystem> MapCoordinateSystem
		{
			get
			{
				if (this.m_mapCoordinateSystem == null && this.MapViewportDef.MapCoordinateSystem != null)
				{
					this.m_mapCoordinateSystem = new ReportEnumProperty<MapCoordinateSystem>(this.MapViewportDef.MapCoordinateSystem.IsExpression, this.MapViewportDef.MapCoordinateSystem.OriginalText, EnumTranslator.TranslateMapCoordinateSystem(this.MapViewportDef.MapCoordinateSystem.StringValue, null));
				}
				return this.m_mapCoordinateSystem;
			}
		}

		public ReportEnumProperty<MapProjection> MapProjection
		{
			get
			{
				if (this.m_mapProjection == null && this.MapViewportDef.MapProjection != null)
				{
					this.m_mapProjection = new ReportEnumProperty<MapProjection>(this.MapViewportDef.MapProjection.IsExpression, this.MapViewportDef.MapProjection.OriginalText, EnumTranslator.TranslateMapProjection(this.MapViewportDef.MapProjection.StringValue, null));
				}
				return this.m_mapProjection;
			}
		}

		public ReportDoubleProperty ProjectionCenterX
		{
			get
			{
				if (this.m_projectionCenterX == null && this.MapViewportDef.ProjectionCenterX != null)
				{
					this.m_projectionCenterX = new ReportDoubleProperty(this.MapViewportDef.ProjectionCenterX);
				}
				return this.m_projectionCenterX;
			}
		}

		public ReportDoubleProperty ProjectionCenterY
		{
			get
			{
				if (this.m_projectionCenterY == null && this.MapViewportDef.ProjectionCenterY != null)
				{
					this.m_projectionCenterY = new ReportDoubleProperty(this.MapViewportDef.ProjectionCenterY);
				}
				return this.m_projectionCenterY;
			}
		}

		public MapLimits MapLimits
		{
			get
			{
				if (this.m_mapLimits == null && this.MapViewportDef.MapLimits != null)
				{
					this.m_mapLimits = new MapLimits(this.MapViewportDef.MapLimits, base.m_map);
				}
				return this.m_mapLimits;
			}
		}

		public MapView MapView
		{
			get
			{
				if (this.m_mapView == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapView mapView = this.MapViewportDef.MapView;
					if (mapView != null)
					{
						if (mapView is AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView)
						{
							this.m_mapView = new MapCustomView((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView)this.MapViewportDef.MapView, base.m_map);
						}
						else if (mapView is AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView)
						{
							this.m_mapView = new MapElementView((AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView)this.MapViewportDef.MapView, base.m_map);
						}
						if (mapView is AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView)
						{
							this.m_mapView = new MapDataBoundView((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView)this.MapViewportDef.MapView, base.m_map);
						}
					}
				}
				return this.m_mapView;
			}
		}

		public ReportDoubleProperty MaximumZoom
		{
			get
			{
				if (this.m_maximumZoom == null && this.MapViewportDef.MaximumZoom != null)
				{
					this.m_maximumZoom = new ReportDoubleProperty(this.MapViewportDef.MaximumZoom);
				}
				return this.m_maximumZoom;
			}
		}

		public ReportDoubleProperty MinimumZoom
		{
			get
			{
				if (this.m_minimumZoom == null && this.MapViewportDef.MinimumZoom != null)
				{
					this.m_minimumZoom = new ReportDoubleProperty(this.MapViewportDef.MinimumZoom);
				}
				return this.m_minimumZoom;
			}
		}

		public ReportSizeProperty ContentMargin
		{
			get
			{
				if (this.m_contentMargin == null && this.MapViewportDef.ContentMargin != null)
				{
					this.m_contentMargin = new ReportSizeProperty(this.MapViewportDef.ContentMargin);
				}
				return this.m_contentMargin;
			}
		}

		public ReportDoubleProperty SimplificationResolution
		{
			get
			{
				if (this.m_simplificationResolution == null && this.MapViewportDef.SimplificationResolution != null)
				{
					this.m_simplificationResolution = new ReportDoubleProperty(this.MapViewportDef.SimplificationResolution);
				}
				return this.m_simplificationResolution;
			}
		}

		public MapGridLines MapMeridians
		{
			get
			{
				if (this.m_mapMeridians == null && this.MapViewportDef.MapMeridians != null)
				{
					this.m_mapMeridians = new MapGridLines(this.MapViewportDef.MapMeridians, base.m_map);
				}
				return this.m_mapMeridians;
			}
		}

		public MapGridLines MapParallels
		{
			get
			{
				if (this.m_mapParallels == null && this.MapViewportDef.MapParallels != null)
				{
					this.m_mapParallels = new MapGridLines(this.MapViewportDef.MapParallels, base.m_map);
				}
				return this.m_mapParallels;
			}
		}

		public ReportBoolProperty GridUnderContent
		{
			get
			{
				if (this.m_gridUnderContent == null && this.MapViewportDef.GridUnderContent != null)
				{
					this.m_gridUnderContent = new ReportBoolProperty(this.MapViewportDef.GridUnderContent);
				}
				return this.m_gridUnderContent;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport MapViewportDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport)base.m_defObject;
			}
		}

		public new MapViewportInstance Instance
		{
			get
			{
				return (MapViewportInstance)this.GetInstance();
			}
		}

		internal MapViewport(AspNetCore.ReportingServices.ReportIntermediateFormat.MapViewport defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapViewportInstance(this);
			}
			return (MapSubItemInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapLimits != null)
			{
				this.m_mapLimits.SetNewContext();
			}
			if (this.m_mapView != null)
			{
				this.m_mapView.SetNewContext();
			}
			if (this.m_mapMeridians != null)
			{
				this.m_mapMeridians.SetNewContext();
			}
			if (this.m_mapParallels != null)
			{
				this.m_mapParallels.SetNewContext();
			}
		}
	}
}
