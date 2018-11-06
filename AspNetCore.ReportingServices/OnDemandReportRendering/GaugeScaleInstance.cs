namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class GaugeScaleInstance : BaseInstance
	{
		private GaugeScale m_defObject;

		private StyleInstance m_style;

		private double? m_interval;

		private double? m_intervalOffset;

		private bool? m_logarithmic;

		private double? m_logarithmicBase;

		private double? m_multiplier;

		private bool? m_reversed;

		private bool? m_tickMarksOnTop;

		private string m_toolTip;

		private bool? m_hidden;

		private double? m_width;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.GaugePanelDef, this.m_defObject.GaugePanelDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue)
				{
					this.m_interval = this.m_defObject.GaugeScaleDef.EvaluateInterval(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue)
				{
					this.m_intervalOffset = this.m_defObject.GaugeScaleDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public bool Logarithmic
		{
			get
			{
				if (!this.m_logarithmic.HasValue)
				{
					this.m_logarithmic = this.m_defObject.GaugeScaleDef.EvaluateLogarithmic(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_logarithmic.Value;
			}
		}

		public double LogarithmicBase
		{
			get
			{
				if (!this.m_logarithmicBase.HasValue)
				{
					this.m_logarithmicBase = this.m_defObject.GaugeScaleDef.EvaluateLogarithmicBase(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_logarithmicBase.Value;
			}
		}

		public double Multiplier
		{
			get
			{
				if (!this.m_multiplier.HasValue)
				{
					this.m_multiplier = this.m_defObject.GaugeScaleDef.EvaluateMultiplier(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_multiplier.Value;
			}
		}

		public bool Reversed
		{
			get
			{
				if (!this.m_reversed.HasValue)
				{
					this.m_reversed = this.m_defObject.GaugeScaleDef.EvaluateReversed(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_reversed.Value;
			}
		}

		public bool TickMarksOnTop
		{
			get
			{
				if (!this.m_tickMarksOnTop.HasValue)
				{
					this.m_tickMarksOnTop = this.m_defObject.GaugeScaleDef.EvaluateTickMarksOnTop(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_tickMarksOnTop.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_defObject.GaugeScaleDef.EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.GaugeScaleDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.GaugeScaleDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		internal GaugeScaleInstance(GaugeScale defObject)
			: base(defObject.GaugePanelDef)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_interval = null;
			this.m_intervalOffset = null;
			this.m_logarithmic = null;
			this.m_logarithmicBase = null;
			this.m_multiplier = null;
			this.m_reversed = null;
			this.m_tickMarksOnTop = null;
			this.m_toolTip = null;
			this.m_hidden = null;
			this.m_width = null;
		}
	}
}
