using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.Collections;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal sealed class RenderReportYukonSnapshotStream : RenderReportYukonSnapshot
	{
		private readonly string m_streamName;

		protected override bool IsRenderStream
		{
			get
			{
				return true;
			}
		}

		public RenderReportYukonSnapshotStream(ProcessingContext pc, RenderingContext rc, ReportProcessing processing, string streamName)
			: base(pc, rc, processing)
		{
			this.m_streamName = streamName;
		}

		protected override bool InvokeRenderer(IRenderingExtension renderer, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.RenderStream(this.m_streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}
	}
}
