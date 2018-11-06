namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointLayerInstance : MapVectorLayerInstance
	{
		private MapPointLayer m_defObject;

		internal MapPointLayerInstance(MapPointLayer defObject)
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
