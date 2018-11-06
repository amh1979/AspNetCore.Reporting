namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAlignTypeInstance : BaseInstance
	{
		private ChartAlignType m_chartAlignTypeDef;

		private bool? m_axesView;

		private bool? m_cursor;

		private bool? m_position;

		private bool? m_innerPlotPosition;

		public bool AxesView
		{
			get
			{
				if (!this.m_axesView.HasValue && !this.m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					this.m_axesView = this.m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateAxesView(this.ReportScopeInstance, this.m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_axesView.Value;
			}
		}

		public bool Cursor
		{
			get
			{
				if (!this.m_cursor.HasValue && !this.m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					this.m_cursor = this.m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateCursor(this.ReportScopeInstance, this.m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_cursor.Value;
			}
		}

		public bool Position
		{
			get
			{
				if (!this.m_position.HasValue && !this.m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					this.m_position = this.m_chartAlignTypeDef.ChartAlignTypeDef.EvaluatePosition(this.ReportScopeInstance, this.m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public bool InnerPlotPosition
		{
			get
			{
				if (!this.m_innerPlotPosition.HasValue && !this.m_chartAlignTypeDef.ChartDef.IsOldSnapshot)
				{
					this.m_innerPlotPosition = this.m_chartAlignTypeDef.ChartAlignTypeDef.EvaluateInnerPlotPosition(this.ReportScopeInstance, this.m_chartAlignTypeDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_innerPlotPosition.Value;
			}
		}

		internal ChartAlignTypeInstance(ChartAlignType chartAlignTypeDef)
			: base(chartAlignTypeDef.ChartDef)
		{
			this.m_chartAlignTypeDef = chartAlignTypeDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_axesView = null;
			this.m_cursor = null;
			this.m_position = null;
			this.m_innerPlotPosition = null;
		}
	}
}
