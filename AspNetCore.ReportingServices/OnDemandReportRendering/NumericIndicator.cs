using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicator : GaugePanelItem
	{
		internal AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator NumericIndicatorDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator)base.m_defObject;
			}
		}

		public new NumericIndicatorInstance Instance
		{
			get
			{
				return (NumericIndicatorInstance)this.GetInstance();
			}
		}

		internal NumericIndicator(AspNetCore.ReportingServices.ReportIntermediateFormat.NumericIndicator defObject, GaugePanel gaugePanel)
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
				base.m_instance = new NumericIndicatorInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
		}
	}
}
