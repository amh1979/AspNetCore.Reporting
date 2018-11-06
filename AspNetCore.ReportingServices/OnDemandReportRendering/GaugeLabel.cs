using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeLabel : GaugePanelItem
	{
		private ReportStringProperty m_text;

		private ReportDoubleProperty m_angle;

		private ReportEnumProperty<GaugeResizeModes> m_resizeMode;

		private ReportSizeProperty m_textShadowOffset;

		private ReportBoolProperty m_useFontPercent;

		public ReportStringProperty Text
		{
			get
			{
				if (this.m_text == null && this.GaugeLabelDef.Text != null)
				{
					this.m_text = new ReportStringProperty(this.GaugeLabelDef.Text);
				}
				return this.m_text;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (this.m_angle == null && this.GaugeLabelDef.Angle != null)
				{
					this.m_angle = new ReportDoubleProperty(this.GaugeLabelDef.Angle);
				}
				return this.m_angle;
			}
		}

		public ReportEnumProperty<GaugeResizeModes> ResizeMode
		{
			get
			{
				if (this.m_resizeMode == null && this.GaugeLabelDef.ResizeMode != null)
				{
					this.m_resizeMode = new ReportEnumProperty<GaugeResizeModes>(this.GaugeLabelDef.ResizeMode.IsExpression, this.GaugeLabelDef.ResizeMode.OriginalText, EnumTranslator.TranslateGaugeResizeModes(this.GaugeLabelDef.ResizeMode.StringValue, null));
				}
				return this.m_resizeMode;
			}
		}

		public ReportSizeProperty TextShadowOffset
		{
			get
			{
				if (this.m_textShadowOffset == null && this.GaugeLabelDef.TextShadowOffset != null)
				{
					this.m_textShadowOffset = new ReportSizeProperty(this.GaugeLabelDef.TextShadowOffset);
				}
				return this.m_textShadowOffset;
			}
		}

		public ReportBoolProperty UseFontPercent
		{
			get
			{
				if (this.m_useFontPercent == null && this.GaugeLabelDef.UseFontPercent != null)
				{
					this.m_useFontPercent = new ReportBoolProperty(this.GaugeLabelDef.UseFontPercent);
				}
				return this.m_useFontPercent;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel GaugeLabelDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel)base.m_defObject;
			}
		}

		public new GaugeLabelInstance Instance
		{
			get
			{
				return (GaugeLabelInstance)this.GetInstance();
			}
		}

		internal GaugeLabel(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeLabel defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override BaseInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new GaugeLabelInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
