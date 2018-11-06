using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class TickMarkStyle : IROMStyleDefinitionContainer
	{
		internal GaugePanel m_gaugePanel;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle m_defObject;

		protected TickMarkStyleInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_distanceFromScale;

		private ReportEnumProperty<GaugeLabelPlacements> m_placement;

		private ReportBoolProperty m_enableGradient;

		private ReportDoubleProperty m_gradientDensity;

		private TopImage m_tickMarkImage;

		private ReportDoubleProperty m_length;

		private ReportDoubleProperty m_width;

		private ReportEnumProperty<GaugeTickMarkShapes> m_shape;

		private ReportBoolProperty m_hidden;

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

		public ReportBoolProperty EnableGradient
		{
			get
			{
				if (this.m_enableGradient == null && this.m_defObject.EnableGradient != null)
				{
					this.m_enableGradient = new ReportBoolProperty(this.m_defObject.EnableGradient);
				}
				return this.m_enableGradient;
			}
		}

		public ReportDoubleProperty GradientDensity
		{
			get
			{
				if (this.m_gradientDensity == null && this.m_defObject.GradientDensity != null)
				{
					this.m_gradientDensity = new ReportDoubleProperty(this.m_defObject.GradientDensity);
				}
				return this.m_gradientDensity;
			}
		}

		public TopImage TickMarkImage
		{
			get
			{
				if (this.m_tickMarkImage == null && this.m_defObject.TickMarkImage != null)
				{
					this.m_tickMarkImage = new TopImage(this.m_defObject.TickMarkImage, this.m_gaugePanel);
				}
				return this.m_tickMarkImage;
			}
		}

		public ReportDoubleProperty Length
		{
			get
			{
				if (this.m_length == null && this.m_defObject.Length != null)
				{
					this.m_length = new ReportDoubleProperty(this.m_defObject.Length);
				}
				return this.m_length;
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

		public ReportEnumProperty<GaugeTickMarkShapes> Shape
		{
			get
			{
				if (this.m_shape == null && this.m_defObject.Shape != null)
				{
					this.m_shape = new ReportEnumProperty<GaugeTickMarkShapes>(this.m_defObject.Shape.IsExpression, this.m_defObject.Shape.OriginalText, EnumTranslator.TranslateGaugeTickMarkShapes(this.m_defObject.Shape.StringValue, null));
				}
				return this.m_shape;
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

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle TickMarkStyleDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public TickMarkStyleInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = this.GetInstance();
				}
				return this.m_instance;
			}
		}

		internal TickMarkStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.TickMarkStyle defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_tickMarkImage != null)
			{
				this.m_tickMarkImage.SetNewContext();
			}
		}

		protected virtual TickMarkStyleInstance GetInstance()
		{
			return new TickMarkStyleInstance(this);
		}
	}
}
