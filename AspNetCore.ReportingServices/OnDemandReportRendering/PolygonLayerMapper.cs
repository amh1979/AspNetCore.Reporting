using AspNetCore.Reporting.Map.WebForms;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class PolygonLayerMapper : VectorLayerMapper
	{
		private CoreShapeManager m_shapeManager;

		private ColorRuleMapper m_polygonColorRuleMapper;

		private PolygonTemplateMapper m_polygonTemplateMapper;

		protected override ISpatialElementCollection SpatialElementCollection
		{
			get
			{
				return this.MapPolygonLayer.MapPolygons;
			}
		}

		private MapPolygonLayer MapPolygonLayer
		{
			get
			{
				return (MapPolygonLayer)base.m_mapVectorLayer;
			}
		}

		internal PolygonLayerMapper(MapPolygonLayer mapPolygonLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapPolygonLayer, coreMap, mapMapper)
		{
			this.m_polygonTemplateMapper = new PolygonTemplateMapper(base.m_mapMapper, this, this.MapPolygonLayer);
			base.m_pointTemplateMapper = base.CreatePointTemplateMapper();
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (this.m_shapeManager == null)
			{
				this.m_shapeManager = new CoreShapeManager(base.m_coreMap, base.m_mapVectorLayer);
			}
			return this.m_shapeManager;
		}

		internal bool HasColorRule(Shape shape)
		{
			if (!this.HasColorRule())
			{
				return false;
			}
			return this.m_polygonColorRuleMapper.HasDataValue(shape);
		}

		private bool HasColorRule()
		{
			MapPolygonRules mapPolygonRules = this.MapPolygonLayer.MapPolygonRules;
			MapColorRule mapColorRule = (mapPolygonRules == null) ? null : mapPolygonRules.MapColorRule;
			return mapColorRule != null;
		}

		protected override void CreateRules()
		{
			MapPolygonRules mapPolygonRules = this.MapPolygonLayer.MapPolygonRules;
			if (mapPolygonRules != null && mapPolygonRules.MapColorRule != null)
			{
				this.m_polygonColorRuleMapper = new ColorRuleMapper(mapPolygonRules.MapColorRule, this, this.GetSpatialElementManager());
				this.m_polygonColorRuleMapper.CreatePolygonRule();
			}
			MapPointRules mapCenterPointRules = this.MapPolygonLayer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				base.CreatePointRules(mapCenterPointRules);
			}
		}

		protected override void RenderRules()
		{
			MapPolygonRules mapPolygonRules = this.MapPolygonLayer.MapPolygonRules;
			if (mapPolygonRules != null && mapPolygonRules.MapColorRule != null)
			{
				this.m_polygonColorRuleMapper.RenderPolygonRule(this.m_polygonTemplateMapper);
			}
			MapPointRules mapCenterPointRules = this.MapPolygonLayer.MapCenterPointRules;
			if (mapCenterPointRules != null)
			{
				base.RenderPointRules(mapCenterPointRules);
			}
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			base.InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			if (hasScope)
			{
				this.RenderPolygonRulesField((Shape)spatialElementInfo.CoreSpatialElement);
			}
			this.RenderPolygonTemplate((MapPolygon)spatialElementInfo.MapSpatialElement, (Shape)spatialElementInfo.CoreSpatialElement, hasScope);
			this.RenderPolygonCenterPoint(spatialElementInfo, hasScope);
		}

		internal override MapPointRules GetMapPointRules()
		{
			return this.MapPolygonLayer.MapCenterPointRules;
		}

		internal override MapPointTemplate GetMapPointTemplate()
		{
			return this.MapPolygonLayer.MapCenterPointTemplate;
		}

		private bool HasCenterPointRule()
		{
			if (!base.HasPointColorRule() && !base.HasPointSizeRule())
			{
				return base.HasMarkerRule();
			}
			return true;
		}

		private bool HasCenterPointTemplate(MapPolygon mapPolygon, MapPointTemplate pointTemplate, bool hasScope)
		{
			if (mapPolygon != null && PointTemplateMapper.PolygonUseCustomTemplate(mapPolygon, hasScope))
			{
				return mapPolygon.MapCenterPointTemplate != null;
			}
			return pointTemplate != null;
		}

		private void RenderPolygonCenterPoint(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			if (!this.HasCenterPointRule() && !this.HasCenterPointTemplate((MapPolygon)spatialElementInfo.MapSpatialElement, this.MapPolygonLayer.MapCenterPointTemplate, hasScope))
			{
				return;
			}
			Symbol symbol = (Symbol)base.GetSymbolManager().CreateSpatialElement();
			symbol.Layer = spatialElementInfo.CoreSpatialElement.Layer;
			symbol.Category = spatialElementInfo.CoreSpatialElement.Category;
			symbol.ParentShape = spatialElementInfo.CoreSpatialElement.Name;
			this.CopyFieldsToPoint((Shape)spatialElementInfo.CoreSpatialElement, symbol);
			base.GetSymbolManager().AddSpatialElement(symbol);
			base.RenderPoint(spatialElementInfo.MapSpatialElement, symbol, hasScope);
		}

		private void CopyFieldsToPoint(Shape shape, Symbol symbol)
		{
			foreach (string key in shape.fields.Keys)
			{
				this.CopyFieldToPoint(shape, symbol, key);
			}
		}

		private void CopyFieldToPoint(Shape shape, Symbol symbol, string fieldName)
		{
			if (base.m_coreMap.SymbolFields.GetByName(fieldName) == null)
			{
				AspNetCore.Reporting.Map.WebForms.Field field = new AspNetCore.Reporting.Map.WebForms.Field();
				field.Name = fieldName;
				field.Type = ((AspNetCore.Reporting.Map.WebForms.Field)base.m_coreMap.ShapeFields.GetByName(fieldName)).Type;
				base.m_coreMap.SymbolFields.Add(field);
			}
			symbol[fieldName] = shape[fieldName];
		}

		private void RenderPolygonRulesField(Shape shape)
		{
			if (this.m_polygonColorRuleMapper != null)
			{
				this.m_polygonColorRuleMapper.SetRuleFieldValue(shape);
			}
		}

		private void RenderPolygonTemplate(MapPolygon mapPolygon, Shape coreShape, bool hasScope)
		{
			this.m_polygonTemplateMapper.Render(mapPolygon, coreShape, hasScope);
		}

		protected override void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
			base.m_pointTemplateMapper.RenderPolygonCenterPoint((MapPolygon)mapSpatialElement, coreSymbol, hasScope);
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Shape;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
			base.m_mapMapper.Simplify((Shape)spatialElement);
		}
	}
}
