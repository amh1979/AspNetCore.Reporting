using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
	internal sealed class ProcessingStreamHandler : IDisposable
	{
		private bool m_allowOnlyTemporaryStreams;

		private CreateAndRegisterStream m_createStreamCallback;

		private List<Stream> m_streams = new List<Stream>();

		public ProcessingStreamHandler(CreateAndRegisterStream createStreamCallback)
		{
			this.m_allowOnlyTemporaryStreams = false;
			this.m_createStreamCallback = createStreamCallback;
		}

		public ProcessingStreamHandler()
		{
			this.m_allowOnlyTemporaryStreams = true;
		}

		public void Dispose()
		{
			foreach (Stream stream in this.m_streams)
			{
				stream.Close();
			}
			this.m_streams.Clear();
		}

		public Stream StreamCallback(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			if (operation == StreamOper.RegisterOnly)
			{
				return null;
			}
			if (this.m_allowOnlyTemporaryStreams && operation != StreamOper.CreateOnly)
			{
				throw new InvalidOperationException("Only temporary streams are allowed by this StreamHandler");
			}
			if ((operation == StreamOper.CreateAndRegister || operation == StreamOper.CreateForPersistedStreams) && this.m_createStreamCallback != null)
			{
				return this.m_createStreamCallback(name, extension, encoding, mimeType, useChunking, operation);
			}
			MemoryStream memoryStream = new MemoryStream();
			this.m_streams.Add(memoryStream);
			return memoryStream;
		}
	}
}
