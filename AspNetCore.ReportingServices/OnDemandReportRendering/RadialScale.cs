using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScale : GaugeScale
	{
		private RadialPointerCollection m_gaugePointers;

		private ReportDoubleProperty m_radius;

		private ReportDoubleProperty m_startAngle;

		private ReportDoubleProperty m_sweepAngle;

		public RadialPointerCollection GaugePointers
		{
			get
			{
				if (this.m_gaugePointers == null && this.RadialScaleDef.GaugePointers != null)
				{
					this.m_gaugePointers = new RadialPointerCollection(this, base.m_gaugePanel);
				}
				return this.m_gaugePointers;
			}
		}

		public ReportDoubleProperty Radius
		{
			get
			{
				if (this.m_radius == null && this.RadialScaleDef.Radius != null)
				{
					this.m_radius = new ReportDoubleProperty(this.RadialScaleDef.Radius);
				}
				return this.m_radius;
			}
		}

		public ReportDoubleProperty StartAngle
		{
			get
			{
				if (this.m_startAngle == null && this.RadialScaleDef.StartAngle != null)
				{
					this.m_startAngle = new ReportDoubleProperty(this.RadialScaleDef.StartAngle);
				}
				return this.m_startAngle;
			}
		}

		public ReportDoubleProperty SweepAngle
		{
			get
			{
				if (this.m_sweepAngle == null && this.RadialScaleDef.SweepAngle != null)
				{
					this.m_sweepAngle = new ReportDoubleProperty(this.RadialScaleDef.SweepAngle);
				}
				return this.m_sweepAngle;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale RadialScaleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale)base.m_defObject;
			}
		}

		public new RadialScaleInstance Instance
		{
			get
			{
				return (RadialScaleInstance)this.GetInstance();
			}
		}

		internal RadialScale(AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override GaugeScaleInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new RadialScaleInstance(this);
			}
			return (GaugeScaleInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_gaugePointers != null)
			{
				this.m_gaugePointers.SetNewContext();
			}
		}
	}
}
