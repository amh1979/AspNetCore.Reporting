using AspNetCore.Reporting.Map.WebForms;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class LineTemplateMapper : SpatialElementTemplateMapper
	{
		private LineLayerMapper m_lineLayerMapper;

		private static string m_defaultLineSizeString = "3.75pt";

		private static ReportSize m_defaultLineWidth = new ReportSize(LineTemplateMapper.m_defaultLineSizeString);

		private MapLineLayer MapLineLayer
		{
			get
			{
				return (MapLineLayer)base.m_mapVectorLayer;
			}
		}

		protected override MapSpatialElementTemplate DefaultTemplate
		{
			get
			{
				return this.MapLineLayer.MapLineTemplate;
			}
		}

		internal LineTemplateMapper(MapMapper mapMapper, LineLayerMapper lineLayerMapper, MapLineLayer mapLineLayer)
			: base(mapMapper, mapLineLayer)
		{
			this.m_lineLayerMapper = lineLayerMapper;
		}

		internal void Render(MapLine mapLine, Path corePath, bool hasScope)
		{
			bool flag = this.UseCustomTemplate(mapLine, hasScope);
			MapLineTemplate mapLineTemplate = (!flag) ? this.MapLineLayer.MapLineTemplate : mapLine.MapLineTemplate;
			this.RenderLineTemplate(mapLineTemplate, corePath, !flag && this.m_lineLayerMapper.HasColorRule(corePath) && hasScope, !flag && this.m_lineLayerMapper.HasSizeRule(corePath) && hasScope, hasScope);
		}

		protected virtual void RenderLineTemplate(MapLineTemplate mapLineTemplate, Path corePath, bool ignoreBackgroundColor, bool ignoreSize, bool hasScope)
		{
			if (mapLineTemplate == null)
			{
				base.RenderStyle(null, null, corePath, ignoreBackgroundColor, hasScope);
			}
			else
			{
				base.RenderSpatialElementTemplate(mapLineTemplate, corePath, ignoreBackgroundColor, hasScope);
				Style style = mapLineTemplate.Style;
				StyleInstance style2 = mapLineTemplate.Instance.Style;
				corePath.LineStyle = base.GetBorderStyle(style, style2, hasScope);
				if (!ignoreSize)
				{
					int size = this.GetSize(mapLineTemplate, hasScope);
					corePath.Width = (float)size;
				}
				ReportEnumProperty<MapLineLabelPlacement> labelPlacement = mapLineTemplate.LabelPlacement;
				PathLabelPosition labelPosition = PathLabelPosition.Above;
				if (labelPlacement != null)
				{
					if (!labelPlacement.IsExpression)
					{
						labelPosition = this.GetLabelPosition(labelPlacement.Value);
					}
					else if (hasScope)
					{
						labelPosition = this.GetLabelPosition(mapLineTemplate.Instance.LabelPlacement);
					}
				}
				corePath.LabelPosition = labelPosition;
			}
		}

		internal int GetSize(MapLineTemplate mapLineTemplate, bool hasScope)
		{
			ReportSizeProperty width = mapLineTemplate.Width;
			if (width != null)
			{
				if (!width.IsExpression)
				{
					return MappingHelper.ToIntPixels(width.Value, base.m_mapMapper.DpiX);
				}
				if (hasScope)
				{
					return MappingHelper.ToIntPixels(mapLineTemplate.Instance.Width, base.m_mapMapper.DpiX);
				}
				return LineTemplateMapper.GetDefaultSize(base.m_mapMapper.DpiX);
			}
			return LineTemplateMapper.GetDefaultSize(base.m_mapMapper.DpiX);
		}

		internal static int GetDefaultSize(float dpi)
		{
			return MappingHelper.ToIntPixels(LineTemplateMapper.m_defaultLineWidth, dpi);
		}

		private bool UseCustomTemplate(MapLine mapLine, bool hasScope)
		{
			if (mapLine == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomLineTemplate = mapLine.UseCustomLineTemplate;
			if (useCustomLineTemplate != null)
			{
				if (!useCustomLineTemplate.IsExpression)
				{
					result = useCustomLineTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapLine.Instance.UseCustomLineTemplate;
				}
			}
			return result;
		}

		private PathLabelPosition GetLabelPosition(MapLineLabelPlacement placement)
		{
			switch (placement)
			{
			case MapLineLabelPlacement.Below:
				return PathLabelPosition.Below;
			case MapLineLabelPlacement.Center:
				return PathLabelPosition.Center;
			default:
				return PathLabelPosition.Above;
			}
		}
	}
}
