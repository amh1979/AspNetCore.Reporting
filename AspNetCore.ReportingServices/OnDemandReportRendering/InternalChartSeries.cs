using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class InternalChartSeries : ChartSeries, IROMActionOwner
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries m_chartSeriesDef;

		private ChartSeriesInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartSeriesType> m_type;

		private ReportEnumProperty<ChartSeriesSubtype> m_subtype;

		private ChartSmartLabel m_smartLabel;

		private ChartEmptyPoints m_emptyPoints;

		private ReportStringProperty m_legendName;

		private ReportStringProperty m_legendText;

		private ReportBoolProperty m_hideInLegend;

		private ReportStringProperty m_chartAreaName;

		private ReportStringProperty m_valueAxisName;

		private ReportStringProperty m_categoryAxisName;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ChartDataLabel m_dataLabel;

		private ChartMarker m_marker;

		private IReportScope m_reportScope;

		private ChartDerivedSeries m_parentDerivedSeries;

		private List<ChartDerivedSeries> m_childrenDerivedSeries;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ChartItemInLegend m_chartItemInLegend;

		public override ChartDataPoint this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_chartDataPoints == null)
					{
						base.m_chartDataPoints = new ChartDataPoint[this.Count];
					}
					if (base.m_chartDataPoints[index] == null)
					{
						base.m_chartDataPoints[index] = new InternalChartDataPoint(base.m_chart, base.m_seriesIndex, index, this.m_chartSeriesDef.DataPoints[index]);
					}
					return base.m_chartDataPoints[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_chartSeriesDef.Cells.Count;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_chartSeriesDef.Name;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chartSeriesDef.UniqueName;
			}
		}

		public override Style Style
		{
			get
			{
				if (this.m_style == null && this.m_chartSeriesDef.StyleClass != null)
				{
					this.m_style = new Style(base.m_chart, this.ReportScope, this.m_chartSeriesDef, base.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		internal override ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && this.m_chartSeriesDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(base.m_chart.RenderingContext, this.ReportScope, this.m_chartSeriesDef.Action, this.m_chartSeriesDef, base.m_chart, ObjectType.Chart, base.m_chart.Name, this);
				}
				return this.m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_customProperties == null)
				{
					this.m_customPropertiesReady = true;
					this.m_customProperties = new CustomPropertyCollection(base.m_chart.ReportScope.ReportScopeInstance, base.m_chart.RenderingContext, null, this.m_chartSeriesDef, ObjectType.Chart, base.m_chart.Name);
				}
				else if (!this.m_customPropertiesReady)
				{
					this.m_customPropertiesReady = true;
					this.m_customProperties.UpdateCustomProperties(this.ReportScope.ReportScopeInstance, this.m_chartSeriesDef, base.m_chart.RenderingContext.OdpContext, ObjectType.Chart, base.m_chart.Name);
				}
				return this.m_customProperties;
			}
		}

		public override ReportEnumProperty<ChartSeriesType> Type
		{
			get
			{
				if (this.m_type == null && this.m_chartSeriesDef.Type != null)
				{
					this.m_type = new ReportEnumProperty<ChartSeriesType>(this.m_chartSeriesDef.Type.IsExpression, this.m_chartSeriesDef.Type.OriginalText, EnumTranslator.TranslateChartSeriesType(this.m_chartSeriesDef.Type.StringValue, null));
				}
				return this.m_type;
			}
		}

		public override ReportEnumProperty<ChartSeriesSubtype> Subtype
		{
			get
			{
				if (this.m_subtype == null && this.m_chartSeriesDef.Subtype != null)
				{
					this.m_subtype = new ReportEnumProperty<ChartSeriesSubtype>(this.m_chartSeriesDef.Subtype.IsExpression, this.m_chartSeriesDef.Subtype.OriginalText, EnumTranslator.TranslateChartSeriesSubtype(this.m_chartSeriesDef.Subtype.StringValue, null));
				}
				return this.m_subtype;
			}
		}

		public override ChartSmartLabel SmartLabel
		{
			get
			{
				if (this.m_smartLabel == null && this.m_chartSeriesDef.ChartSmartLabel != null)
				{
					this.m_smartLabel = new ChartSmartLabel(this, this.m_chartSeriesDef.ChartSmartLabel, base.m_chart);
				}
				return this.m_smartLabel;
			}
		}

		public override ChartEmptyPoints EmptyPoints
		{
			get
			{
				if (this.m_emptyPoints == null && this.m_chartSeriesDef.EmptyPoints != null)
				{
					this.m_emptyPoints = new ChartEmptyPoints(this, this.m_chartSeriesDef.EmptyPoints, base.m_chart);
				}
				return this.m_emptyPoints;
			}
		}

		public override ReportStringProperty LegendName
		{
			get
			{
				if (this.m_legendName == null && this.m_chartSeriesDef.LegendName != null)
				{
					this.m_legendName = new ReportStringProperty(this.m_chartSeriesDef.LegendName);
				}
				return this.m_legendName;
			}
		}

		internal override ReportStringProperty LegendText
		{
			get
			{
				if (this.m_legendText == null && this.m_chartSeriesDef.LegendText != null)
				{
					this.m_legendText = new ReportStringProperty(this.m_chartSeriesDef.LegendText);
				}
				return this.m_legendText;
			}
		}

		internal override ReportBoolProperty HideInLegend
		{
			get
			{
				if (this.m_hideInLegend == null && this.m_chartSeriesDef.HideInLegend != null)
				{
					this.m_hideInLegend = new ReportBoolProperty(this.m_chartSeriesDef.HideInLegend);
				}
				return this.m_hideInLegend;
			}
		}

		public override ReportStringProperty ChartAreaName
		{
			get
			{
				if (this.m_chartAreaName == null && this.m_chartSeriesDef.ChartAreaName != null)
				{
					this.m_chartAreaName = new ReportStringProperty(this.m_chartSeriesDef.ChartAreaName);
				}
				return this.m_chartAreaName;
			}
		}

		public override ReportStringProperty ValueAxisName
		{
			get
			{
				if (this.m_valueAxisName == null && this.m_chartSeriesDef.ValueAxisName != null)
				{
					this.m_valueAxisName = new ReportStringProperty(this.m_chartSeriesDef.ValueAxisName);
				}
				return this.m_valueAxisName;
			}
		}

		public override ReportStringProperty CategoryAxisName
		{
			get
			{
				if (this.m_categoryAxisName == null && this.m_chartSeriesDef.CategoryAxisName != null)
				{
					this.m_categoryAxisName = new ReportStringProperty(this.m_chartSeriesDef.CategoryAxisName);
				}
				return this.m_categoryAxisName;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (this.m_dataLabel == null && this.m_chartSeriesDef.DataLabel != null)
				{
					this.m_dataLabel = new ChartDataLabel(this, this.m_chartSeriesDef.DataLabel, base.m_chart);
				}
				return this.m_dataLabel;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (this.m_marker == null && this.m_chartSeriesDef.Marker != null)
				{
					this.m_marker = new ChartMarker(this, this.m_chartSeriesDef.Marker, base.m_chart);
				}
				return this.m_marker;
			}
		}

		internal override ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && this.m_chartSeriesDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartSeriesDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public override ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && this.m_chartSeriesDef.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_chartSeriesDef.Hidden);
				}
				return this.m_hidden;
			}
		}

		public override ChartItemInLegend ChartItemInLegend
		{
			get
			{
				if (this.m_chartItemInLegend == null && this.m_chartSeriesDef.ChartItemInLegend != null)
				{
					this.m_chartItemInLegend = new ChartItemInLegend(this, this.m_chartSeriesDef.ChartItemInLegend, base.m_chart);
				}
				return this.m_chartItemInLegend;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries ChartSeriesDef
		{
			get
			{
				return this.m_chartSeriesDef;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return base.m_chart;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_reportScope == null)
				{
					if (this.m_parentDerivedSeries == null)
					{
						this.m_reportScope = base.m_chart.GetChartMember(this);
					}
					else
					{
						this.m_reportScope = this.m_parentDerivedSeries.ReportScope;
					}
				}
				return this.m_reportScope;
			}
		}

		public override ChartSeriesInstance Instance
		{
			get
			{
				if (base.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartSeriesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal List<ChartDerivedSeries> ChildrenDerivedSeries
		{
			get
			{
				if (this.m_childrenDerivedSeries == null)
				{
					this.m_childrenDerivedSeries = base.m_chart.GetChildrenDerivedSeries(this.Name);
				}
				return this.m_childrenDerivedSeries;
			}
		}

		internal InternalChartSeries(ChartDerivedSeries parentDerivedSeries)
			: this(parentDerivedSeries.ChartDef, 0, parentDerivedSeries.ChartDerivedSeriesDef.Series)
		{
			this.m_parentDerivedSeries = parentDerivedSeries;
		}

		internal InternalChartSeries(Chart chart, int seriesIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartSeries seriesDef)
			: base(chart, seriesIndex)
		{
			this.m_chartSeriesDef = seriesDef;
		}

		internal override void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_emptyPoints != null)
			{
				this.m_emptyPoints.SetNewContext();
			}
			if (this.m_smartLabel != null)
			{
				this.m_smartLabel.SetNewContext();
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.SetNewContext();
			}
			if (this.m_marker != null)
			{
				this.m_marker.SetNewContext();
			}
			if (base.m_chartDataPoints != null)
			{
				ChartDataPoint[] chartDataPoints = base.m_chartDataPoints;
				foreach (ChartDataPoint chartDataPoint in chartDataPoints)
				{
					if (chartDataPoint != null)
					{
						chartDataPoint.SetNewContext();
					}
				}
			}
			List<ChartDerivedSeries> childrenDerivedSeries = this.ChildrenDerivedSeries;
			if (childrenDerivedSeries != null)
			{
				foreach (ChartDerivedSeries item in childrenDerivedSeries)
				{
					if (item != null)
					{
						item.SetNewContext();
					}
				}
			}
			if (this.m_chartItemInLegend != null)
			{
				this.m_chartItemInLegend.SetNewContext();
			}
			this.m_customPropertiesReady = false;
		}
	}
}
