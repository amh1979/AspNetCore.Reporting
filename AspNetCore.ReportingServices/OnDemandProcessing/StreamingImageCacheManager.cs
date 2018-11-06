using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class StreamingImageCacheManager : ImageCacheManager
	{
		internal StreamingImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
			: base(odpMetadata, chunkFactory)
		{
		}

		protected override bool ExtractCachedExternalImagePropertiesIfValid(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName)
		{
			imageData = imageInfo.GetCachedImageData();
			if (imageData != null)
			{
				streamName = imageInfo.StreamName;
				mimeType = imageInfo.MimeType;
				return true;
			}
			streamName = null;
			mimeType = null;
			return false;
		}

		public override string AddExternalImage(string value, byte[] imageData, string mimeType)
		{
			Global.Tracer.Assert(imageData != null, "Missing imageData for external image");
			string text = ImageHelper.GenerateImageStreamName();
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo);
			if (!base.m_odpMetadata.TryGetExternalImage(value, out imageInfo))
			{
				imageInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo(text, mimeType);
				base.m_odpMetadata.AddExternalImage(value, imageInfo);
			}
			imageInfo.SetCachedImageData(imageData);
			return text;
		}

		public override byte[] GetCachedExternalImageBytes(string value)
		{
			Global.Tracer.Assert(false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override bool TryGetEmbeddedImage(string value, AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName)
		{
			if (embeddingMode == AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Package)
			{
				imageData = null;
				mimeType = null;
				streamName = null;
				return true;
			}
			Global.Tracer.Assert(false, "Embedded Images are not supported in Streaming ODP mode.");
			throw new InvalidOperationException("Embedded Images are not supported in Streaming ODP mode.");
		}

		public override byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override bool TryGetDatabaseImage(string uniqueName, out string streamName)
		{
			streamName = null;
			return false;
		}

		public override string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext)
		{
			return ImageHelper.GenerateImageStreamName();
		}

		public override byte[] GetCachedDatabaseImageBytes(string chunkName)
		{
			Global.Tracer.Assert(false, "Chunks are not supported in this processing mode.");
			throw new InvalidOperationException("Chunks are not supported in this processing mode.");
		}

		public override string EnsureTransparentImageIsCached(string mimeType, byte[] imageData)
		{
			return ImageHelper.GenerateImageStreamName();
		}
	}
}
