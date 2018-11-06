using AspNetCore.ReportingServices.Interfaces;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CreateAndRegisterStreamHandler : IStreamHandler
	{
		private string m_streamName;

		private CreateAndRegisterStream m_createStreamCallback;

		internal CreateAndRegisterStreamHandler(string streamName, CreateAndRegisterStream createStreamCallback)
		{
			this.m_streamName = streamName;
			this.m_createStreamCallback = createStreamCallback;
		}

		public Stream OpenStream()
		{
			return this.m_createStreamCallback(this.m_streamName, null, null, null, true, StreamOper.CreateOnly);
		}
	}
}
