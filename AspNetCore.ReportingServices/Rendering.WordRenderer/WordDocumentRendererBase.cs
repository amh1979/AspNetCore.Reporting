using AspNetCore.Reporting;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal abstract class WordDocumentRendererBase : IRenderingExtension, IExtension
	{
		public virtual string LocalizedName
		{
			get
			{
				return WordRenderRes.WordLocalizedName;
			}
		}

        public InternalLocalReport Report { get; set; }

        public WordDocumentRendererBase()
		{
		}

		internal abstract IWordWriter NewWordWriter();

		protected abstract WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName);

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		public bool Render(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable someProps, CreateAndRegisterStream createAndRegisterStream)
		{
			double pageHeight = 1.7976931348623157E+308;
			using (SPBProcessing.SPBProcessing sPBProcessing = new SPBProcessing.SPBProcessing(report, createAndRegisterStream, pageHeight))
			{
				DeviceInfo deviceInfo2 = new DeviceInfo(deviceInfo);
				SPBContext sPBContext = new SPBContext();
				sPBContext.StartPage = 0;
				sPBContext.EndPage = 0;
				sPBContext.MeasureItems = false;
				sPBContext.AddSecondaryStreamNames = true;
				sPBContext.AddToggledItems = deviceInfo2.ExpandToggles;
				sPBContext.AddFirstPageHeaderFooter = true;
				sPBProcessing.SetContext(sPBContext);
				using (IWordWriter writer = this.NewWordWriter())
				{
					WordRenderer wordRenderer = this.NewWordRenderer(createAndRegisterStream, deviceInfo2, sPBProcessing, writer, report.Name);
					try
					{
						return wordRenderer.Render();
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
			}
		}

		public bool RenderStream(string streamName, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable someProps, CreateAndRegisterStream createAndRegisterStream)
		{
			return false;
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
