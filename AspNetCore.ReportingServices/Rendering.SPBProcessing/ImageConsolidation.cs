using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class ImageConsolidation
	{
		protected const float SUPPORTED_DPI = 96f;

		protected const float DPI_TOLERANCE = 0.02f;

		private const float OUTPUT_DPI = 96f;

		public static string STREAMPREFIX = "IMGCON_";

		public List<ImageInfo> ImageInfos = new List<ImageInfo>();

		public int MaxHeight;

		public int MaxWidth;

		public int CurrentOffset;

		private static int MAXIMAGECONSOLIDATION_TOTALSIZE = 200000;

		private static int MAXIMAGECONSOLIDATION_PERIMAGESIZE = ImageConsolidation.MAXIMAGECONSOLIDATION_TOTALSIZE / 10;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private int m_currentByteCount;

		private int m_ignoreOffsetTill = -1;

		private string m_imagePrefix;

		public int IgnoreOffsetTill
		{
			get
			{
				return this.m_ignoreOffsetTill;
			}
		}

		public ImageConsolidation(CreateAndRegisterStream createAndRegisterStream)
			: this(createAndRegisterStream, -1)
		{
		}

		public ImageConsolidation(CreateAndRegisterStream createAndRegisterStream, int ignoreOffsetTill)
		{
			this.m_createAndRegisterStream = createAndRegisterStream;
			this.m_ignoreOffsetTill = ignoreOffsetTill;
		}

		public System.Drawing.Rectangle AppendImage(Stream imageStream)
		{
			if (imageStream == null)
			{
				return System.Drawing.Rectangle.Empty;
			}
			long length = imageStream.Length;
			if (length > ImageConsolidation.MAXIMAGECONSOLIDATION_PERIMAGESIZE)
			{
				return System.Drawing.Rectangle.Empty;
			}
			if (this.m_currentByteCount + length > ImageConsolidation.MAXIMAGECONSOLIDATION_TOTALSIZE)
			{
				this.RenderToStream();
				if (this.m_ignoreOffsetTill > -1 && this.m_ignoreOffsetTill + 1 == this.CurrentOffset)
				{
					return System.Drawing.Rectangle.Empty;
				}
			}
			ImageInfo imageInfo = new ImageInfo();
			imageInfo.ImageData = imageStream;
			long position = imageStream.Position;
			float dpiX = 0f;
			float dpiY = 0f;
			int num = 1;
			ImageFormat imageFormat = ImageFormat.Png;
			try
			{
				using (System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream))
				{
					imageInfo.Width = image.Width;
					imageInfo.Height = image.Height;
					dpiX = image.HorizontalResolution;
					dpiY = image.VerticalResolution;
					imageFormat = image.RawFormat;
					num = image.FrameDimensionsList.Length;
					if (num == 1)
					{
						num = image.GetFrameCount(new FrameDimension(image.FrameDimensionsList[0]));
					}
				}
			}
			catch (Exception)
			{
				return System.Drawing.Rectangle.Empty;
			}
			if (this.IsDPISupported(dpiX, dpiY) && num == 1 && !(imageFormat.Guid != ImageFormat.Png.Guid))
			{
				System.Drawing.Rectangle result = System.Drawing.Rectangle.Empty;
				if (this.CurrentOffset >= this.m_ignoreOffsetTill)
				{
					this.ImageInfos.Add(imageInfo);
					imageStream.Position = position;
					result = new System.Drawing.Rectangle(0, this.MaxHeight, imageInfo.Width, imageInfo.Height);
					this.MaxHeight += imageInfo.Height;
					this.MaxWidth = Math.Max(this.MaxWidth, imageInfo.Width);
				}
				this.m_currentByteCount += (int)length;
				return result;
			}
			return System.Drawing.Rectangle.Empty;
		}

		public System.Drawing.Image Render()
		{
			if (this.ImageInfos.Count != 0 && this.MaxWidth != 0 && this.MaxHeight != 0)
			{
				Bitmap bitmap = new Bitmap(this.MaxWidth, this.MaxHeight);
				if (bitmap.HorizontalResolution != 96.0 || bitmap.VerticalResolution != 96.0)
				{
					bitmap.SetResolution(96f, 96f);
				}
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					int num = 0;
					foreach (ImageInfo imageInfo in this.ImageInfos)
					{
						imageInfo.RenderAndDispose(g, 0, num);
						num += imageInfo.Height;
					}
				}
				this.ImageInfos.Clear();
				return bitmap;
			}
			return null;
		}

		public static string GetStreamName(string reportName, int page)
		{
			if (page > 0)
			{
				return ImageConsolidation.STREAMPREFIX + page.ToString(CultureInfo.InvariantCulture);
			}
			return ImageConsolidation.STREAMPREFIX;
		}

		public string GetStreamName()
		{
			return this.m_imagePrefix + this.CurrentOffset;
		}

		public void SetName(string reportName, int pageNumber)
		{
			this.m_imagePrefix = ImageConsolidation.STREAMPREFIX + pageNumber.ToString(CultureInfo.InvariantCulture) + "_";
		}

		public void RenderToStream()
		{
			if (this.m_currentByteCount > 0 && this.ImageInfos.Count > 0)
			{
				string streamName = this.GetStreamName();
				Stream stream = this.m_createAndRegisterStream(streamName, "png", null, PageContext.PNG_MIME_TYPE, false, StreamOper.CreateAndRegister);
				using (System.Drawing.Image image = this.Render())
				{
					if (image != null)
					{
						image.Save(stream, ImageFormat.Png);
					}
				}
			}
			this.CurrentOffset++;
			this.m_currentByteCount = 0;
			this.MaxHeight = 0;
			this.MaxWidth = 0;
		}

		public void ResetCancelPage()
		{
			if (this.CurrentOffset > 0 && this.m_ignoreOffsetTill < this.CurrentOffset)
			{
				this.m_ignoreOffsetTill = this.CurrentOffset;
			}
			this.CurrentOffset = 0;
			foreach (ImageInfo imageInfo in this.ImageInfos)
			{
				imageInfo.Dispose();
			}
			this.ImageInfos.Clear();
			this.m_currentByteCount = 0;
			this.MaxHeight = 0;
			this.MaxWidth = 0;
		}

		public void Reset()
		{
			this.CurrentOffset = 0;
		}

		private bool IsDPISupported(float dpiX, float dpiY)
		{
			if (95.9800033569336 < dpiX && 96.0199966430664 > dpiX && 95.9800033569336 < dpiY)
			{
				return 96.0199966430664 > dpiY;
			}
			return false;
		}
	}
}
