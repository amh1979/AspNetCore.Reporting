using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicator : GaugePanelItem
	{
		private GaugeInputValue m_gaugeInputValue;

		private ReportEnumProperty<GaugeStateIndicatorStyles> m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		private ReportDoubleProperty m_scaleFactor;

		private IndicatorStateCollection m_indicatorStates;

		private ReportEnumProperty<GaugeResizeModes> m_resizeMode;

		private ReportDoubleProperty m_angle;

		private ReportEnumProperty<GaugeTransformationType> m_transformationType;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private string m_compiledStateName;

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				if (this.m_gaugeInputValue == null && this.StateIndicatorDef.GaugeInputValue != null)
				{
					this.m_gaugeInputValue = new GaugeInputValue(this.StateIndicatorDef.GaugeInputValue, base.m_gaugePanel);
				}
				return this.m_gaugeInputValue;
			}
		}

		public ReportEnumProperty<GaugeTransformationType> TransformationType
		{
			get
			{
				if (this.m_transformationType == null && this.StateIndicatorDef.TransformationType != null)
				{
					this.m_transformationType = new ReportEnumProperty<GaugeTransformationType>(this.StateIndicatorDef.TransformationType.IsExpression, this.StateIndicatorDef.TransformationType.OriginalText, EnumTranslator.TranslateGaugeTransformationType(this.StateIndicatorDef.TransformationType.StringValue, null));
				}
				return this.m_transformationType;
			}
		}

		public string TransformationScope
		{
			get
			{
				return this.StateIndicatorDef.TransformationScope;
			}
		}

		public GaugeInputValue MaximumValue
		{
			get
			{
				if (this.m_maximumValue == null && this.StateIndicatorDef.MaximumValue != null)
				{
					this.m_maximumValue = new GaugeInputValue(this.StateIndicatorDef.MaximumValue, base.m_gaugePanel);
				}
				return this.m_maximumValue;
			}
		}

		public GaugeInputValue MinimumValue
		{
			get
			{
				if (this.m_minimumValue == null && this.StateIndicatorDef.MinimumValue != null)
				{
					this.m_minimumValue = new GaugeInputValue(this.StateIndicatorDef.MinimumValue, base.m_gaugePanel);
				}
				return this.m_minimumValue;
			}
		}

		public ReportEnumProperty<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				if (this.m_indicatorStyle == null && this.StateIndicatorDef.IndicatorStyle != null)
				{
					this.m_indicatorStyle = new ReportEnumProperty<GaugeStateIndicatorStyles>(this.StateIndicatorDef.IndicatorStyle.IsExpression, this.StateIndicatorDef.IndicatorStyle.OriginalText, EnumTranslator.TranslateGaugeStateIndicatorStyles(this.StateIndicatorDef.IndicatorStyle.StringValue, null));
				}
				return this.m_indicatorStyle;
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				if (this.m_indicatorImage == null && this.StateIndicatorDef.IndicatorImage != null)
				{
					this.m_indicatorImage = new IndicatorImage(this.StateIndicatorDef.IndicatorImage, base.m_gaugePanel);
				}
				return this.m_indicatorImage;
			}
		}

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (this.m_scaleFactor == null && this.StateIndicatorDef.ScaleFactor != null)
				{
					this.m_scaleFactor = new ReportDoubleProperty(this.StateIndicatorDef.ScaleFactor);
				}
				return this.m_scaleFactor;
			}
		}

		public IndicatorStateCollection IndicatorStates
		{
			get
			{
				if (this.m_indicatorStates == null && this.StateIndicatorDef.IndicatorStates != null)
				{
					this.m_indicatorStates = new IndicatorStateCollection(this, base.m_gaugePanel);
				}
				return this.m_indicatorStates;
			}
		}

		public ReportEnumProperty<GaugeResizeModes> ResizeMode
		{
			get
			{
				if (this.m_resizeMode == null && this.StateIndicatorDef.ResizeMode != null)
				{
					this.m_resizeMode = new ReportEnumProperty<GaugeResizeModes>(this.StateIndicatorDef.ResizeMode.IsExpression, this.StateIndicatorDef.ResizeMode.OriginalText, EnumTranslator.TranslateGaugeResizeModes(this.StateIndicatorDef.ResizeMode.StringValue, null));
				}
				return this.m_resizeMode;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (this.m_angle == null && this.StateIndicatorDef.Angle != null)
				{
					this.m_angle = new ReportDoubleProperty(this.StateIndicatorDef.Angle);
				}
				return this.m_angle;
			}
		}

		public string StateDataElementName
		{
			get
			{
				return this.StateIndicatorDef.StateDataElementName;
			}
		}

		public DataElementOutputTypes StateDataElementOutput
		{
			get
			{
				return this.StateIndicatorDef.StateDataElementOutput;
			}
		}

		public string CompiledStateName
		{
			get
			{
				base.m_gaugePanel.ProcessCompiledInstances();
				return this.m_compiledStateName;
			}
			set
			{
				this.m_compiledStateName = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator StateIndicatorDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)base.m_defObject;
			}
		}

		public new StateIndicatorInstance Instance
		{
			get
			{
				return (StateIndicatorInstance)this.GetInstance();
			}
		}

		internal StateIndicator(AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
		}

		internal override BaseInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new StateIndicatorInstance(this);
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
			if (this.m_gaugeInputValue != null)
			{
				this.m_gaugeInputValue.SetNewContext();
			}
			if (this.m_indicatorImage != null)
			{
				this.m_indicatorImage.SetNewContext();
			}
			if (this.m_indicatorStates != null)
			{
				this.m_indicatorStates.SetNewContext();
			}
			if (this.m_maximumValue != null)
			{
				this.m_maximumValue.SetNewContext();
			}
			if (this.m_minimumValue != null)
			{
				this.m_minimumValue.SetNewContext();
			}
		}
	}
}
