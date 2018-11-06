using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorState : GaugePanelObjectCollectionItem
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState m_defObject;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ReportColorProperty m_color;

		private ReportDoubleProperty m_scaleFactor;

		private ReportEnumProperty<GaugeStateIndicatorStyles> m_indicatorStyle;

		private IndicatorImage m_indicatorImage;

		public string Name
		{
			get
			{
				return this.m_defObject.Name;
			}
		}

		public GaugeInputValue StartValue
		{
			get
			{
				if (this.m_startValue == null && this.m_defObject.StartValue != null)
				{
					this.m_startValue = new GaugeInputValue(this.m_defObject.StartValue, this.m_gaugePanel);
				}
				return this.m_startValue;
			}
		}

		public GaugeInputValue EndValue
		{
			get
			{
				if (this.m_endValue == null && this.m_defObject.EndValue != null)
				{
					this.m_endValue = new GaugeInputValue(this.m_defObject.EndValue, this.m_gaugePanel);
				}
				return this.m_endValue;
			}
		}

		public ReportColorProperty Color
		{
			get
			{
				if (this.m_color == null && this.m_defObject.Color != null)
				{
					ExpressionInfo color = this.m_defObject.Color;
					if (color != null)
					{
						this.m_color = new ReportColorProperty(color.IsExpression, this.m_defObject.Color.OriginalText, color.IsExpression ? null : new ReportColor(color.StringValue.Trim(), true), color.IsExpression ? new ReportColor("", System.Drawing.Color.Empty, true) : null);
					}
				}
				return this.m_color;
			}
		}

		public ReportDoubleProperty ScaleFactor
		{
			get
			{
				if (this.m_scaleFactor == null && this.m_defObject.ScaleFactor != null)
				{
					this.m_scaleFactor = new ReportDoubleProperty(this.m_defObject.ScaleFactor);
				}
				return this.m_scaleFactor;
			}
		}

		public ReportEnumProperty<GaugeStateIndicatorStyles> IndicatorStyle
		{
			get
			{
				if (this.m_indicatorStyle == null && this.m_defObject.IndicatorStyle != null)
				{
					this.m_indicatorStyle = new ReportEnumProperty<GaugeStateIndicatorStyles>(this.m_defObject.IndicatorStyle.IsExpression, this.m_defObject.IndicatorStyle.OriginalText, EnumTranslator.TranslateGaugeStateIndicatorStyles(this.m_defObject.IndicatorStyle.StringValue, null));
				}
				return this.m_indicatorStyle;
			}
		}

		public IndicatorImage IndicatorImage
		{
			get
			{
				if (this.m_indicatorImage == null && this.m_defObject.IndicatorImage != null)
				{
					this.m_indicatorImage = new IndicatorImage(this.m_defObject.IndicatorImage, this.m_gaugePanel);
				}
				return this.m_indicatorImage;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState IndicatorStateDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public IndicatorStateInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new IndicatorStateInstance(this);
				}
				return (IndicatorStateInstance)base.m_instance;
			}
		}

		internal IndicatorState(AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorState defObject, GaugePanel gaugePanel)
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
			if (this.m_startValue != null)
			{
				this.m_startValue.SetNewContext();
			}
			if (this.m_endValue != null)
			{
				this.m_endValue.SetNewContext();
			}
			if (this.m_indicatorImage != null)
			{
				this.m_indicatorImage.SetNewContext();
			}
		}
	}
}
