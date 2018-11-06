using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointer : GaugePointer
	{
		private ReportEnumProperty<LinearPointerTypes> m_type;

		private Thermometer m_thermometer;

		public ReportEnumProperty<LinearPointerTypes> Type
		{
			get
			{
				if (this.m_type == null && this.LinearPointerDef.Type != null)
				{
					this.m_type = new ReportEnumProperty<LinearPointerTypes>(this.LinearPointerDef.Type.IsExpression, this.LinearPointerDef.Type.OriginalText, EnumTranslator.TranslateLinearPointerTypes(this.LinearPointerDef.Type.StringValue, null));
				}
				return this.m_type;
			}
		}

		public Thermometer Thermometer
		{
			get
			{
				if (this.m_thermometer == null && this.LinearPointerDef.Thermometer != null)
				{
					this.m_thermometer = new Thermometer(this.LinearPointerDef.Thermometer, base.m_gaugePanel);
				}
				return this.m_thermometer;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer LinearPointerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer)base.m_defObject;
			}
		}

		public new LinearPointerInstance Instance
		{
			get
			{
				return (LinearPointerInstance)this.GetInstance();
			}
		}

		internal LinearPointer(AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			base.m_defObject = defObject;
			base.m_gaugePanel = gaugePanel;
		}

		internal override GaugePointerInstance GetInstance()
		{
			if (base.m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new LinearPointerInstance(this);
			}
			return (GaugePointerInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_thermometer != null)
			{
				this.m_thermometer.SetNewContext();
			}
		}
	}
}
