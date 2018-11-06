namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSeriesInstance : BaseInstance
	{
		private InternalChartSeries m_chartSeriesDef;

		private StyleInstance m_style;

		private ChartSeriesType? m_type;

		private ChartSeriesSubtype? m_subtype;

		private string m_legendName;

		private string m_legendText;

		private bool? m_hideInLegend;

		private string m_chartAreaName;

		private string m_valueAxisName;

		private string m_categoryAxisName;

		private string m_toolTip;

		private bool? m_hidden;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartSeriesDef, this.m_chartSeriesDef.ChartDef, this.m_chartSeriesDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartSeriesType Type
		{
			get
			{
				if (!this.m_type.HasValue)
				{
					if (this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
					{
						this.m_type = this.m_chartSeriesDef.Type.Value;
					}
					else
					{
						this.m_type = this.m_chartSeriesDef.ChartSeriesDef.EvaluateType(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_type.Value;
			}
		}

		public ChartSeriesSubtype Subtype
		{
			get
			{
				if (!this.m_subtype.HasValue)
				{
					if (this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
					{
						this.m_subtype = this.m_chartSeriesDef.Subtype.Value;
					}
					else
					{
						this.m_subtype = this.m_chartSeriesDef.ChartSeriesDef.EvaluateSubtype(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_subtype.Value;
			}
		}

		public string LegendName
		{
			get
			{
				if (this.m_legendName == null && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_legendName = this.m_chartSeriesDef.ChartSeriesDef.EvaluateLegendName(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_legendName;
			}
		}

		internal string LegendText
		{
			get
			{
				if (this.m_legendText == null && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_legendText = this.m_chartSeriesDef.ChartSeriesDef.EvaluateLegendText(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_legendText;
			}
		}

		internal bool HideInLegend
		{
			get
			{
				if (!this.m_hideInLegend.HasValue && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_hideInLegend = this.m_chartSeriesDef.ChartSeriesDef.EvaluateHideInLegend(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hideInLegend.Value;
			}
		}

		public string ChartAreaName
		{
			get
			{
				if (this.m_chartAreaName == null && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_chartAreaName = this.m_chartSeriesDef.ChartSeriesDef.EvaluateChartAreaName(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_chartAreaName;
			}
		}

		public string ValueAxisName
		{
			get
			{
				if (this.m_valueAxisName == null && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_valueAxisName = this.m_chartSeriesDef.ChartSeriesDef.EvaluateValueAxisName(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_valueAxisName;
			}
		}

		public string CategoryAxisName
		{
			get
			{
				if (this.m_categoryAxisName == null && !this.m_chartSeriesDef.ChartDef.IsOldSnapshot)
				{
					this.m_categoryAxisName = this.m_chartSeriesDef.ChartSeriesDef.EvaluateCategoryAxisName(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_categoryAxisName;
			}
		}

		internal string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_chartSeriesDef.ChartSeriesDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_chartSeriesDef.ChartSeriesDef.EvaluateHidden(this.ReportScopeInstance, this.m_chartSeriesDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		internal ChartSeriesInstance(InternalChartSeries chartSeriesDef)
			: base(chartSeriesDef.ReportScope)
		{
			this.m_chartSeriesDef = chartSeriesDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_type = null;
			this.m_subtype = null;
			this.m_legendName = null;
			this.m_legendText = null;
			this.m_hideInLegend = null;
			this.m_chartAreaName = null;
			this.m_valueAxisName = null;
			this.m_categoryAxisName = null;
			this.m_toolTip = null;
			this.m_hidden = null;
		}
	}
}
