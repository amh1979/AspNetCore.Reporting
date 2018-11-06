using AspNetCore.Reporting.Map.WebForms;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class LineLayerMapper : VectorLayerMapper
	{
		private CorePathManager m_pathManager;

		private ColorRuleMapper m_lineColorRuleMapper;

		private SizeRuleMapper m_lineSizeRuleMapper;

		private LineTemplateMapper m_lineTemplateMapper;

		protected override ISpatialElementCollection SpatialElementCollection
		{
			get
			{
				return this.MapLineLayer.MapLines;
			}
		}

		private MapLineLayer MapLineLayer
		{
			get
			{
				return (MapLineLayer)base.m_mapVectorLayer;
			}
		}

		internal LineLayerMapper(MapLineLayer mapLineLayer, MapControl coreMap, MapMapper mapMapper)
			: base(mapLineLayer, coreMap, mapMapper)
		{
			if (mapLineLayer.MapLineTemplate != null)
			{
				this.m_lineTemplateMapper = new LineTemplateMapper(base.m_mapMapper, this, this.MapLineLayer);
			}
		}

		protected override CoreSpatialElementManager GetSpatialElementManager()
		{
			if (this.m_pathManager == null)
			{
				this.m_pathManager = new CorePathManager(base.m_coreMap, base.m_mapVectorLayer);
			}
			return this.m_pathManager;
		}

		internal bool HasColorRule(Path path)
		{
			if (!this.HasColorRule())
			{
				return false;
			}
			return this.m_lineColorRuleMapper.HasDataValue(path);
		}

		private bool HasColorRule()
		{
			MapLineRules mapLineRules = this.MapLineLayer.MapLineRules;
			MapColorRule mapColorRule = (mapLineRules == null) ? null : mapLineRules.MapColorRule;
			return mapColorRule != null;
		}

		internal bool HasSizeRule(Path path)
		{
			if (!this.HasSizeRule())
			{
				return false;
			}
			return this.m_lineSizeRuleMapper.HasDataValue(path);
		}

		private bool HasSizeRule()
		{
			MapLineRules mapLineRules = this.MapLineLayer.MapLineRules;
			MapSizeRule mapSizeRule = (mapLineRules == null) ? null : mapLineRules.MapSizeRule;
			return mapSizeRule != null;
		}

		protected override void CreateRules()
		{
			MapLineRules mapLineRules = this.MapLineLayer.MapLineRules;
			if (mapLineRules != null)
			{
				if (mapLineRules.MapColorRule != null)
				{
					this.m_lineColorRuleMapper = new ColorRuleMapper(mapLineRules.MapColorRule, this, this.GetSpatialElementManager());
					this.m_lineColorRuleMapper.CreatePathRule();
				}
				if (mapLineRules.MapSizeRule != null)
				{
					this.m_lineSizeRuleMapper = new SizeRuleMapper(mapLineRules.MapSizeRule, this, this.GetSpatialElementManager());
					this.m_lineSizeRuleMapper.CreatePathRule();
				}
			}
		}

		protected override void RenderRules()
		{
			MapLineRules mapLineRules = this.MapLineLayer.MapLineRules;
			if (mapLineRules != null)
			{
				if (mapLineRules.MapColorRule != null)
				{
					this.m_lineColorRuleMapper.RenderLineRule(this.m_lineTemplateMapper, this.GetLegendSize());
				}
				if (mapLineRules.MapSizeRule != null)
				{
					this.m_lineSizeRuleMapper.RenderLineRule(this.m_lineTemplateMapper, this.GetLegendColor());
				}
			}
		}

		private Color? GetLegendColor()
		{
			return this.m_lineTemplateMapper.GetBackgroundColor(false);
		}

		private int? GetLegendSize()
		{
			if (this.m_lineTemplateMapper == null)
			{
				return LineTemplateMapper.GetDefaultSize(base.m_mapMapper.DpiX);
			}
			return this.m_lineTemplateMapper.GetSize(this.MapLineLayer.MapLineTemplate, false);
		}

		protected override void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope)
		{
			base.InitializeSpatialElement(spatialElementInfo.CoreSpatialElement);
			if (hasScope)
			{
				this.RenderLineRuleFields((Path)spatialElementInfo.CoreSpatialElement);
			}
			this.RenderLineTemplate((MapLine)spatialElementInfo.MapSpatialElement, (Path)spatialElementInfo.CoreSpatialElement, hasScope);
		}

		protected void RenderLineRuleFields(Path corePath)
		{
			if (this.m_lineColorRuleMapper != null)
			{
				this.m_lineColorRuleMapper.SetRuleFieldValue(corePath);
			}
			if (this.m_lineSizeRuleMapper != null)
			{
				this.m_lineSizeRuleMapper.SetRuleFieldValue(corePath);
			}
		}

		private void RenderLineTemplate(MapLine mapLine, Path path, bool hasScope)
		{
			this.m_lineTemplateMapper.Render(mapLine, path, hasScope);
		}

		internal override bool IsValidSpatialElement(ISpatialElement spatialElement)
		{
			return spatialElement is Path;
		}

		internal override void OnSpatialElementAdded(ISpatialElement spatialElement)
		{
			base.m_mapMapper.Simplify((Path)spatialElement);
		}
	}
}
