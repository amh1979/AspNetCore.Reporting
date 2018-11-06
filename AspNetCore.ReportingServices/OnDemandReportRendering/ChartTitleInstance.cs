namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitleInstance : BaseInstance
	{
		private ChartTitle m_chartTitleDef;

		private StyleInstance m_style;

		private bool m_captionEvaluated;

		private string m_caption;

		private bool? m_hidden;

		private int? m_dockOffset;

		private bool? m_dockOutsideChartArea;

		private string m_toolTip;

		private ChartTitlePositions? m_position;

		private TextOrientations? m_textOrientation;

		public string Caption
		{
			get
			{
				if (!this.m_captionEvaluated)
				{
					this.m_captionEvaluated = true;
					if (this.m_chartTitleDef.ChartDef.IsOldSnapshot)
					{
						this.m_caption = this.m_chartTitleDef.RenderChartTitleInstance.Caption;
					}
					else
					{
						this.m_caption = this.m_chartTitleDef.ChartTitleDef.EvaluateCaption(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_caption;
			}
		}

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartTitleDef, this.m_chartTitleDef.ChartDef, this.m_chartTitleDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartTitlePositions Position
		{
			get
			{
				if (!this.m_position.HasValue && !this.m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_position = this.m_chartTitleDef.ChartTitleDef.EvaluatePosition(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue && !this.m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_hidden = this.m_chartTitleDef.ChartTitleDef.EvaluateHidden(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public int DockOffset
		{
			get
			{
				if (!this.m_dockOffset.HasValue && !this.m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_dockOffset = this.m_chartTitleDef.ChartTitleDef.EvaluateDockOffset(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_dockOffset.Value;
			}
		}

		public bool DockOutsideChartArea
		{
			get
			{
				if (!this.m_dockOutsideChartArea.HasValue && !this.m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_dockOutsideChartArea = this.m_chartTitleDef.ChartTitleDef.EvaluateDockOutsideChartArea(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_dockOutsideChartArea.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chartTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_toolTip = this.m_chartTitleDef.ChartTitleDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!this.m_textOrientation.HasValue)
				{
					this.m_textOrientation = this.m_chartTitleDef.ChartTitleDef.EvaluateTextOrientation(this.ReportScopeInstance, this.m_chartTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_textOrientation.Value;
			}
		}

		internal ChartTitleInstance(ChartTitle chartTitleDef)
			: base(chartTitleDef.ChartDef)
		{
			this.m_chartTitleDef = chartTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_captionEvaluated = false;
			this.m_hidden = null;
			this.m_dockOffset = null;
			this.m_dockOutsideChartArea = null;
			this.m_toolTip = null;
			this.m_position = null;
			this.m_textOrientation = null;
		}
	}
}
