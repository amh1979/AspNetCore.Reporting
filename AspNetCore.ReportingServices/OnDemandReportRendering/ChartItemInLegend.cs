using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartItemInLegend : IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend m_chartItemInLegendDef;

		private ChartItemInLegendInstance m_instance;

		private ActionInfo m_actionInfo;

		private ReportStringProperty m_legendText;

		private ChartDataPoint m_dataPoint;

		private InternalChartSeries m_chartSeries;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		public string UniqueName
		{
			get
			{
				return this.InstancePath.UniqueName + 'x' + "InLegend";
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartItemInLegendDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.ReportScope, this.m_chartItemInLegendDef.Action, this.InstancePath, this.m_chart, ObjectType.Chart, this.m_chart.Name, this);
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

		public ReportStringProperty LegendText
		{
			get
			{
				if (this.m_legendText == null && !this.m_chart.IsOldSnapshot && this.m_chartItemInLegendDef.LegendText != null)
				{
					this.m_legendText = new ReportStringProperty(this.m_chartItemInLegendDef.LegendText);
				}
				return this.m_legendText;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartItemInLegendDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartItemInLegendDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && this.m_chartItemInLegendDef.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_chartItemInLegendDef.Hidden);
				}
				return this.m_hidden;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_dataPoint != null)
				{
					return this.m_dataPoint;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ReportScope;
				}
				return this.m_chart;
			}
		}

		private IInstancePath InstancePath
		{
			get
			{
				if (this.m_dataPoint != null)
				{
					return this.m_dataPoint.DataPointDef;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ChartSeriesDef;
				}
				return this.m_chart.ChartDef;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend ChartItemInLegendDef
		{
			get
			{
				return this.m_chartItemInLegendDef;
			}
		}

		public ChartItemInLegendInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartItemInLegendInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartItemInLegend(InternalChartSeries chartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegendDef, Chart chart)
		{
			this.m_chartSeries = chartSeries;
			this.m_chartItemInLegendDef = chartItemInLegendDef;
			this.m_chart = chart;
		}

		internal ChartItemInLegend(InternalChartDataPoint chartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartItemInLegend chartItemInLegendDef, Chart chart)
		{
			this.m_dataPoint = chartDataPoint;
			this.m_chartItemInLegendDef = chartItemInLegendDef;
			this.m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
		}
	}
}
