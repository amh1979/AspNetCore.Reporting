using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisScaleBreak : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak m_chartAxisScaleBreakDef;

		private ChartAxisScaleBreakInstance m_instance;

		private Style m_style;

		private ReportBoolProperty m_enabled;

		private ReportEnumProperty<ChartBreakLineType> m_breakLineType;

		private ReportIntProperty m_collapsibleSpaceThreshold;

		private ReportIntProperty m_maxNumberOfBreaks;

		private ReportDoubleProperty m_spacing;

		private ReportEnumProperty<ChartAutoBool> m_includeZero;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartAxisScaleBreakDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportBoolProperty Enabled
		{
			get
			{
				if (this.m_enabled == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.Enabled != null)
				{
					this.m_enabled = new ReportBoolProperty(this.m_chartAxisScaleBreakDef.Enabled);
				}
				return this.m_enabled;
			}
		}

		public ReportEnumProperty<ChartBreakLineType> BreakLineType
		{
			get
			{
				if (this.m_breakLineType == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.BreakLineType != null)
				{
					this.m_breakLineType = new ReportEnumProperty<ChartBreakLineType>(this.m_chartAxisScaleBreakDef.BreakLineType.IsExpression, this.m_chartAxisScaleBreakDef.BreakLineType.OriginalText, EnumTranslator.TranslateChartBreakLineType(this.m_chartAxisScaleBreakDef.BreakLineType.StringValue, null));
				}
				return this.m_breakLineType;
			}
		}

		public ReportIntProperty CollapsibleSpaceThreshold
		{
			get
			{
				if (this.m_collapsibleSpaceThreshold == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold != null)
				{
					this.m_collapsibleSpaceThreshold = new ReportIntProperty(this.m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.IsExpression, this.m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.OriginalText, this.m_chartAxisScaleBreakDef.CollapsibleSpaceThreshold.IntValue, 25);
				}
				return this.m_collapsibleSpaceThreshold;
			}
		}

		public ReportIntProperty MaxNumberOfBreaks
		{
			get
			{
				if (this.m_maxNumberOfBreaks == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.MaxNumberOfBreaks != null)
				{
					this.m_maxNumberOfBreaks = new ReportIntProperty(this.m_chartAxisScaleBreakDef.MaxNumberOfBreaks.IsExpression, this.m_chartAxisScaleBreakDef.MaxNumberOfBreaks.OriginalText, this.m_chartAxisScaleBreakDef.MaxNumberOfBreaks.IntValue, 5);
				}
				return this.m_maxNumberOfBreaks;
			}
		}

		public ReportDoubleProperty Spacing
		{
			get
			{
				if (this.m_spacing == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.Spacing != null)
				{
					this.m_spacing = new ReportDoubleProperty(this.m_chartAxisScaleBreakDef.Spacing);
				}
				return this.m_spacing;
			}
		}

		public ReportEnumProperty<ChartAutoBool> IncludeZero
		{
			get
			{
				if (this.m_includeZero == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisScaleBreakDef.IncludeZero != null)
				{
					this.m_includeZero = new ReportEnumProperty<ChartAutoBool>(this.m_chartAxisScaleBreakDef.IncludeZero.IsExpression, this.m_chartAxisScaleBreakDef.IncludeZero.OriginalText, (!this.m_chartAxisScaleBreakDef.IncludeZero.IsExpression) ? EnumTranslator.TranslateChartAutoBool(this.m_chartAxisScaleBreakDef.IncludeZero.StringValue, null) : ChartAutoBool.Auto);
				}
				return this.m_includeZero;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak ChartAxisScaleBreakDef
		{
			get
			{
				return this.m_chartAxisScaleBreakDef;
			}
		}

		public ChartAxisScaleBreakInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartAxisScaleBreakInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartAxisScaleBreak(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisScaleBreak chartAxisScaleBreakDef, Chart chart)
		{
			this.m_chartAxisScaleBreakDef = chartAxisScaleBreakDef;
			this.m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
