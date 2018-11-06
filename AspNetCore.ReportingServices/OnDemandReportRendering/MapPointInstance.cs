using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointInstance : MapSpatialElementInstance
	{
		private MapPoint m_defObject;

		private bool? m_useCustomPointTemplate;

		public bool UseCustomPointTemplate
		{
			get
			{
				if (!this.m_useCustomPointTemplate.HasValue)
				{
					this.m_useCustomPointTemplate = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint)this.m_defObject.MapSpatialElementDef).EvaluateUseCustomPointTemplate(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_useCustomPointTemplate.Value;
			}
		}

		internal MapPointInstance(MapPoint defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_useCustomPointTemplate = null;
		}
	}
}
