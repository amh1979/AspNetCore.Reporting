namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerInstance : BaseInstance
	{
		private MapMarker m_defObject;

		private MapMarkerStyle? m_mapMarkerStyle;

		public MapMarkerStyle MapMarkerStyle
		{
			get
			{
				if (!this.m_mapMarkerStyle.HasValue)
				{
					this.m_mapMarkerStyle = this.m_defObject.MapMarkerDef.EvaluateMapMarkerStyle(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_mapMarkerStyle.Value;
			}
		}

		internal MapMarkerInstance(MapMarker defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_mapMarkerStyle = null;
		}
	}
}
