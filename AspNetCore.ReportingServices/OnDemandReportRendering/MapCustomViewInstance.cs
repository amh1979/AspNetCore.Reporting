using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomViewInstance : MapViewInstance
	{
		private MapCustomView m_defObject;

		private double? m_centerX;

		private double? m_centerY;

		public double CenterX
		{
			get
			{
				if (!this.m_centerX.HasValue)
				{
					this.m_centerX = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView)this.m_defObject.MapViewDef).EvaluateCenterX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_centerX.Value;
			}
		}

		public double CenterY
		{
			get
			{
				if (!this.m_centerY.HasValue)
				{
					this.m_centerY = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView)this.m_defObject.MapViewDef).EvaluateCenterY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_centerY.Value;
			}
		}

		internal MapCustomViewInstance(MapCustomView defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_centerX = null;
			this.m_centerY = null;
		}
	}
}
