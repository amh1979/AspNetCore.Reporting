namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonLayerInstance : MapVectorLayerInstance
	{
		private MapPolygonLayer m_defObject;

		internal MapPolygonLayerInstance(MapPolygonLayer defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
