using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataRegionInstance : MapSpatialDataInstance
	{
		private MapSpatialDataRegion m_defObject;

		private object m_vectorData;

		public object VectorData
		{
			get
			{
				if (this.m_vectorData == null)
				{
					this.m_vectorData = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)this.m_defObject.MapSpatialDataDef).EvaluateVectorData(this.m_defObject.ReportScope.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_vectorData;
			}
		}

		internal MapSpatialDataRegionInstance(MapSpatialDataRegion defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_vectorData = null;
		}
	}
}
