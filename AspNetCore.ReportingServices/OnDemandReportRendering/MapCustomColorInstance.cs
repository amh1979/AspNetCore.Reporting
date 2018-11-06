namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorInstance : BaseInstance
	{
		private MapCustomColor m_defObject;

		private ReportColor m_color;

		public ReportColor Color
		{
			get
			{
				if (this.m_color == null)
				{
					this.m_color = new ReportColor(this.m_defObject.MapCustomColorDef.EvaluateColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_color;
			}
		}

		internal MapCustomColorInstance(MapCustomColor defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_color = null;
		}
	}
}
