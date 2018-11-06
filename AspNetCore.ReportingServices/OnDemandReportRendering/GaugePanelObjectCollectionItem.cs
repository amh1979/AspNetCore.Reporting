namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelObjectCollectionItem
	{
		protected BaseInstance m_instance;

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
