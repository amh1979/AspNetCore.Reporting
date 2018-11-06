using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileLayer : MapLayer
	{
		private ReportStringProperty m_serviceUrl;

		private ReportEnumProperty<MapTileStyle> m_tileStyle;

		private MapTileCollection m_mapTiles;

		private ReportBoolProperty m_useSecureConnection;

		public ReportStringProperty ServiceUrl
		{
			get
			{
				if (this.m_serviceUrl == null && this.MapTileLayerDef.ServiceUrl != null)
				{
					this.m_serviceUrl = new ReportStringProperty(this.MapTileLayerDef.ServiceUrl);
				}
				return this.m_serviceUrl;
			}
		}

		public ReportEnumProperty<MapTileStyle> TileStyle
		{
			get
			{
				if (this.m_tileStyle == null && this.MapTileLayerDef.TileStyle != null)
				{
					this.m_tileStyle = new ReportEnumProperty<MapTileStyle>(this.MapTileLayerDef.TileStyle.IsExpression, this.MapTileLayerDef.TileStyle.OriginalText, EnumTranslator.TranslateMapTileStyle(this.MapTileLayerDef.TileStyle.StringValue, null));
				}
				return this.m_tileStyle;
			}
		}

		public ReportBoolProperty UseSecureConnection
		{
			get
			{
				if (this.m_useSecureConnection == null && this.MapTileLayerDef.UseSecureConnection != null)
				{
					this.m_useSecureConnection = new ReportBoolProperty(this.MapTileLayerDef.UseSecureConnection);
				}
				return this.m_useSecureConnection;
			}
		}

		public MapTileCollection MapTiles
		{
			get
			{
				if (this.m_mapTiles == null && this.MapTileLayerDef.MapTiles != null)
				{
					this.m_mapTiles = new MapTileCollection(this, base.m_map);
				}
				return this.m_mapTiles;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer MapTileLayerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)base.MapLayerDef;
			}
		}

		public new MapTileLayerInstance Instance
		{
			get
			{
				return (MapTileLayerInstance)this.GetInstance();
			}
		}

		internal MapTileLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapLayerInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapTileLayerInstance(this);
			}
			return (MapLayerInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapTiles != null)
			{
				this.m_mapTiles.SetNewContext();
			}
		}
	}
}
