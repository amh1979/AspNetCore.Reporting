using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineTemplate : MapSpatialElementTemplate
	{
		private ReportSizeProperty m_width;

		private ReportEnumProperty<MapLineLabelPlacement> m_labelPlacement;

		public ReportSizeProperty Width
		{
			get
			{
				if (this.m_width == null && this.MapLineTemplateDef.Width != null)
				{
					this.m_width = new ReportSizeProperty(this.MapLineTemplateDef.Width);
				}
				return this.m_width;
			}
		}

		public ReportEnumProperty<MapLineLabelPlacement> LabelPlacement
		{
			get
			{
				if (this.m_labelPlacement == null && this.MapLineTemplateDef.LabelPlacement != null)
				{
					this.m_labelPlacement = new ReportEnumProperty<MapLineLabelPlacement>(this.MapLineTemplateDef.LabelPlacement.IsExpression, this.MapLineTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapLineLabelPlacement(this.MapLineTemplateDef.LabelPlacement.StringValue, null));
				}
				return this.m_labelPlacement;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate MapLineTemplateDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate)base.MapSpatialElementTemplateDef;
			}
		}

		public new MapLineTemplateInstance Instance
		{
			get
			{
				return (MapLineTemplateInstance)this.GetInstance();
			}
		}

		internal MapLineTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate defObject, MapLineLayer mapLineLayer, Map map)
			: base(defObject, mapLineLayer, map)
		{
		}

		internal override MapSpatialElementTemplateInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapLineTemplateInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
