using AspNetCore.Reporting.Map.WebForms;
using System;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class SizeRuleMapper : RuleMapper
	{
		internal SizeRuleMapper(MapSizeRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal void RenderSymbolRule(PointTemplateMapper symbolTemplateMapper, Color? color, MarkerStyle? markerStyle)
		{
			SymbolRule symbolRule = (SymbolRule)base.m_coreRule;
			base.SetRuleLegendProperties(symbolRule);
			base.SetRuleDistribution(symbolRule);
			this.SetSymbolRuleSizes(symbolRule.PredefinedSymbols);
			this.InitializePredefinedSymbols(symbolRule.PredefinedSymbols, symbolTemplateMapper, color, markerStyle);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper symbolTemplateMapper, Color? color, MarkerStyle? markerStyle)
		{
			foreach (PredefinedSymbol predefinedSymbol in predefinedSymbols)
			{
				if (color.HasValue)
				{
					predefinedSymbol.Color = color.Value;
				}
				if (markerStyle.HasValue)
				{
					predefinedSymbol.MarkerStyle = markerStyle.Value;
				}
				base.InitializePredefinedSymbols(predefinedSymbol, symbolTemplateMapper);
			}
		}

		private void SetSymbolRuleSizes(PredefinedSymbolCollection customSymbols)
		{
			int bucketCount = base.GetBucketCount();
			if (bucketCount != 0)
			{
				double startSize = this.GetStartSize();
				double num = (this.GetEndSize() - startSize) / (double)bucketCount;
				MapBucketCollection mapBuckets = base.m_mapRule.MapBuckets;
				bool flag = base.GetDistributionType() == MapRuleDistributionType.Custom;
				for (int i = 0; i < bucketCount; i++)
				{
					PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
					PredefinedSymbol predefinedSymbol2 = predefinedSymbol;
					PredefinedSymbol predefinedSymbol3 = predefinedSymbol;
					float num4 = predefinedSymbol2.Width = (predefinedSymbol3.Height = (float)(int)Math.Round(startSize + (double)i * num));
					if (flag)
					{
						MapBucket bucket = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[i];
						predefinedSymbol.FromValue = base.GetFromValue(bucket);
						predefinedSymbol.ToValue = base.GetToValue(bucket);
					}
					customSymbols.Add(predefinedSymbol);
				}
			}
		}

		private double GetStartSize()
		{
			MapSizeRule mapSizeRule = (MapSizeRule)base.m_mapRule;
			ReportSizeProperty startSize = mapSizeRule.StartSize;
			ReportSize size = startSize.IsExpression ? mapSizeRule.Instance.StartSize : startSize.Value;
			return MappingHelper.ToPixels(size, base.m_mapMapper.DpiX);
		}

		private double GetEndSize()
		{
			MapSizeRule mapSizeRule = (MapSizeRule)base.m_mapRule;
			ReportSizeProperty startSize = mapSizeRule.StartSize;
			startSize = mapSizeRule.EndSize;
			ReportSize size = startSize.IsExpression ? mapSizeRule.Instance.EndSize : startSize.Value;
			return MappingHelper.ToPixels(size, base.m_mapMapper.DpiX);
		}

		internal virtual PathWidthRule CreatePathRule()
		{
			PathWidthRule pathWidthRule = (PathWidthRule)(base.m_coreRule = new PathWidthRule());
			pathWidthRule.Text = "";
			pathWidthRule.Category = base.m_mapVectorLayer.Name;
			pathWidthRule.Field = "";
			base.m_coreMap.PathRules.Add(pathWidthRule);
			base.SetRuleFieldName();
			return pathWidthRule;
		}

		internal void RenderLineRule(SpatialElementTemplateMapper spatialElementTemplateMapper, Color? color)
		{
			PathWidthRule pathWidthRule = (PathWidthRule)base.m_coreRule;
			pathWidthRule.UseCustomWidths = true;
			base.SetRuleLegendProperties(pathWidthRule);
			base.SetRuleDistribution(pathWidthRule);
			this.SetPathRuleSizes(pathWidthRule.CustomWidths);
		}

		private void SetPathRuleSizes(CustomWidthCollection customWidths)
		{
			int bucketCount = base.GetBucketCount();
			if (bucketCount != 0)
			{
				double startSize = this.GetStartSize();
				double num = (this.GetEndSize() - startSize) / (double)bucketCount;
				MapBucketCollection mapBuckets = base.m_mapRule.MapBuckets;
				bool flag = base.GetDistributionType() == MapRuleDistributionType.Custom;
				for (int i = 0; i < bucketCount; i++)
				{
					CustomWidth customWidth = new CustomWidth();
					customWidth.Width = (float)(int)Math.Round(startSize + (double)i * num);
					if (flag)
					{
						MapBucket bucket = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[i];
						customWidth.FromValue = base.GetFromValue(bucket);
						customWidth.ToValue = base.GetToValue(bucket);
					}
					customWidths.Add(customWidth);
				}
			}
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.SizeOnly;
			return symbolRule;
		}
	}
}
