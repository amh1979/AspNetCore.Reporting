using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ExternalImageDataHandler : ImageDataHandler
	{
		public override AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.External;
			}
		}

		protected override ProcessingErrorCode ErrorCodeForSourceType
		{
			get
			{
				return ProcessingErrorCode.rsInvalidExternalImageProperty;
			}
		}

		public ExternalImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string GetImageDataId()
		{
			List<string> list = default(List<string>);
			bool flag = default(bool);
			string text = base.m_image.GetValueAsString(out list, out flag);
			if (flag || string.IsNullOrEmpty(text))
			{
				text = null;
			}
			return text;
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
			AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext = base.m_reportElement.RenderingContext;
			string text = null;
			imageData = null;
			imageDataId = valueAsString;
			bool flag2 = default(bool);
			if (base.CacheManager.TryGetExternalImage(valueAsString, out imageData, out mimeType, out text, out flag2))
			{
				if (flag2)
				{
					imageDataId = null;
				}
			}
			else if (!this.GetExternalImage(renderingContext, valueAsString, out imageData, out mimeType) || imageData == null)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidExternalImageProperty, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName, text);
				base.CacheManager.AddFailedExternalImage(valueAsString);
				text = null;
				imageDataId = null;
			}
			else
			{
				text = base.CacheManager.AddExternalImage(valueAsString, imageData, mimeType);
			}
			return text;
		}

		private bool GetExternalImage(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string path, out byte[] imageData, out string mimeType)
		{
			imageData = null;
			mimeType = null;
			try
			{
				if (!renderingContext.OdpContext.TopLevelContext.ReportContext.IsSupportedProtocol(path, true))
				{
					renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName, path, "http://, https://, ftp://, file:, mailto:, or news:");
				}
				else
				{
					bool flag = default(bool);
					renderingContext.OdpContext.GetResource(path, out imageData, out mimeType, out flag);
					if (imageData != null && !AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateMimeType(mimeType))
					{
						renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, "MIMEType", mimeType);
						mimeType = null;
						imageData = null;
					}
					if (flag)
					{
						renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName);
					}
				}
			}
			catch (Exception ex)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidImageReference, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName, ex.Message);
				return false;
			}
			return true;
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedExternalImageBytes(imageDataId);
		}
	}
}
