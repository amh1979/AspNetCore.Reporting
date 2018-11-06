namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapObjectCollectionItem : IMapObjectCollectionItem
	{
		protected BaseInstance m_instance;

		void IMapObjectCollectionItem.SetNewContext()
		{
			this.SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
