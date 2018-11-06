namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTickMarksInstance : BaseInstance
	{
		private ChartTickMarks m_chartTickMarksDef;

		private StyleInstance m_style;

		private ChartAutoBool? m_enabled;

		private ChartTickMarksType? m_type;

		private double? m_length;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartTickMarksDef, this.m_chartTickMarksDef.ChartDef, this.m_chartTickMarksDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartAutoBool Enabled
		{
			get
			{
				if (!this.m_enabled.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					string val = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateEnabled(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
					this.m_enabled = EnumTranslator.TranslateChartAutoBool(val, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return this.m_enabled.Value;
			}
		}

		public ChartTickMarksType Type
		{
			get
			{
				if (!this.m_type.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_type = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateType(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_type.Value;
			}
		}

		public double Length
		{
			get
			{
				if (!this.m_length.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_length = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateLength(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_length.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_interval = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateInterval(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!this.m_intervalType.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalType = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalType(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffset = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!this.m_intervalOffsetType.HasValue && !this.m_chartTickMarksDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffsetType = this.m_chartTickMarksDef.ChartTickMarksDef.EvaluateIntervalOffsetType(this.ReportScopeInstance, this.m_chartTickMarksDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffsetType.Value;
			}
		}

		internal ChartTickMarksInstance(ChartTickMarks chartTickMarksDef)
			: base(chartTickMarksDef.ChartDef)
		{
			this.m_chartTickMarksDef = chartTickMarksDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_enabled = null;
			this.m_type = null;
			this.m_length = null;
			this.m_interval = null;
			this.m_intervalType = null;
			this.m_intervalOffset = null;
			this.m_intervalOffsetType = null;
		}
	}
}
