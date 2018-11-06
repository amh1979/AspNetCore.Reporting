using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class ImageInformation
	{
		internal const string GIFMIMETYPE = "image/gif";

		internal const string JPGMIMETYPE = "image/jpg";

		internal const string JPEGMIMETYPE = "image/jpeg";

		internal const string PNGMIMETYPE = "image/png";

		internal const string BMPMIMETYPE = "image/bmp";

		internal const string XPNGMIMETYPE = "image/x-png";

		private Stream m_imageData;

		private RPLFormat.Sizings m_imageSizings = RPLFormat.Sizings.Fit;

		private ImageFormat m_imageFormat;

		private string m_imageName;

		private int m_width;

		private int m_height;

		private float m_horizontalResolution;

		private float m_verticalResolution;

		private string m_hyperlinkURL;

		private bool m_hyperlinkIsBookmark;

		private PaddingInformation m_paddings;

		internal Stream ImageData
		{
			get
			{
				return this.m_imageData;
			}
			set
			{
				this.m_imageData = value;
			}
		}

		internal string ImageName
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

		internal RPLFormat.Sizings ImageSizings
		{
			get
			{
				return this.m_imageSizings;
			}
			set
			{
				this.m_imageSizings = value;
			}
		}

		internal ImageFormat ImageFormat
		{
			get
			{
				if (this.m_imageFormat == null)
				{
					this.CalculateMetrics();
				}
				return this.m_imageFormat;
			}
			set
			{
				this.m_imageFormat = value;
			}
		}

		internal int Width
		{
			get
			{
				if (this.m_width == 0)
				{
					this.CalculateMetrics();
				}
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal int Height
		{
			get
			{
				if (this.m_height == 0)
				{
					this.CalculateMetrics();
				}
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal float HorizontalResolution
		{
			get
			{
				if (this.m_horizontalResolution == 0.0)
				{
					this.CalculateMetrics();
				}
				return this.m_horizontalResolution;
			}
			set
			{
				this.m_horizontalResolution = value;
			}
		}

		internal float VerticalResolution
		{
			get
			{
				if (this.m_verticalResolution == 0.0)
				{
					this.CalculateMetrics();
				}
				return this.m_verticalResolution;
			}
			set
			{
				this.m_verticalResolution = value;
			}
		}

		internal string HyperlinkURL
		{
			get
			{
				return this.m_hyperlinkURL;
			}
			set
			{
				this.m_hyperlinkURL = value;
			}
		}

		internal bool HyperlinkIsBookmark
		{
			get
			{
				return this.m_hyperlinkIsBookmark;
			}
			set
			{
				this.m_hyperlinkIsBookmark = value;
			}
		}

		internal PaddingInformation Paddings
		{
			get
			{
				return this.m_paddings;
			}
			set
			{
				this.m_paddings = value;
			}
		}

		internal RPLFormat.Sizings Sizings
		{
			set
			{
				this.m_imageSizings = value;
			}
		}

		internal ImageInformation()
		{
		}

		internal void ReadImage(IExcelGenerator excel, RPLImageData image, string imageName, RPLReport report)
		{
			if (excel != null && image != null && report != null && imageName != null)
			{
				this.SetMimeType(image.ImageMimeType);
				this.m_imageName = imageName;
				if (image.ImageData != null)
				{
					this.m_imageData = excel.CreateStream(imageName);
					this.m_imageData.Write(image.ImageData, 0, image.ImageData.Length);
					goto IL_008d;
				}
				if (image.ImageDataOffset > 0)
				{
					this.m_imageData = excel.CreateStream(imageName);
					report.GetImage(image.ImageDataOffset, this.m_imageData);
					goto IL_008d;
				}
				this.m_imageData = null;
				image.ImageData = null;
			}
			return;
			IL_008d:
			image.ImageData = null;
			if (image.GDIImageProps != null)
			{
				this.m_width = image.GDIImageProps.Width;
				this.m_height = image.GDIImageProps.Height;
				this.m_verticalResolution = image.GDIImageProps.VerticalResolution;
				this.m_horizontalResolution = image.GDIImageProps.HorizontalResolution;
				this.m_imageFormat = image.GDIImageProps.RawFormat;
			}
		}

		internal void SetMimeType(string mimeType)
		{
			if (mimeType != null)
			{
				if (mimeType.Equals("image/gif"))
				{
					this.m_imageFormat = ImageFormat.Gif;
				}
				else if (mimeType.Equals("image/png"))
				{
					this.m_imageFormat = ImageFormat.Png;
				}
				else
				{
					if (!mimeType.Equals("image/jpg") && !mimeType.Equals("image/jpeg"))
					{
						if (mimeType.Equals("image/bmp"))
						{
							this.m_imageFormat = ImageFormat.Png;
							return;
						}
						throw new ReportRenderingException(ExcelRenderRes.UnknownImageFormat(mimeType));
					}
					this.m_imageFormat = ImageFormat.Jpeg;
				}
			}
		}

		private void CalculateMetrics()
		{
			if (this.m_imageData != null && 0 != this.m_imageData.Length)
			{
				this.m_imageData.Position = 0L;
				System.Drawing.Image image = System.Drawing.Image.FromStream(this.m_imageData);
				this.m_imageFormat = image.RawFormat;
				this.Width = image.Width;
				this.Height = image.Height;
				this.HorizontalResolution = image.HorizontalResolution;
				this.VerticalResolution = image.VerticalResolution;
				image.Dispose();
			}
		}
	}
}
