namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonRulesInstance : BaseInstance
	{
		private MapPolygonRules m_defObject;

		internal MapPolygonRulesInstance(MapPolygonRules defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
