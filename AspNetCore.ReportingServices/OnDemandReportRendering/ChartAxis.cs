using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxis : ChartObjectCollectionItem<ChartAxisInstance>, IROMStyleDefinitionContainer
	{
		internal enum TickMarks
		{
			None,
			Inside,
			Outside,
			Cross
		}

		internal enum Locations
		{
			Default,
			Opposite
		}

		private ChartGridLines m_majorGridlines;

		private ChartGridLines m_minorGridlines;

		private ReportVariantProperty m_crossAt;

		private ReportVariantProperty m_min;

		private ReportVariantProperty m_max;

		private ChartAxisTitle m_title;

		private Style m_style;

		private bool m_isCategory;

		private Chart m_chart;

		private AxisInstance m_renderAxisInstance;

		private Axis m_renderAxisDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis m_axisDef;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ChartStripLineCollection m_chartStripLines;

		private ReportEnumProperty<ChartAutoBool> m_visible;

		private ReportEnumProperty<ChartAutoBool> m_margin;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		private ReportDoubleProperty m_labelInterval;

		private ReportEnumProperty<ChartIntervalType> m_labelIntervalType;

		private ReportDoubleProperty m_labelIntervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_labelIntervalOffsetType;

		private ReportBoolProperty m_variableAutoInterval;

		private ChartTickMarks m_majorTickMarks;

		private ChartTickMarks m_minorTickMarks;

		private ReportBoolProperty m_marksAlwaysAtPlotEdge;

		private ReportBoolProperty m_reverse;

		private ReportEnumProperty<ChartAxisLocation> m_location;

		private ReportBoolProperty m_interlaced;

		private ReportColorProperty m_interlacedColor;

		private ReportBoolProperty m_logScale;

		private ReportDoubleProperty m_logBase;

		private ReportBoolProperty m_hideLabels;

		private ReportDoubleProperty m_angle;

		private ReportBoolProperty m_preventFontShrink;

		private ReportBoolProperty m_preventFontGrow;

		private ReportBoolProperty m_preventLabelOffset;

		private ReportBoolProperty m_preventWordWrap;

		private ReportEnumProperty<ChartAxisLabelRotation> m_allowLabelRotation;

		private ReportBoolProperty m_includeZero;

		private ReportBoolProperty m_labelsAutoFitDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportSizeProperty m_maxFontSize;

		private ReportBoolProperty m_offsetLabels;

		private ReportBoolProperty m_hideEndLabels;

		private ReportEnumProperty<ChartAxisArrow> m_arrows;

		private ChartAxisScaleBreak m_axisScaleBreak;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_style = new Style(this.m_renderAxisDef.StyleClass, this.m_renderAxisInstance.StyleAttributeValues, this.m_chart.RenderingContext);
					}
					else if (this.m_axisDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_axisDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ChartAxisTitle Title
		{
			get
			{
				if (this.m_title == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_title = new ChartAxisTitle(this.m_renderAxisDef.Title, this.m_renderAxisInstance.Title, this.m_chart);
					}
					else if (this.m_axisDef.Title != null)
					{
						this.m_title = new ChartAxisTitle(this.m_axisDef.Title, this.m_chart);
					}
				}
				return this.m_title;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					if (!this.m_isCategory)
					{
						return AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.Mode.ValueAxis.ToString();
					}
					return AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis.Mode.CategoryAxis.ToString();
				}
				return this.m_axisDef.AxisName;
			}
		}

		public ChartGridLines MajorGridLines
		{
			get
			{
				if (this.m_majorGridlines == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_axisDef.MajorGridLines != null)
						{
							this.m_majorGridlines = new ChartGridLines(this.m_renderAxisDef.MajorGridLines, this.m_renderAxisInstance.MajorGridLinesStyleAttributeValues, this.m_chart);
						}
					}
					else if (this.m_axisDef.MajorGridLines != null)
					{
						this.m_majorGridlines = new ChartGridLines(this.m_axisDef.MajorGridLines, this.m_chart);
					}
				}
				return this.m_majorGridlines;
			}
		}

		public ChartGridLines MinorGridLines
		{
			get
			{
				if (this.m_minorGridlines == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderAxisDef.MinorGridLines != null)
						{
							this.m_minorGridlines = new ChartGridLines(this.m_renderAxisDef.MinorGridLines, this.m_renderAxisInstance.MinorGridLinesStyleAttributeValues, this.m_chart);
						}
					}
					else if (this.m_axisDef.MinorGridLines != null)
					{
						this.m_minorGridlines = new ChartGridLines(this.m_axisDef.MinorGridLines, this.m_chart);
					}
				}
				return this.m_minorGridlines;
			}
		}

		public ReportVariantProperty CrossAt
		{
			get
			{
				if (this.m_crossAt == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderAxisDef.CrossAt != null)
						{
							this.m_crossAt = new ReportVariantProperty(this.m_renderAxisDef.CrossAt);
						}
					}
					else if (this.m_axisDef.CrossAt != null)
					{
						this.m_crossAt = new ReportVariantProperty(this.m_axisDef.CrossAt);
					}
				}
				return this.m_crossAt;
			}
		}

		public ChartStripLineCollection StripLines
		{
			get
			{
				if (this.m_chartStripLines == null && !this.m_chart.IsOldSnapshot && this.AxisDef.StripLines != null)
				{
					this.m_chartStripLines = new ChartStripLineCollection(this, this.m_chart);
				}
				return this.m_chartStripLines;
			}
		}

		public bool Scalar
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return this.m_renderAxisDef.Scalar;
				}
				return this.m_axisDef.Scalar;
			}
		}

		public ReportVariantProperty Minimum
		{
			get
			{
				if (this.m_min == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderAxisDef.Min != null)
						{
							this.m_min = new ReportVariantProperty(this.m_renderAxisDef.Min);
						}
					}
					else if (this.m_axisDef.Minimum != null)
					{
						this.m_min = new ReportVariantProperty(this.m_axisDef.Minimum);
					}
				}
				return this.m_min;
			}
		}

		public ReportVariantProperty Maximum
		{
			get
			{
				if (this.m_max == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderAxisDef.Max != null)
						{
							this.m_max = new ReportVariantProperty(this.m_renderAxisDef.Max);
						}
					}
					else if (this.m_axisDef.Maximum != null)
					{
						this.m_max = new ReportVariantProperty(this.m_axisDef.Maximum);
					}
				}
				return this.m_max;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_customProperties == null)
				{
					this.m_customPropertiesReady = true;
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_customProperties = new CustomPropertyCollection(this.m_chart.RenderingContext, new AspNetCore.ReportingServices.ReportRendering.CustomPropertyCollection(this.m_renderAxisDef.CustomProperties, this.m_renderAxisInstance.CustomPropertyInstances));
					}
					else
					{
						this.m_customProperties = new CustomPropertyCollection(this.m_chart.ReportScope.ReportScopeInstance, this.m_chart.RenderingContext, null, this.m_axisDef, ObjectType.Chart, this.m_chart.Name);
					}
				}
				else if (!this.m_customPropertiesReady)
				{
					this.m_customPropertiesReady = true;
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_customProperties.UpdateCustomProperties(new AspNetCore.ReportingServices.ReportRendering.CustomPropertyCollection(this.m_renderAxisDef.CustomProperties, this.m_renderAxisInstance.CustomPropertyInstances));
					}
					else
					{
						this.m_customProperties.UpdateCustomProperties(this.m_chart.ReportScope.ReportScopeInstance, this.m_axisDef, this.m_chart.RenderingContext.OdpContext, ObjectType.Chart, this.m_chart.Name);
					}
				}
				return this.m_customProperties;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Visible
		{
			get
			{
				if (this.m_visible == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_visible = new ReportEnumProperty<ChartAutoBool>((ChartAutoBool)(this.m_renderAxisDef.Visible ? 1 : 2));
					}
					else if (this.m_axisDef.Visible != null)
					{
						this.m_visible = new ReportEnumProperty<ChartAutoBool>(this.m_axisDef.Visible.IsExpression, this.m_axisDef.Visible.OriginalText, (!this.m_axisDef.Visible.IsExpression) ? EnumTranslator.TranslateChartAutoBool(this.m_axisDef.Visible.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return this.m_visible;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Margin
		{
			get
			{
				if (this.m_margin == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_margin = new ReportEnumProperty<ChartAutoBool>((ChartAutoBool)(this.m_renderAxisDef.Margin ? 1 : 2));
					}
					else if (this.m_axisDef.Margin != null)
					{
						this.m_margin = new ReportEnumProperty<ChartAutoBool>(this.m_axisDef.Margin.IsExpression, this.m_axisDef.Margin.OriginalText, (!this.m_axisDef.Margin.IsExpression) ? EnumTranslator.TranslateChartAutoBool(this.m_axisDef.Margin.StringValue, null) : ChartAutoBool.Auto);
					}
				}
				return this.m_margin;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.m_axisDef.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (this.m_intervalType == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.IntervalType != null)
				{
					this.m_intervalType = new ReportEnumProperty<ChartIntervalType>(this.m_axisDef.IntervalType.IsExpression, this.m_axisDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_axisDef.IntervalType.StringValue, null));
				}
				return this.m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.m_axisDef.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (this.m_intervalOffsetType == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.IntervalOffsetType != null)
				{
					this.m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(this.m_axisDef.IntervalOffsetType.IsExpression, this.m_axisDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_axisDef.IntervalOffsetType.StringValue, null));
				}
				return this.m_intervalOffsetType;
			}
		}

		public ReportDoubleProperty LabelInterval
		{
			get
			{
				if (this.m_labelInterval == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.LabelInterval != null)
				{
					this.m_labelInterval = new ReportDoubleProperty(this.m_axisDef.LabelInterval);
				}
				return this.m_labelInterval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> LabelIntervalType
		{
			get
			{
				if (this.m_labelIntervalType == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.LabelIntervalType != null)
				{
					this.m_labelIntervalType = new ReportEnumProperty<ChartIntervalType>(this.m_axisDef.LabelIntervalType.IsExpression, this.m_axisDef.LabelIntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_axisDef.LabelIntervalType.StringValue, null));
				}
				return this.m_labelIntervalType;
			}
		}

		public ReportDoubleProperty LabelIntervalOffset
		{
			get
			{
				if (this.m_labelIntervalOffset == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.LabelIntervalOffset != null)
				{
					this.m_labelIntervalOffset = new ReportDoubleProperty(this.m_axisDef.LabelIntervalOffset);
				}
				return this.m_labelIntervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> LabelIntervalOffsetType
		{
			get
			{
				if (this.m_labelIntervalOffsetType == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.LabelIntervalOffsetType != null)
				{
					this.m_labelIntervalOffsetType = new ReportEnumProperty<ChartIntervalType>(this.m_axisDef.LabelIntervalOffsetType.IsExpression, this.m_axisDef.LabelIntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(this.m_axisDef.LabelIntervalOffsetType.StringValue, null));
				}
				return this.m_labelIntervalOffsetType;
			}
		}

		public ReportBoolProperty VariableAutoInterval
		{
			get
			{
				if (this.m_variableAutoInterval == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.VariableAutoInterval != null)
				{
					this.m_variableAutoInterval = new ReportBoolProperty(this.m_axisDef.VariableAutoInterval);
				}
				return this.m_variableAutoInterval;
			}
		}

		public ChartTickMarks MajorTickMarks
		{
			get
			{
				if (this.m_majorTickMarks == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_majorTickMarks = new ChartTickMarks(this.m_renderAxisDef.MajorTickMarks, this.m_chart);
					}
					else if (this.m_axisDef.MajorTickMarks != null)
					{
						this.m_majorTickMarks = new ChartTickMarks(this.m_axisDef.MajorTickMarks, this.m_chart);
					}
				}
				return this.m_majorTickMarks;
			}
		}

		public ChartTickMarks MinorTickMarks
		{
			get
			{
				if (this.m_minorTickMarks == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_minorTickMarks = new ChartTickMarks(this.m_renderAxisDef.MinorTickMarks, this.m_chart);
					}
					else if (this.m_axisDef.MinorTickMarks != null)
					{
						this.m_minorTickMarks = new ChartTickMarks(this.m_axisDef.MinorTickMarks, this.m_chart);
					}
				}
				return this.m_minorTickMarks;
			}
		}

		public ReportBoolProperty MarksAlwaysAtPlotEdge
		{
			get
			{
				if (this.m_marksAlwaysAtPlotEdge == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.MarksAlwaysAtPlotEdge != null)
				{
					this.m_marksAlwaysAtPlotEdge = new ReportBoolProperty(this.m_axisDef.MarksAlwaysAtPlotEdge);
				}
				return this.m_marksAlwaysAtPlotEdge;
			}
		}

		public ReportBoolProperty Reverse
		{
			get
			{
				if (this.m_reverse == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
						expressionInfo.BoolValue = this.m_renderAxisDef.Reverse;
						return new ReportBoolProperty(expressionInfo);
					}
					if (this.m_axisDef.Reverse != null)
					{
						this.m_reverse = new ReportBoolProperty(this.m_axisDef.Reverse);
					}
				}
				return this.m_reverse;
			}
		}

		public ReportEnumProperty<ChartAxisLocation> Location
		{
			get
			{
				if (this.m_location == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.Location != null)
				{
					this.m_location = new ReportEnumProperty<ChartAxisLocation>(this.m_axisDef.Location.IsExpression, this.m_axisDef.Location.OriginalText, EnumTranslator.TranslateChartAxisLocation(this.m_axisDef.Location.StringValue, null));
				}
				return this.m_location;
			}
		}

		public ReportBoolProperty Interlaced
		{
			get
			{
				if (this.m_interlaced == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
						expressionInfo.BoolValue = this.m_renderAxisDef.Interlaced;
						this.m_interlaced = new ReportBoolProperty(expressionInfo);
					}
					else if (this.m_axisDef.Interlaced != null)
					{
						this.m_interlaced = new ReportBoolProperty(this.m_axisDef.Interlaced);
					}
				}
				return this.m_interlaced;
			}
		}

		public ReportColorProperty InterlacedColor
		{
			get
			{
				if (this.m_interlacedColor == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.InterlacedColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo interlacedColor = this.m_axisDef.InterlacedColor;
					this.m_interlacedColor = new ReportColorProperty(interlacedColor.IsExpression, interlacedColor.OriginalText, interlacedColor.IsExpression ? null : new ReportColor(interlacedColor.StringValue.Trim(), true), interlacedColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
				}
				return this.m_interlacedColor;
			}
		}

		public ReportBoolProperty LogScale
		{
			get
			{
				if (this.m_logScale == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo();
						expressionInfo.BoolValue = this.m_renderAxisDef.LogScale;
						this.m_logScale = new ReportBoolProperty(expressionInfo);
					}
					else if (this.m_axisDef.LogScale != null)
					{
						this.m_logScale = new ReportBoolProperty(this.m_axisDef.LogScale);
					}
				}
				return this.m_logScale;
			}
		}

		public ReportDoubleProperty LogBase
		{
			get
			{
				if (this.m_logBase == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.LogBase != null)
				{
					this.m_logBase = new ReportDoubleProperty(this.m_axisDef.LogBase);
				}
				return this.m_logBase;
			}
		}

		public ReportBoolProperty HideLabels
		{
			get
			{
				if (this.m_hideLabels == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.HideLabels != null)
				{
					this.m_hideLabels = new ReportBoolProperty(this.m_axisDef.HideLabels);
				}
				return this.m_hideLabels;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (this.m_angle == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						return null;
					}
					if (this.m_axisDef.Angle != null)
					{
						this.m_angle = new ReportDoubleProperty(this.m_axisDef.Angle);
					}
				}
				return this.m_angle;
			}
		}

		public ReportEnumProperty<ChartAxisArrow> Arrows
		{
			get
			{
				if (this.m_arrows == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.Arrows != null)
				{
					this.m_arrows = new ReportEnumProperty<ChartAxisArrow>(this.m_axisDef.Arrows.IsExpression, this.m_axisDef.Arrows.OriginalText, EnumTranslator.TranslateChartAxisArrow(this.m_axisDef.Arrows.StringValue, null));
				}
				return this.m_arrows;
			}
		}

		public ReportBoolProperty PreventFontShrink
		{
			get
			{
				if (this.m_preventFontShrink == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_preventFontShrink = new ReportBoolProperty();
					}
					else if (this.m_axisDef.PreventFontShrink != null)
					{
						this.m_preventFontShrink = new ReportBoolProperty(this.m_axisDef.PreventFontShrink);
					}
				}
				return this.m_preventFontShrink;
			}
		}

		public ReportBoolProperty PreventFontGrow
		{
			get
			{
				if (this.m_preventFontGrow == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_preventFontGrow = new ReportBoolProperty();
					}
					else if (this.m_axisDef.PreventFontGrow != null)
					{
						this.m_preventFontGrow = new ReportBoolProperty(this.m_axisDef.PreventFontGrow);
					}
				}
				return this.m_preventFontGrow;
			}
		}

		public ReportBoolProperty PreventLabelOffset
		{
			get
			{
				if (this.m_preventLabelOffset == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_preventLabelOffset = new ReportBoolProperty();
					}
					else if (this.m_axisDef.PreventLabelOffset != null)
					{
						this.m_preventLabelOffset = new ReportBoolProperty(this.m_axisDef.PreventLabelOffset);
					}
				}
				return this.m_preventLabelOffset;
			}
		}

		public ReportBoolProperty PreventWordWrap
		{
			get
			{
				if (this.m_preventWordWrap == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_preventWordWrap = new ReportBoolProperty();
					}
					else if (this.m_axisDef.PreventWordWrap != null)
					{
						this.m_preventWordWrap = new ReportBoolProperty(this.m_axisDef.PreventWordWrap);
					}
				}
				return this.m_preventWordWrap;
			}
		}

		public ReportEnumProperty<ChartAxisLabelRotation> AllowLabelRotation
		{
			get
			{
				if (this.m_allowLabelRotation == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.AllowLabelRotation != null)
				{
					this.m_allowLabelRotation = new ReportEnumProperty<ChartAxisLabelRotation>(this.m_axisDef.AllowLabelRotation.IsExpression, this.m_axisDef.AllowLabelRotation.OriginalText, EnumTranslator.TranslateChartAxisLabelRotation(this.m_axisDef.AllowLabelRotation.StringValue, null));
				}
				return this.m_allowLabelRotation;
			}
		}

		public ReportBoolProperty IncludeZero
		{
			get
			{
				if (this.m_includeZero == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.IncludeZero != null)
				{
					this.m_includeZero = new ReportBoolProperty(this.m_axisDef.IncludeZero);
				}
				return this.m_includeZero;
			}
		}

		public ReportBoolProperty LabelsAutoFitDisabled
		{
			get
			{
				if (this.m_labelsAutoFitDisabled == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_labelsAutoFitDisabled = new ReportBoolProperty();
					}
					else if (this.m_axisDef.LabelsAutoFitDisabled != null)
					{
						this.m_labelsAutoFitDisabled = new ReportBoolProperty(this.m_axisDef.LabelsAutoFitDisabled);
					}
				}
				return this.m_labelsAutoFitDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.MinFontSize != null)
				{
					this.m_minFontSize = new ReportSizeProperty(this.m_axisDef.MinFontSize);
				}
				return this.m_minFontSize;
			}
		}

		public ReportSizeProperty MaxFontSize
		{
			get
			{
				if (this.m_maxFontSize == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.MaxFontSize != null)
				{
					this.m_maxFontSize = new ReportSizeProperty(this.m_axisDef.MaxFontSize);
				}
				return this.m_maxFontSize;
			}
		}

		public ReportBoolProperty OffsetLabels
		{
			get
			{
				if (this.m_offsetLabels == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.OffsetLabels != null)
				{
					this.m_offsetLabels = new ReportBoolProperty(this.m_axisDef.OffsetLabels);
				}
				return this.m_offsetLabels;
			}
		}

		public ReportBoolProperty HideEndLabels
		{
			get
			{
				if (this.m_hideEndLabels == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.HideEndLabels != null)
				{
					this.m_hideEndLabels = new ReportBoolProperty(this.m_axisDef.HideEndLabels);
				}
				return this.m_hideEndLabels;
			}
		}

		public ChartAxisScaleBreak AxisScaleBreak
		{
			get
			{
				if (this.m_axisScaleBreak == null && !this.m_chart.IsOldSnapshot && this.m_axisDef.AxisScaleBreak != null)
				{
					this.m_axisScaleBreak = new ChartAxisScaleBreak(this.m_axisDef.AxisScaleBreak, this.m_chart);
				}
				return this.m_axisScaleBreak;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis AxisDef
		{
			get
			{
				return this.m_axisDef;
			}
		}

		internal AxisInstance RenderAxisInstance
		{
			get
			{
				return this.m_renderAxisInstance;
			}
		}

		public ChartAxisInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartAxisInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartAxis(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis axisDef, Chart chart)
		{
			this.m_axisDef = axisDef;
			this.m_chart = chart;
		}

		internal ChartAxis(Axis renderAxisDef, AxisInstance renderAxisInstance, Chart chart, bool isCategory)
		{
			this.m_renderAxisDef = renderAxisDef;
			this.m_renderAxisInstance = renderAxisInstance;
			this.m_chart = chart;
			this.m_isCategory = isCategory;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_majorGridlines != null)
			{
				this.m_majorGridlines.SetNewContext();
			}
			if (this.m_minorGridlines != null)
			{
				this.m_minorGridlines.SetNewContext();
			}
			if (this.m_title != null)
			{
				this.m_title.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_chartStripLines != null)
			{
				this.m_chartStripLines.SetNewContext();
			}
			if (this.m_majorTickMarks != null)
			{
				this.m_majorTickMarks.SetNewContext();
			}
			if (this.m_minorTickMarks != null)
			{
				this.m_minorTickMarks.SetNewContext();
			}
			if (this.m_axisScaleBreak != null)
			{
				this.m_axisScaleBreak.SetNewContext();
			}
			this.m_customPropertiesReady = false;
		}
	}
}
