using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartEmptyPoints : IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private InternalChartSeries m_chartSeries;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints m_chartEmptyPointsDef;

		private ChartEmptyPointsInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ChartMarker m_marker;

		private ChartDataLabel m_dataLabel;

		private ReportVariantProperty m_axisLabel;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ReportStringProperty m_toolTip;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartEmptyPointsDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chartSeries.ReportScope, this.m_chartEmptyPointsDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chartSeries.ChartSeriesDef.UniqueName + 'x' + "EmptyPoint";
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartEmptyPointsDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chartSeries.ReportScope, this.m_chartEmptyPointsDef.Action, this.m_chartSeries.ChartSeriesDef, this.m_chart, ObjectType.Chart, this.m_chartEmptyPointsDef.Name, this);
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

		public ChartMarker Marker
		{
			get
			{
				if (this.m_marker == null && !this.m_chart.IsOldSnapshot && this.m_chartEmptyPointsDef.Marker != null)
				{
					this.m_marker = new ChartMarker(this.m_chartSeries, this.m_chartEmptyPointsDef.Marker, this.m_chart);
				}
				return this.m_marker;
			}
		}

		public ChartDataLabel DataLabel
		{
			get
			{
				if (this.m_dataLabel == null && !this.m_chart.IsOldSnapshot && this.m_chartEmptyPointsDef.DataLabel != null)
				{
					this.m_dataLabel = new ChartDataLabel(this.m_chartSeries, this.m_chartEmptyPointsDef.DataLabel, this.m_chart);
				}
				return this.m_dataLabel;
			}
		}

		public ReportVariantProperty AxisLabel
		{
			get
			{
				if (this.m_axisLabel == null && !this.m_chart.IsOldSnapshot && this.m_chartEmptyPointsDef.AxisLabel != null)
				{
					this.m_axisLabel = new ReportVariantProperty(this.m_chartEmptyPointsDef.AxisLabel);
				}
				return this.m_axisLabel;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					bool isOldSnapshot = this.m_chart.IsOldSnapshot;
					if (this.m_chartEmptyPointsDef.ToolTip != null)
					{
						this.m_toolTip = new ReportStringProperty(this.m_chartEmptyPointsDef.ToolTip);
					}
				}
				return this.m_toolTip;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return null;
				}
				if (this.m_customProperties == null)
				{
					this.m_customPropertiesReady = true;
					this.m_customProperties = new CustomPropertyCollection(this.m_chart.ReportScope.ReportScopeInstance, this.m_chart.RenderingContext, null, this.m_chartEmptyPointsDef, ObjectType.Chart, this.m_chart.Name);
				}
				else if (!this.m_customPropertiesReady)
				{
					this.m_customPropertiesReady = true;
					this.m_customProperties.UpdateCustomProperties(this.m_chartSeries.ReportScope.ReportScopeInstance, this.m_chartEmptyPointsDef, this.m_chart.RenderingContext.OdpContext, ObjectType.Chart, this.m_chart.Name);
				}
				return this.m_customProperties;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_chartSeries.ReportScope;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints ChartEmptyPointsDef
		{
			get
			{
				return this.m_chartEmptyPointsDef;
			}
		}

		public ChartEmptyPointsInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartEmptyPointsInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartEmptyPoints(InternalChartSeries chartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartEmptyPoints chartEmptyPointsDef, Chart chart)
		{
			this.m_chartEmptyPointsDef = chartEmptyPointsDef;
			this.m_chart = chart;
			this.m_chartSeries = chartSeries;
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
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_marker != null)
			{
				this.m_marker.SetNewContext();
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.SetNewContext();
			}
			this.m_customPropertiesReady = false;
		}
	}
}
