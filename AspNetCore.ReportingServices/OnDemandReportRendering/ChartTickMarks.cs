using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTickMarks : IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks m_chartTickMarksDef;

		private ChartTickMarksInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<ChartAutoBool> m_enabled;

		private ReportEnumProperty<ChartTickMarksType> m_type;

		private ReportDoubleProperty m_length;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartTickMarksDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Enabled
		{
			get
			{
				if (this.m_enabled == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_type != null)
						{
							this.m_enabled = new ReportEnumProperty<ChartAutoBool>((ChartAutoBool)((this.m_type.Value != 0) ? 1 : 2));
						}
					}
					else if (this.m_chartTickMarksDef.Enabled != null)
					{
						this.m_enabled = new ReportEnumProperty<ChartAutoBool>(this.m_chartTickMarksDef.Enabled.IsExpression, this.m_chartTickMarksDef.Enabled.OriginalText, (!this.m_chartTickMarksDef.Enabled.IsExpression) ? EnumTranslator.TranslateChartAutoBool(this.m_chartTickMarksDef.Enabled.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return this.m_enabled;
			}
		}

		public ReportEnumProperty<ChartTickMarksType> Type
		{
			get
			{
				if (this.m_type == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.Type != null)
				{
					this.m_type = new ReportEnumProperty<ChartTickMarksType>(this.m_chartTickMarksDef.Type.IsExpression, this.m_chartTickMarksDef.Type.OriginalText, EnumTranslator.TranslateChartTickMarksType(this.m_chartTickMarksDef.Type.StringValue, null));
				}
				return this.m_type;
			}
		}

		public ReportDoubleProperty Length
		{
			get
			{
				if (this.m_length == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.Length != null)
				{
					this.m_length = new ReportDoubleProperty(this.m_chartTickMarksDef.Length);
				}
				return this.m_length;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.m_chartTickMarksDef.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (this.m_intervalType == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.IntervalType != null)
				{
					this.m_intervalType = new ReportEnumProperty<ChartIntervalType>(this.m_chartTickMarksDef.IntervalType.IsExpression, this.m_chartTickMarksDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_chartTickMarksDef.IntervalType.StringValue, null));
				}
				return this.m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.m_chartTickMarksDef.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (this.m_intervalOffsetType == null && !this.m_chart.IsOldSnapshot && this.m_chartTickMarksDef.IntervalOffsetType != null)
				{
					this.m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(this.m_chartTickMarksDef.IntervalOffsetType.IsExpression, this.m_chartTickMarksDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_chartTickMarksDef.IntervalOffsetType.StringValue, null));
				}
				return this.m_intervalOffsetType;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks ChartTickMarksDef
		{
			get
			{
				return this.m_chartTickMarksDef;
			}
		}

		public ChartTickMarksInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartTickMarksInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartTickMarks(Axis.TickMarks type, Chart chart)
		{
			this.m_type = new ReportEnumProperty<ChartTickMarksType>(this.GetTickMarksType(type));
			this.m_chart = chart;
		}

		internal ChartTickMarks(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartTickMarks chartTickMarksDef, Chart chart)
		{
			this.m_chartTickMarksDef = chartTickMarksDef;
			this.m_chart = chart;
		}

		private ChartTickMarksType GetTickMarksType(Axis.TickMarks tickMarks)
		{
			switch (tickMarks)
			{
			case Axis.TickMarks.Cross:
				return ChartTickMarksType.Cross;
			case Axis.TickMarks.Inside:
				return ChartTickMarksType.Inside;
			case Axis.TickMarks.Outside:
				return ChartTickMarksType.Outside;
			default:
				return ChartTickMarksType.None;
			}
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
