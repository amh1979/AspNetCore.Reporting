using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScalePin : TickMarkStyle
	{
		private ReportDoubleProperty m_location;

		private ReportBoolProperty m_enable;

		private PinLabel m_pinLabel;

		public ReportDoubleProperty Location
		{
			get
			{
				if (this.m_location == null && this.ScalePinDef.Location != null)
				{
					this.m_location = new ReportDoubleProperty(this.ScalePinDef.Location);
				}
				return this.m_location;
			}
		}

		public ReportBoolProperty Enable
		{
			get
			{
				if (this.m_enable == null && this.ScalePinDef.Enable != null)
				{
					this.m_enable = new ReportBoolProperty(this.ScalePinDef.Enable);
				}
				return this.m_enable;
			}
		}

		public PinLabel PinLabel
		{
			get
			{
				if (this.m_pinLabel == null && this.ScalePinDef.PinLabel != null)
				{
					this.m_pinLabel = new PinLabel(this.ScalePinDef.PinLabel, base.m_gaugePanel);
				}
				return this.m_pinLabel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin ScalePinDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin)base.m_defObject;
			}
		}

		public new ScalePinInstance Instance
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
				return (ScalePinInstance)base.m_instance;
			}
		}

		internal ScalePin(AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin defObject, GaugePanel gaugePanel)
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
			if (this.m_pinLabel != null)
			{
				this.m_pinLabel.SetNewContext();
			}
		}

		protected override TickMarkStyleInstance GetInstance()
		{
			return new ScalePinInstance(this);
		}
	}
}
