using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataPointInstance : BaseInstance, IReportScopeInstance
	{
		private ChartDataPoint m_chartDataPointDef;

		private StyleInstance m_style;

		private object m_axisLabel;

		private bool m_isNewContext = true;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartDataPointDef, this.m_chartDataPointDef, this.m_chartDataPointDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public object AxisLabel
		{
			get
			{
				if (this.m_axisLabel == null && !this.m_chartDataPointDef.ChartDef.IsOldSnapshot)
				{
					this.m_axisLabel = this.m_chartDataPointDef.DataPointDef.EvaluateAxisLabel(this.ReportScopeInstance, this.m_chartDataPointDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_axisLabel;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_chartDataPointDef.DataPointDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartDataPointDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		string IReportScopeInstance.UniqueName
		{
			get
			{
				if (this.m_chartDataPointDef.ChartDef.IsOldSnapshot)
				{
					return this.m_chartDataPointDef.ChartDef.ID + 'i' + this.m_chartDataPointDef.RenderItem.InstanceInfo.DataPointIndex.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_chartDataPointDef.DataPointDef.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_isNewContext;
			}
			set
			{
				this.m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return base.m_reportScope;
			}
		}

		internal ChartDataPointInstance(ChartDataPoint chartDataPointDef)
			: base(chartDataPointDef)
		{
			this.m_chartDataPointDef = chartDataPointDef;
		}

		internal override void SetNewContext()
		{
			if (!this.m_isNewContext)
			{
				this.m_isNewContext = true;
				base.SetNewContext();
			}
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_axisLabel = null;
			this.m_toolTip = null;
		}
	}
}
