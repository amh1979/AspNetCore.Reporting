using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScale : GaugeScale
	{
		private LinearPointerCollection m_gaugePointers;

		private ReportDoubleProperty m_startMargin;

		private ReportDoubleProperty m_endMargin;

		private ReportDoubleProperty m_position;

		public LinearPointerCollection GaugePointers
		{
			get
			{
				if (this.m_gaugePointers == null && this.LinearScaleDef.GaugePointers != null)
				{
					this.m_gaugePointers = new LinearPointerCollection(this, base.m_gaugePanel);
				}
				return this.m_gaugePointers;
			}
		}

		public ReportDoubleProperty StartMargin
		{
			get
			{
				if (this.m_startMargin == null && this.LinearScaleDef.StartMargin != null)
				{
					this.m_startMargin = new ReportDoubleProperty(this.LinearScaleDef.StartMargin);
				}
				return this.m_startMargin;
			}
		}

		public ReportDoubleProperty EndMargin
		{
			get
			{
				if (this.m_endMargin == null && this.LinearScaleDef.EndMargin != null)
				{
					this.m_endMargin = new ReportDoubleProperty(this.LinearScaleDef.EndMargin);
				}
				return this.m_endMargin;
			}
		}

		public ReportDoubleProperty Position
		{
			get
			{
				if (this.m_position == null && this.LinearScaleDef.Position != null)
				{
					this.m_position = new ReportDoubleProperty(this.LinearScaleDef.Position);
				}
				return this.m_position;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale LinearScaleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale)base.m_defObject;
			}
		}

		public new LinearScaleInstance Instance
		{
			get
			{
				return (LinearScaleInstance)this.GetInstance();
			}
		}

		internal LinearScale(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale defObject, GaugePanel gaugePanel)
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
				base.m_instance = new LinearScaleInstance(this);
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
