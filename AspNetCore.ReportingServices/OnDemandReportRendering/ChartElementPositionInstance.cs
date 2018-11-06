namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartElementPositionInstance : BaseInstance
	{
		private ChartElementPosition m_defObject;

		private double? m_top;

		private double? m_left;

		private double? m_height;

		private double? m_width;

		public double Top
		{
			get
			{
				if (!this.m_top.HasValue)
				{
					this.m_top = this.m_defObject.ChartElementPositionDef.EvaluateTop(this.ReportScopeInstance, this.m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_top.Value;
			}
		}

		public double Left
		{
			get
			{
				if (!this.m_left.HasValue)
				{
					this.m_left = this.m_defObject.ChartElementPositionDef.EvaluateLeft(this.ReportScopeInstance, this.m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_left.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!this.m_height.HasValue)
				{
					this.m_height = this.m_defObject.ChartElementPositionDef.EvaluateHeight(this.ReportScopeInstance, this.m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_height.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.ChartElementPositionDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		internal ChartElementPositionInstance(ChartElementPosition defObject)
			: base(defObject.ChartDef)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_top = null;
			this.m_left = null;
			this.m_height = null;
			this.m_width = null;
		}
	}
}
