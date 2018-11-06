using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineInstance : MapSpatialElementInstance
	{
		private MapLine m_defObject;

		private bool? m_useCustomLineTemplate;

		public bool UseCustomLineTemplate
		{
			get
			{
				if (!this.m_useCustomLineTemplate.HasValue)
				{
					this.m_useCustomLineTemplate = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine)this.m_defObject.MapSpatialElementDef).EvaluateUseCustomLineTemplate(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_useCustomLineTemplate.Value;
			}
		}

		internal MapLineInstance(MapLine defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_useCustomLineTemplate = null;
		}
	}
}
