using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeTickMarks : TickMarkStyle
	{
		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		public ReportDoubleProperty Interval
		{
			get
			{
				if (this.m_interval == null && this.GaugeTickMarksDef.Interval != null)
				{
					this.m_interval = new ReportDoubleProperty(this.GaugeTickMarksDef.Interval);
				}
				return this.m_interval;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (this.m_intervalOffset == null && this.GaugeTickMarksDef.IntervalOffset != null)
				{
					this.m_intervalOffset = new ReportDoubleProperty(this.GaugeTickMarksDef.IntervalOffset);
				}
				return this.m_intervalOffset;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks GaugeTickMarksDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)base.m_defObject;
			}
		}

		public new GaugeTickMarksInstance Instance
		{
			get
			{
				if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = this.GetInstance();
				}
				return (GaugeTickMarksInstance)base.m_instance;
			}
		}

		internal GaugeTickMarks(AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}

		protected override TickMarkStyleInstance GetInstance()
		{
			return new GaugeTickMarksInstance(this);
		}
	}
}
