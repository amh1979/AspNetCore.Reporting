using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapElementViewInstance : MapViewInstance
	{
		private MapElementView m_defObject;

		private string m_layerName;

		public string LayerName
		{
			get
			{
				if (this.m_layerName == null)
				{
					this.m_layerName = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapElementView)this.m_defObject.MapViewDef).EvaluateLayerName(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_layerName;
			}
		}

		internal MapElementViewInstance(MapElementView defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_layerName = null;
		}
	}
}
