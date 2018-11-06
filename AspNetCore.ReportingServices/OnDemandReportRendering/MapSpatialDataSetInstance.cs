using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataSetInstance : MapSpatialDataInstance
	{
		private MapSpatialDataSet m_defObject;

		private string m_dataSetName;

		private string m_spatialField;

		public string DataSetName
		{
			get
			{
				if (this.m_dataSetName == null)
				{
					this.m_dataSetName = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)this.m_defObject.MapSpatialDataDef).EvaluateDataSetName(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_dataSetName;
			}
		}

		public string SpatialField
		{
			get
			{
				if (this.m_spatialField == null)
				{
					this.m_spatialField = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)this.m_defObject.MapSpatialDataDef).EvaluateSpatialField(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_spatialField;
			}
		}

		internal MapSpatialDataSetInstance(MapSpatialDataSet defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_dataSetName = null;
			this.m_spatialField = null;
		}
	}
}
