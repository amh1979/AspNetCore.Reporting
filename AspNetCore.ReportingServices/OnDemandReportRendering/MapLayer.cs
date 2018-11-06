using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapLayer : MapObjectCollectionItem
	{
		protected Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer m_defObject;

		private ReportEnumProperty<MapVisibilityMode> m_visibilityMode;

		private ReportDoubleProperty m_minimumZoom;

		private ReportDoubleProperty m_maximumZoom;

		private ReportDoubleProperty m_transparency;

		public string Name
		{
			get
			{
				return this.m_defObject.Name;
			}
		}

		public ReportEnumProperty<MapVisibilityMode> VisibilityMode
		{
			get
			{
				if (this.m_visibilityMode == null && this.m_defObject.VisibilityMode != null)
				{
					this.m_visibilityMode = new ReportEnumProperty<MapVisibilityMode>(this.m_defObject.VisibilityMode.IsExpression, this.m_defObject.VisibilityMode.OriginalText, EnumTranslator.TranslateMapVisibilityMode(this.m_defObject.VisibilityMode.StringValue, null));
				}
				return this.m_visibilityMode;
			}
		}

		public ReportDoubleProperty MinimumZoom
		{
			get
			{
				if (this.m_minimumZoom == null && this.m_defObject.MinimumZoom != null)
				{
					this.m_minimumZoom = new ReportDoubleProperty(this.m_defObject.MinimumZoom);
				}
				return this.m_minimumZoom;
			}
		}

		public ReportDoubleProperty MaximumZoom
		{
			get
			{
				if (this.m_maximumZoom == null && this.m_defObject.MaximumZoom != null)
				{
					this.m_maximumZoom = new ReportDoubleProperty(this.m_defObject.MaximumZoom);
				}
				return this.m_maximumZoom;
			}
		}

		public ReportDoubleProperty Transparency
		{
			get
			{
				if (this.m_transparency == null && this.m_defObject.Transparency != null)
				{
					this.m_transparency = new ReportDoubleProperty(this.m_defObject.Transparency);
				}
				return this.m_transparency;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer MapLayerDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal MapLayerInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLayer defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal abstract MapLayerInstance GetInstance();

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
