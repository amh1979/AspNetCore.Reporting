namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendInstance : BaseInstance
	{
		private ChartLegend m_legendDef;

		private StyleInstance m_style;

		private bool? m_hidden;

		private ChartLegendPositions? m_position;

		private ChartLegendLayouts? m_layout;

		private bool? m_dockOutsideChartArea;

		private bool? m_autoFitTextDisabled;

		private ReportSize m_minFontSize;

		private ChartSeparators? m_headerSeparator;

		private ReportColor m_headerSeparatorColor;

		private ChartSeparators? m_columnSeparator;

		private ReportColor m_columnSeparatorColor;

		private int? m_columnSpacing;

		private bool? m_interlacedRows;

		private ReportColor m_interlacedRowsColor;

		private bool? m_equallySpacedItems;

		private ChartAutoBool? m_reversed;

		private int? m_maxAutoSize;

		private int? m_textWrapThreshold;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_legendDef, this.m_legendDef.ChartDef, this.m_legendDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_hidden = this.m_legendDef.ChartLegendDef.EvaluateHidden(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public ChartLegendPositions Position
		{
			get
			{
				if (!this.m_position.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_position = this.m_legendDef.ChartLegendDef.EvaluatePosition(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public ChartLegendLayouts Layout
		{
			get
			{
				if (!this.m_layout.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_layout = this.m_legendDef.ChartLegendDef.EvaluateLayout(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_layout.Value;
			}
		}

		public bool DockOutsideChartArea
		{
			get
			{
				if (!this.m_dockOutsideChartArea.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_dockOutsideChartArea = this.m_legendDef.ChartLegendDef.EvaluateDockOutsideChartArea(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_dockOutsideChartArea.Value;
			}
		}

		public bool AutoFitTextDisabled
		{
			get
			{
				if (!this.m_autoFitTextDisabled.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_autoFitTextDisabled = this.m_legendDef.ChartLegendDef.EvaluateAutoFitTextDisabled(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_autoFitTextDisabled.Value;
			}
		}

		public ReportSize MinFontSize
		{
			get
			{
				if (this.m_minFontSize == null && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_minFontSize = new ReportSize(this.m_legendDef.ChartLegendDef.EvaluateMinFontSize(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_minFontSize;
			}
		}

		public ChartSeparators HeaderSeparator
		{
			get
			{
				if (!this.m_headerSeparator.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_headerSeparator = this.m_legendDef.ChartLegendDef.EvaluateHeaderSeparator(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_headerSeparator.Value;
			}
		}

		public ReportColor HeaderSeparatorColor
		{
			get
			{
				if (this.m_headerSeparatorColor == null && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_headerSeparatorColor = new ReportColor(this.m_legendDef.ChartLegendDef.EvaluateHeaderSeparatorColor(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_headerSeparatorColor;
			}
		}

		public ChartSeparators ColumnSeparator
		{
			get
			{
				if (!this.m_columnSeparator.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_columnSeparator = this.m_legendDef.ChartLegendDef.EvaluateColumnSeparator(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_columnSeparator.Value;
			}
		}

		public ReportColor ColumnSeparatorColor
		{
			get
			{
				if (this.m_columnSeparatorColor == null && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_columnSeparatorColor = new ReportColor(this.m_legendDef.ChartLegendDef.EvaluateColumnSeparatorColor(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_columnSeparatorColor;
			}
		}

		public int ColumnSpacing
		{
			get
			{
				if (!this.m_columnSpacing.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_columnSpacing = this.m_legendDef.ChartLegendDef.EvaluateColumnSpacing(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_columnSpacing.Value;
			}
		}

		public bool InterlacedRows
		{
			get
			{
				if (!this.m_interlacedRows.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_interlacedRows = this.m_legendDef.ChartLegendDef.EvaluateInterlacedRows(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interlacedRows.Value;
			}
		}

		public ReportColor InterlacedRowsColor
		{
			get
			{
				if (this.m_interlacedRowsColor == null && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_interlacedRowsColor = new ReportColor(this.m_legendDef.ChartLegendDef.EvaluateInterlacedRowsColor(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_interlacedRowsColor;
			}
		}

		public bool EquallySpacedItems
		{
			get
			{
				if (!this.m_equallySpacedItems.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_equallySpacedItems = this.m_legendDef.ChartLegendDef.EvaluateEquallySpacedItems(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_equallySpacedItems.Value;
			}
		}

		public ChartAutoBool Reversed
		{
			get
			{
				if (!this.m_reversed.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_reversed = this.m_legendDef.ChartLegendDef.EvaluateReversed(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_reversed.Value;
			}
		}

		public int MaxAutoSize
		{
			get
			{
				if (!this.m_maxAutoSize.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_maxAutoSize = this.m_legendDef.ChartLegendDef.EvaluateMaxAutoSize(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_maxAutoSize.Value;
			}
		}

		public int TextWrapThreshold
		{
			get
			{
				if (!this.m_textWrapThreshold.HasValue && !this.m_legendDef.ChartDef.IsOldSnapshot)
				{
					this.m_textWrapThreshold = this.m_legendDef.ChartLegendDef.EvaluateTextWrapThreshold(this.ReportScopeInstance, this.m_legendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_textWrapThreshold.Value;
			}
		}

		internal ChartLegendInstance(ChartLegend legendDef)
			: base(legendDef.ChartDef)
		{
			this.m_legendDef = legendDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_hidden = null;
			this.m_position = null;
			this.m_layout = null;
			this.m_dockOutsideChartArea = null;
			this.m_autoFitTextDisabled = null;
			this.m_minFontSize = null;
			this.m_headerSeparator = null;
			this.m_headerSeparatorColor = null;
			this.m_columnSeparator = null;
			this.m_columnSeparatorColor = null;
			this.m_columnSpacing = null;
			this.m_interlacedRows = null;
			this.m_interlacedRowsColor = null;
			this.m_equallySpacedItems = null;
			this.m_reversed = null;
			this.m_maxAutoSize = null;
			this.m_textWrapThreshold = null;
		}
	}
}
