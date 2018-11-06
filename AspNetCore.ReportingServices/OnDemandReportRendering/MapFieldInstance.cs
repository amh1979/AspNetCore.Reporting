namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldInstance : BaseInstance
	{
		private MapField m_defObject;

		internal MapFieldInstance(MapField defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
