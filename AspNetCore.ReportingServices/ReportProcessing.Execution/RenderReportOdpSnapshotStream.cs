using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.Collections;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpSnapshotStream : RenderReportOdpSnapshot
	{
		private readonly string m_streamName;

		protected override bool IsRenderStream
		{
			get
			{
				return true;
			}
		}

		public RenderReportOdpSnapshotStream(ProcessingContext pc, RenderingContext rc, IConfiguration configuration, string streamName)
			: base(pc, rc, configuration)
		{
			this.m_streamName = streamName;
		}

		protected override bool InvokeRenderer(IRenderingExtension renderer, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.RenderStream(this.m_streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}
	}
}
