using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Thermometer : IROMStyleDefinitionContainer
	{
		private GaugePanel m_gaugePanel;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer m_defObject;

		private ThermometerInstance m_instance;

		private Style m_style;

		private ReportDoubleProperty m_bulbOffset;

		private ReportDoubleProperty m_bulbSize;

		private ReportEnumProperty<GaugeThermometerStyles> m_thermometerStyle;

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

		public ReportDoubleProperty BulbOffset
		{
			get
			{
				if (this.m_bulbOffset == null && this.m_defObject.BulbOffset != null)
				{
					this.m_bulbOffset = new ReportDoubleProperty(this.m_defObject.BulbOffset);
				}
				return this.m_bulbOffset;
			}
		}

		public ReportDoubleProperty BulbSize
		{
			get
			{
				if (this.m_bulbSize == null && this.m_defObject.BulbSize != null)
				{
					this.m_bulbSize = new ReportDoubleProperty(this.m_defObject.BulbSize);
				}
				return this.m_bulbSize;
			}
		}

		public ReportEnumProperty<GaugeThermometerStyles> ThermometerStyle
		{
			get
			{
				if (this.m_thermometerStyle == null && this.m_defObject.ThermometerStyle != null)
				{
					this.m_thermometerStyle = new ReportEnumProperty<GaugeThermometerStyles>(this.m_defObject.ThermometerStyle.IsExpression, this.m_defObject.ThermometerStyle.OriginalText, EnumTranslator.TranslateGaugeThermometerStyles(this.m_defObject.ThermometerStyle.StringValue, null));
				}
				return this.m_thermometerStyle;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer ThermometerDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public ThermometerInstance Instance
		{
			get
			{
				if (this.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ThermometerInstance(this);
				}
				return this.m_instance;
			}
		}

		internal Thermometer(AspNetCore.ReportingServices.ReportIntermediateFormat.Thermometer defObject, GaugePanel gaugePanel)
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
