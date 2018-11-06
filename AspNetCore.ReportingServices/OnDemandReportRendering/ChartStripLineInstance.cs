using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLineInstance : BaseInstance
	{
		private ChartStripLine m_chartStripLineDef;

		private StyleInstance m_style;

		private string m_title;

		private int? m_titleAngle;

		private TextOrientations? m_textOrientation;

		private string m_toolTip;

		private double? m_interval;

		private ChartIntervalType? m_intervalType;

		private double? m_intervalOffset;

		private ChartIntervalType? m_intervalOffsetType;

		private double? m_stripWidth;

		private ChartIntervalType? m_stripWidthType;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartStripLineDef, this.m_chartStripLineDef.ChartDef, this.m_chartStripLineDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string Title
		{
			get
			{
				if (this.m_title == null && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_title = this.m_chartStripLineDef.ChartStripLineDef.EvaluateTitle(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_title;
			}
		}

		[Obsolete("Use TextOrientation instead.")]
		public int TitleAngle
		{
			get
			{
				if (!this.m_titleAngle.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_titleAngle = this.m_chartStripLineDef.ChartStripLineDef.EvaluateTitleAngle(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_titleAngle.Value;
			}
		}

		public TextOrientations TextOrientation
		{
			get
			{
				if (!this.m_textOrientation.HasValue)
				{
					this.m_textOrientation = this.m_chartStripLineDef.ChartStripLineDef.EvaluateTextOrientation(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_textOrientation.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_toolTip = this.m_chartStripLineDef.ChartStripLineDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_interval = this.m_chartStripLineDef.ChartStripLineDef.EvaluateInterval(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public ChartIntervalType IntervalType
		{
			get
			{
				if (!this.m_intervalType.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalType = this.m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalType(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalType.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffset = this.m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public ChartIntervalType IntervalOffsetType
		{
			get
			{
				if (!this.m_intervalOffsetType.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_intervalOffsetType = this.m_chartStripLineDef.ChartStripLineDef.EvaluateIntervalOffsetType(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffsetType.Value;
			}
		}

		public double StripWidth
		{
			get
			{
				if (!this.m_stripWidth.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_stripWidth = this.m_chartStripLineDef.ChartStripLineDef.EvaluateStripWidth(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_stripWidth.Value;
			}
		}

		public ChartIntervalType StripWidthType
		{
			get
			{
				if (!this.m_stripWidthType.HasValue && !this.m_chartStripLineDef.ChartDef.IsOldSnapshot)
				{
					this.m_stripWidthType = this.m_chartStripLineDef.ChartStripLineDef.EvaluateStripWidthType(this.ReportScopeInstance, this.m_chartStripLineDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_stripWidthType.Value;
			}
		}

		internal ChartStripLineInstance(ChartStripLine chartStripLineDef)
			: base(chartStripLineDef.ChartDef)
		{
			this.m_chartStripLineDef = chartStripLineDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_title = null;
			this.m_textOrientation = null;
			this.m_titleAngle = null;
			this.m_toolTip = null;
			this.m_interval = null;
			this.m_intervalType = null;
			this.m_intervalOffset = null;
			this.m_intervalOffsetType = null;
			this.m_stripWidth = null;
			this.m_stripWidthType = null;
		}
	}
}
