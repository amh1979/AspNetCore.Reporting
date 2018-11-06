namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnInstance : BaseInstance
	{
		private ChartLegendColumn m_chartLegendColumnDef;

		private StyleInstance m_style;

		private ChartColumnType? m_columnType;

		private string m_value;

		private string m_toolTip;

		private ReportSize m_minimumWidth;

		private ReportSize m_maximumWidth;

		private int? m_seriesSymbolWidth;

		private int? m_seriesSymbolHeight;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartLegendColumnDef, this.m_chartLegendColumnDef.ChartDef, this.m_chartLegendColumnDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartColumnType ColumnType
		{
			get
			{
				if (!this.m_columnType.HasValue && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_columnType = this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateColumnType(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_columnType.Value;
			}
		}

		public string Value
		{
			get
			{
				if (this.m_value == null && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_value = this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateValue(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_toolTip = this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public ReportSize MinimumWidth
		{
			get
			{
				if (this.m_minimumWidth == null && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_minimumWidth = new ReportSize(this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateMinimumWidth(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_minimumWidth;
			}
		}

		public ReportSize MaximumWidth
		{
			get
			{
				if (this.m_maximumWidth == null && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_maximumWidth = new ReportSize(this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateMaximumWidth(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_maximumWidth;
			}
		}

		public int SeriesSymbolWidth
		{
			get
			{
				if (!this.m_seriesSymbolWidth.HasValue && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_seriesSymbolWidth = this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateSeriesSymbolWidth(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_seriesSymbolWidth.Value;
			}
		}

		public int SeriesSymbolHeight
		{
			get
			{
				if (!this.m_seriesSymbolHeight.HasValue && !this.m_chartLegendColumnDef.ChartDef.IsOldSnapshot)
				{
					this.m_seriesSymbolHeight = this.m_chartLegendColumnDef.ChartLegendColumnDef.EvaluateSeriesSymbolHeight(this.ReportScopeInstance, this.m_chartLegendColumnDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_seriesSymbolHeight.Value;
			}
		}

		internal ChartLegendColumnInstance(ChartLegendColumn chartLegendColumnDef)
			: base(chartLegendColumnDef.ChartDef)
		{
			this.m_chartLegendColumnDef = chartLegendColumnDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_columnType = null;
			this.m_value = null;
			this.m_toolTip = null;
			this.m_minimumWidth = null;
			this.m_maximumWidth = null;
			this.m_seriesSymbolWidth = null;
			this.m_seriesSymbolHeight = null;
		}
	}
}
