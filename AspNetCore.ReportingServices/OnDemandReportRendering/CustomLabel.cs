using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomLabel : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel m_defObject;

		private Style m_style;

		private ReportStringProperty m_text;

		private ReportBoolProperty m_allowUpsideDown;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportDoubleProperty m_fontAngle;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_rotateLabel;

		private TickMarkStyle m_tickMarkStyle;

		private ReportDoubleProperty m_value;

		private ReportBoolProperty m_hidden;

		private ReportBoolProperty m_useFontPercent;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_gaugePanel, this.m_gaugePanel, this.m_defObject, this.m_gaugePanel.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string Name
		{
			get
			{
				return this.m_defObject.Name;
			}
		}

		public ReportStringProperty Text
		{
			get
			{
				if (this.m_text == null && this.m_defObject.Text != null)
				{
					this.m_text = new ReportStringProperty(this.m_defObject.Text);
				}
				return this.m_text;
			}
		}

		public ReportBoolProperty AllowUpsideDown
		{
			get
			{
				if (this.m_allowUpsideDown == null && this.m_defObject.AllowUpsideDown != null)
				{
					this.m_allowUpsideDown = new ReportBoolProperty(this.m_defObject.AllowUpsideDown);
				}
				return this.m_allowUpsideDown;
			}
		}

		public ReportDoubleProperty DistanceFromScale
		{
			get
			{
				if (this.m_distanceFromScale == null && this.m_defObject.DistanceFromScale != null)
				{
					this.m_distanceFromScale = new ReportDoubleProperty(this.m_defObject.DistanceFromScale);
				}
				return this.m_distanceFromScale;
			}
		}

		public ReportDoubleProperty FontAngle
		{
			get
			{
				if (this.m_fontAngle == null && this.m_defObject.FontAngle != null)
				{
					this.m_fontAngle = new ReportDoubleProperty(this.m_defObject.FontAngle);
				}
				return this.m_fontAngle;
			}
		}

		public ReportEnumProperty<GaugeLabelPlacements> Placement
		{
			get
			{
				if (this.m_placement == null && this.m_defObject.Placement != null)
				{
					this.m_placement = new ReportEnumProperty<GaugeLabelPlacements>(this.m_defObject.Placement.IsExpression, this.m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugeLabelPlacements(this.m_defObject.Placement.StringValue, null));
				}
				return this.m_placement;
			}
		}

		public ReportBoolProperty RotateLabel
		{
			get
			{
				if (this.m_rotateLabel == null && this.m_defObject.RotateLabel != null)
				{
					this.m_rotateLabel = new ReportBoolProperty(this.m_defObject.RotateLabel);
				}
				return this.m_rotateLabel;
			}
		}

		public TickMarkStyle TickMarkStyle
		{
			get
			{
				if (this.m_tickMarkStyle == null && this.m_defObject.TickMarkStyle != null)
				{
					this.m_tickMarkStyle = new TickMarkStyle(this.m_defObject.TickMarkStyle, this.m_gaugePanel);
				}
				return this.m_tickMarkStyle;
			}
		}

		public ReportDoubleProperty Value
		{
			get
			{
				if (this.m_value == null && this.m_defObject.Value != null)
				{
					this.m_value = new ReportDoubleProperty(this.m_defObject.Value);
				}
				return this.m_value;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && this.m_defObject.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_defObject.Hidden);
				}
				return this.m_hidden;
			}
		}

		public ReportBoolProperty UseFontPercent
		{
			get
			{
				if (this.m_useFontPercent == null && this.m_defObject.UseFontPercent != null)
				{
					this.m_useFontPercent = new ReportBoolProperty(this.m_defObject.UseFontPercent);
				}
				return this.m_useFontPercent;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel CustomLabelDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public CustomLabelInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new CustomLabelInstance(this);
				}
				return (CustomLabelInstance)base.m_instance;
			}
		}

		internal CustomLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomLabel defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_tickMarkStyle != null)
			{
				this.m_tickMarkStyle.SetNewContext();
			}
		}
	}
}
