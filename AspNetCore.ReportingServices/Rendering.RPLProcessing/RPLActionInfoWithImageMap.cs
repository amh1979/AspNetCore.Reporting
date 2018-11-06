namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLActionInfoWithImageMap : RPLActionInfo
	{
		private RPLImageMapCollection m_imageMaps;

		public RPLImageMapCollection ImageMaps
		{
			get
			{
				return this.m_imageMaps;
			}
			set
			{
				this.m_imageMaps = value;
			}
		}

		internal RPLActionInfoWithImageMap()
		{
		}

		internal RPLActionInfoWithImageMap(int actionCount)
			: base(actionCount)
		{
		}
	}
}
