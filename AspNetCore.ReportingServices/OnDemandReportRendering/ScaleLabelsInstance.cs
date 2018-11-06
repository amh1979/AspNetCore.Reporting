namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleLabelsInstance : BaseInstance
	{
		private ScaleLabels m_defObject;

		private StyleInstance m_style;

		private double? m_interval;

		private double? m_intervalOffset;

		private bool? m_allowUpsideDown;

		private double? m_distanceFromScale;

		private double? m_fontAngle;

		private GaugeLabelPlacements? m_placement;

		private bool? m_rotateLabels;

		private bool? m_showEndLabels;

		private bool? m_hidden;

		private bool? m_useFontPercent;

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
					this.m_interval = this.m_defObject.ScaleLabelsDef.EvaluateInterval(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_intervalOffset = this.m_defObject.ScaleLabelsDef.EvaluateIntervalOffset(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		public bool AllowUpsideDown
		{
			get
			{
				if (!this.m_allowUpsideDown.HasValue)
				{
					this.m_allowUpsideDown = this.m_defObject.ScaleLabelsDef.EvaluateAllowUpsideDown(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_allowUpsideDown.Value;
			}
		}

		public double DistanceFromScale
		{
			get
			{
				if (!this.m_distanceFromScale.HasValue)
				{
					this.m_distanceFromScale = this.m_defObject.ScaleLabelsDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_distanceFromScale.Value;
			}
		}

		public double FontAngle
		{
			get
			{
				if (!this.m_fontAngle.HasValue)
				{
					this.m_fontAngle = this.m_defObject.ScaleLabelsDef.EvaluateFontAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_fontAngle.Value;
			}
		}

		public GaugeLabelPlacements Placement
		{
			get
			{
				if (!this.m_placement.HasValue)
				{
					this.m_placement = this.m_defObject.ScaleLabelsDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_placement.Value;
			}
		}

		public bool RotateLabels
		{
			get
			{
				if (!this.m_rotateLabels.HasValue)
				{
					this.m_rotateLabels = this.m_defObject.ScaleLabelsDef.EvaluateRotateLabels(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_rotateLabels.Value;
			}
		}

		public bool ShowEndLabels
		{
			get
			{
				if (!this.m_showEndLabels.HasValue)
				{
					this.m_showEndLabels = this.m_defObject.ScaleLabelsDef.EvaluateShowEndLabels(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_showEndLabels.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.ScaleLabelsDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!this.m_useFontPercent.HasValue)
				{
					this.m_useFontPercent = this.m_defObject.ScaleLabelsDef.EvaluateUseFontPercent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_useFontPercent.Value;
			}
		}

		internal ScaleLabelsInstance(ScaleLabels defObject)
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
			this.m_allowUpsideDown = null;
			this.m_distanceFromScale = null;
			this.m_fontAngle = null;
			this.m_placement = null;
			this.m_rotateLabels = null;
			this.m_showEndLabels = null;
			this.m_hidden = null;
			this.m_useFontPercent = null;
		}
	}
}
