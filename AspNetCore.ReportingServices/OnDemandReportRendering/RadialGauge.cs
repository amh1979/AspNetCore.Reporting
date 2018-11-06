using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGauge : Gauge
	{
		private RadialScaleCollection m_gaugeScales;

		private ReportDoubleProperty m_pivotX;

		private ReportDoubleProperty m_pivotY;

		public RadialScaleCollection GaugeScales
		{
			get
			{
				if (this.m_gaugeScales == null && this.RadialGaugeDef.GaugeScales != null)
				{
					this.m_gaugeScales = new RadialScaleCollection(this, base.m_gaugePanel);
				}
				return this.m_gaugeScales;
			}
		}

		public ReportDoubleProperty PivotX
		{
			get
			{
				if (this.m_pivotX == null && this.RadialGaugeDef.PivotX != null)
				{
					this.m_pivotX = new ReportDoubleProperty(this.RadialGaugeDef.PivotX);
				}
				return this.m_pivotX;
			}
		}

		public ReportDoubleProperty PivotY
		{
			get
			{
				if (this.m_pivotY == null && this.RadialGaugeDef.PivotY != null)
				{
					this.m_pivotY = new ReportDoubleProperty(this.RadialGaugeDef.PivotY);
				}
				return this.m_pivotY;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge RadialGaugeDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge)base.m_defObject;
			}
		}

		public new RadialGaugeInstance Instance
		{
			get
			{
				return (RadialGaugeInstance)this.GetInstance();
			}
		}

		internal RadialGauge(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge defObject, GaugePanel gaugePanel)
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
				base.m_instance = new RadialGaugeInstance(this);
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
