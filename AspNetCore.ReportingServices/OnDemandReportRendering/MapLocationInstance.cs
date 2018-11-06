namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLocationInstance : BaseInstance
	{
		private MapLocation m_defObject;

		private double? m_left;

		private double? m_top;

		private Unit? m_unit;

		public double Left
		{
			get
			{
				if (!this.m_left.HasValue)
				{
					this.m_left = this.m_defObject.MapLocationDef.EvaluateLeft(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_left.Value;
			}
		}

		public double Top
		{
			get
			{
				if (!this.m_top.HasValue)
				{
					this.m_top = this.m_defObject.MapLocationDef.EvaluateTop(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_top.Value;
			}
		}

		public Unit Unit
		{
			get
			{
				if (!this.m_unit.HasValue)
				{
					this.m_unit = this.m_defObject.MapLocationDef.EvaluateUnit(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_unit.Value;
			}
		}

		internal MapLocationInstance(MapLocation defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_left = null;
			this.m_top = null;
			this.m_unit = null;
		}
	}
}
