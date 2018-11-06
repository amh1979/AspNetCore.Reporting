namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomLabelInstance : BaseInstance
	{
		private CustomLabel m_defObject;

		private StyleInstance m_style;

		private string m_text;

		private bool? m_allowUpsideDown;

		private double? m_distanceFromScale;

		private double? m_fontAngle;

		private GaugeLabelPlacements? m_placement;

		private bool? m_rotateLabel;

		private double? m_value;

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

		public string Text
		{
			get
			{
				if (this.m_text == null)
				{
					this.m_text = this.m_defObject.CustomLabelDef.EvaluateText(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_allowUpsideDown = this.m_defObject.CustomLabelDef.EvaluateAllowUpsideDown(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_distanceFromScale = this.m_defObject.CustomLabelDef.EvaluateDistanceFromScale(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_fontAngle = this.m_defObject.CustomLabelDef.EvaluateFontAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_placement = this.m_defObject.CustomLabelDef.EvaluatePlacement(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_rotateLabel = this.m_defObject.CustomLabelDef.EvaluateRotateLabel(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_rotateLabel.Value;
			}
		}

		public double Value
		{
			get
			{
				if (!this.m_value.HasValue)
				{
					this.m_value = this.m_defObject.CustomLabelDef.EvaluateValue(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_value.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.CustomLabelDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
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
					this.m_useFontPercent = this.m_defObject.CustomLabelDef.EvaluateUseFontPercent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_useFontPercent.Value;
			}
		}

		internal CustomLabelInstance(CustomLabel defObject)
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
			this.m_value = null;
			this.m_hidden = null;
			this.m_useFontPercent = null;
		}
	}
}
