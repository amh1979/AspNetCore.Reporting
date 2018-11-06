namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartNoMoveDirectionsInstance : BaseInstance
	{
		private ChartNoMoveDirections m_chartNoMoveDirectionsDef;

		private bool? m_up;

		private bool? m_down;

		private bool? m_left;

		private bool? m_right;

		private bool? m_upLeft;

		private bool? m_upRight;

		private bool? m_downLeft;

		private bool? m_downRight;

		public bool Up
		{
			get
			{
				if (!this.m_up.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_up = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUp(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_up.Value;
			}
		}

		public bool Down
		{
			get
			{
				if (!this.m_down.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_down = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDown(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_down.Value;
			}
		}

		public bool Left
		{
			get
			{
				if (!this.m_left.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_left = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateLeft(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_left.Value;
			}
		}

		public bool Right
		{
			get
			{
				if (!this.m_right.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_right = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateRight(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_right.Value;
			}
		}

		public bool UpLeft
		{
			get
			{
				if (!this.m_upLeft.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_upLeft = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUpLeft(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_upLeft.Value;
			}
		}

		public bool UpRight
		{
			get
			{
				if (!this.m_upRight.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_upRight = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateUpRight(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_upRight.Value;
			}
		}

		public bool DownLeft
		{
			get
			{
				if (!this.m_downLeft.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_downLeft = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDownLeft(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_downLeft.Value;
			}
		}

		public bool DownRight
		{
			get
			{
				if (!this.m_downRight.HasValue && !this.m_chartNoMoveDirectionsDef.ChartDef.IsOldSnapshot)
				{
					this.m_downRight = this.m_chartNoMoveDirectionsDef.ChartNoMoveDirectionsDef.EvaluateDownRight(this.ReportScopeInstance, this.m_chartNoMoveDirectionsDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_downRight.Value;
			}
		}

		internal ChartNoMoveDirectionsInstance(ChartNoMoveDirections chartNoMoveDirectionsDef)
			: base(chartNoMoveDirectionsDef.ReportScope)
		{
			this.m_chartNoMoveDirectionsDef = chartNoMoveDirectionsDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_up = null;
			this.m_down = null;
			this.m_left = null;
			this.m_right = null;
			this.m_upLeft = null;
			this.m_upRight = null;
			this.m_downLeft = null;
			this.m_downRight = null;
		}
	}
}
