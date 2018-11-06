using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackFrame : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame m_defObject;

		private BackFrameInstance m_instance;

		private Style m_style;

		private ReportEnumProperty<GaugeFrameStyles> m_frameStyle;

		private ReportEnumProperty<GaugeFrameShapes> m_frameShape;

		private ReportDoubleProperty m_frameWidth;

		private ReportEnumProperty<GaugeGlassEffects> m_glassEffect;

		private FrameBackground m_frameBackground;

		private FrameImage m_frameImage;

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

		public ReportEnumProperty<GaugeFrameStyles> FrameStyle
		{
			get
			{
				if (this.m_frameStyle == null && this.m_defObject.FrameStyle != null)
				{
					this.m_frameStyle = new ReportEnumProperty<GaugeFrameStyles>(this.m_defObject.FrameStyle.IsExpression, this.m_defObject.FrameStyle.OriginalText, EnumTranslator.TranslateGaugeFrameStyles(this.m_defObject.FrameStyle.StringValue, null));
				}
				return this.m_frameStyle;
			}
		}

		public ReportEnumProperty<GaugeFrameShapes> FrameShape
		{
			get
			{
				if (this.m_frameShape == null && this.m_defObject.FrameShape != null)
				{
					this.m_frameShape = new ReportEnumProperty<GaugeFrameShapes>(this.m_defObject.FrameShape.IsExpression, this.m_defObject.FrameShape.OriginalText, EnumTranslator.TranslateGaugeFrameShapes(this.m_defObject.FrameShape.StringValue, null));
				}
				return this.m_frameShape;
			}
		}

		public ReportDoubleProperty FrameWidth
		{
			get
			{
				if (this.m_frameWidth == null && this.m_defObject.FrameWidth != null)
				{
					this.m_frameWidth = new ReportDoubleProperty(this.m_defObject.FrameWidth);
				}
				return this.m_frameWidth;
			}
		}

		public ReportEnumProperty<GaugeGlassEffects> GlassEffect
		{
			get
			{
				if (this.m_glassEffect == null && this.m_defObject.GlassEffect != null)
				{
					this.m_glassEffect = new ReportEnumProperty<GaugeGlassEffects>(this.m_defObject.GlassEffect.IsExpression, this.m_defObject.GlassEffect.OriginalText, EnumTranslator.TranslateGaugeGlassEffects(this.m_defObject.GlassEffect.StringValue, null));
				}
				return this.m_glassEffect;
			}
		}

		public FrameBackground FrameBackground
		{
			get
			{
				if (this.m_frameBackground == null && this.m_defObject.FrameBackground != null)
				{
					this.m_frameBackground = new FrameBackground(this.m_defObject.FrameBackground, this.m_gaugePanel);
				}
				return this.m_frameBackground;
			}
		}

		public FrameImage FrameImage
		{
			get
			{
				if (this.m_frameImage == null && this.m_defObject.FrameImage != null)
				{
					this.m_frameImage = new FrameImage(this.m_defObject.FrameImage, this.m_gaugePanel);
				}
				return this.m_frameImage;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame BackFrameDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public BackFrameInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new BackFrameInstance(this);
				}
				return this.m_instance;
			}
		}

		internal BackFrame(AspNetCore.ReportingServices.ReportIntermediateFormat.BackFrame defObject, GaugePanel gaugePanel)
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
			if (this.m_frameBackground != null)
			{
				this.m_frameBackground.SetNewContext();
			}
			if (this.m_frameImage != null)
			{
				this.m_frameImage.SetNewContext();
			}
		}
	}
}
