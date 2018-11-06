using AspNetCore.Reporting.Map.WebForms;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ColorRuleMapper : RuleMapper
	{
		private static Color m_defaultFromColor = Color.Green;

		private static Color m_defaultMiddleColor = Color.Yellow;

		private static Color m_defaultToColor = Color.Red;

		internal ColorRuleMapper(MapColorRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal PathRule CreatePathRule()
		{
			PathRule pathRule = (PathRule)(base.m_coreRule = new PathRule());
			pathRule.BorderColor = Color.Empty;
			pathRule.Text = "";
			pathRule.Category = base.m_mapVectorLayer.Name;
			pathRule.Field = "";
			base.m_coreMap.PathRules.Add(pathRule);
			base.SetRuleFieldName();
			return pathRule;
		}

		internal void RenderPolygonRule(PolygonTemplateMapper shapeTemplateMapper)
		{
			ShapeRule shapeRule = (ShapeRule)base.m_coreRule;
			base.SetRuleLegendProperties(shapeRule);
			base.SetRuleDistribution(shapeRule);
			shapeRule.ShowInColorSwatch = this.GetShowInColorScale();
			if (base.m_mapRule is MapColorRangeRule)
			{
				this.RenderPolygonColorRangeRule(shapeRule);
			}
			else if (base.m_mapRule is MapColorPaletteRule)
			{
				this.RenderPolygonColorPaletteRule(shapeRule);
			}
			else
			{
				this.RenderPolygonCustomColorRule(shapeRule);
			}
			this.InitializeCustomColors(shapeRule.CustomColors, shapeTemplateMapper);
		}

		internal void RenderSymbolRule(PointTemplateMapper symbolTemplateMapper, int? size, MarkerStyle? markerStyle)
		{
			SymbolRule symbolRule = (SymbolRule)base.m_coreRule;
			base.SetRuleLegendProperties(symbolRule);
			base.SetRuleDistribution(symbolRule);
			symbolRule.ShowInColorSwatch = this.GetShowInColorScale();
			if (base.m_mapRule is MapColorRangeRule)
			{
				this.RenderSymbolColorRangeRule(symbolRule);
			}
			else if (base.m_mapRule is MapColorPaletteRule)
			{
				this.RenderSymbolColorPaletteRule(symbolRule);
			}
			else
			{
				this.SetSymbolRuleColors(this.GetCustomColors(((MapCustomColorRule)base.m_mapRule).MapCustomColors), symbolRule.PredefinedSymbols);
			}
			this.InitializePredefinedSymbols(symbolRule.PredefinedSymbols, symbolTemplateMapper, size, markerStyle);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper symbolTemplateMapper, int? size, MarkerStyle? markerStyle)
		{
			foreach (PredefinedSymbol predefinedSymbol4 in predefinedSymbols)
			{
				if (size.HasValue)
				{
					PredefinedSymbol predefinedSymbol2 = predefinedSymbol4;
					PredefinedSymbol predefinedSymbol3 = predefinedSymbol4;
					float num3 = predefinedSymbol2.Width = (predefinedSymbol3.Height = (float)size.Value);
				}
				if (markerStyle.HasValue)
				{
					predefinedSymbol4.MarkerStyle = markerStyle.Value;
				}
				base.InitializePredefinedSymbols(predefinedSymbol4, symbolTemplateMapper);
			}
		}

		internal void RenderLineRule(LineTemplateMapper pathTemplateMapper, int? size)
		{
			PathRule pathRule = (PathRule)base.m_coreRule;
			base.SetRuleLegendProperties(pathRule);
			base.SetRuleDistribution(pathRule);
			pathRule.ShowInColorSwatch = this.GetShowInColorScale();
			if (base.m_mapRule is MapColorRangeRule)
			{
				this.RenderLineColorRangeRule(pathRule);
			}
			else if (base.m_mapRule is MapColorPaletteRule)
			{
				this.RenderLineColorPaletteRule(pathRule);
			}
			else
			{
				this.RenderLineCustomColorRule(pathRule);
			}
			this.InitializePathRule(pathRule, pathTemplateMapper, size);
		}

		private void InitializePathRule(PathRule pathRule, LineTemplateMapper pathTemplateMapper, int? size)
		{
			this.InitializeCustomColors(pathRule.CustomColors, pathTemplateMapper);
		}

		private void InitializeCustomColors(CustomColorCollection customColors, SpatialElementTemplateMapper spatialEementTemplateMapper)
		{
			foreach (CustomColor customColor in customColors)
			{
				customColor.BorderColor = spatialEementTemplateMapper.GetBorderColor(false);
				customColor.SecondaryColor = spatialEementTemplateMapper.GetBackGradientEndColor(false);
				customColor.GradientType = spatialEementTemplateMapper.GetGradientType(false);
				customColor.HatchStyle = spatialEementTemplateMapper.GetHatchStyle(false);
				customColor.LegendText = "";
				customColor.Text = "";
			}
		}

		private void RenderLineColorRangeRule(PathRule pathRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)base.m_mapRule;
			this.RenderPathCustomColors(pathRule, ColoringMode.ColorRange, MapColorPalette.Dundas, this.GetFromColor(colorRangeRule), this.GetMiddleColor(colorRangeRule), this.GetToColor(colorRangeRule));
		}

		private void RenderLineColorPaletteRule(PathRule pathRule)
		{
			this.RenderPathCustomColors(pathRule, ColoringMode.DistinctColors, this.GetColorPalette(), Color.Empty, Color.Empty, Color.Empty);
		}

		private void RenderLineCustomColorRule(PathRule pathRule)
		{
			pathRule.UseCustomColors = true;
			this.SetRuleColors(this.GetCustomColors(((MapCustomColorRule)base.m_mapRule).MapCustomColors), pathRule.CustomColors);
		}

		private Color GetFromColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty startColor = colorRangeRule.StartColor;
			Color defaultFromColor = ColorRuleMapper.m_defaultFromColor;
			if (startColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(startColor, ref defaultFromColor))
				{
					return defaultFromColor;
				}
				if (colorRangeRule.Instance.StartColor != null)
				{
					return colorRangeRule.Instance.StartColor.ToColor();
				}
			}
			return defaultFromColor;
		}

		private Color GetMiddleColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty middleColor = colorRangeRule.MiddleColor;
			Color defaultMiddleColor = ColorRuleMapper.m_defaultMiddleColor;
			if (middleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(middleColor, ref defaultMiddleColor))
				{
					return defaultMiddleColor;
				}
				if (colorRangeRule.Instance.MiddleColor != null)
				{
					return colorRangeRule.Instance.MiddleColor.ToColor();
				}
			}
			return defaultMiddleColor;
		}

		private Color GetToColor(MapColorRangeRule colorRangeRule)
		{
			ReportColorProperty endColor = colorRangeRule.EndColor;
			Color defaultToColor = ColorRuleMapper.m_defaultToColor;
			if (endColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(endColor, ref defaultToColor))
				{
					return defaultToColor;
				}
				if (colorRangeRule.Instance.EndColor != null)
				{
					return colorRangeRule.Instance.EndColor.ToColor();
				}
			}
			return defaultToColor;
		}

		private bool GetShowInColorScale()
		{
			ReportBoolProperty showInColorScale = ((MapColorRule)base.m_mapRule).ShowInColorScale;
			if (showInColorScale != null)
			{
				if (!showInColorScale.IsExpression)
				{
					return showInColorScale.Value;
				}
				return ((MapColorRule)base.m_mapRule).Instance.ShowInColorScale;
			}
			return false;
		}

		private MapColorPalette GetColorPalette()
		{
			MapColorPaletteRule mapColorPaletteRule = (MapColorPaletteRule)base.m_mapRule;
			ReportEnumProperty<MapPalette> palette = mapColorPaletteRule.Palette;
			if (palette != null)
			{
				if (!palette.IsExpression)
				{
					return this.GetMapColorPalette(palette.Value);
				}
				return this.GetMapColorPalette(mapColorPaletteRule.Instance.Palette);
			}
			return MapColorPalette.Random;
		}

		private Color[] GetCustomColors(MapCustomColorCollection mapCustomColors)
		{
			Color[] array = new Color[mapCustomColors.Count];
			for (int i = 0; i < mapCustomColors.Count; i++)
			{
				array[i] = this.GetCustomColor(((ReportElementCollectionBase<MapCustomColor>)mapCustomColors)[i]);
			}
			return array;
		}

		private void SetRuleColors(Color[] colorRange, CustomColorCollection customColors)
		{
			MapBucketCollection mapBuckets = base.m_mapRule.MapBuckets;
			bool flag = base.GetDistributionType() == MapRuleDistributionType.Custom;
			int bucketCount = base.GetBucketCount();
			for (int i = 0; i < bucketCount; i++)
			{
				CustomColor customColor = new CustomColor();
				if (i < colorRange.Length)
				{
					customColor.Color = colorRange[i];
				}
				else
				{
					customColor.Color = Color.Empty;
				}
				if (flag)
				{
					MapBucket bucket = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[i];
					customColor.FromValue = base.GetFromValue(bucket);
					customColor.ToValue = base.GetToValue(bucket);
				}
				customColors.Add(customColor);
			}
		}

		private void SetSymbolRuleColors(Color[] colorRange, PredefinedSymbolCollection customSymbols)
		{
			MapBucketCollection mapBuckets = base.m_mapRule.MapBuckets;
			bool flag = base.GetDistributionType() == MapRuleDistributionType.Custom;
			int bucketCount = base.GetBucketCount();
			for (int i = 0; i < bucketCount; i++)
			{
				PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
				if (i < colorRange.Length)
				{
					predefinedSymbol.Color = colorRange[i];
				}
				else
				{
					predefinedSymbol.Color = Color.Empty;
				}
				if (flag)
				{
					MapBucket bucket = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[i];
					predefinedSymbol.FromValue = base.GetFromValue(bucket);
					predefinedSymbol.ToValue = base.GetToValue(bucket);
				}
				customSymbols.Add(predefinedSymbol);
			}
		}

		private void RenderPolygonColorRangeRule(ShapeRule shapeRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)base.m_mapRule;
			this.RenderShapeCustomColors(shapeRule, ColoringMode.ColorRange, MapColorPalette.Dundas, this.GetFromColor(colorRangeRule), this.GetMiddleColor(colorRangeRule), this.GetToColor(colorRangeRule));
		}

		private void RenderPolygonColorPaletteRule(ShapeRule shapeRule)
		{
			this.RenderShapeCustomColors(shapeRule, ColoringMode.DistinctColors, this.GetColorPalette(), Color.Empty, Color.Empty, Color.Empty);
		}

		private void RenderPolygonCustomColorRule(ShapeRule shapeRule)
		{
			shapeRule.UseCustomColors = true;
			this.SetRuleColors(this.GetCustomColors(((MapCustomColorRule)base.m_mapRule).MapCustomColors), shapeRule.CustomColors);
		}

		private void RenderSymbolColorRangeRule(SymbolRule symbolRule)
		{
			MapColorRangeRule colorRangeRule = (MapColorRangeRule)base.m_mapRule;
			this.SetSymbolRuleColors(symbolRule.GetColors(ColoringMode.ColorRange, MapColorPalette.Dundas, this.GetFromColor(colorRangeRule), this.GetMiddleColor(colorRangeRule), this.GetToColor(colorRangeRule), base.GetBucketCount()), symbolRule.PredefinedSymbols);
		}

		private void RenderSymbolColorPaletteRule(SymbolRule symbolRule)
		{
			this.SetSymbolRuleColors(symbolRule.GetColors(ColoringMode.DistinctColors, this.GetColorPalette(), Color.Empty, Color.Empty, Color.Empty, base.GetBucketCount()), symbolRule.PredefinedSymbols);
		}

		private void RenderShapeCustomColors(ShapeRule shapeRule, ColoringMode coloringMode, MapColorPalette palette, Color fromColor, Color middleColor, Color toColor)
		{
			shapeRule.UseCustomColors = true;
			this.SetRuleColors(shapeRule.GetColors(coloringMode, palette, fromColor, middleColor, toColor, base.GetBucketCount()), shapeRule.CustomColors);
		}

		private void RenderPathCustomColors(PathRule pathRule, ColoringMode coloringMode, MapColorPalette palette, Color fromColor, Color middleColor, Color toColor)
		{
			pathRule.UseCustomColors = true;
			this.SetRuleColors(pathRule.GetColors(coloringMode, palette, fromColor, middleColor, toColor, base.GetBucketCount()), pathRule.CustomColors);
		}

		private MapColorPalette GetMapColorPalette(MapPalette palette)
		{
			switch (palette)
			{
			case MapPalette.BrightPastel:
				return MapColorPalette.Dundas;
			case MapPalette.Light:
				return MapColorPalette.Light;
			case MapPalette.SemiTransparent:
				return MapColorPalette.SemiTransparent;
			case MapPalette.Pacific:
				return MapColorPalette.Pacific;
			default:
				return MapColorPalette.Random;
			}
		}

		private Color GetCustomColor(MapCustomColor mapCustomColor)
		{
			ReportColorProperty color = mapCustomColor.Color;
			Color empty = Color.Empty;
			if (color != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(color, ref empty))
				{
					return empty;
				}
				ReportColor color2 = mapCustomColor.Instance.Color;
				if (color2 != null)
				{
					return color2.ToColor();
				}
			}
			return empty;
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.ColorOnly;
			return symbolRule;
		}
	}
}
