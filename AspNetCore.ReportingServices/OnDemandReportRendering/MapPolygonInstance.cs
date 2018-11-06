using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonInstance : MapSpatialElementInstance
	{
		private MapPolygon m_defObject;

		private bool? m_useCustomPolygonTemplate;

		private bool? m_useCustomCenterPointTemplate;

		public bool UseCustomPolygonTemplate
		{
			get
			{
				if (!this.m_useCustomPolygonTemplate.HasValue)
				{
					this.m_useCustomPolygonTemplate = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon)this.m_defObject.MapSpatialElementDef).EvaluateUseCustomPolygonTemplate(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_useCustomPolygonTemplate.Value;
			}
		}

		public bool UseCustomCenterPointTemplate
		{
			get
			{
				if (!this.m_useCustomCenterPointTemplate.HasValue)
				{
					this.m_useCustomCenterPointTemplate = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon)this.m_defObject.MapSpatialElementDef).EvaluateUseCustomCenterPointTemplate(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_useCustomCenterPointTemplate.Value;
			}
		}

		internal MapPolygonInstance(MapPolygon defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_useCustomPolygonTemplate = null;
			this.m_useCustomCenterPointTemplate = null;
		}
	}
}
