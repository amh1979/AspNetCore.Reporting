using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartNoMoveDirections
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections m_chartNoMoveDirectionsDef;

		private ChartNoMoveDirectionsInstance m_instance;

		private ReportBoolProperty m_up;

		private ReportBoolProperty m_down;

		private ReportBoolProperty m_left;

		private ReportBoolProperty m_right;

		private ReportBoolProperty m_upLeft;

		private ReportBoolProperty m_upRight;

		private ReportBoolProperty m_downLeft;

		private ReportBoolProperty m_downRight;

		private InternalChartSeries m_chartSeries;

		public ReportBoolProperty Up
		{
			get
			{
				if (this.m_up == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.Up != null)
				{
					this.m_up = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.Up);
				}
				return this.m_up;
			}
		}

		public ReportBoolProperty Down
		{
			get
			{
				if (this.m_down == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.Down != null)
				{
					this.m_down = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.Down);
				}
				return this.m_down;
			}
		}

		public ReportBoolProperty Left
		{
			get
			{
				if (this.m_left == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.Left != null)
				{
					this.m_left = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.Left);
				}
				return this.m_left;
			}
		}

		public ReportBoolProperty Right
		{
			get
			{
				if (this.m_right == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.Right != null)
				{
					this.m_right = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.Right);
				}
				return this.m_right;
			}
		}

		public ReportBoolProperty UpLeft
		{
			get
			{
				if (this.m_upLeft == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.UpLeft != null)
				{
					this.m_upLeft = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.UpLeft);
				}
				return this.m_upLeft;
			}
		}

		public ReportBoolProperty UpRight
		{
			get
			{
				if (this.m_upRight == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.UpRight != null)
				{
					this.m_upRight = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.UpRight);
				}
				return this.m_upRight;
			}
		}

		public ReportBoolProperty DownLeft
		{
			get
			{
				if (this.m_downLeft == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.DownLeft != null)
				{
					this.m_downLeft = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.DownLeft);
				}
				return this.m_downLeft;
			}
		}

		public ReportBoolProperty DownRight
		{
			get
			{
				if (this.m_downRight == null && !this.m_chart.IsOldSnapshot && this.m_chartNoMoveDirectionsDef.DownRight != null)
				{
					this.m_downRight = new ReportBoolProperty(this.m_chartNoMoveDirectionsDef.DownRight);
				}
				return this.m_downRight;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries.ReportScope;
				}
				return this.m_chart;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections ChartNoMoveDirectionsDef
		{
			get
			{
				return this.m_chartNoMoveDirectionsDef;
			}
		}

		public ChartNoMoveDirectionsInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartNoMoveDirectionsInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartNoMoveDirections(InternalChartSeries chartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartNoMoveDirections chartNoMoveDirectionsDef, Chart chart)
		{
			this.m_chartSeries = chartSeries;
			this.m_chartNoMoveDirectionsDef = chartNoMoveDirectionsDef;
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
