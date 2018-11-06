using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class ImageCacheManager
	{
		protected readonly OnDemandMetadata m_odpMetadata;

		protected readonly IChunkFactory m_chunkFactory;

		public ImageCacheManager(OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			this.m_odpMetadata = odpMetadata;
			this.m_chunkFactory = chunkFactory;
		}

		public bool TryGetExternalImage(string value, out byte[] imageData, out string mimeType, out string streamName, out bool wasError)
		{
			imageData = null;
			mimeType = null;
			streamName = null;
			OnDemandMetadata odpMetadata = this.m_odpMetadata;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo);
			if (odpMetadata.TryGetExternalImage(value, out imageInfo))
			{
				if (imageInfo.ErrorOccurred)
				{
					wasError = true;
					return true;
				}
				wasError = false;
				return this.ExtractCachedExternalImagePropertiesIfValid(imageInfo, out imageData, out mimeType, out streamName);
			}
			wasError = false;
			return false;
		}

		protected abstract bool ExtractCachedExternalImagePropertiesIfValid(AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo, out byte[] imageData, out string mimeType, out string streamName);

		public abstract string AddExternalImage(string value, byte[] imageData, string mimeType);

		public abstract byte[] GetCachedExternalImageBytes(string value);

		public void AddFailedExternalImage(string value)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo imageInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo(null, null);
			imageInfo.ErrorOccurred = true;
			this.m_odpMetadata.AddExternalImage(value, imageInfo);
		}

		public abstract bool TryGetEmbeddedImage(string value, AspNetCore.ReportingServices.OnDemandReportRendering.Image.EmbeddingModes embeddingMode, OnDemandProcessingContext odpContext, out byte[] imageData, out string mimeType, out string streamName);

		public abstract byte[] GetCachedEmbeddedImageBytes(string imageName, OnDemandProcessingContext odpContext);

		public abstract bool TryGetDatabaseImage(string uniqueName, out string streamName);

		public abstract string AddDatabaseImage(string uniqueName, byte[] imageData, string mimeType, OnDemandProcessingContext odpContext);

		public abstract byte[] GetCachedDatabaseImageBytes(string chunkName);

		public abstract string EnsureTransparentImageIsCached(string mimeType, byte[] imageData);
	}
}
