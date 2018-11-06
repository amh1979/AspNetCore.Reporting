using AspNetCore.Reporting.Map.WebForms;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class PolygonTemplateMapper : SpatialElementTemplateMapper
	{
		private PolygonLayerMapper m_polygonLayerMapper;

		private MapPolygonLayer MapPolygonLayer
		{
			get
			{
				return (MapPolygonLayer)base.m_mapVectorLayer;
			}
		}

		protected override MapSpatialElementTemplate DefaultTemplate
		{
			get
			{
				return this.MapPolygonLayer.MapPolygonTemplate;
			}
		}

		internal PolygonTemplateMapper(MapMapper mapMapper, PolygonLayerMapper polygonLayerMapper, MapPolygonLayer mapPolygonLayer)
			: base(mapMapper, mapPolygonLayer)
		{
			this.m_polygonLayerMapper = polygonLayerMapper;
		}

		internal void Render(MapPolygon mapPolygon, Shape coreShape, bool hasScope)
		{
			bool flag = PolygonTemplateMapper.UseCustomTemplate(mapPolygon, hasScope);
			MapPolygonTemplate mapPolygonTemplate = (!flag) ? this.MapPolygonLayer.MapPolygonTemplate : mapPolygon.MapPolygonTemplate;
			bool ignoreBackgroundColor = !flag && this.m_polygonLayerMapper.HasColorRule(coreShape) && hasScope;
			if (mapPolygonTemplate == null)
			{
				base.RenderStyle(null, null, coreShape, ignoreBackgroundColor, hasScope);
				coreShape.BorderStyle = base.GetBorderStyle(null, null, hasScope);
			}
			else
			{
				base.RenderSpatialElementTemplate(mapPolygonTemplate, coreShape, ignoreBackgroundColor, hasScope);
				Style style = mapPolygonTemplate.Style;
				StyleInstance style2 = mapPolygonTemplate.Instance.Style;
				coreShape.BorderStyle = base.GetBorderStyle(style, style2, hasScope);
				ReportDoubleProperty scaleFactor = mapPolygonTemplate.ScaleFactor;
				if (scaleFactor != null)
				{
					if (!scaleFactor.IsExpression)
					{
						coreShape.ScaleFactor = scaleFactor.Value;
					}
					else if (hasScope)
					{
						coreShape.ScaleFactor = mapPolygonTemplate.Instance.ScaleFactor;
					}
				}
				ReportDoubleProperty centerPointOffsetX = mapPolygonTemplate.CenterPointOffsetX;
				if (centerPointOffsetX != null)
				{
					if (!centerPointOffsetX.IsExpression)
					{
						coreShape.CentralPointOffset.X = centerPointOffsetX.Value;
					}
					else if (hasScope)
					{
						coreShape.CentralPointOffset.X = mapPolygonTemplate.Instance.CenterPointOffsetX;
					}
				}
				centerPointOffsetX = mapPolygonTemplate.CenterPointOffsetY;
				if (centerPointOffsetX != null)
				{
					if (!centerPointOffsetX.IsExpression)
					{
						coreShape.CentralPointOffset.Y = centerPointOffsetX.Value;
					}
					else if (hasScope)
					{
						coreShape.CentralPointOffset.Y = mapPolygonTemplate.Instance.CenterPointOffsetY;
					}
				}
				ReportEnumProperty<MapAutoBool> showLabel = mapPolygonTemplate.ShowLabel;
				if (showLabel != null)
				{
					if (!showLabel.IsExpression)
					{
						coreShape.TextVisibility = this.GetTextVisibility(showLabel.Value);
					}
					else if (hasScope)
					{
						coreShape.TextVisibility = this.GetTextVisibility(mapPolygonTemplate.Instance.ShowLabel);
					}
				}
				ReportEnumProperty<MapPolygonLabelPlacement> labelPlacement = mapPolygonTemplate.LabelPlacement;
				if (labelPlacement != null)
				{
					if (!labelPlacement.IsExpression)
					{
						coreShape.TextAlignment = this.GetTextAlignment(labelPlacement.Value);
					}
					else if (hasScope)
					{
						coreShape.TextAlignment = this.GetTextAlignment(mapPolygonTemplate.Instance.LabelPlacement);
					}
				}
			}
		}

		private static bool UseCustomTemplate(MapPolygon mapPolygon, bool hasScope)
		{
			if (mapPolygon == null)
			{
				return false;
			}
			bool result = false;
			ReportBoolProperty useCustomPolygonTemplate = mapPolygon.UseCustomPolygonTemplate;
			if (useCustomPolygonTemplate != null)
			{
				if (!useCustomPolygonTemplate.IsExpression)
				{
					result = useCustomPolygonTemplate.Value;
				}
				else if (hasScope)
				{
					result = mapPolygon.Instance.UseCustomPolygonTemplate;
				}
			}
			return result;
		}

		private ContentAlignment GetTextAlignment(MapPolygonLabelPlacement placement)
		{
			switch (placement)
			{
			case MapPolygonLabelPlacement.BottomCenter:
				return ContentAlignment.BottomCenter;
			case MapPolygonLabelPlacement.BottomLeft:
				return ContentAlignment.BottomLeft;
			case MapPolygonLabelPlacement.BottomRight:
				return ContentAlignment.BottomRight;
			case MapPolygonLabelPlacement.MiddleLeft:
				return ContentAlignment.MiddleLeft;
			case MapPolygonLabelPlacement.MiddleRight:
				return ContentAlignment.MiddleRight;
			case MapPolygonLabelPlacement.TopCenter:
				return ContentAlignment.TopCenter;
			case MapPolygonLabelPlacement.TopLeft:
				return ContentAlignment.TopLeft;
			case MapPolygonLabelPlacement.TopRight:
				return ContentAlignment.TopRight;
			default:
				return ContentAlignment.MiddleCenter;
			}
		}

		private TextVisibility GetTextVisibility(MapAutoBool value)
		{
			switch (value)
			{
			case MapAutoBool.True:
				return TextVisibility.Shown;
			case MapAutoBool.False:
				return TextVisibility.Hidden;
			default:
				return TextVisibility.Auto;
			}
		}
	}
}
