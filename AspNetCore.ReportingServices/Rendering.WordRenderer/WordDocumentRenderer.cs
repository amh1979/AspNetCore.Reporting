using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class WordDocumentRenderer : WordDocumentRendererBase
	{
		internal override IWordWriter NewWordWriter()
		{
			return new Word97Writer();
		}

		protected override WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName)
		{
			return new Word97Renderer(createAndRegisterStream, spbProcessing, writer, deviceInfoObj, reportName);
		}
	}
}
