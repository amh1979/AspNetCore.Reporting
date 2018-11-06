namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PinLabelInstance : BaseInstance
	{
		private PinLabel m_defObject;

		private StyleInstance m_style;

		private string m_text;

		private bool? m_allowUpsideDown;

		private double? m_distanceFromScale;

		private double? m_fontAngle;

		private GaugeLabelPlacements? m_placement;

		private bool? m_rotateLabel;

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

		public string Text
		{
			get
			{
				if (this.m_text == null)
				{
					this.m_text = this.m_defObject.PinLabelDef.EvaluateText(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_text;
			}
		}

		public bool AllowUpsideDown
		{
			get
			{
				if (!this.m_allowUpsideDown.HasValue)
				{
					this.m_allowUpsideDown = this.m_defObject.PinLabelDef.EvaluateAllowUpsideDown(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_distanceFromScale = this.m_defObject.PinLabelDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_fontAngle = this.m_defObject.PinLabelDef.EvaluateFontAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_placement = this.m_defObject.PinLabelDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_placement.Value;
			}
		}

		public bool RotateLabel
		{
			get
			{
				if (!this.m_rotateLabel.HasValue)
				{
					this.m_rotateLabel = this.m_defObject.PinLabelDef.EvaluateRotateLabel(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_rotateLabel.Value;
			}
		}

		public bool UseFontPercent
		{
			get
			{
				if (!this.m_useFontPercent.HasValue)
				{
					this.m_useFontPercent = this.m_defObject.PinLabelDef.EvaluateUseFontPercent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_useFontPercent.Value;
			}
		}

		internal PinLabelInstance(PinLabel defObject)
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
			this.m_text = null;
			this.m_allowUpsideDown = null;
			this.m_distanceFromScale = null;
			this.m_fontAngle = null;
			this.m_placement = null;
			this.m_rotateLabel = null;
			this.m_useFontPercent = null;
		}
	}
}
