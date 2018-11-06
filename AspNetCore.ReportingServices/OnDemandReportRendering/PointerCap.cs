using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerCap : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap m_defObject;

		private PointerCapInstance m_instance;

		private Style m_style;

		private CapImage m_capImage;

		private ReportBoolProperty m_onTop;

		private ReportBoolProperty m_reflection;

		private ReportEnumProperty<GaugeCapStyles> m_capStyle;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

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

		public CapImage CapImage
		{
			get
			{
				if (this.m_capImage == null && this.m_defObject.CapImage != null)
				{
					this.m_capImage = new CapImage(this.m_defObject.CapImage, this.m_gaugePanel);
				}
				return this.m_capImage;
			}
		}

		public ReportBoolProperty OnTop
		{
			get
			{
				if (this.m_onTop == null && this.m_defObject.OnTop != null)
				{
					this.m_onTop = new ReportBoolProperty(this.m_defObject.OnTop);
				}
				return this.m_onTop;
			}
		}

		public ReportBoolProperty Reflection
		{
			get
			{
				if (this.m_reflection == null && this.m_defObject.Reflection != null)
				{
					this.m_reflection = new ReportBoolProperty(this.m_defObject.Reflection);
				}
				return this.m_reflection;
			}
		}

		public ReportEnumProperty<GaugeCapStyles> CapStyle
		{
			get
			{
				if (this.m_capStyle == null && this.m_defObject.CapStyle != null)
				{
					this.m_capStyle = new ReportEnumProperty<GaugeCapStyles>(this.m_defObject.CapStyle.IsExpression, this.m_defObject.CapStyle.OriginalText, EnumTranslator.TranslateGaugeCapStyles(this.m_defObject.CapStyle.StringValue, null));
				}
				return this.m_capStyle;
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

		public ReportDoubleProperty Width
		{
			get
			{
				if (this.m_width == null && this.m_defObject.Width != null)
				{
					this.m_width = new ReportDoubleProperty(this.m_defObject.Width);
				}
				return this.m_width;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap PointerCapDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public PointerCapInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new PointerCapInstance(this);
				}
				return this.m_instance;
			}
		}

		internal PointerCap(AspNetCore.ReportingServices.ReportIntermediateFormat.PointerCap defObject, GaugePanel gaugePanel)
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
			if (this.m_capImage != null)
			{
				this.m_capImage.SetNewContext();
			}
		}
	}
}
