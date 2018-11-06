using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonTemplate : MapSpatialElementTemplate
	{
		private ReportDoubleProperty m_scaleFactor;

		private ReportDoubleProperty m_centerPointOffsetX;

		private ReportDoubleProperty m_centerPointOffsetY;

		private ReportEnumProperty<MapAutoBool> m_showLabel;

		private ReportEnumProperty<MapPolygonLabelPlacement> m_labelPlacement;

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (this.m_scaleFactor == null && this.MapPolygonTemplateDef.ScaleFactor != null)
				{
					this.m_scaleFactor = new ReportDoubleProperty(this.MapPolygonTemplateDef.ScaleFactor);
				}
				return this.m_scaleFactor;
			}
		}

		public ReportDoubleProperty CenterPointOffsetX
		{
			get
			{
				if (this.m_centerPointOffsetX == null && this.MapPolygonTemplateDef.CenterPointOffsetX != null)
				{
					this.m_centerPointOffsetX = new ReportDoubleProperty(this.MapPolygonTemplateDef.CenterPointOffsetX);
				}
				return this.m_centerPointOffsetX;
			}
		}

		public ReportDoubleProperty CenterPointOffsetY
		{
			get
			{
				if (this.m_centerPointOffsetY == null && this.MapPolygonTemplateDef.CenterPointOffsetY != null)
				{
					this.m_centerPointOffsetY = new ReportDoubleProperty(this.MapPolygonTemplateDef.CenterPointOffsetY);
				}
				return this.m_centerPointOffsetY;
			}
		}

		public ReportEnumProperty<MapAutoBool> ShowLabel
		{
			get
			{
				if (this.m_showLabel == null && this.MapPolygonTemplateDef.ShowLabel != null)
				{
					this.m_showLabel = new ReportEnumProperty<MapAutoBool>(this.MapPolygonTemplateDef.ShowLabel.IsExpression, this.MapPolygonTemplateDef.ShowLabel.OriginalText, EnumTranslator.TranslateMapAutoBool(this.MapPolygonTemplateDef.ShowLabel.StringValue, null));
				}
				return this.m_showLabel;
			}
		}

		public ReportEnumProperty<MapPolygonLabelPlacement> LabelPlacement
		{
			get
			{
				if (this.m_labelPlacement == null && this.MapPolygonTemplateDef.LabelPlacement != null)
				{
					this.m_labelPlacement = new ReportEnumProperty<MapPolygonLabelPlacement>(this.MapPolygonTemplateDef.LabelPlacement.IsExpression, this.MapPolygonTemplateDef.LabelPlacement.OriginalText, EnumTranslator.TranslateMapPolygonLabelPlacement(this.MapPolygonTemplateDef.LabelPlacement.StringValue, null));
				}
				return this.m_labelPlacement;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate MapPolygonTemplateDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)base.MapSpatialElementTemplateDef;
			}
		}

		public new MapPolygonTemplateInstance Instance
		{
			get
			{
				return (MapPolygonTemplateInstance)this.GetInstance();
			}
		}

		internal MapPolygonTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate defObject, MapPolygonLayer shapeLayer, Map map)
			: base(defObject, shapeLayer, map)
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
				base.m_instance = new MapPolygonTemplateInstance(this);
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
