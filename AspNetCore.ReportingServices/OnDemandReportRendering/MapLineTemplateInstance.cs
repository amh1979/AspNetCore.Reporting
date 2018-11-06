using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapLineTemplate m_defObject;

		private ReportSize m_width;

		private MapLineLabelPlacement? m_labelPlacement;

		public ReportSize Width
		{
			get
			{
				if (this.m_width == null)
				{
					this.m_width = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateWidth(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_width;
			}
		}

		public MapLineLabelPlacement LabelPlacement
		{
			get
			{
				if (!this.m_labelPlacement.HasValue)
				{
					this.m_labelPlacement = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelPlacement.Value;
			}
		}

		internal MapLineTemplateInstance(MapLineTemplate defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_width = null;
			this.m_labelPlacement = null;
		}
	}
}
