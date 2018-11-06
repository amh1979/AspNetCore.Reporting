using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageData
	{
		private string m_imageName;

		private string m_imageMimeType;

		private bool m_isShared;

		private long m_imageDataOffset = -1L;

		private byte[] m_stream;

		private GDIImageProps m_gdiImageProps;

		private Rectangle m_offsets = Rectangle.Empty;

		public string ImageName
		{
			get
			{
				return this.m_imageName;
			}
			set
			{
				this.m_imageName = value;
			}
		}

		public string ImageMimeType
		{
			get
			{
				return this.m_imageMimeType;
			}
			set
			{
				this.m_imageMimeType = value;
			}
		}

		public long ImageDataOffset
		{
			get
			{
				return this.m_imageDataOffset;
			}
			set
			{
				this.m_imageDataOffset = value;
			}
		}

		public byte[] ImageData
		{
			get
			{
				return this.m_stream;
			}
			set
			{
				this.m_stream = value;
			}
		}

		public GDIImageProps GDIImageProps
		{
			get
			{
				return this.m_gdiImageProps;
			}
			set
			{
				this.m_gdiImageProps = value;
			}
		}

		public bool IsShared
		{
			get
			{
				return this.m_isShared;
			}
			set
			{
				this.m_isShared = value;
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
	}
}
