using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class InternalImage : ImageBase
	{
		private Image.SourceType m_imageType;

		private object m_valueObject;

		private RenderingContext m_renderingContext;

		private bool m_transparent;

		private static byte[] m_transparentImage;

		private byte[] m_imageData;

		private string m_MIMEType;

		private WeakReference m_imageDataRef;

		private string m_streamName;

		private ImageMapAreaInstanceList m_imageMapAreas;

		internal byte[] ImageData
		{
			get
			{
				if (this.m_imageData != null)
				{
					return this.m_imageData;
				}
				byte[] array = (this.m_imageDataRef != null) ? ((byte[])this.m_imageDataRef.Target) : null;
				if (array == null)
				{
					ImageInfo imageInfo = null;
					switch (this.m_imageType)
					{
					case Image.SourceType.External:
					{
						string imageValue2 = this.ImageValue;
						if (imageValue2 != null)
						{
							imageInfo = this.m_renderingContext.ImageStreamNames[imageValue2];
						}
						break;
					}
					case Image.SourceType.Embedded:
					{
						string imageValue = this.ImageValue;
						if (imageValue != null && this.m_renderingContext.EmbeddedImages != null)
						{
							imageInfo = this.m_renderingContext.EmbeddedImages[imageValue];
						}
						break;
					}
					}
					if (imageInfo != null && imageInfo.ImageDataRef != null)
					{
						this.m_imageDataRef = imageInfo.ImageDataRef;
						array = (byte[])this.m_imageDataRef.Target;
					}
					if (array == null)
					{
						string mIMEType = default(string);
						this.GetImageData(out array, out mIMEType);
						if (this.m_renderingContext.CacheState)
						{
							this.m_imageData = array;
							this.m_MIMEType = mIMEType;
						}
						else
						{
							this.m_imageDataRef = new WeakReference(array);
							if (imageInfo != null)
							{
								imageInfo.ImageDataRef = this.m_imageDataRef;
							}
						}
					}
				}
				return array;
			}
		}

		internal string MIMEType
		{
			get
			{
				string mIMEType = this.m_MIMEType;
				if (mIMEType == null)
				{
					this.GetImageMimeType(out mIMEType);
					if (this.m_renderingContext.CacheState)
					{
						this.m_MIMEType = mIMEType;
					}
				}
				return mIMEType;
			}
		}

		internal string StreamName
		{
			get
			{
				string streamName = this.m_streamName;
				if (streamName == null)
				{
					string text = null;
					this.GetImageInfo(out streamName, out text);
					if (this.m_renderingContext.CacheState)
					{
						this.m_streamName = streamName;
					}
				}
				return streamName;
			}
		}

		internal ImageMapAreaInstanceList ImageMapAreaInstances
		{
			get
			{
				return this.m_imageMapAreas;
			}
		}

		private string ImageValue
		{
			get
			{
				return this.m_valueObject as string;
			}
		}

		private ImageData Data
		{
			get
			{
				return this.m_valueObject as ImageData;
			}
		}

		internal static byte[] TransparentImage
		{
			get
			{
				if (InternalImage.m_transparentImage == null)
				{
					MemoryStream memoryStream = new MemoryStream(45);
					AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.RuntimeRICollection.FetchTransparentImage(memoryStream);
					InternalImage.m_transparentImage = memoryStream.ToArray();
				}
				return InternalImage.m_transparentImage;
			}
		}

		internal InternalImage(Image.SourceType imgType, string mimeType, object valueObject, RenderingContext rc)
		{
			this.m_imageType = imgType;
			this.m_MIMEType = mimeType;
			this.m_valueObject = valueObject;
			this.m_renderingContext = rc;
			this.m_transparent = false;
		}

		internal InternalImage(Image.SourceType imgType, string mimeType, object valueObject, RenderingContext rc, bool brokenImage, ImageMapAreaInstanceList imageMapAreas)
		{
			this.m_imageType = imgType;
			this.m_MIMEType = mimeType;
			this.m_valueObject = valueObject;
			this.m_renderingContext = rc;
			this.m_transparent = (!brokenImage && null == valueObject);
			if (!brokenImage)
			{
				this.m_imageMapAreas = imageMapAreas;
			}
		}

		private static void ReadStream(Stream input, out byte[] streamContents)
		{
			if (input == null)
			{
				streamContents = null;
			}
			else
			{
				int num = 1024;
				using (MemoryStream memoryStream = new MemoryStream(num))
				{
					byte[] buffer = new byte[num];
					int num2 = 0;
					while ((num2 = input.Read(buffer, 0, num)) > 0)
					{
						memoryStream.Write(buffer, 0, num2);
					}
					streamContents = memoryStream.ToArray();
				}
			}
		}

		private void GetImageData(out byte[] imageData, out string mimeType)
		{
			if (this.m_transparent)
			{
				mimeType = "image/gif";
				imageData = InternalImage.TransparentImage;
			}
			else
			{
				string streamName = this.StreamName;
				if (streamName != null)
				{
					using (Stream input = this.m_renderingContext.GetChunkCallback(streamName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, out mimeType))
					{
						InternalImage.ReadStream(input, out imageData);
					}
				}
				else
				{
					imageData = null;
					mimeType = null;
					string urlString = this.GetUrlString();
					if (urlString == null)
					{
						ImageData data = this.Data;
						if (data != null)
						{
							imageData = data.Data;
							mimeType = data.MIMEType;
						}
					}
				}
			}
		}

		private void GetImageInfo(out string streamName, out string mimeType)
		{
			streamName = null;
			mimeType = null;
			switch (this.m_imageType)
			{
			case Image.SourceType.External:
			{
				string imageValue2 = this.ImageValue;
				if (imageValue2 != null)
				{
					ImageInfo imageInfo2 = this.m_renderingContext.ImageStreamNames[imageValue2];
					if (imageInfo2 != null)
					{
						streamName = imageInfo2.StreamName;
						mimeType = imageInfo2.MimeType;
					}
				}
				break;
			}
			case Image.SourceType.Embedded:
			{
				string imageValue = this.ImageValue;
				if (imageValue != null && this.m_renderingContext.EmbeddedImages != null)
				{
					ImageInfo imageInfo = this.m_renderingContext.EmbeddedImages[imageValue];
					if (imageInfo != null)
					{
						streamName = imageInfo.StreamName;
						mimeType = imageInfo.MimeType;
					}
				}
				break;
			}
			case Image.SourceType.Database:
				streamName = this.ImageValue;
				break;
			}
		}

		private void GetImageMimeType(out string mimeType)
		{
			if (this.m_transparent)
			{
				mimeType = "image/gif";
			}
			else
			{
				string text = null;
				this.GetImageInfo(out text, out mimeType);
				if (mimeType == null)
				{
					if (text != null)
					{
						mimeType = this.m_renderingContext.GetChunkMimeType(text, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
					}
					else
					{
						mimeType = null;
						string urlString = this.GetUrlString();
						if (urlString == null)
						{
							ImageData data = this.Data;
							if (data != null)
							{
								mimeType = data.MIMEType;
							}
						}
					}
				}
			}
		}

		private string GetUrlString()
		{
			if (this.m_imageType != 0)
			{
				return null;
			}
			return this.ImageValue;
		}
	}
}
