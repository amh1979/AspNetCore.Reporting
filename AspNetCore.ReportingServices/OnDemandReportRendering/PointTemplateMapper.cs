using AspNetCore.Reporting.Map.WebForms;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class PointTemplateMapper : SpatialElementTemplateMapper
	{
		private VectorLayerMapper m_vectorLayerMapper;

		private static string m_defaultSymbolSizeString = "5.25pt";

		private static ReportSize m_defaultSymbolSize = new ReportSize(PointTemplateMapper.m_defaultSymbolSizeString);

		private MapPointLayer MapPointLayer
		{
			get
			{
				return (MapPointLayer)base.m_mapVectorLayer;
			}
		}

		protected override MapSpatialElementTemplate DefaultTemplate
		{
			get
			{
				if (base.m_mapVectorLayer is MapPolygonLayer)
				{
					return ((MapPolygonLayer)base.m_mapVectorLayer).MapCenterPointTemplate;
				}
				return this.MapPointLayer.MapPointTemplate;
			}
		}

		internal PointTemplateMapper(MapMapper mapMapper, VectorLayerMapper vectorLayerMapper, MapVectorLayer mapVectorLayer)
			: base(mapMapper, mapVectorLayer)
		{
			this.m_vectorLayerMapper = vectorLayerMapper;
		}

		internal void Render(MapPoint mapPoint, Symbol coreSymbol, bool hasScope)
		{
			bool flag = PointTemplateMapper.UseCustomTemplate(mapPoint, hasScope);
			MapPointTemplate mapPointTemplate = (!flag) ? this.MapPointLayer.MapPointTemplate : mapPoint.MapPointTemplate;
			this.RenderPointTemplate(mapPointTemplate, coreSymbol, flag, !flag && this.m_vectorLayerMapper.HasPointColorRule(coreSymbol) && hasScope, !flag && this.m_vectorLayerMapper.HasPointSizeRule(coreSymbol) && hasScope, !flag && this.m_vectorLayerMapper.HasMarkerRule(coreSymbol) && hasScope, hasScope);
		}

		internal void RenderPolygonCenterPoint(MapPolygon mapPolygon, Symbol coreSymbol, bool hasScope)
		{
			bool flag = PointTemplateMapper.PolygonUseCustomTemplate(mapPolygon, hasScope);
			MapPointTemplate mapPointTemplate = (!flag) ? this.m_vectorLayerMapper.GetMapPointTemplate() : mapPolygon.MapCenterPointTemplate;
			this.RenderPointTemplate(mapPointTemplate, coreSymbol, flag, !flag && this.m_vectorLayerMapper.HasPointColorRule(coreSymbol) && hasScope, !flag && this.m_vectorLayerMapper.HasPointSizeRule(coreSymbol) && hasScope, !flag && this.m_vectorLayerMapper.HasMarkerRule(coreSymbol) && hasScope, hasScope);
		}

		protected virtual void RenderPointTemplate(MapPointTemplate mapPointTemplate, Symbol coreSymbol, bool customTemplate, bool ignoreBackgroundColor, bool ignoreSize, bool ignoreMarker, bool hasScope)
		{
			if (mapPointTemplate == null)
			{
				base.RenderStyle(null, null, coreSymbol, ignoreBackgroundColor, hasScope);
				coreSymbol.BorderStyle = base.GetBorderStyle(null, null, hasScope);
			}
			else
			{
				base.RenderSpatialElementTemplate(mapPointTemplate, coreSymbol, ignoreBackgroundColor, hasScope);
				Style style = mapPointTemplate.Style;
				StyleInstance style2 = mapPointTemplate.Instance.Style;
				coreSymbol.BorderStyle = base.GetBorderStyle(style, style2, hasScope);
				if (!ignoreSize)
				{
					int size = this.GetSize(mapPointTemplate, hasScope);
					float num3 = coreSymbol.Width = (coreSymbol.Height = (float)size);
				}
				ReportEnumProperty<MapPointLabelPlacement> labelPlacement = mapPointTemplate.LabelPlacement;
				TextAlignment textAlignment = TextAlignment.Bottom;
				if (labelPlacement != null)
				{
					if (!labelPlacement.IsExpression)
					{
						textAlignment = this.GetTextAlignment(labelPlacement.Value);
					}
					else if (hasScope)
					{
						textAlignment = this.GetTextAlignment(mapPointTemplate.Instance.LabelPlacement);
					}
				}
				coreSymbol.TextAlignment = textAlignment;
			}
		}

		internal int GetSize(MapPointTemplate mapPointTemplate, bool hasScope)
		{
			ReportSizeProperty size = mapPointTemplate.Size;
			if (size != null)
			{
				if (!size.IsExpression)
				{
					return MappingHelper.ToIntPixels(size.Value, base.m_mapMapper.DpiX);
				}
				if (hasScope)
				{
					return MappingHelper.ToIntPixels(mapPointTemplate.Instance.Size, base.m_mapMapper.DpiX);
				}
				return PointTemplateMapper.GetDefaultSymbolSize(base.m_mapMapper.DpiX);
			}
			return PointTemplateMapper.GetDefaultSymbolSize(base.m_mapMapper.DpiX);
		}

		internal static int GetDefaultSymbolSize(float dpi)
		{
			return MappingHelper.ToIntPixels(PointTemplateMapper.m_defaultSymbolSize, dpi);
		}

		private static bool UseCustomTemplate(MapPoint mapPoint, bool hasScope)
		{
			if (mapPoint == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomPointTemplate = mapPoint.UseCustomPointTemplate;
			if (useCustomPointTemplate != null)
			{
				if (!useCustomPointTemplate.IsExpression)
				{
					result = useCustomPointTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapPoint.Instance.UseCustomPointTemplate;
				}
			}
			return result;
		}

		internal static bool PolygonUseCustomTemplate(MapPolygon mapPolygon, bool hasScope)
		{
			if (mapPolygon == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomCenterPointTemplate = mapPolygon.UseCustomCenterPointTemplate;
			if (useCustomCenterPointTemplate != null)
			{
				if (!useCustomCenterPointTemplate.IsExpression)
				{
					result = useCustomCenterPointTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapPolygon.Instance.UseCustomCenterPointTemplate;
				}
			}
			return result;
		}

		private TextAlignment GetTextAlignment(MapPointLabelPlacement placement)
		{
			switch (placement)
			{
			case MapPointLabelPlacement.Center:
				return TextAlignment.Center;
			case MapPointLabelPlacement.Left:
				return TextAlignment.Left;
			case MapPointLabelPlacement.Right:
				return TextAlignment.Right;
			case MapPointLabelPlacement.Top:
				return TextAlignment.Top;
			default:
				return TextAlignment.Bottom;
			}
		}
	}
}
