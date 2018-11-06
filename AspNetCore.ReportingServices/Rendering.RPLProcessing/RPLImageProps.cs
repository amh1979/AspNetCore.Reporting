namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageProps : RPLItemProps
	{
		private RPLImageData m_image;

		private RPLActionInfo m_actionInfo;

		private RPLActionInfoWithImageMap[] m_actionImageMapAreas;

		public RPLImageData Image
		{
			get
			{
				return this.m_image;
			}
			set
			{
				this.m_image = value;
			}
		}

		public RPLActionInfoWithImageMap[] ActionImageMapAreas
		{
			get
			{
				return this.m_actionImageMapAreas;
			}
			set
			{
				this.m_actionImageMapAreas = value;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return this.m_actionInfo;
			}
			set
			{
				this.m_actionInfo = value;
			}
		}

		internal RPLImageProps()
		{
		}
	}
}
