namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisInstance : BaseInstance
	{
		private ChartAxis m_axisDef;

		private StyleInstance m_style;

		private object m_min;

		private object m_max;

		private object m_crossAt;

		private ChartAutoBool? m_visible;

		private ChartAutoBool? m_margin;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		private double? m_labelInterval;

		private ChartIntervalType? m_labelIntervalType;

		private double? m_labelIntervalOffset;

		private ChartIntervalType? m_labelIntervalOffsetType;

		private bool? m_variableAutoInterval;

		private bool? m_marksAlwaysAtPlotEdge;

		private bool? m_reverse;

		private ChartAxisLocation? m_location;

		private bool? m_interlaced;

		private ReportColor m_interlacedColor;

		private bool? m_logScale;

		private double? m_logBase;

		private bool? m_hideLabels;

		private double? m_angle;

		private bool? m_preventFontShrink;

		private bool? m_preventFontGrow;

		private bool? m_preventLabelOffset;

		private bool? m_preventWordWrap;

		private ChartAxisLabelRotation? m_allowLabelRotation;

		private bool? m_includeZero;

		private bool? m_labelsAutoFitDisabled;

		private ReportSize m_minFontSize;

		private ReportSize m_maxFontSize;

		private bool? m_offsetLabels;

		private bool? m_hideEndLabels;

		private ChartAxisArrow? m_arrows;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_axisDef, this.m_axisDef.ChartDef, this.m_axisDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public object CrossAt
		{
			get
			{
				if (this.m_crossAt == null)
				{
					if (this.m_axisDef.ChartDef.IsOldSnapshot)
					{
						this.m_crossAt = this.m_axisDef.RenderAxisInstance.CrossAtValue;
					}
					else
					{
						this.m_crossAt = this.m_axisDef.AxisDef.EvaluateCrossAt(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_crossAt;
			}
		}

		public object Minimum
		{
			get
			{
				if (this.m_min == null)
				{
					if (this.m_axisDef.ChartDef.IsOldSnapshot)
					{
						this.m_min = this.m_axisDef.RenderAxisInstance.MinValue;
					}
					else
					{
						this.m_min = this.m_axisDef.AxisDef.EvaluateMin(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_min;
			}
		}

		public object Maximum
		{
			get
			{
				if (this.m_max == null)
				{
					if (this.m_axisDef.ChartDef.IsOldSnapshot)
					{
						this.m_max = this.m_axisDef.RenderAxisInstance.MaxValue;
					}
					else
					{
						this.m_max = this.m_axisDef.AxisDef.EvaluateMax(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_max;
			}
		}

		public ChartAutoBool Visible
		{
			get
			{
				if (!this.m_visible.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					string val = this.m_axisDef.AxisDef.EvaluateVisible(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
					this.m_visible = EnumTranslator.TranslateChartAutoBool(val, this.m_axisDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return this.m_visible.Value;
			}
		}

		public ChartAutoBool Margin
		{
			get
			{
				if (!this.m_margin.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					string val = this.m_axisDef.AxisDef.EvaluateMargin(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
					this.m_margin = EnumTranslator.TranslateChartAutoBool(val, this.m_axisDef.ChartDef.RenderingContext.OdpContext.ReportRuntime);
				}
				return this.m_margin.Value;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_interval = this.m_axisDef.AxisDef.EvaluateInterval(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!this.m_intervalType.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalType = this.m_axisDef.AxisDef.EvaluateIntervalType(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffset = this.m_axisDef.AxisDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!this.m_intervalOffsetType.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffsetType = this.m_axisDef.AxisDef.EvaluateIntervalOffsetType(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffsetType.Value;
			}
		}

		public double LabelInterval
		{
			get
			{
				if (!this.m_labelInterval.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_labelInterval = this.m_axisDef.AxisDef.EvaluateLabelInterval(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_labelInterval.Value;
			}
		}

		public ChartIntervalType LabelIntervalType
		{
			get
			{
				if (!this.m_labelIntervalType.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_labelIntervalType = this.m_axisDef.AxisDef.EvaluateLabelIntervalType(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_labelIntervalType.Value;
			}
		}

		public double LabelIntervalOffset
		{
			get
			{
				if (!this.m_labelIntervalOffset.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_labelIntervalOffset = this.m_axisDef.AxisDef.EvaluateLabelIntervalOffset(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_labelIntervalOffset.Value;
			}
		}

		public ChartIntervalType LabelIntervalOffsetType
		{
			get
			{
				if (!this.m_labelIntervalOffsetType.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_labelIntervalOffsetType = this.m_axisDef.AxisDef.EvaluateLabelIntervalOffsetType(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_labelIntervalOffsetType.Value;
			}
		}

		public bool VariableAutoInterval
		{
			get
			{
				if (!this.m_variableAutoInterval.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_variableAutoInterval = this.m_axisDef.AxisDef.EvaluateVariableAutoInterval(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_variableAutoInterval.Value;
			}
		}

		public bool MarksAlwaysAtPlotEdge
		{
			get
			{
				if (!this.m_marksAlwaysAtPlotEdge.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_marksAlwaysAtPlotEdge = this.m_axisDef.AxisDef.EvaluateMarksAlwaysAtPlotEdge(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_marksAlwaysAtPlotEdge.Value;
			}
		}

		public bool Reverse
		{
			get
			{
				if (!this.m_reverse.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_reverse = this.m_axisDef.AxisDef.EvaluateReverse(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_reverse.Value;
			}
		}

		public ChartAxisLocation Location
		{
			get
			{
				if (!this.m_location.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_location = this.m_axisDef.AxisDef.EvaluateLocation(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_location.Value;
			}
		}

		public bool Interlaced
		{
			get
			{
				if (!this.m_interlaced.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_interlaced = this.m_axisDef.AxisDef.EvaluateInterlaced(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interlaced.Value;
			}
		}

		public ReportColor InterlacedColor
		{
			get
			{
				if (this.m_interlacedColor == null && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_interlacedColor = new ReportColor(this.m_axisDef.AxisDef.EvaluateInterlacedColor(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_interlacedColor;
			}
		}

		public bool LogScale
		{
			get
			{
				if (!this.m_logScale.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_logScale = this.m_axisDef.AxisDef.EvaluateLogScale(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_logScale.Value;
			}
		}

		public double LogBase
		{
			get
			{
				if (!this.m_logBase.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_logBase = this.m_axisDef.AxisDef.EvaluateLogBase(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_logBase.Value;
			}
		}

		public bool HideLabels
		{
			get
			{
				if (!this.m_hideLabels.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_hideLabels = this.m_axisDef.AxisDef.EvaluateHideLabels(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hideLabels.Value;
			}
		}

		public double Angle
		{
			get
			{
				if (!this.m_angle.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_angle = this.m_axisDef.AxisDef.EvaluateAngle(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_angle.Value;
			}
		}

		public ChartAxisArrow Arrows
		{
			get
			{
				if (!this.m_arrows.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_arrows = this.m_axisDef.AxisDef.EvaluateArrows(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_arrows.Value;
			}
		}

		public bool PreventFontShrink
		{
			get
			{
				if (!this.m_preventFontShrink.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_preventFontShrink = this.m_axisDef.AxisDef.EvaluatePreventFontShrink(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_preventFontShrink.Value;
			}
		}

		public bool PreventFontGrow
		{
			get
			{
				if (!this.m_preventFontGrow.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_preventFontGrow = this.m_axisDef.AxisDef.EvaluatePreventFontGrow(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_preventFontGrow.Value;
			}
		}

		public bool PreventLabelOffset
		{
			get
			{
				if (!this.m_preventLabelOffset.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_preventLabelOffset = this.m_axisDef.AxisDef.EvaluatePreventLabelOffset(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_preventLabelOffset.Value;
			}
		}

		public bool PreventWordWrap
		{
			get
			{
				if (!this.m_preventWordWrap.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_preventWordWrap = this.m_axisDef.AxisDef.EvaluatePreventWordWrap(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_preventWordWrap.Value;
			}
		}

		public ChartAxisLabelRotation AllowLabelRotation
		{
			get
			{
				if (!this.m_allowLabelRotation.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_allowLabelRotation = this.m_axisDef.AxisDef.EvaluateAllowLabelRotation(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_allowLabelRotation.Value;
			}
		}

		public bool IncludeZero
		{
			get
			{
				if (!this.m_includeZero.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_includeZero = this.m_axisDef.AxisDef.EvaluateIncludeZero(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_includeZero.Value;
			}
		}

		public bool LabelsAutoFitDisabled
		{
			get
			{
				if (!this.m_labelsAutoFitDisabled.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_labelsAutoFitDisabled = this.m_axisDef.AxisDef.EvaluateLabelsAutoFitDisabled(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_labelsAutoFitDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_minFontSize = new ReportSize(this.m_axisDef.AxisDef.EvaluateMinFontSize(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_minFontSize;
			}
		}

		public ReportSize MaxFontSize
		{
			get
			{
				if (this.m_maxFontSize == null && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_maxFontSize = new ReportSize(this.m_axisDef.AxisDef.EvaluateMaxFontSize(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_maxFontSize;
			}
		}

		public bool OffsetLabels
		{
			get
			{
				if (!this.m_offsetLabels.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_offsetLabels = this.m_axisDef.AxisDef.EvaluateOffsetLabels(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_offsetLabels.Value;
			}
		}

		public bool HideEndLabels
		{
			get
			{
				if (!this.m_hideEndLabels.HasValue && !this.m_axisDef.ChartDef.IsOldSnapshot)
				{
					this.m_hideEndLabels = this.m_axisDef.AxisDef.EvaluateHideEndLabels(this.ReportScopeInstance, this.m_axisDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hideEndLabels.Value;
			}
		}

		internal ChartAxisInstance(ChartAxis axisDef)
			: base(axisDef.ChartDef)
		{
			this.m_axisDef = axisDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_min = null;
			this.m_max = null;
			this.m_crossAt = null;
			this.m_visible = null;
			this.m_margin = null;
			this.m_interval = null;
			this.m_intervalType = null;
			this.m_intervalOffset = null;
			this.m_intervalOffsetType = null;
			this.m_labelInterval = null;
			this.m_labelIntervalType = null;
			this.m_labelIntervalOffset = null;
			this.m_labelIntervalOffsetType = null;
			this.m_variableAutoInterval = null;
			this.m_marksAlwaysAtPlotEdge = null;
			this.m_reverse = null;
			this.m_location = null;
			this.m_interlaced = null;
			this.m_interlacedColor = null;
			this.m_logScale = null;
			this.m_logBase = null;
			this.m_hideLabels = null;
			this.m_angle = null;
			this.m_arrows = null;
			this.m_preventFontShrink = null;
			this.m_preventFontGrow = null;
			this.m_preventLabelOffset = null;
			this.m_preventWordWrap = null;
			this.m_allowLabelRotation = null;
			this.m_includeZero = null;
			this.m_labelsAutoFitDisabled = null;
			this.m_minFontSize = null;
			this.m_maxFontSize = null;
			this.m_offsetLabels = null;
			this.m_hideEndLabels = null;
		}
	}
}
