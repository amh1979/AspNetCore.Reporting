using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DatabaseImageDataHandler : ImageDataHandler
	{
		public override AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType Source
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database;
			}
		}

		protected override ProcessingErrorCode ErrorCodeForSourceType
		{
			get
			{
				return ProcessingErrorCode.rsInvalidDatabaseImageProperty;
			}
		}

		public DatabaseImageDataHandler(ReportElement reportElement, IBaseImage image)
			: base(reportElement, image)
		{
		}

		protected override string CalculateImageProperties(out string mimeType, out byte[] imageData, out string imageDataId, out List<string> fieldsUsedInValue, out bool isNullImage)
		{
			fieldsUsedInValue = null;
			isNullImage = false;
			mimeType = base.m_image.GetMIMETypeValue();
			mimeType = AspNetCore.ReportingServices.ReportPublishing.ProcessingValidator.ValidateMimeType(mimeType, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.MIMETypePropertyName, base.m_reportElement.RenderingContext.OdpContext.ErrorContext);
			if (mimeType == null)
			{
				return base.GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			string instanceUniqueName = base.m_reportElement.InstanceUniqueName;
			string text = default(string);
			if (base.CacheManager.TryGetDatabaseImage(instanceUniqueName, out text))
			{
				imageData = null;
				imageDataId = text;
				return text;
			}
			bool flag = default(bool);
			imageData = base.m_image.GetImageData(out fieldsUsedInValue, out flag);
			if (flag)
			{
				base.m_reportElement.RenderingContext.OdpContext.ErrorContext.Register(this.ErrorCodeForSourceType, Severity.Warning, base.m_image.ObjectType, base.m_image.ObjectName, base.m_image.ImageDataPropertyName, base.m_image.Value.ExpressionString);
				return base.GetErrorImageProperties(out mimeType, out imageData, out imageDataId);
			}
			if (imageData != null && imageData.Length != 0)
			{
				text = (imageDataId = base.CacheManager.AddDatabaseImage(instanceUniqueName, imageData, mimeType, base.m_reportElement.RenderingContext.OdpContext));
				return text;
			}
			isNullImage = true;
			return base.GetTransparentImageProperties(out mimeType, out imageData, out imageDataId);
		}

		protected override byte[] LoadExistingImageData(string imageDataId)
		{
			return base.CacheManager.GetCachedDatabaseImageBytes(imageDataId);
		}
	}
}
