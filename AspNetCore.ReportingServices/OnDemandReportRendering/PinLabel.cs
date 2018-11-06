using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PinLabel : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel m_defObject;

		private PinLabelInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_text;

		private ReportBoolProperty m_allowUpsideDown;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportDoubleProperty m_fontAngle;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_rotateLabel;

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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel PinLabelDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public PinLabelInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new PinLabelInstance(this);
				}
				return this.m_instance;
			}
		}

		internal PinLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.PinLabel defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
