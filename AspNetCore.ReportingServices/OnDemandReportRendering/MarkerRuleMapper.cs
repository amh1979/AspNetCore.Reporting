using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class MarkerRuleMapper : RuleMapper
	{
		internal MarkerRuleMapper(MapMarkerRule mapColorRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
			: base(mapColorRule, vectorLayerMapper, coreSpatialElementManager)
		{
		}

		internal void RenderPointRule(PointTemplateMapper pointTemplateMapper, Color? color, int? size)
		{
			SymbolRule symbolRule = (SymbolRule)base.m_coreRule;
			base.SetRuleLegendProperties(symbolRule);
			base.SetRuleDistribution(symbolRule);
			this.SetSymbolRuleMarkers(symbolRule.PredefinedSymbols);
			this.InitializePredefinedSymbols(symbolRule.PredefinedSymbols, pointTemplateMapper, color, size);
		}

		private void InitializePredefinedSymbols(PredefinedSymbolCollection predefinedSymbols, PointTemplateMapper spatialElementTemplateMapper, Color? color, int? size)
		{
			foreach (PredefinedSymbol predefinedSymbol4 in predefinedSymbols)
			{
				if (color.HasValue)
				{
					predefinedSymbol4.Color = color.Value;
				}
				if (size.HasValue)
				{
					PredefinedSymbol predefinedSymbol2 = predefinedSymbol4;
					PredefinedSymbol predefinedSymbol3 = predefinedSymbol4;
					float num3 = predefinedSymbol2.Width = (predefinedSymbol3.Height = (float)size.Value);
				}
				base.InitializePredefinedSymbols(predefinedSymbol4, spatialElementTemplateMapper);
			}
		}

		private void SetSymbolRuleMarkers(PredefinedSymbolCollection customSymbols)
		{
			int bucketCount = base.GetBucketCount();
			MapMarkerRule mapMarkerRule = (MapMarkerRule)base.m_mapRule;
			MapMarkerCollection mapMarkers = mapMarkerRule.MapMarkers;
			int count = mapMarkers.Count;
			MapBucketCollection mapBuckets = base.m_mapRule.MapBuckets;
			bool flag = base.GetDistributionType() == MapRuleDistributionType.Custom;
			for (int i = 0; i < bucketCount; i++)
			{
				PredefinedSymbol predefinedSymbol = new PredefinedSymbol();
				if (i < count)
				{
					this.RenderMarker(predefinedSymbol, ((ReportElementCollectionBase<MapMarker>)mapMarkers)[i]);
				}
				else
				{
					predefinedSymbol.MarkerStyle = MarkerStyle.None;
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

		private void RenderMarker(PredefinedSymbol customSymbol, MapMarker mapMarker)
		{
			MapMarkerStyle markerStyle = MapMapper.GetMarkerStyle(mapMarker, true);
			if (markerStyle != MapMarkerStyle.Image)
			{
				customSymbol.MarkerStyle = MapMapper.GetMarkerStyle(markerStyle);
			}
			else
			{
				MapMarkerImage mapMarkerImage = mapMarker.MapMarkerImage;
				if (mapMarkerImage == null)
				{
					throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, base.m_mapRule.MapDef.Name, base.m_mapVectorLayer.Name, "MapMarkerImage"));
				}
				customSymbol.Image = base.m_mapMapper.AddImage(mapMarkerImage);
				customSymbol.ImageResizeMode = base.m_mapMapper.GetImageResizeMode(mapMarkerImage);
				customSymbol.ImageTransColor = base.m_mapMapper.GetImageTransColor(mapMarkerImage);
			}
		}

		internal override SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = base.CreateSymbolRule();
			symbolRule.AffectedAttributes = AffectedSymbolAttributes.MarkerOnly;
			return symbolRule;
		}
	}
}
