namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAreaInstance : BaseInstance
	{
		private ChartArea m_chartAreaDef;

		private StyleInstance m_style;

		private bool? m_hidden;

		private ChartAreaAlignOrientations? m_alignOrientation;

		private bool? m_equallySizedAxesFont;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartAreaDef, this.m_chartAreaDef.ChartDef, this.m_chartAreaDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue && !this.m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					this.m_hidden = this.m_chartAreaDef.ChartAreaDef.EvaluateHidden(this.ReportScopeInstance, this.m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public ChartAreaAlignOrientations AlignOrientation
		{
			get
			{
				if (!this.m_alignOrientation.HasValue && !this.m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					this.m_alignOrientation = this.m_chartAreaDef.ChartAreaDef.EvaluateAlignOrientation(this.ReportScopeInstance, this.m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_alignOrientation.Value;
			}
		}

		public bool EquallySizedAxesFont
		{
			get
			{
				if (!this.m_equallySizedAxesFont.HasValue && !this.m_chartAreaDef.ChartDef.IsOldSnapshot)
				{
					this.m_equallySizedAxesFont = this.m_chartAreaDef.ChartAreaDef.EvaluateEquallySizedAxesFont(this.ReportScopeInstance, this.m_chartAreaDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_equallySizedAxesFont.Value;
			}
		}

		internal ChartAreaInstance(ChartArea chartAreaDef)
			: base(chartAreaDef.ChartDef)
		{
			this.m_chartAreaDef = chartAreaDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_hidden = null;
			this.m_alignOrientation = null;
			this.m_equallySizedAxesFont = null;
		}
	}
}
