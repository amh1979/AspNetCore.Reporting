using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarker : MapObjectCollectionItem
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker m_defObject;

		private ReportEnumProperty<MapMarkerStyle> m_mapMarkerStyle;

		private MapMarkerImage m_mapMarkerImage;

		public ReportEnumProperty<MapMarkerStyle> MapMarkerStyle
		{
			get
			{
				if (this.m_mapMarkerStyle == null && this.m_defObject.MapMarkerStyle != null)
				{
					this.m_mapMarkerStyle = new ReportEnumProperty<MapMarkerStyle>(this.m_defObject.MapMarkerStyle.IsExpression, this.m_defObject.MapMarkerStyle.OriginalText, EnumTranslator.TranslateMapMarkerStyle(this.m_defObject.MapMarkerStyle.StringValue, null));
				}
				return this.m_mapMarkerStyle;
			}
		}

		public MapMarkerImage MapMarkerImage
		{
			get
			{
				if (this.m_mapMarkerImage == null && this.m_defObject.MapMarkerImage != null)
				{
					this.m_mapMarkerImage = new MapMarkerImage(this.m_defObject.MapMarkerImage, this.m_map);
				}
				return this.m_mapMarkerImage;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker MapMarkerDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapMarkerInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new MapMarkerInstance(this);
				}
				return (MapMarkerInstance)base.m_instance;
			}
		}

		internal MapMarker(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarker defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapMarkerImage != null)
			{
				this.m_mapMarkerImage.SetNewContext();
			}
		}
	}
}
