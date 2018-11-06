using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class SymbolMarkerTemplateMapper : PointTemplateMapper
	{
		private string sharedImageName;

		internal SymbolMarkerTemplateMapper(MapMapper mapMapper, VectorLayerMapper vectorLayerMapper, MapVectorLayer mapVectorLayer)
			: base(mapMapper, vectorLayerMapper, mapVectorLayer)
		{
		}

		protected override void RenderPointTemplate(MapPointTemplate mapPointTemplate, Symbol coreSymbol, bool customTemplate, bool ignoreBackgoundColor, bool ignoreSize, bool ignoreMarker, bool hasScope)
		{
			base.RenderPointTemplate(mapPointTemplate, coreSymbol, customTemplate, ignoreBackgoundColor, ignoreSize, ignoreMarker, hasScope);
			if (!ignoreMarker)
			{
				MapMarker mapMarker = ((MapMarkerTemplate)mapPointTemplate).MapMarker;
				MapMarkerStyle markerStyle = MapMapper.GetMarkerStyle(mapMarker, hasScope);
				if (markerStyle != MapMarkerStyle.Image)
				{
					coreSymbol.MarkerStyle = MapMapper.GetMarkerStyle(markerStyle);
				}
				else
				{
					MapMarkerImage mapMarkerImage = mapMarker.MapMarkerImage;
					if (mapMarkerImage == null)
					{
						throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name, base.m_mapVectorLayer.Name, "MapMarkerImage"));
					}
					string image;
					if (this.CanShareMarkerImage(mapMarkerImage, customTemplate))
					{
						if (this.sharedImageName == null)
						{
							this.sharedImageName = base.m_mapMapper.AddImage(mapMarkerImage);
						}
						image = this.sharedImageName;
					}
					else
					{
						image = base.m_mapMapper.AddImage(mapMarkerImage);
					}
					coreSymbol.Image = image;
					coreSymbol.ImageResizeMode = base.m_mapMapper.GetImageResizeMode(mapMarkerImage);
					coreSymbol.ImageTransColor = base.m_mapMapper.GetImageTransColor(mapMarkerImage);
				}
			}
		}

		private bool CanShareMarkerImage(MapMarkerImage mapMarkerImage, bool customTemplate)
		{
			if (!mapMarkerImage.MIMEType.IsExpression && !mapMarkerImage.Value.IsExpression)
			{
				return !customTemplate;
			}
			return false;
		}
	}
}
