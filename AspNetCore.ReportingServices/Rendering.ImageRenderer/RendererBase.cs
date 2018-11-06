using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal class RendererBase : IRenderingExtension, IExtension
	{
		public virtual string LocalizedName
		{
			get
			{
				return null;
			}
		}


        public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		protected virtual void ParseDeviceInfo(ref NameValueCollection deviceInfo)
		{
		}

		protected static bool ParseDeviceInfoBoolean(string boolValue, bool defaultValue)
		{
			bool result = default(bool);
			if (bool.TryParse(boolValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		protected static int ParseDeviceInfoInt32(string intValue, int defaultValue)
		{
			int result = default(int);
			if (int.TryParse(intValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		protected virtual void Render(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
		}

		public bool Render(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				this.ParseDeviceInfo(ref deviceInfo);
				this.Render(report, deviceInfo, renderProperties, createAndRegisterStream);
				return true;
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, true);
			}
		}

		public bool RenderStream(string streamName, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return false;
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
