using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class EmbeddedImageDataHandler : ImageDataHandler
	{
		public override AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded;
			}
		}

		protected override ProcessingErrorCode ErrorCodeForSourceType
		{
			get
			{
				return ProcessingErrorCode.rsInvalidEmbeddedImageProperty;
			}
		}

		private OnDemandProcessingContext OdpContext
		{
			get
			{
				return base.m_reportElement.RenderingContext.OdpContext;
			}
		}

		public EmbeddedImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			isNullImage = false;
			bool flag = default(bool);
			string valueAsString = base.m_image.GetValueAsString(out fieldsUsedInValue, out flag);
			if (flag)
			{
				return base.GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			if (string.IsNullOrEmpty(valueAsString))
			{
				isNullImage = true;
				return base.GetTransparentImageProperties(out mimeType, out imageData, out imageDataId);
			}
			imageDataId = valueAsString;
			string result = default(string);
			if (!base.CacheManager.TryGetEmbeddedImage(valueAsString, base.m_image.EmbeddingMode, this.OdpContext, out imageData, out mimeType, out result))
			{
				this.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName, valueAsString);
				return base.GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			return result;
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedEmbeddedImageBytes(imageDataId, this.OdpContext);
		}
	}
}
