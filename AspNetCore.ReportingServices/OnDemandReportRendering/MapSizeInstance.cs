namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeInstance : BaseInstance
	{
		private MapSize m_defObject;

		private double? m_width;

		private double? m_height;

		private Unit? m_unit;

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.MapSizeDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!this.m_height.HasValue)
				{
					this.m_height = this.m_defObject.MapSizeDef.EvaluateHeight(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_height.Value;
			}
		}

		public Unit Unit
		{
			get
			{
				if (!this.m_unit.HasValue)
				{
					this.m_unit = this.m_defObject.MapSizeDef.EvaluateUnit(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_unit.Value;
			}
		}

		internal MapSizeInstance(MapSize defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_width = null;
			this.m_height = null;
			this.m_unit = null;
		}
	}
}
