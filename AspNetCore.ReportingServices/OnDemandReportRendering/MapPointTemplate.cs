using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapPointTemplate : MapSpatialElementTemplate
	{
		private ReportSizeProperty m_size;

		private ReportEnumProperty<MapPointLabelPlacement> m_labelPlacement;

		public ReportSizeProperty Size
		{
			get
			{
				if (this.m_size == null && this.MapPointTemplateDef.Size != null)
				{
					this.m_size = new ReportSizeProperty(this.MapPointTemplateDef.Size);
				}
				return this.m_size;
			}
		}

		public ReportEnumProperty<MapPointLabelPlacement> LabelPlacement
		{
			get
			{
				if (this.m_labelPlacement == null && this.MapPointTemplateDef.LabelPlacement != null)
				{
					this.m_labelPlacement = new ReportEnumProperty<MapPointLabelPlacement>(this.MapPointTemplateDef.LabelPlacement.IsExpression, this.MapPointTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapPointLabelPlacement(this.MapPointTemplateDef.LabelPlacement.StringValue, null));
				}
				return this.m_labelPlacement;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate MapPointTemplateDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate)base.MapSpatialElementTemplateDef;
			}
		}

		internal new MapPointTemplateInstance Instance
		{
			get
			{
				return (MapPointTemplateInstance)this.GetInstance();
			}
		}

		internal MapPointTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
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
