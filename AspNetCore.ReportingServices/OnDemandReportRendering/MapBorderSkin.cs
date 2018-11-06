using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBorderSkin : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin m_defObject;

		private MapBorderSkinInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<MapBorderSkinType> m_mapBorderSkinType;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_map, this.m_map.ReportScope, this.m_defObject, this.m_map.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<MapBorderSkinType> MapBorderSkinType
		{
			get
			{
				if (this.m_mapBorderSkinType == null && this.m_defObject.MapBorderSkinType != null)
				{
					this.m_mapBorderSkinType = new ReportEnumProperty<MapBorderSkinType>(this.m_defObject.MapBorderSkinType.IsExpression, this.m_defObject.MapBorderSkinType.OriginalText, EnumTranslator.TranslateMapBorderSkinType(this.m_defObject.MapBorderSkinType.StringValue, null));
				}
				return this.m_mapBorderSkinType;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin MapBorderSkinDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapBorderSkinInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapBorderSkinInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapBorderSkin(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBorderSkin defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
