using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLDynamicImageProps : RPLItemProps
	{
		private string m_streamName;

		private long m_dynamicImageContentOffset = -1L;

		private Stream m_dynamicImageContent;

		private RPLActionInfoWithImageMap[] m_actionImageMaps;

		private Rectangle m_offsets = Rectangle.Empty;

		public string StreamName
		{
			get
			{
				return this.m_streamName;
			}
			set
			{
				this.m_streamName = value;
			}
		}

		public long DynamicImageContentOffset
		{
			get
			{
				return this.m_dynamicImageContentOffset;
			}
			set
			{
				this.m_dynamicImageContentOffset = value;
			}
		}

		public Stream DynamicImageContent
		{
			get
			{
				return this.m_dynamicImageContent;
			}
			set
			{
				this.m_dynamicImageContent = value;
			}
		}

		public RPLActionInfoWithImageMap[] ActionImageMapAreas
		{
			get
			{
				return this.m_actionImageMaps;
			}
			set
			{
				this.m_actionImageMaps = value;
			}
		}

		public Rectangle ImageConsolidationOffsets
		{
			get
			{
				return this.m_offsets;
			}
			set
			{
				this.m_offsets = value;
			}
		}

		internal RPLDynamicImageProps()
		{
		}
	}
}
