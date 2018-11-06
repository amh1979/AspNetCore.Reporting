namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartObjectCollectionItem<T> where T : BaseInstance
	{
		protected T m_instance;

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
