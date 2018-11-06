namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisScaleBreakInstance : BaseInstance
	{
		private ChartAxisScaleBreak m_chartAxisScaleBreakDef;

		private StyleInstance m_style;

		private bool? m_enabled;

		private ChartBreakLineType? m_breakLineType;

		private int? m_collapsibleSpaceThreshold;

		private int? m_maxNumberOfBreaks;

		private double? m_spacing;

		private ChartAutoBool? m_includeZero;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartAxisScaleBreakDef, this.m_chartAxisScaleBreakDef.ChartDef, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public bool Enabled
		{
			get
			{
				if (!this.m_enabled.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					this.m_enabled = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateEnabled(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_enabled.Value;
			}
		}

		public ChartBreakLineType BreakLineType
		{
			get
			{
				if (!this.m_breakLineType.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					this.m_breakLineType = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateBreakLineType(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_breakLineType.Value;
			}
		}

		public int CollapsibleSpaceThreshold
		{
			get
			{
				if (!this.m_collapsibleSpaceThreshold.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					this.m_collapsibleSpaceThreshold = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateCollapsibleSpaceThreshold(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_collapsibleSpaceThreshold.Value;
			}
		}

		public int MaxNumberOfBreaks
		{
			get
			{
				if (!this.m_maxNumberOfBreaks.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					this.m_maxNumberOfBreaks = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateMaxNumberOfBreaks(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_maxNumberOfBreaks.Value;
			}
		}

		public double Spacing
		{
			get
			{
				if (!this.m_spacing.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					this.m_spacing = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateSpacing(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_spacing.Value;
			}
		}

		public ChartAutoBool IncludeZero
		{
			get
			{
				if (!this.m_includeZero.HasValue && !this.m_chartAxisScaleBreakDef.ChartDef.IsOldSnapshot)
				{
					string val = this.m_chartAxisScaleBreakDef.ChartAxisScaleBreakDef.EvaluateIncludeZero(this.ReportScopeInstance, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext);
					this.m_includeZero = EnumTranslator.TranslateChartAutoBool(val, this.m_chartAxisScaleBreakDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return this.m_includeZero.Value;
			}
		}

		internal ChartAxisScaleBreakInstance(ChartAxisScaleBreak chartAxisScaleBreakDef)
			: base(chartAxisScaleBreakDef.ChartDef)
		{
			this.m_chartAxisScaleBreakDef = chartAxisScaleBreakDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_enabled = null;
			this.m_breakLineType = null;
			this.m_collapsibleSpaceThreshold = null;
			this.m_maxNumberOfBreaks = null;
			this.m_spacing = null;
			this.m_includeZero = null;
		}
	}
}
