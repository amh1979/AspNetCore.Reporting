using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLRendering
{
	internal sealed class RPLRenderer : IRenderingExtension, IExtension, ITotalPages
	{
		private const string PARAM_START_PAGE = "StartPage";

		private const string PARAM_END_PAGE = "EndPage";

		private const string PARAM_MEASURE_ITEMS = "MeasureItems";

		private const string PARAM_TOGGLE_ITEMS = "ToggleItems";

		private const string PARAM_SECONDARY_STREAMS = "SecondaryStreams";

		private const string PARAM_STREAM_NAMES = "StreamNames";

		private const string PARAM_RPL_VERSION = "RPLVersion";

		private const string PARAM_IMAGE_CONSOLIDATION = "ImageConsolidation";

		private const string PARAM_CONVERT_IMAGES = "ConvertImages";

		private const string MIME_EXTENSION = "rpl";

		private const string MIME_TYPE = "application/octet-stream";

		private string m_rplVersion;

		private SPBContext m_spbContext = new SPBContext();

		public string LocalizedName
		{
			get
			{
				return RenderRes.RPLLocalizedName;
			}
		}


        static RPLRenderer()
		{
		}

		internal static SecondaryStreams ParseSecondaryStreamsParam(string secondaryStreamStr, SecondaryStreams defaultValue)
		{
			if (SecondaryStreams.Embedded.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Embedded;
			}
			if (SecondaryStreams.Server.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Server;
			}
			if (SecondaryStreams.Temporary.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Temporary;
			}
			return defaultValue;
		}

		internal static bool ParseBool(string boolValue, bool defaultValue)
		{
			bool result = default(bool);
			if (bool.TryParse(boolValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static int ParseInt(string intValue, int defaultValue)
		{
			int result = default(int);
			if (int.TryParse(intValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static double ParseDouble(string doubleValue, double defaultValue)
		{
			double result = default(double);
			if (double.TryParse(doubleValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		private void ParseParameters(NameValueCollection deviceInfo)
		{
			this.m_spbContext.StartPage = RPLRenderer.ParseInt(deviceInfo["StartPage"], 1);
			this.m_spbContext.EndPage = RPLRenderer.ParseInt(deviceInfo["EndPage"], 1);
			this.m_spbContext.MeasureItems = RPLRenderer.ParseBool(deviceInfo["MeasureItems"], false);
			this.m_spbContext.AddToggledItems = RPLRenderer.ParseBool(deviceInfo["ToggleItems"], false);
			this.m_spbContext.SecondaryStreams = RPLRenderer.ParseSecondaryStreamsParam(deviceInfo["SecondaryStreams"], SecondaryStreams.Server);
			this.m_spbContext.AddSecondaryStreamNames = RPLRenderer.ParseBool(deviceInfo["StreamNames"], true);
			this.m_spbContext.UseImageConsolidation = RPLRenderer.ParseBool(deviceInfo["ImageConsolidation"], false);
			this.m_spbContext.ConvertImages = RPLRenderer.ParseBool(deviceInfo["ConvertImages"], false);
			this.m_rplVersion = deviceInfo["RPLVersion"];
		}

		public bool Render(OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				this.ParseParameters(deviceInfo);
				Stream outputStream = createAndRegisterStream(report.Name, "rpl", null, "application/octet-stream", false, StreamOper.CreateAndRegister);
				SPBProcessing.SPBProcessing sPBProcessing = null;
				using (sPBProcessing = new SPBProcessing.SPBProcessing(report, createAndRegisterStream, true, this.m_rplVersion, ref renderProperties))
				{
					sPBProcessing.SetContext(this.m_spbContext);
					sPBProcessing.GetNextPage(outputStream);
					sPBProcessing.UpdateRenderProperties(ref renderProperties);
				}
				return false;
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
			try
			{
				if (string.IsNullOrEmpty(streamName))
				{
					return false;
				}
				return SPBProcessing.SPBProcessing.RenderSecondaryStream(report, createAndRegisterStream, streamName);
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

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
