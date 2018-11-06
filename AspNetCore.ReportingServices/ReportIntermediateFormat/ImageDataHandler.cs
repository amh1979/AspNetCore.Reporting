using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class ImageDataHandler
	{
		protected readonly IBaseImage m_image;

		protected readonly ReportElement m_reportElement;

		private string m_mimeType;

		private byte[] m_imageData;

		private string m_imageDataId;

		private string m_streamName;

		private List<string> m_fieldsUsedInValue;

		private bool m_isNullImage;

		private bool m_isCachePopulated;

		private static readonly byte[] TransparentImageBytes = new byte[43]
		{
			71,
			73,
			70,
			56,
			57,
			97,
			1,
			0,
			1,
			0,
			240,
			0,
			0,
			219,
			223,
			239,
			0,
			0,
			0,
			33,
			249,
			4,
			1,
			0,
			0,
			0,
			0,
			44,
			0,
			0,
			0,
			0,
			1,
			0,
			1,
			0,
			0,
			2,
			2,
			68,
			1,
			0,
			59
		};

		private static readonly string TransparentImageMimeType = "image/gif";

		public abstract AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get;
		}

		public string MIMEType
		{
			get
			{
				this.EnsureCacheIsPopulated();
				return this.m_mimeType;
			}
		}

		public byte[] ImageData
		{
			get
			{
				this.EnsureCacheIsPopulated();
				if (this.m_imageData == null && this.m_imageDataId != null)
				{
					this.m_imageData = this.LoadExistingImageData(this.m_imageDataId);
					if (this.m_imageData == null)
					{
						this.m_reportElement.RenderingContext.OdpContext.ErrorContext.Register(this.ErrorCodeForSourceType, Severity.Warning, this.m_image.ObjectType, this.m_image.ObjectName, this.m_image.ImageDataPropertyName, this.m_imageDataId);
					}
					this.m_imageDataId = null;
				}
				return this.m_imageData;
			}
		}

		public string ImageDataId
		{
			get
			{
				if (this.m_imageDataId == null)
				{
					this.m_imageDataId = this.GetImageDataId();
				}
				return this.m_imageDataId;
			}
		}

		public string StreamName
		{
			get
			{
				this.EnsureCacheIsPopulated();
				return this.m_streamName;
			}
		}

		public List<string> FieldsUsedInValue
		{
			get
			{
				this.EnsureCacheIsPopulated();
				return this.m_fieldsUsedInValue;
			}
		}

		public bool IsNullImage
		{
			get
			{
				if (this.GetIsNullImage())
				{
					return true;
				}
				this.EnsureCacheIsPopulated();
				return this.m_isNullImage;
			}
		}

		protected abstract ProcessingErrorCode ErrorCodeForSourceType
		{
			get;
		}

		protected ImageCacheManager CacheManager
		{
			get
			{
				return this.m_reportElement.RenderingContext.OdpContext.ImageCacheManager;
			}
		}

		public ImageDataHandler(ReportElement reportElement, IBaseImage image)
		{
			this.m_reportElement = reportElement;
			this.m_image = image;
		}

		private bool GetIsNullImage()
		{
			if (string.IsNullOrEmpty(this.m_image.Value.ExpressionString))
			{
				return true;
			}
			return false;
		}

		public void ClearCache()
		{
			this.m_mimeType = null;
			this.m_imageData = null;
			this.m_imageDataId = null;
			this.m_streamName = null;
			this.m_fieldsUsedInValue = null;
			this.m_isNullImage = false;
			this.m_isCachePopulated = false;
		}

		private void EnsureCacheIsPopulated()
		{
			if (!this.m_isCachePopulated)
			{
				this.m_isCachePopulated = true;
				this.m_streamName = this.GetCalculatedImageProperties(out this.m_mimeType, out this.m_imageData, out this.m_imageDataId, out this.m_fieldsUsedInValue, out this.m_isNullImage);
			}
		}

		private string GetCalculatedImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			if (this.GetIsNullImage())
			{
				fieldsUsedInValue = null;
				imageDataId = null;
				isNullImage = true;
				return this.m_image.GetTransparentImageProperties(out mimeType, out imageData);
			}
			return this.CalculateImageProperties(out mimeType, out imageData, out imageDataId, out fieldsUsedInValue, out isNullImage);
		}

		protected abstract string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage);

		protected virtual string GetImageDataId()
		{
			this.EnsureCacheIsPopulated();
			return this.m_imageDataId;
		}

		protected abstract byte[] LoadExistingImageData(string imageDataId);

		protected string GetTransparentImageProperties(out string mimeType, out byte[] imageData, out string imageDataId)
		{
			imageDataId = null;
			return this.m_image.GetTransparentImageProperties(out mimeType, out imageData);
		}

		protected string GetErrorImageProperties(out string mimeType, out byte[] imageData, out string imageDataId)
		{
			mimeType = null;
			imageData = null;
			imageDataId = null;
			return null;
		}

		public string LoadAndCacheTransparentImage(out string mimeType, out byte[] imageData)
		{
			imageData = new byte[ImageDataHandler.TransparentImageBytes.Length];
			Array.Copy(ImageDataHandler.TransparentImageBytes, imageData, imageData.Length);
			mimeType = ImageDataHandler.TransparentImageMimeType;
			return this.CacheManager.EnsureTransparentImageIsCached(mimeType, imageData);
		}
	}
}
