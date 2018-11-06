using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapPointTemplateInstance : MapSpatialElementTemplateInstance
	{
		private MapPointTemplate m_defObject;

		private ReportSize m_size;

		private MapPointLabelPlacement? m_labelPlacement;

		public ReportSize Size
		{
			get
			{
				if (this.m_size == null)
				{
					this.m_size = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateSize(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_size;
			}
		}

		public MapPointLabelPlacement LabelPlacement
		{
			get
			{
				if (!this.m_labelPlacement.HasValue)
				{
					this.m_labelPlacement = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate)this.m_defObject.MapSpatialElementTemplateDef).EvaluateLabelPlacement(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_labelPlacement.Value;
			}
		}

		internal MapPointTemplateInstance(MapPointTemplate defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_size = null;
			this.m_labelPlacement = null;
		}
	}
}
