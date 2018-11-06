using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapPolygonTemplate m_defObject;

		private double? m_scaleFactor;

		private double? m_centerPointOffsetX;

		private double? m_centerPointOffsetY;

		private MapAutoBool? m_showLabel;

		private MapPolygonLabelPlacement? m_labelPlacement;

		public double ScaleFactor
		{
			get
			{
				if (!this.m_scaleFactor.HasValue)
				{
					this.m_scaleFactor = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateScaleFactor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_scaleFactor.Value;
			}
		}

		public double CenterPointOffsetX
		{
			get
			{
				if (!this.m_centerPointOffsetX.HasValue)
				{
					this.m_centerPointOffsetX = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateCenterPointOffsetX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_centerPointOffsetX.Value;
			}
		}

		public double CenterPointOffsetY
		{
			get
			{
				if (!this.m_centerPointOffsetY.HasValue)
				{
					this.m_centerPointOffsetY = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateCenterPointOffsetY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_centerPointOffsetY.Value;
			}
		}

		public MapAutoBool ShowLabel
		{
			get
			{
				if (!this.m_showLabel.HasValue)
				{
					this.m_showLabel = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateShowLabel(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_showLabel.Value;
			}
		}

		public MapPolygonLabelPlacement LabelPlacement
		{
			get
			{
				if (!this.m_labelPlacement.HasValue)
				{
					this.m_labelPlacement = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelPlacement.Value;
			}
		}

		internal MapPolygonTemplateInstance(MapPolygonTemplate defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_scaleFactor = null;
			this.m_centerPointOffsetX = null;
			this.m_centerPointOffsetY = null;
			this.m_showLabel = null;
			this.m_labelPlacement = null;
		}
	}
}
