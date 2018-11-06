using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGauge : Gauge
	{
		private LinearScaleCollection m_gaugeScales;

		private ReportEnumProperty<GaugeOrientations> m_orientation;

		public LinearScaleCollection GaugeScales
		{
			get
			{
				if (this.m_gaugeScales == null && this.LinearGaugeDef.GaugeScales != null)
				{
					this.m_gaugeScales = new LinearScaleCollection(this, base.m_gaugePanel);
				}
				return this.m_gaugeScales;
			}
		}

		public ReportEnumProperty<GaugeOrientations> Orientation
		{
			get
			{
				if (this.m_orientation == null && this.LinearGaugeDef.Orientation != null)
				{
					this.m_orientation = new ReportEnumProperty<GaugeOrientations>(this.LinearGaugeDef.Orientation.IsExpression, this.LinearGaugeDef.Orientation.OriginalText, EnumTranslator.TranslateGaugeOrientations(this.LinearGaugeDef.Orientation.StringValue, null));
				}
				return this.m_orientation;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge LinearGaugeDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge)base.m_defObject;
			}
		}

		public new LinearGaugeInstance Instance
		{
			get
			{
				return (LinearGaugeInstance)this.GetInstance();
			}
		}

		internal LinearGauge(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge defObject, GaugePanel gaugePanel)
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
				base.m_instance = new LinearGaugeInstance(this);
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
			if (this.m_gaugeScales != null)
			{
				this.m_gaugeScales.SetNewContext();
			}
		}
	}
}
