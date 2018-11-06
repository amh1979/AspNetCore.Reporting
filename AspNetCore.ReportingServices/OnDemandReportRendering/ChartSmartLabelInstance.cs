namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartSmartLabelInstance : BaseInstance
	{
		private ChartSmartLabel m_chartSmartLabelDef;

		private ChartAllowOutsideChartArea? m_allowOutSidePlotArea;

		private ReportColor m_calloutBackColor;

		private ChartCalloutLineAnchor? m_calloutLineAnchor;

		private ReportColor m_calloutLineColor;

		private ChartCalloutLineStyle? m_calloutLineStyle;

		private ReportSize m_calloutLineWidth;

		private ChartCalloutStyle? m_calloutStyle;

		private bool? m_showOverlapped;

		private bool? m_markerOverlapping;

		private ReportSize m_maxMovingDistance;

		private ReportSize m_minMovingDistance;

		private bool? m_disabled;

		public ChartAllowOutsideChartArea AllowOutSidePlotArea
		{
			get
			{
				if (!this.m_allowOutSidePlotArea.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_allowOutSidePlotArea = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateAllowOutSidePlotArea(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_allowOutSidePlotArea.Value;
			}
		}

		public ReportColor CalloutBackColor
		{
			get
			{
				if (this.m_calloutBackColor == null && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutBackColor = new ReportColor(this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutBackColor(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_calloutBackColor;
			}
		}

		public ChartCalloutLineAnchor CalloutLineAnchor
		{
			get
			{
				if (!this.m_calloutLineAnchor.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutLineAnchor = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineAnchor(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_calloutLineAnchor.Value;
			}
		}

		public ReportColor CalloutLineColor
		{
			get
			{
				if (this.m_calloutLineColor == null && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutLineColor = new ReportColor(this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineColor(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext), true);
				}
				return this.m_calloutLineColor;
			}
		}

		public ChartCalloutLineStyle CalloutLineStyle
		{
			get
			{
				if (!this.m_calloutLineStyle.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutLineStyle = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineStyle(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_calloutLineStyle.Value;
			}
		}

		public ReportSize CalloutLineWidth
		{
			get
			{
				if (this.m_calloutLineWidth == null && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutLineWidth = new ReportSize(this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutLineWidth(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_calloutLineWidth;
			}
		}

		public ChartCalloutStyle CalloutStyle
		{
			get
			{
				if (!this.m_calloutStyle.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_calloutStyle = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateCalloutStyle(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_calloutStyle.Value;
			}
		}

		public bool ShowOverlapped
		{
			get
			{
				if (!this.m_showOverlapped.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_showOverlapped = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateShowOverlapped(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_showOverlapped.Value;
			}
		}

		public bool MarkerOverlapping
		{
			get
			{
				if (!this.m_markerOverlapping.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_markerOverlapping = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMarkerOverlapping(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_markerOverlapping.Value;
			}
		}

		public bool Disabled
		{
			get
			{
				if (!this.m_disabled.HasValue && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_disabled = this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateDisabled(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_disabled.Value;
			}
		}

		public ReportSize MaxMovingDistance
		{
			get
			{
				if (this.m_maxMovingDistance == null && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_maxMovingDistance = new ReportSize(this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMaxMovingDistance(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_maxMovingDistance;
			}
		}

		public ReportSize MinMovingDistance
		{
			get
			{
				if (this.m_minMovingDistance == null && !this.m_chartSmartLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_minMovingDistance = new ReportSize(this.m_chartSmartLabelDef.ChartSmartLabelDef.EvaluateMinMovingDistance(this.ReportScopeInstance, this.m_chartSmartLabelDef.ChartDef.RenderingContext.OdpContext));
				}
				return this.m_minMovingDistance;
			}
		}

		internal ChartSmartLabelInstance(ChartSmartLabel chartSmartLabelDef)
			: base(chartSmartLabelDef.ReportScope)
		{
			this.m_chartSmartLabelDef = chartSmartLabelDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_allowOutSidePlotArea = null;
			this.m_calloutBackColor = null;
			this.m_calloutLineAnchor = null;
			this.m_calloutLineColor = null;
			this.m_calloutLineStyle = null;
			this.m_calloutLineWidth = null;
			this.m_calloutStyle = null;
			this.m_showOverlapped = null;
			this.m_markerOverlapping = null;
			this.m_maxMovingDistance = null;
			this.m_minMovingDistance = null;
			this.m_disabled = null;
		}
	}
}
