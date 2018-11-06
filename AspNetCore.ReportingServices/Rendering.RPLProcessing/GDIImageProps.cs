using System.Drawing;
using System.Drawing.Imaging;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class GDIImageProps
	{
		private int m_width;

		private int m_height;

		private float m_horizontalResolution;

		private float m_verticalResolution;

		private ImageFormat m_rawFormat;

		public int Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		public int Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		public float VerticalResolution
		{
			get
			{
				return this.m_verticalResolution;
			}
			set
			{
				this.m_verticalResolution = value;
			}
		}

		public float HorizontalResolution
		{
			get
			{
				return this.m_horizontalResolution;
			}
			set
			{
				this.m_horizontalResolution = value;
			}
		}

		public ImageFormat RawFormat
		{
			get
			{
				return this.m_rawFormat;
			}
			set
			{
				this.m_rawFormat = value;
			}
		}

		internal GDIImageProps()
		{
		}

		public GDIImageProps(Image image)
		{
			this.m_width = image.Width;
			this.m_height = image.Height;
			this.m_horizontalResolution = image.HorizontalResolution;
			this.m_verticalResolution = image.VerticalResolution;
			this.m_rawFormat = image.RawFormat;
		}
	}
}
