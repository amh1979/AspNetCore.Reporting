using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumn : ChartObjectCollectionItem<ChartLegendColumnInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn m_chartLegendColumnDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportEnumProperty<ChartColumnType> m_columnType;

		private ReportStringProperty m_value;

		private ReportStringProperty m_toolTip;

		private ReportSizeProperty m_minimumWidth;

		private ReportSizeProperty m_maximumWidth;

		private ReportIntProperty m_seriesSymbolWidth;

		private ReportIntProperty m_seriesSymbolHeight;

		private ChartLegendColumnHeader m_header;

		public Style Style
		{
			get
			{
				if (this.m_style == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.StyleClass != null)
				{
					this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartLegendColumnDef, this.m_chart.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_chart.ChartDef.UniqueName + 'x' + this.m_chartLegendColumnDef.ID;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.Action != null)
				{
					this.m_actionInfo = new ActionInfo(this.m_chart.RenderingContext, this.m_chart, this.m_chartLegendColumnDef.Action, this.m_chart.ChartDef, this.m_chart, ObjectType.Chart, this.m_chartLegendColumnDef.Name, this);
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

		public ReportEnumProperty<ChartColumnType> ColumnType
		{
			get
			{
				if (this.m_columnType == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.ColumnType != null)
				{
					this.m_columnType = new ReportEnumProperty<ChartColumnType>(this.m_chartLegendColumnDef.ColumnType.IsExpression, this.m_chartLegendColumnDef.ColumnType.OriginalText, EnumTranslator.TranslateChartColumnType(this.m_chartLegendColumnDef.ColumnType.StringValue, null));
				}
				return this.m_columnType;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (this.m_value == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.Value != null)
				{
					this.m_value = new ReportStringProperty(this.m_chartLegendColumnDef.Value);
				}
				return this.m_value;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_chartLegendColumnDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		public ReportSizeProperty MinimumWidth
		{
			get
			{
				if (this.m_minimumWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.MinimumWidth != null)
				{
					this.m_minimumWidth = new ReportSizeProperty(this.m_chartLegendColumnDef.MinimumWidth);
				}
				return this.m_minimumWidth;
			}
		}

		public ReportSizeProperty MaximumWidth
		{
			get
			{
				if (this.m_maximumWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.MaximumWidth != null)
				{
					this.m_maximumWidth = new ReportSizeProperty(this.m_chartLegendColumnDef.MaximumWidth);
				}
				return this.m_maximumWidth;
			}
		}

		public ReportIntProperty SeriesSymbolWidth
		{
			get
			{
				if (this.m_seriesSymbolWidth == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.SeriesSymbolWidth != null)
				{
					this.m_seriesSymbolWidth = new ReportIntProperty(this.m_chartLegendColumnDef.SeriesSymbolWidth.IsExpression, this.m_chartLegendColumnDef.SeriesSymbolWidth.OriginalText, this.m_chartLegendColumnDef.SeriesSymbolWidth.IntValue, 200);
				}
				return this.m_seriesSymbolWidth;
			}
		}

		public ReportIntProperty SeriesSymbolHeight
		{
			get
			{
				if (this.m_seriesSymbolHeight == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.SeriesSymbolHeight != null)
				{
					this.m_seriesSymbolHeight = new ReportIntProperty(this.m_chartLegendColumnDef.SeriesSymbolHeight.IsExpression, this.m_chartLegendColumnDef.SeriesSymbolHeight.OriginalText, this.m_chartLegendColumnDef.SeriesSymbolHeight.IntValue, 70);
				}
				return this.m_seriesSymbolHeight;
			}
		}

		public ChartLegendColumnHeader Header
		{
			get
			{
				if (this.m_header == null && !this.m_chart.IsOldSnapshot && this.m_chartLegendColumnDef.Header != null)
				{
					this.m_header = new ChartLegendColumnHeader(this.m_chartLegendColumnDef.Header, this.m_chart);
				}
				return this.m_header;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn ChartLegendColumnDef
		{
			get
			{
				return this.m_chartLegendColumnDef;
			}
		}

		public ChartLegendColumnInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartLegendColumnInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartLegendColumn(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartLegendColumn chartLegendColumnDef, Chart chart)
		{
			this.m_chartLegendColumnDef = chartLegendColumnDef;
			this.m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_header != null)
			{
				this.m_header.SetNewContext();
			}
		}
	}
}
