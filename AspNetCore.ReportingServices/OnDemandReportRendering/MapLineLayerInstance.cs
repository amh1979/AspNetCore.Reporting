namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineLayerInstance : MapVectorLayerInstance
	{
		private MapLineLayer m_defObject;

		internal MapLineLayerInstance(MapLineLayer defObject)
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
