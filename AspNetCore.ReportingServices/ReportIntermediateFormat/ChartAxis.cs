using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class ChartAxis : ChartStyleContainer, IPersistable, ICustomPropertiesHolder
	{
		internal enum Mode
		{
			CategoryAxis,
			ValueAxis
		}

		internal enum ExpressionType
		{
			Min,
			Max,
			CrossAt
		}

		protected string m_name;

		private ChartAxisTitle m_title;

		private ChartGridLines m_majorGridLines;

		private ChartGridLines m_minorGridLines;

		private DataValueList m_customProperties;

		private List<ChartStripLine> m_chartStripLines;

		private ExpressionInfo m_visible;

		private ExpressionInfo m_margin;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		private ChartTickMarks m_majorTickMarks;

		private ChartTickMarks m_minorTickMarks;

		private ExpressionInfo m_marksAlwaysAtPlotEdge;

		private ExpressionInfo m_reverse;

		private ExpressionInfo m_location;

		private ExpressionInfo m_interlaced;

		private ExpressionInfo m_interlacedColor;

		private ExpressionInfo m_logScale;

		private ExpressionInfo m_logBase;

		private ExpressionInfo m_hideLabels;

		private ExpressionInfo m_angle;

		private ExpressionInfo m_arrows;

		private ExpressionInfo m_preventFontShrink;

		private ExpressionInfo m_preventFontGrow;

		private ExpressionInfo m_preventLabelOffset;

		private ExpressionInfo m_preventWordWrap;

		private ExpressionInfo m_allowLabelRotation;

		private ExpressionInfo m_includeZero;

		private ExpressionInfo m_labelsAutoFitDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_maxFontSize;

		private ExpressionInfo m_offsetLabels;

		private ExpressionInfo m_hideEndLabels;

		private ChartAxisScaleBreak m_axisScaleBreak;

		private ExpressionInfo m_crossAt;

		private ExpressionInfo m_min;

		private ExpressionInfo m_max;

		private bool m_scalar;

		private bool m_autoCrossAt = true;

		private bool m_autoScaleMin = true;

		private bool m_autoScaleMax = true;

		private int m_exprHostID;

		private ExpressionInfo m_variableAutoInterval;

		private ExpressionInfo m_labelInterval;

		private ExpressionInfo m_labelIntervalType;

		private ExpressionInfo m_labelIntervalOffset;

		private ExpressionInfo m_labelIntervalOffsetType;

		[NonSerialized]
		private ChartAxisExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartAxis.GetDeclaration();

		internal string AxisName
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ChartAxisTitle Title
		{
			get
			{
				return this.m_title;
			}
			set
			{
				this.m_title = value;
			}
		}

		internal ChartGridLines MajorGridLines
		{
			get
			{
				return this.m_majorGridLines;
			}
			set
			{
				this.m_majorGridLines = value;
			}
		}

		internal ChartGridLines MinorGridLines
		{
			get
			{
				return this.m_minorGridLines;
			}
			set
			{
				this.m_minorGridLines = value;
			}
		}

		internal List<ChartStripLine> StripLines
		{
			get
			{
				return this.m_chartStripLines;
			}
			set
			{
				this.m_chartStripLines = value;
			}
		}

		public DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal bool Scalar
		{
			get
			{
				return this.m_scalar;
			}
			set
			{
				this.m_scalar = value;
			}
		}

		internal ExpressionInfo Minimum
		{
			get
			{
				return this.m_min;
			}
			set
			{
				this.m_min = value;
			}
		}

		internal ExpressionInfo Maximum
		{
			get
			{
				return this.m_max;
			}
			set
			{
				this.m_max = value;
			}
		}

		internal ExpressionInfo CrossAt
		{
			get
			{
				return this.m_crossAt;
			}
			set
			{
				this.m_crossAt = value;
			}
		}

		internal bool AutoCrossAt
		{
			get
			{
				return this.m_autoCrossAt;
			}
			set
			{
				this.m_autoCrossAt = value;
			}
		}

		internal bool AutoScaleMin
		{
			get
			{
				return this.m_autoScaleMin;
			}
			set
			{
				this.m_autoScaleMin = value;
			}
		}

		internal bool AutoScaleMax
		{
			get
			{
				return this.m_autoScaleMax;
			}
			set
			{
				this.m_autoScaleMax = value;
			}
		}

		internal ExpressionInfo Visible
		{
			get
			{
				return this.m_visible;
			}
			set
			{
				this.m_visible = value;
			}
		}

		internal ExpressionInfo Margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				this.m_margin = value;
			}
		}

		internal ExpressionInfo Interval
		{
			get
			{
				return this.m_interval;
			}
			set
			{
				this.m_interval = value;
			}
		}

		internal ExpressionInfo IntervalType
		{
			get
			{
				return this.m_intervalType;
			}
			set
			{
				this.m_intervalType = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return this.m_intervalOffset;
			}
			set
			{
				this.m_intervalOffset = value;
			}
		}

		internal ExpressionInfo IntervalOffsetType
		{
			get
			{
				return this.m_intervalOffsetType;
			}
			set
			{
				this.m_intervalOffsetType = value;
			}
		}

		internal ChartTickMarks MajorTickMarks
		{
			get
			{
				return this.m_majorTickMarks;
			}
			set
			{
				this.m_majorTickMarks = value;
			}
		}

		internal ChartTickMarks MinorTickMarks
		{
			get
			{
				return this.m_minorTickMarks;
			}
			set
			{
				this.m_minorTickMarks = value;
			}
		}

		internal ExpressionInfo MarksAlwaysAtPlotEdge
		{
			get
			{
				return this.m_marksAlwaysAtPlotEdge;
			}
			set
			{
				this.m_marksAlwaysAtPlotEdge = value;
			}
		}

		internal ExpressionInfo Reverse
		{
			get
			{
				return this.m_reverse;
			}
			set
			{
				this.m_reverse = value;
			}
		}

		internal ExpressionInfo Location
		{
			get
			{
				return this.m_location;
			}
			set
			{
				this.m_location = value;
			}
		}

		internal ExpressionInfo Interlaced
		{
			get
			{
				return this.m_interlaced;
			}
			set
			{
				this.m_interlaced = value;
			}
		}

		internal ExpressionInfo InterlacedColor
		{
			get
			{
				return this.m_interlacedColor;
			}
			set
			{
				this.m_interlacedColor = value;
			}
		}

		internal ExpressionInfo LogScale
		{
			get
			{
				return this.m_logScale;
			}
			set
			{
				this.m_logScale = value;
			}
		}

		internal ExpressionInfo LogBase
		{
			get
			{
				return this.m_logBase;
			}
			set
			{
				this.m_logBase = value;
			}
		}

		internal ExpressionInfo HideLabels
		{
			get
			{
				return this.m_hideLabels;
			}
			set
			{
				this.m_hideLabels = value;
			}
		}

		internal ExpressionInfo Angle
		{
			get
			{
				return this.m_angle;
			}
			set
			{
				this.m_angle = value;
			}
		}

		internal ExpressionInfo Arrows
		{
			get
			{
				return this.m_arrows;
			}
			set
			{
				this.m_arrows = value;
			}
		}

		internal ExpressionInfo PreventFontShrink
		{
			get
			{
				return this.m_preventFontShrink;
			}
			set
			{
				this.m_preventFontShrink = value;
			}
		}

		internal ExpressionInfo PreventFontGrow
		{
			get
			{
				return this.m_preventFontGrow;
			}
			set
			{
				this.m_preventFontGrow = value;
			}
		}

		internal ExpressionInfo PreventLabelOffset
		{
			get
			{
				return this.m_preventLabelOffset;
			}
			set
			{
				this.m_preventLabelOffset = value;
			}
		}

		internal ExpressionInfo PreventWordWrap
		{
			get
			{
				return this.m_preventWordWrap;
			}
			set
			{
				this.m_preventWordWrap = value;
			}
		}

		internal ExpressionInfo AllowLabelRotation
		{
			get
			{
				return this.m_allowLabelRotation;
			}
			set
			{
				this.m_allowLabelRotation = value;
			}
		}

		internal ExpressionInfo IncludeZero
		{
			get
			{
				return this.m_includeZero;
			}
			set
			{
				this.m_includeZero = value;
			}
		}

		internal ExpressionInfo LabelsAutoFitDisabled
		{
			get
			{
				return this.m_labelsAutoFitDisabled;
			}
			set
			{
				this.m_labelsAutoFitDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return this.m_minFontSize;
			}
			set
			{
				this.m_minFontSize = value;
			}
		}

		internal ExpressionInfo MaxFontSize
		{
			get
			{
				return this.m_maxFontSize;
			}
			set
			{
				this.m_maxFontSize = value;
			}
		}

		internal ExpressionInfo OffsetLabels
		{
			get
			{
				return this.m_offsetLabels;
			}
			set
			{
				this.m_offsetLabels = value;
			}
		}

		internal ExpressionInfo HideEndLabels
		{
			get
			{
				return this.m_hideEndLabels;
			}
			set
			{
				this.m_hideEndLabels = value;
			}
		}

		internal ChartAxisScaleBreak AxisScaleBreak
		{
			get
			{
				return this.m_axisScaleBreak;
			}
			set
			{
				this.m_axisScaleBreak = value;
			}
		}

		internal ExpressionInfo VariableAutoInterval
		{
			get
			{
				return this.m_variableAutoInterval;
			}
			set
			{
				this.m_variableAutoInterval = value;
			}
		}

		internal ExpressionInfo LabelInterval
		{
			get
			{
				return this.m_labelInterval;
			}
			set
			{
				this.m_labelInterval = value;
			}
		}

		internal ExpressionInfo LabelIntervalType
		{
			get
			{
				return this.m_labelIntervalType;
			}
			set
			{
				this.m_labelIntervalType = value;
			}
		}

		internal ExpressionInfo LabelIntervalOffset
		{
			get
			{
				return this.m_labelIntervalOffset;
			}
			set
			{
				this.m_labelIntervalOffset = value;
			}
		}

		internal ExpressionInfo LabelIntervalOffsetType
		{
			get
			{
				return this.m_labelIntervalOffsetType;
			}
			set
			{
				this.m_labelIntervalOffsetType = value;
			}
		}

		internal ChartAxisExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int ExpressionHostID
		{
			get
			{
				return this.m_exprHostID;
			}
		}

		internal ChartAxis()
		{
		}

		internal ChartAxis(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartAxisExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_title != null && this.m_exprHost.TitleHost != null)
			{
				this.m_title.SetExprHost(this.m_exprHost.TitleHost, reportObjectModel);
			}
			if (this.m_majorGridLines != null && this.m_exprHost.MajorGridLinesHost != null)
			{
				this.m_majorGridLines.SetExprHost(this.m_exprHost.MajorGridLinesHost, reportObjectModel);
			}
			if (this.m_minorGridLines != null && this.m_exprHost.MinorGridLinesHost != null)
			{
				this.m_minorGridLines.SetExprHost(this.m_exprHost.MinorGridLinesHost, reportObjectModel);
			}
			if (this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(null != this.m_customProperties, "(null != m_customProperties)");
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			IList<ChartStripLineExprHost> chartStripLinesHostsRemotable = this.m_exprHost.ChartStripLinesHostsRemotable;
			if (this.m_chartStripLines != null && chartStripLinesHostsRemotable != null)
			{
				for (int i = 0; i < this.m_chartStripLines.Count; i++)
				{
					ChartStripLine chartStripLine = this.m_chartStripLines[i];
					if (chartStripLine != null && chartStripLine.ExpressionHostID > -1)
					{
						chartStripLine.SetExprHost(chartStripLinesHostsRemotable[chartStripLine.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (this.m_majorTickMarks != null && this.m_exprHost.MajorTickMarksHost != null)
			{
				this.m_majorTickMarks.SetExprHost(this.m_exprHost.MajorTickMarksHost, reportObjectModel);
			}
			if (this.m_minorTickMarks != null && this.m_exprHost.MinorTickMarksHost != null)
			{
				this.m_minorTickMarks.SetExprHost(this.m_exprHost.MinorTickMarksHost, reportObjectModel);
			}
			if (this.m_axisScaleBreak != null && this.m_exprHost.AxisScaleBreakHost != null)
			{
				this.m_axisScaleBreak.SetExprHost(this.m_exprHost.AxisScaleBreakHost, reportObjectModel);
			}
		}

		internal virtual void Initialize(InitializationContext context, bool isValueAxis)
		{
			string propertyName = this.GetPropertyName(isValueAxis);
			context.ExprHostBuilder.ChartAxisStart(this.m_name, isValueAxis);
			if (base.m_styleClass != null)
			{
				base.m_styleClass.Initialize(context);
			}
			if (this.m_title != null)
			{
				this.m_title.Initialize(context);
			}
			if (this.m_minorGridLines != null)
			{
				this.m_minorGridLines.Initialize(context, false);
			}
			if (this.m_majorGridLines != null)
			{
				this.m_majorGridLines.Initialize(context, true);
			}
			if (this.m_min != null)
			{
				if (this.m_min.InitializeAxisExpression(propertyName + ".Minimum", context))
				{
					context.ExprHostBuilder.AxisMin(this.m_min);
				}
				else
				{
					this.m_min = null;
				}
			}
			if (this.m_max != null)
			{
				if (this.m_max.InitializeAxisExpression(propertyName + ".Maximum", context))
				{
					context.ExprHostBuilder.AxisMax(this.m_max);
				}
				else
				{
					this.m_max = null;
				}
			}
			if (this.m_crossAt != null)
			{
				if (this.m_crossAt.InitializeAxisExpression(propertyName + ".CrossAt", context))
				{
					context.ExprHostBuilder.AxisCrossAt(this.m_crossAt);
				}
				else
				{
					this.m_crossAt = null;
				}
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(propertyName + ".", context);
			}
			if (this.m_chartStripLines != null)
			{
				for (int i = 0; i < this.m_chartStripLines.Count; i++)
				{
					this.m_chartStripLines[i].Initialize(context, i);
				}
			}
			if (this.m_visible != null)
			{
				this.m_visible.Initialize("Visible", context);
				context.ExprHostBuilder.ChartAxisVisible(this.m_visible);
			}
			if (this.m_margin != null)
			{
				this.m_margin.Initialize("Margin", context);
				context.ExprHostBuilder.ChartAxisMargin(this.m_margin);
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartAxisInterval(this.m_interval);
			}
			if (this.m_intervalType != null)
			{
				this.m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartAxisIntervalType(this.m_intervalType);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartAxisIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_intervalOffsetType != null)
			{
				this.m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartAxisIntervalOffsetType(this.m_intervalOffsetType);
			}
			if (this.m_majorTickMarks != null)
			{
				this.m_majorTickMarks.Initialize(context, true);
			}
			if (this.m_minorTickMarks != null)
			{
				this.m_minorTickMarks.Initialize(context, false);
			}
			if (this.m_marksAlwaysAtPlotEdge != null)
			{
				this.m_marksAlwaysAtPlotEdge.Initialize("MarksAlwaysAtPlotEdge", context);
				context.ExprHostBuilder.ChartAxisMarksAlwaysAtPlotEdge(this.m_marksAlwaysAtPlotEdge);
			}
			if (this.m_reverse != null)
			{
				this.m_reverse.Initialize("Reverse", context);
				context.ExprHostBuilder.ChartAxisReverse(this.m_reverse);
			}
			if (this.m_location != null)
			{
				this.m_location.Initialize("Location", context);
				context.ExprHostBuilder.ChartAxisLocation(this.m_location);
			}
			if (this.m_interlaced != null)
			{
				this.m_interlaced.Initialize("Interlaced", context);
				context.ExprHostBuilder.ChartAxisInterlaced(this.m_interlaced);
			}
			if (this.m_interlacedColor != null)
			{
				this.m_interlacedColor.Initialize("InterlacedColor", context);
				context.ExprHostBuilder.ChartAxisInterlacedColor(this.m_interlacedColor);
			}
			if (this.m_logScale != null)
			{
				this.m_logScale.Initialize("LogScale", context);
				context.ExprHostBuilder.ChartAxisLogScale(this.m_logScale);
			}
			if (this.m_logBase != null)
			{
				this.m_logBase.Initialize("LogBase", context);
				context.ExprHostBuilder.ChartAxisLogBase(this.m_logBase);
			}
			if (this.m_hideLabels != null)
			{
				this.m_hideLabels.Initialize("HideLabels", context);
				context.ExprHostBuilder.ChartAxisHideLabels(this.m_hideLabels);
			}
			if (this.m_angle != null)
			{
				this.m_angle.Initialize("Angle", context);
				context.ExprHostBuilder.ChartAxisAngle(this.m_angle);
			}
			if (this.m_arrows != null)
			{
				this.m_arrows.Initialize("Arrows", context);
				context.ExprHostBuilder.ChartAxisArrows(this.m_arrows);
			}
			if (this.m_preventFontShrink != null)
			{
				this.m_preventFontShrink.Initialize("PreventFontShrink", context);
				context.ExprHostBuilder.ChartAxisPreventFontShrink(this.m_preventFontShrink);
			}
			if (this.m_preventFontGrow != null)
			{
				this.m_preventFontGrow.Initialize("PreventFontGrow", context);
				context.ExprHostBuilder.ChartAxisPreventFontGrow(this.m_preventFontGrow);
			}
			if (this.m_preventLabelOffset != null)
			{
				this.m_preventLabelOffset.Initialize("PreventLabelOffset", context);
				context.ExprHostBuilder.ChartAxisPreventLabelOffset(this.m_preventLabelOffset);
			}
			if (this.m_preventWordWrap != null)
			{
				this.m_preventWordWrap.Initialize("PreventWordWrap", context);
				context.ExprHostBuilder.ChartAxisPreventWordWrap(this.m_preventWordWrap);
			}
			if (this.m_allowLabelRotation != null)
			{
				this.m_allowLabelRotation.Initialize("AllowLabelRotation", context);
				context.ExprHostBuilder.ChartAxisAllowLabelRotation(this.m_allowLabelRotation);
			}
			if (this.m_includeZero != null)
			{
				this.m_includeZero.Initialize("IncludeZero", context);
				context.ExprHostBuilder.ChartAxisIncludeZero(this.m_includeZero);
			}
			if (this.m_labelsAutoFitDisabled != null)
			{
				this.m_labelsAutoFitDisabled.Initialize("LabelsAutoFitDisabled", context);
				context.ExprHostBuilder.ChartAxisLabelsAutoFitDisabled(this.m_labelsAutoFitDisabled);
			}
			if (this.m_minFontSize != null)
			{
				this.m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.ChartAxisMinFontSize(this.m_minFontSize);
			}
			if (this.m_maxFontSize != null)
			{
				this.m_maxFontSize.Initialize("MaxFontSize", context);
				context.ExprHostBuilder.ChartAxisMaxFontSize(this.m_maxFontSize);
			}
			if (this.m_offsetLabels != null)
			{
				this.m_offsetLabels.Initialize("OffsetLabels", context);
				context.ExprHostBuilder.ChartAxisOffsetLabels(this.m_offsetLabels);
			}
			if (this.m_hideEndLabels != null)
			{
				this.m_hideEndLabels.Initialize("HideEndLabels", context);
				context.ExprHostBuilder.ChartAxisHideEndLabels(this.m_hideEndLabels);
			}
			if (this.m_axisScaleBreak != null)
			{
				this.m_axisScaleBreak.Initialize(context);
			}
			if (this.m_variableAutoInterval != null)
			{
				this.m_variableAutoInterval.Initialize("VariableAutoInterval", context);
				context.ExprHostBuilder.ChartAxisVariableAutoInterval(this.m_variableAutoInterval);
			}
			if (this.m_labelInterval != null)
			{
				this.m_labelInterval.Initialize("LabelInterval", context);
				context.ExprHostBuilder.ChartAxisLabelInterval(this.m_labelInterval);
			}
			if (this.m_labelIntervalType != null)
			{
				this.m_labelIntervalType.Initialize("LabelIntervalType", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalType(this.m_labelIntervalType);
			}
			if (this.m_labelIntervalOffset != null)
			{
				this.m_labelIntervalOffset.Initialize("LabelIntervalOffset", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalOffset(this.m_labelIntervalOffset);
			}
			if (this.m_labelIntervalOffsetType != null)
			{
				this.m_labelIntervalOffsetType.Initialize("LabelIntervalOffsetType", context);
				context.ExprHostBuilder.ChartAxisLabelIntervalOffsetType(this.m_labelIntervalOffsetType);
			}
			this.m_exprHostID = context.ExprHostBuilder.ChartAxisEnd(isValueAxis);
		}

		private string GetPropertyName(bool isValueAxis)
		{
			if (!isValueAxis)
			{
				if (this.m_name == null)
				{
					this.m_name = "CategoryAxis";
					return this.m_name;
				}
				return "CategoryAxis_" + this.m_name;
			}
			if (this.m_name == null)
			{
				this.m_name = "ValueAxis";
				return this.m_name;
			}
			return "ValueAxis_" + this.m_name;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxis chartAxis = (ChartAxis)base.PublishClone(context);
			if (this.m_title != null)
			{
				chartAxis.m_title = (ChartAxisTitle)this.m_title.PublishClone(context);
			}
			if (this.m_majorGridLines != null)
			{
				chartAxis.m_majorGridLines = (ChartGridLines)this.m_majorGridLines.PublishClone(context);
			}
			if (this.m_minorGridLines != null)
			{
				chartAxis.m_minorGridLines = (ChartGridLines)this.m_minorGridLines.PublishClone(context);
			}
			if (this.m_crossAt != null)
			{
				chartAxis.m_crossAt = (ExpressionInfo)this.m_crossAt.PublishClone(context);
			}
			if (this.m_min != null)
			{
				chartAxis.m_min = (ExpressionInfo)this.m_min.PublishClone(context);
			}
			if (this.m_max != null)
			{
				chartAxis.m_max = (ExpressionInfo)this.m_max.PublishClone(context);
			}
			if (this.m_customProperties != null)
			{
				chartAxis.m_customProperties = new DataValueList(this.m_customProperties.Count);
				foreach (DataValue customProperty in this.m_customProperties)
				{
					chartAxis.m_customProperties.Add(customProperty.PublishClone(context));
				}
			}
			if (this.m_chartStripLines != null)
			{
				chartAxis.m_chartStripLines = new List<ChartStripLine>(this.m_chartStripLines.Count);
				foreach (ChartStripLine chartStripLine in this.m_chartStripLines)
				{
					chartAxis.m_chartStripLines.Add((ChartStripLine)chartStripLine.PublishClone(context));
				}
			}
			if (this.m_visible != null)
			{
				chartAxis.m_visible = (ExpressionInfo)this.m_visible.PublishClone(context);
			}
			if (this.m_margin != null)
			{
				chartAxis.m_margin = (ExpressionInfo)this.m_margin.PublishClone(context);
			}
			if (this.m_interval != null)
			{
				chartAxis.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalType != null)
			{
				chartAxis.m_intervalType = (ExpressionInfo)this.m_intervalType.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				chartAxis.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_intervalOffsetType != null)
			{
				chartAxis.m_intervalOffsetType = (ExpressionInfo)this.m_intervalOffsetType.PublishClone(context);
			}
			if (this.m_majorTickMarks != null)
			{
				chartAxis.m_majorTickMarks = (ChartTickMarks)this.m_majorTickMarks.PublishClone(context);
			}
			if (this.m_minorTickMarks != null)
			{
				chartAxis.m_minorTickMarks = (ChartTickMarks)this.m_minorTickMarks.PublishClone(context);
			}
			if (this.m_marksAlwaysAtPlotEdge != null)
			{
				chartAxis.m_marksAlwaysAtPlotEdge = (ExpressionInfo)this.m_marksAlwaysAtPlotEdge.PublishClone(context);
			}
			if (this.m_reverse != null)
			{
				chartAxis.m_reverse = (ExpressionInfo)this.m_reverse.PublishClone(context);
			}
			if (this.m_location != null)
			{
				chartAxis.m_location = (ExpressionInfo)this.m_location.PublishClone(context);
			}
			if (this.m_interlaced != null)
			{
				chartAxis.m_interlaced = (ExpressionInfo)this.m_interlaced.PublishClone(context);
			}
			if (this.m_interlacedColor != null)
			{
				chartAxis.m_interlacedColor = (ExpressionInfo)this.m_interlacedColor.PublishClone(context);
			}
			if (this.m_logScale != null)
			{
				chartAxis.m_logScale = (ExpressionInfo)this.m_logScale.PublishClone(context);
			}
			if (this.m_logBase != null)
			{
				chartAxis.m_logBase = (ExpressionInfo)this.m_logBase.PublishClone(context);
			}
			if (this.m_hideLabels != null)
			{
				chartAxis.m_hideLabels = (ExpressionInfo)this.m_hideLabels.PublishClone(context);
			}
			if (this.m_angle != null)
			{
				chartAxis.m_angle = (ExpressionInfo)this.m_angle.PublishClone(context);
			}
			if (this.m_arrows != null)
			{
				chartAxis.m_arrows = (ExpressionInfo)this.m_arrows.PublishClone(context);
			}
			if (this.m_preventFontShrink != null)
			{
				chartAxis.m_preventFontShrink = (ExpressionInfo)this.m_preventFontShrink.PublishClone(context);
			}
			if (this.m_preventFontGrow != null)
			{
				chartAxis.m_preventFontGrow = (ExpressionInfo)this.m_preventFontGrow.PublishClone(context);
			}
			if (this.m_preventLabelOffset != null)
			{
				chartAxis.m_preventLabelOffset = (ExpressionInfo)this.m_preventLabelOffset.PublishClone(context);
			}
			if (this.m_preventWordWrap != null)
			{
				chartAxis.m_preventWordWrap = (ExpressionInfo)this.m_preventWordWrap.PublishClone(context);
			}
			if (this.m_allowLabelRotation != null)
			{
				chartAxis.m_allowLabelRotation = (ExpressionInfo)this.m_allowLabelRotation.PublishClone(context);
			}
			if (this.m_includeZero != null)
			{
				chartAxis.m_includeZero = (ExpressionInfo)this.m_includeZero.PublishClone(context);
			}
			if (this.m_labelsAutoFitDisabled != null)
			{
				chartAxis.m_labelsAutoFitDisabled = (ExpressionInfo)this.m_labelsAutoFitDisabled.PublishClone(context);
			}
			if (this.m_minFontSize != null)
			{
				chartAxis.m_minFontSize = (ExpressionInfo)this.m_minFontSize.PublishClone(context);
			}
			if (this.m_maxFontSize != null)
			{
				chartAxis.m_maxFontSize = (ExpressionInfo)this.m_maxFontSize.PublishClone(context);
			}
			if (this.m_offsetLabels != null)
			{
				chartAxis.m_offsetLabels = (ExpressionInfo)this.m_offsetLabels.PublishClone(context);
			}
			if (this.m_hideEndLabels != null)
			{
				chartAxis.m_hideEndLabels = (ExpressionInfo)this.m_hideEndLabels.PublishClone(context);
			}
			if (this.m_axisScaleBreak != null)
			{
				chartAxis.m_axisScaleBreak = (ChartAxisScaleBreak)this.m_axisScaleBreak.PublishClone(context);
			}
			if (this.m_variableAutoInterval != null)
			{
				chartAxis.m_variableAutoInterval = (ExpressionInfo)this.m_variableAutoInterval.PublishClone(context);
			}
			if (this.m_labelInterval != null)
			{
				chartAxis.m_labelInterval = (ExpressionInfo)this.m_hideEndLabels.PublishClone(context);
			}
			if (this.m_labelIntervalType != null)
			{
				chartAxis.m_labelIntervalType = (ExpressionInfo)this.m_hideEndLabels.PublishClone(context);
			}
			if (this.m_labelIntervalOffset != null)
			{
				chartAxis.m_labelIntervalOffset = (ExpressionInfo)this.m_labelIntervalOffset.PublishClone(context);
			}
			if (this.m_labelIntervalOffsetType != null)
			{
				chartAxis.m_labelIntervalOffsetType = (ExpressionInfo)this.m_labelIntervalOffsetType.PublishClone(context);
			}
			return chartAxis;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.AxisTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisTitle));
			list.Add(new MemberInfo(MemberName.MajorGridLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines));
			list.Add(new MemberInfo(MemberName.MinorGridLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines));
			list.Add(new MemberInfo(MemberName.CrossAt, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Scalar, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Minimum, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Maximum, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartStripLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStripLine));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.Visible, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Margin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MajorTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks));
			list.Add(new MemberInfo(MemberName.MinorTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks));
			list.Add(new MemberInfo(MemberName.MarksAlwaysAtPlotEdge, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reverse, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Location, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interlaced, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Angle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Arrows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventFontShrink, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventFontGrow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventLabelOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PreventWordWrap, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowLabelRotation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IncludeZero, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelsAutoFitDisabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxFontSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.OffsetLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideEndLabels, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AxisScaleBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak));
			list.Add(new MemberInfo(MemberName.AutoCrossAt, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoScaleMin, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AutoScaleMax, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.VariableAutoInterval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelInterval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelIntervalOffsetType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartAxis.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.AxisTitle:
					writer.Write(this.m_title);
					break;
				case MemberName.MajorGridLines:
					writer.Write(this.m_majorGridLines);
					break;
				case MemberName.MinorGridLines:
					writer.Write(this.m_minorGridLines);
					break;
				case MemberName.CrossAt:
					writer.Write(this.m_crossAt);
					break;
				case MemberName.AutoCrossAt:
					writer.Write(this.m_autoCrossAt);
					break;
				case MemberName.Scalar:
					writer.Write(this.m_scalar);
					break;
				case MemberName.Minimum:
					writer.Write(this.m_min);
					break;
				case MemberName.Maximum:
					writer.Write(this.m_max);
					break;
				case MemberName.AutoScaleMin:
					writer.Write(this.m_autoScaleMin);
					break;
				case MemberName.AutoScaleMax:
					writer.Write(this.m_autoScaleMax);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.ChartStripLines:
					writer.Write(this.m_chartStripLines);
					break;
				case MemberName.Visible:
					writer.Write(this.m_visible);
					break;
				case MemberName.Margin:
					writer.Write(this.m_margin);
					break;
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalType:
					writer.Write(this.m_intervalType);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
					break;
				case MemberName.IntervalOffsetType:
					writer.Write(this.m_intervalOffsetType);
					break;
				case MemberName.MajorTickMarks:
					writer.Write(this.m_majorTickMarks);
					break;
				case MemberName.MinorTickMarks:
					writer.Write(this.m_minorTickMarks);
					break;
				case MemberName.MarksAlwaysAtPlotEdge:
					writer.Write(this.m_marksAlwaysAtPlotEdge);
					break;
				case MemberName.Reverse:
					writer.Write(this.m_reverse);
					break;
				case MemberName.Location:
					writer.Write(this.m_location);
					break;
				case MemberName.Interlaced:
					writer.Write(this.m_interlaced);
					break;
				case MemberName.InterlacedColor:
					writer.Write(this.m_interlacedColor);
					break;
				case MemberName.LogScale:
					writer.Write(this.m_logScale);
					break;
				case MemberName.LogBase:
					writer.Write(this.m_logBase);
					break;
				case MemberName.HideLabels:
					writer.Write(this.m_hideLabels);
					break;
				case MemberName.Angle:
					writer.Write(this.m_angle);
					break;
				case MemberName.Arrows:
					writer.Write(this.m_arrows);
					break;
				case MemberName.PreventFontShrink:
					writer.Write(this.m_preventFontShrink);
					break;
				case MemberName.PreventFontGrow:
					writer.Write(this.m_preventFontGrow);
					break;
				case MemberName.PreventLabelOffset:
					writer.Write(this.m_preventLabelOffset);
					break;
				case MemberName.PreventWordWrap:
					writer.Write(this.m_preventWordWrap);
					break;
				case MemberName.AllowLabelRotation:
					writer.Write(this.m_allowLabelRotation);
					break;
				case MemberName.IncludeZero:
					writer.Write(this.m_includeZero);
					break;
				case MemberName.LabelsAutoFitDisabled:
					writer.Write(this.m_labelsAutoFitDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(this.m_minFontSize);
					break;
				case MemberName.MaxFontSize:
					writer.Write(this.m_maxFontSize);
					break;
				case MemberName.OffsetLabels:
					writer.Write(this.m_offsetLabels);
					break;
				case MemberName.HideEndLabels:
					writer.Write(this.m_hideEndLabels);
					break;
				case MemberName.AxisScaleBreak:
					writer.Write(this.m_axisScaleBreak);
					break;
				case MemberName.VariableAutoInterval:
					writer.Write(this.m_variableAutoInterval);
					break;
				case MemberName.LabelInterval:
					writer.Write(this.m_labelInterval);
					break;
				case MemberName.LabelIntervalType:
					writer.Write(this.m_labelIntervalType);
					break;
				case MemberName.LabelIntervalOffset:
					writer.Write(this.m_labelIntervalOffset);
					break;
				case MemberName.LabelIntervalOffsetType:
					writer.Write(this.m_labelIntervalOffsetType);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ChartAxis.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.AxisTitle:
					this.m_title = (ChartAxisTitle)reader.ReadRIFObject();
					break;
				case MemberName.MajorGridLines:
					this.m_majorGridLines = (ChartGridLines)reader.ReadRIFObject();
					break;
				case MemberName.MinorGridLines:
					this.m_minorGridLines = (ChartGridLines)reader.ReadRIFObject();
					break;
				case MemberName.CrossAt:
					this.m_crossAt = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoCrossAt:
					this.m_autoCrossAt = reader.ReadBoolean();
					break;
				case MemberName.Scalar:
					this.m_scalar = reader.ReadBoolean();
					break;
				case MemberName.Minimum:
					this.m_min = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Maximum:
					this.m_max = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AutoScaleMin:
					this.m_autoScaleMin = reader.ReadBoolean();
					break;
				case MemberName.AutoScaleMax:
					this.m_autoScaleMax = reader.ReadBoolean();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.ChartStripLines:
					this.m_chartStripLines = reader.ReadGenericListOfRIFObjects<ChartStripLine>();
					break;
				case MemberName.Visible:
					this.m_visible = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Margin:
					this.m_margin = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalType:
					this.m_intervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffsetType:
					this.m_intervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MajorTickMarks:
					this.m_majorTickMarks = (ChartTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MinorTickMarks:
					this.m_minorTickMarks = (ChartTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MarksAlwaysAtPlotEdge:
					this.m_marksAlwaysAtPlotEdge = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reverse:
					this.m_reverse = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Location:
					this.m_location = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interlaced:
					this.m_interlaced = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedColor:
					this.m_interlacedColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogScale:
					this.m_logScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogBase:
					this.m_logBase = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideLabels:
					this.m_hideLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Angle:
					this.m_angle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Arrows:
					this.m_arrows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventFontShrink:
					this.m_preventFontShrink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventFontGrow:
					this.m_preventFontGrow = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventLabelOffset:
					this.m_preventLabelOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PreventWordWrap:
					this.m_preventWordWrap = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowLabelRotation:
					this.m_allowLabelRotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IncludeZero:
					this.m_includeZero = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelsAutoFitDisabled:
					this.m_labelsAutoFitDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					this.m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxFontSize:
					this.m_maxFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.OffsetLabels:
					this.m_offsetLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideEndLabels:
					this.m_hideEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AxisScaleBreak:
					this.m_axisScaleBreak = (ChartAxisScaleBreak)reader.ReadRIFObject();
					break;
				case MemberName.VariableAutoInterval:
					this.m_variableAutoInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelInterval:
					this.m_labelInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalType:
					this.m_labelIntervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalOffset:
					this.m_labelIntervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelIntervalOffsetType:
					this.m_labelIntervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxis;
		}

		internal object EvaluateCrossAt(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (this.m_crossAt == null)
			{
				return null;
			}
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(this.m_exprHost, this.m_crossAt, base.Name, "CrossAt", ExpressionType.CrossAt);
		}

		internal object EvaluateMin(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (this.m_min == null)
			{
				return null;
			}
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(this.m_exprHost, this.m_min, base.Name, "Minimum", ExpressionType.Min);
		}

		internal object EvaluateMax(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			if (this.m_max == null)
			{
				return null;
			}
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartAxisValueExpression(this.m_exprHost, this.m_max, base.Name, "Maximum", ExpressionType.Max);
		}

		internal ChartAxisArrow EvaluateArrows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisArrow(context.ReportRuntime.EvaluateChartAxisArrowsExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal string EvaluateVisible(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisVisibleExpression(this, base.m_chart.Name);
		}

		internal string EvaluateMargin(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMarginExpression(this, base.m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIntervalExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisIntervalTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIntervalOffsetExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisIntervalOffsetTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateMarksAlwaysAtPlotEdge(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMarksAlwaysAtPlotEdgeExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateReverse(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisReverseExpression(this, base.m_chart.Name);
		}

		internal ChartAxisLocation EvaluateLocation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisLocation(context.ReportRuntime.EvaluateChartAxisLocationExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateInterlaced(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisInterlacedExpression(this, base.m_chart.Name);
		}

		internal string EvaluateInterlacedColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisInterlacedColorExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateLogScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLogScaleExpression(this, base.m_chart.Name);
		}

		internal double EvaluateLogBase(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLogBaseExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateHideLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisHideLabelsExpression(this, base.m_chart.Name);
		}

		internal double EvaluateAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisAngleExpression(this, base.m_chart.Name);
		}

		internal bool EvaluatePreventFontShrink(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventFontShrinkExpression(this, base.m_chart.Name);
		}

		internal bool EvaluatePreventFontGrow(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventFontGrowExpression(this, base.m_chart.Name);
		}

		internal bool EvaluatePreventLabelOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventLabelOffsetExpression(this, base.m_chart.Name);
		}

		internal bool EvaluatePreventWordWrap(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisPreventWordWrapExpression(this, base.m_chart.Name);
		}

		internal ChartAxisLabelRotation EvaluateAllowLabelRotation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAxisLabelRotation(context.ReportRuntime.EvaluateChartAxisAllowLabelRotationExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal bool EvaluateIncludeZero(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisIncludeZeroExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateLabelsAutoFitDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelsAutoFitDisabledExpression(this, base.m_chart.Name);
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMinFontSizeExpression(this, base.m_chart.Name);
		}

		internal string EvaluateMaxFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisMaxFontSizeExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateOffsetLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisOffsetLabelsExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateHideEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisHideEndLabelsExpression(this, base.m_chart.Name);
		}

		internal bool EvaluateVariableAutoInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisVariableAutoIntervalExpression(this, base.m_chart.Name);
		}

		internal double EvaluateLabelInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelIntervalExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateLabelIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisLabelIntervalTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateLabelIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisLabelIntervalOffsetsExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateLabelIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartAxisLabelIntervalOffsetTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}
	}
}
