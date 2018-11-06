using AspNetCore.Reporting.Map.WebForms;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class PointLayerMapper : VectorLayerMapper
	{
		private CoreSymbolManager m_symbolManager;

		protected override ISpatialElementCollection SpatialElementCollection
		{
			get
			{
				return this.MapPointLayer.MapPoints;
			}
		}

		private MapPointLayer MapPointLayer
		{
			get
			{
				return (MapPointLayer)base.m_mapVectorLayer;
			}
		}

		internal PointLayerMapper(MapPointLayer mapPointLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapPointLayer, coreMap, mapMapper)
		{
			if (mapPointLayer.MapPointTemplate != null)
			{
				base.m_pointTemplateMapper = base.CreatePointTemplateMapper();
			}
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (this.m_symbolManager == null)
			{
				this.m_symbolManager = new CoreSymbolManager(base.m_coreMap, base.m_mapVectorLayer);
			}
			return this.m_symbolManager;
		}

		protected override void CreateRules()
		{
			MapPointRules mapPointRules = this.MapPointLayer.MapPointRules;
			if (mapPointRules != null)
			{
				base.CreatePointRules(mapPointRules);
			}
		}

		protected override void RenderRules()
		{
			MapPointRules mapPointRules = this.MapPointLayer.MapPointRules;
			if (mapPointRules != null)
			{
				base.RenderPointRules(mapPointRules);
			}
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			base.InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			base.RenderPoint((MapPoint)spatialElementInfo.MapSpatialElement, (Symbol)spatialElementInfo.CoreSpatialElement, hasScope);
		}

		protected override void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
			base.m_pointTemplateMapper.Render((MapPoint)mapSpatialElement, coreSymbol, hasScope);
		}

		internal override MapPointRules GetMapPointRules()
		{
			return this.MapPointLayer.MapPointRules;
		}

		internal override MapPointTemplate GetMapPointTemplate()
		{
			return this.MapPointLayer.MapPointTemplate;
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Symbol;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
		}
	}
}
