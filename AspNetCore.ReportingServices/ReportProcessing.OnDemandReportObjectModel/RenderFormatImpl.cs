using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class RenderFormatImpl : RenderFormatImplBase
	{
		internal const string InteractivityRenderFormat = "RPL";

		private OnDemandProcessingContext m_odpContext;

		private string m_format;

		private bool m_isInteractiveFormat;

		private ReadOnlyNameValueCollection m_deviceInfo;

		private ReadOnlyNameValueCollection m_emptyDeviceInfo;

		internal override string Name
		{
			get
			{
				this.SetRenderFormatUsed();
				if (this.IsRenderFormatAccessEnabled())
				{
					return this.m_format;
				}
				return null;
			}
		}

		internal override bool IsInteractive
		{
			get
			{
				if (this.m_odpContext.IsTablixProcessingMode)
				{
					return false;
				}
				this.SetRenderFormatUsed();
				return this.IsInteractiveFormat();
			}
		}

		internal override ReadOnlyNameValueCollection DeviceInfo
		{
			get
			{
				this.SetRenderFormatUsed();
				if (this.IsRenderFormatAccessEnabled())
				{
					return this.m_deviceInfo;
				}
				return this.m_emptyDeviceInfo;
			}
		}

		internal RenderFormatImpl(OnDemandProcessingContext odpContext)
		{
			this.m_odpContext = odpContext;
			this.m_emptyDeviceInfo = new ReadOnlyNameValueCollection(new NameValueCollection(0));
			if (this.m_odpContext.TopLevelContext.ReportContext.RSRequestParameters != null)
			{
				NameValueCollection renderingParameters = this.m_odpContext.TopLevelContext.ReportContext.RSRequestParameters.RenderingParameters;
				if (renderingParameters != null)
				{
					this.m_deviceInfo = new ReadOnlyNameValueCollection(renderingParameters);
				}
			}
			this.m_format = RenderFormatImpl.NormalizeRenderFormat(this.m_odpContext.TopLevelContext.ReportContext, out this.m_isInteractiveFormat);
			if (this.m_deviceInfo == null)
			{
				this.m_deviceInfo = this.m_emptyDeviceInfo;
			}
		}

		internal static string NormalizeRenderFormat(ICatalogItemContext reportContext, out bool isInteractiveFormat)
		{
			string text = null;
			if (reportContext.RSRequestParameters != null)
			{
				text = reportContext.RSRequestParameters.FormatParamValue;
			}
			if (text == null)
			{
				text = "RPL";
				isInteractiveFormat = true;
			}
			else if (RenderFormatImpl.IsRenderFormat(text, "RPL") || RenderFormatImpl.IsRenderFormat(text, "RGDI") || RenderFormatImpl.IsRenderFormat(text, "HTML4.0") || RenderFormatImpl.IsRenderFormat(text, "HTML5") || RenderFormatImpl.IsRenderFormat(text, "MHTML"))
			{
				isInteractiveFormat = true;
			}
			else
			{
				isInteractiveFormat = false;
			}
			return text;
		}

		private bool IsRenderFormatAccessEnabled()
		{
			if (this.m_odpContext.IsTablixProcessingMode)
			{
				return false;
			}
			if (this.IsInteractiveFormat() && !this.m_odpContext.IsUnrestrictedRenderFormatReferenceMode)
			{
				return false;
			}
			return true;
		}

		private bool IsInteractiveFormat()
		{
			return this.m_isInteractiveFormat;
		}

		internal static bool IsRenderFormat(string format, string targetFormat)
		{
			return 0 == ReportProcessing.CompareWithInvariantCulture(format, targetFormat, true);
		}

		private void RegisterWarning(string propertyName)
		{
			if (this.m_odpContext.ReportRuntime.RuntimeErrorContext != null)
			{
				this.m_odpContext.ReportRuntime.RuntimeErrorContext.Register(ProcessingErrorCode.rsInvalidRenderFormatUsage, Severity.Warning, this.m_odpContext.ReportRuntime.ObjectType, this.m_odpContext.ReportRuntime.ObjectName, this.m_odpContext.ReportRuntime.PropertyName);
			}
		}

		private void SetRenderFormatUsed()
		{
			if (!this.m_odpContext.IsTablixProcessingMode)
			{
				this.m_odpContext.HasRenderFormatDependencyInDocumentMap = true;
			}
		}
	}
}
