namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisTitleInstance : BaseInstance
	{
		private ChartAxisTitle m_chartAxisTitleDef;

		private StyleInstance m_style;

		private bool m_captionEvaluated;

		private string m_caption;

		private ChartAxisTitlePositions? m_position;

		private TextOrientations? m_textOrientation;

		public string Caption
		{
			get
			{
				if (!this.m_captionEvaluated)
				{
					this.m_captionEvaluated = true;
					if (this.m_chartAxisTitleDef.ChartDef.IsOldSnapshot)
					{
						this.m_caption = this.m_chartAxisTitleDef.RenderChartTitleInstance.Caption;
					}
					else
					{
						this.m_caption = this.m_chartAxisTitleDef.ChartAxisTitleDef.EvaluateCaption(this.ReportScopeInstance, this.m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
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
					this.m_style = new StyleInstance(this.m_chartAxisTitleDef, this.m_chartAxisTitleDef.ChartDef, this.m_chartAxisTitleDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ChartAxisTitlePositions Position
		{
			get
			{
				if (!this.m_position.HasValue && !this.m_chartAxisTitleDef.ChartDef.IsOldSnapshot)
				{
					this.m_position = this.m_chartAxisTitleDef.ChartAxisTitleDef.EvaluatePosition(this.ReportScopeInstance, this.m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!this.m_textOrientation.HasValue)
				{
					this.m_textOrientation = this.m_chartAxisTitleDef.ChartAxisTitleDef.EvaluateTextOrientation(this.ReportScopeInstance, this.m_chartAxisTitleDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_textOrientation.Value;
			}
		}

		internal ChartAxisTitleInstance(ChartAxisTitle chartAxisTitleDef)
			: base(chartAxisTitleDef.ChartDef)
		{
			this.m_chartAxisTitleDef = chartAxisTitleDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_captionEvaluated = false;
			this.m_position = null;
			this.m_textOrientation = null;
		}
	}
}
