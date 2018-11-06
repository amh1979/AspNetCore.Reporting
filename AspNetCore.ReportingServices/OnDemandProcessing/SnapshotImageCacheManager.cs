using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class SnapshotImageCacheManager : ImageCacheManager
	{
		public SnapshotImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
			: base(odpMetadata, chunkFactory)
		{
		}

		protected override bool ExtractCachedExternalImagePropertiesIfValid(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName)
		{
			imageData = imageInfo.GetCachedImageData();
			streamName = imageInfo.StreamName;
			mimeType = imageInfo.MimeType;
			return true;
		}

		public override string AddExternalImage(string value, byte[] imageData, string mimeType)
		{
			Global.Tracer.Assert(imageData != null, "Missing imageData for external image");
			string text = ImageHelper.StoreImageDataInChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, base.m_odpMetadata, base.m_chunkFactory);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo(text, mimeType);
			imageInfo.SetCachedImageData(imageData);
			base.m_odpMetadata.AddExternalImage(value, imageInfo);
			return text;
		}

		public override byte[] GetCachedExternalImageBytes(string value)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo);
			bool condition = base.m_odpMetadata.TryGetExternalImage(value, out imageInfo);
			Global.Tracer.Assert(condition, "Missing ImageInfo for external image");
			byte[] array = this.ReadImageDataFromChunk(imageInfo.StreamName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
			if (array != null)
			{
				imageInfo.SetCachedImageData(array);
			}
			return array;
		}

		public override bool TryGetEmbeddedImage(string value, AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName)
		{
			Global.Tracer.Assert(embeddingMode == AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes.Inline, "Invalid image embedding mode");
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = null;
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages = odpContext.EmbeddedImages;
			if (embeddedImages != null && embeddedImages.TryGetValue(value, out imageInfo))
			{
				imageData = imageInfo.GetCachedImageData();
				streamName = imageInfo.StreamName;
				mimeType = imageInfo.MimeType;
				return true;
			}
			imageData = null;
			mimeType = null;
			streamName = null;
			return false;
		}

		public override byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo);
			bool condition = odpContext.EmbeddedImages.TryGetValue(imageName, out imageInfo);
			Global.Tracer.Assert(condition, "Missing ImageInfo for embedded image");
			byte[] array = this.ReadImageDataFromChunk(imageInfo.StreamName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.StaticImage);
			if (array != null)
			{
				imageInfo.SetCachedImageData(array);
			}
			return array;
		}

		public override bool TryGetDatabaseImage(string uniqueName, out string streamName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = base.m_odpMetadata.ReportSnapshot;
			return reportSnapshot.TryGetImageChunkName(uniqueName, out streamName);
		}

		public override string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext)
		{
			if (odpContext.IsPageHeaderFooter)
			{
				return null;
			}
			string text = ImageHelper.StoreImageDataInChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, base.m_odpMetadata, base.m_chunkFactory);
			base.m_odpMetadata.ReportSnapshot.AddImageChunkName(uniqueName, text);
			return text;
		}

		public override byte[] GetCachedDatabaseImageBytes(string chunkName)
		{
			return this.ReadImageDataFromChunk(chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image);
		}

		public override string EnsureTransparentImageIsCached(string mimeType, byte[] imageData)
		{
			string text = base.m_odpMetadata.TransparentImageChunkName;
			if (text == null)
			{
				text = ImageHelper.StoreImageDataInChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Image, imageData, mimeType, base.m_odpMetadata, base.m_chunkFactory);
				base.m_odpMetadata.TransparentImageChunkName = text;
			}
			return text;
		}

		private byte[] ReadImageDataFromChunk(string chunkName, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes chunkType)
		{
			byte[] array = null;
			string text = default(string);
			Stream chunk = base.m_chunkFactory.GetChunk(chunkName, chunkType, ChunkMode.Open, out text);
			Global.Tracer.Assert(chunk != null, "Could not find expected image data chunk.  Name='{0}', Type={1}", chunkName, chunkType);
			using (chunk)
			{
				return StreamSupport.ReadToEndUsingLength(chunk);
			}
		}
	}
}
