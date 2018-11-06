using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartElementPosition
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition m_defObject;

		private ChartElementPositionInstance m_instance;

		private ReportDoubleProperty m_top;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_height;

		private ReportDoubleProperty m_width;

		public ReportDoubleProperty Top
		{
			get
			{
				if (this.m_top == null && this.m_defObject.Top != null)
				{
					this.m_top = new ReportDoubleProperty(this.m_defObject.Top);
				}
				return this.m_top;
			}
		}

		public ReportDoubleProperty Left
		{
			get
			{
				if (this.m_left == null && this.m_defObject.Left != null)
				{
					this.m_left = new ReportDoubleProperty(this.m_defObject.Left);
				}
				return this.m_left;
			}
		}

		public ReportDoubleProperty Height
		{
			get
			{
				if (this.m_height == null && this.m_defObject.Height != null)
				{
					this.m_height = new ReportDoubleProperty(this.m_defObject.Height);
				}
				return this.m_height;
			}
		}

		public ReportDoubleProperty Width
		{
			get
			{
				if (this.m_width == null && this.m_defObject.Width != null)
				{
					this.m_width = new ReportDoubleProperty(this.m_defObject.Width);
				}
				return this.m_width;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition ChartElementPositionDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public ChartElementPositionInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartElementPositionInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartElementPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartElementPosition defObject, Chart chart)
		{
			this.m_defObject = defObject;
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
