namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class CachedSharedImageInfo
	{
		private string m_streamName;

		private ItemBoundaries m_itemBoundaries;

		internal string StreamName
		{
			get
			{
				return this.m_streamName;
			}
		}

		internal ItemBoundaries ImageBounderies
		{
			get
			{
				return this.m_itemBoundaries;
			}
		}

		internal CachedSharedImageInfo(string streamName, ItemBoundaries itemBoundaries)
		{
			this.m_streamName = streamName;
			this.m_itemBoundaries = itemBoundaries;
		}
	}
}
