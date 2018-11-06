namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class NumericIndicatorInstance : GaugePanelItemInstance
	{
		private NumericIndicator m_defObject;

		internal NumericIndicatorInstance(NumericIndicator defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
