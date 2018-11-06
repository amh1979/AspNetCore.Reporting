using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAlignType
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType m_chartAlignTypeDef;

		private ChartAlignTypeInstance m_instance;

		private ReportBoolProperty m_axesView;

		private ReportBoolProperty m_cursor;

		private ReportBoolProperty m_position;

		private ReportBoolProperty m_innerPlotPosition;

		public ReportBoolProperty AxesView
		{
			get
			{
				if (this.m_axesView == null && !this.m_chart.IsOldSnapshot && this.m_chartAlignTypeDef.AxesView != null)
				{
					this.m_axesView = new ReportBoolProperty(this.m_chartAlignTypeDef.AxesView);
				}
				return this.m_axesView;
			}
		}

		public ReportBoolProperty Cursor
		{
			get
			{
				if (this.m_cursor == null && !this.m_chart.IsOldSnapshot && this.m_chartAlignTypeDef.Cursor != null)
				{
					this.m_cursor = new ReportBoolProperty(this.m_chartAlignTypeDef.Cursor);
				}
				return this.m_cursor;
			}
		}

		public ReportBoolProperty Position
		{
			get
			{
				if (this.m_position == null && !this.m_chart.IsOldSnapshot && this.m_chartAlignTypeDef.Position != null)
				{
					this.m_position = new ReportBoolProperty(this.m_chartAlignTypeDef.Position);
				}
				return this.m_position;
			}
		}

		public ReportBoolProperty InnerPlotPosition
		{
			get
			{
				if (this.m_innerPlotPosition == null && !this.m_chart.IsOldSnapshot && this.m_chartAlignTypeDef.InnerPlotPosition != null)
				{
					this.m_innerPlotPosition = new ReportBoolProperty(this.m_chartAlignTypeDef.InnerPlotPosition);
				}
				return this.m_innerPlotPosition;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType ChartAlignTypeDef
		{
			get
			{
				return this.m_chartAlignTypeDef;
			}
		}

		public ChartAlignTypeInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartAlignTypeInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartAlignType(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAlignType chartAlignTypeDef, Chart chart)
		{
			this.m_chartAlignTypeDef = chartAlignTypeDef;
			this.m_chart = chart;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
