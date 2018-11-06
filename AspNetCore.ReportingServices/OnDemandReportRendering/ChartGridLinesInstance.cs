namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartGridLinesInstance : BaseInstance
	{
		private ChartGridLines m_gridLinesDef;

		private StyleInstance m_style;

		private ChartAutoBool? m_enabled;

		private double? m_interval;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalType;

		private ChartIntervalType? m_intervalOffsetType;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_gridLinesDef, this.m_gridLinesDef.ChartDef, this.m_gridLinesDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartAutoBool Enabled
		{
			get
			{
				if (!this.m_enabled.HasValue && !this.m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					this.m_enabled = this.m_gridLinesDef.ChartGridLinesDef.EvaluateEnabled(this.ReportScopeInstance, this.m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_enabled.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue && !this.m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					this.m_interval = this.m_gridLinesDef.ChartGridLinesDef.EvaluateInterval(this.ReportScopeInstance, this.m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue && !this.m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffset = this.m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!this.m_intervalType.HasValue && !this.m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalType = this.m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalType(this.ReportScopeInstance, this.m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalType.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!this.m_intervalOffsetType.HasValue && !this.m_gridLinesDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffsetType = this.m_gridLinesDef.ChartGridLinesDef.EvaluateIntervalOffsetType(this.ReportScopeInstance, this.m_gridLinesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffsetType.Value;
			}
		}

		internal ChartGridLinesInstance(ChartGridLines gridlinesDef)
			: base(gridlinesDef.ChartDef)
		{
			this.m_gridLinesDef = gridlinesDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_enabled = null;
			this.m_interval = null;
			this.m_intervalOffset = null;
			this.m_intervalType = null;
			this.m_intervalOffsetType = null;
		}
	}
}
