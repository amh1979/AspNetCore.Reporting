namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCellInstance : BaseInstance
	{
		private ChartLegendCustomItemCell m_chartLegendCustomItemCellDef;

		private StyleInstance m_style;

		private ChartCellType? m_cellType;

		private string m_text;

		private int? m_cellSpan;

		private string m_toolTip;

		private int? m_imageWidth;

		private int? m_imageHeight;

		private int? m_symbolHeight;

		private int? m_symbolWidth;

		private ChartCellAlignment? m_alignment;

		private int? m_topMargin;

		private int? m_bottomMargin;

		private int? m_leftMargin;

		private int? m_rightMargin;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartLegendCustomItemCellDef, this.m_chartLegendCustomItemCellDef.ChartDef, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartCellType CellType
		{
			get
			{
				if (!this.m_cellType.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_cellType = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateCellType(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_cellType.Value;
			}
		}

		public string Text
		{
			get
			{
				if (this.m_text == null && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_text = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateText(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_text;
			}
		}

		public int CellSpan
		{
			get
			{
				if (!this.m_cellSpan.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_cellSpan = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateCellSpan(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_cellSpan.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_toolTip = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public int ImageWidth
		{
			get
			{
				if (!this.m_imageWidth.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_imageWidth = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateImageWidth(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_imageWidth.Value;
			}
		}

		public int ImageHeight
		{
			get
			{
				if (!this.m_imageHeight.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_imageHeight = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateImageHeight(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_imageHeight.Value;
			}
		}

		public int SymbolHeight
		{
			get
			{
				if (!this.m_symbolHeight.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_symbolHeight = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateSymbolHeight(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_symbolHeight.Value;
			}
		}

		public int SymbolWidth
		{
			get
			{
				if (!this.m_symbolWidth.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_symbolWidth = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateSymbolWidth(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_symbolWidth.Value;
			}
		}

		public ChartCellAlignment Alignment
		{
			get
			{
				if (!this.m_alignment.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_alignment = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateAlignment(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_alignment.Value;
			}
		}

		public int TopMargin
		{
			get
			{
				if (!this.m_topMargin.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_topMargin = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateTopMargin(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_topMargin.Value;
			}
		}

		public int BottomMargin
		{
			get
			{
				if (!this.m_bottomMargin.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_bottomMargin = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateBottomMargin(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_bottomMargin.Value;
			}
		}

		public int LeftMargin
		{
			get
			{
				if (!this.m_leftMargin.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_leftMargin = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateLeftMargin(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_leftMargin.Value;
			}
		}

		public int RightMargin
		{
			get
			{
				if (!this.m_rightMargin.HasValue && !this.m_chartLegendCustomItemCellDef.ChartDef.IsOldSnapshot)
				{
					this.m_rightMargin = this.m_chartLegendCustomItemCellDef.ChartLegendCustomItemCellDef.EvaluateRightMargin(this.ReportScopeInstance, this.m_chartLegendCustomItemCellDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_rightMargin.Value;
			}
		}

		internal ChartLegendCustomItemCellInstance(ChartLegendCustomItemCell chartLegendCustomItemCellDef)
			: base(chartLegendCustomItemCellDef.ChartDef)
		{
			this.m_chartLegendCustomItemCellDef = chartLegendCustomItemCellDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_cellType = null;
			this.m_text = null;
			this.m_cellSpan = null;
			this.m_toolTip = null;
			this.m_imageWidth = null;
			this.m_imageHeight = null;
			this.m_symbolHeight = null;
			this.m_symbolWidth = null;
			this.m_alignment = null;
			this.m_topMargin = null;
			this.m_bottomMargin = null;
			this.m_leftMargin = null;
			this.m_rightMargin = null;
		}
	}
}
