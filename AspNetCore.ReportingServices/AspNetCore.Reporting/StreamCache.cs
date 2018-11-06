using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class StreamCache : IDisposable
	{
		private CachedStream m_mainStream;

		private bool m_mainStreamDetached;

		private Dictionary<string, CachedStream> m_secondaryStreams = new Dictionary<string, CachedStream>();

		private CreateStreamDelegate m_createMainStreamDelegate;

		public StreamCache()
			: this(null)
		{
		}

		public StreamCache(CreateStreamDelegate createMainStreamDelegate)
		{
			this.m_createMainStreamDelegate = (createMainStreamDelegate ?? new CreateStreamDelegate(StreamCache.DefaultCreateStreamDelegate));
		}

		public void Dispose()
		{
			this.Clear();
		}

		public void Clear()
		{
			if (!this.m_mainStreamDetached && this.m_mainStream != null)
			{
				this.m_mainStream.Dispose();
			}
			foreach (CachedStream value in this.m_secondaryStreams.Values)
			{
				value.Dispose();
			}
			this.m_mainStream = null;
			this.m_mainStreamDetached = false;
			this.m_secondaryStreams.Clear();
		}

		public Stream StreamCallback(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			int num;
			switch (operation)
			{
			case StreamOper.RegisterOnly:
				return null;
			case StreamOper.CreateAndRegister:
				if (this.m_mainStream == null)
				{
					num = ((!this.m_mainStreamDetached) ? 1 : 0);
					break;
				}
				goto default;
			default:
				num = 0;
				break;
			}
			bool flag = (byte)num != 0;
			CreateStreamDelegate createStreamDelegate = flag ? this.m_createMainStreamDelegate : new CreateStreamDelegate(StreamCache.DefaultCreateStreamDelegate);
			CachedStream cachedStream = new CachedStream(createStreamDelegate(), encoding, mimeType, extension);
			if (operation == StreamOper.CreateAndRegister)
			{
				if (flag)
				{
					this.m_mainStream = cachedStream;
				}
				else
				{
					this.m_secondaryStreams.Add(name, cachedStream);
				}
			}
			return cachedStream.Stream;
		}

		public Stream GetMainStream(bool detach)
		{
			return this.GetMainStream(detach, out Encoding text, out string text2, out string text3);
		}

		public Stream GetMainStream(bool detach, out Encoding encoding, out string mimeType, out string fileExtension)
		{
			Stream result = CachedStream.Extract(this.m_mainStream, out encoding, out mimeType, out fileExtension);
			if (detach)
			{
				this.m_mainStreamDetached = detach;
				this.m_mainStream = null;
			}
			return result;
		}

		public byte[] GetMainStream(out Encoding encoding, out string mimeType, out string fileExtension)
		{
			Stream mainStream = this.GetMainStream(false, out encoding, out mimeType, out fileExtension);
			return this.StreamToBytes(mainStream);
		}

		public byte[] GetSecondaryStream(bool remove, string name, out Encoding encoding, out string mimeType, out string fileExtension)
		{
			CachedStream cachedStream = default(CachedStream);
			bool flag = this.m_secondaryStreams.TryGetValue(name, out cachedStream);
			Stream stream = CachedStream.Extract(cachedStream, out encoding, out mimeType, out fileExtension);
			byte[] result = this.StreamToBytes(stream);
			if (flag && remove)
			{
				this.m_secondaryStreams.Remove(name);
				cachedStream.Dispose();
			}
			return result;
		}

		public void MoveSecondaryStreamsTo(StreamCache other)
		{
			foreach (KeyValuePair<string, CachedStream> secondaryStream in this.m_secondaryStreams)
			{
				other.m_secondaryStreams.Add(secondaryStream.Key, secondaryStream.Value);
			}
			this.m_secondaryStreams.Clear();
		}

		private byte[] StreamToBytes(Stream stream)
		{
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				for (int i = 0; i < stream.Length; i += stream.Read(array, i, (int)stream.Length))
				{
				}
				return array;
			}
			return null;
		}

		private static Stream DefaultCreateStreamDelegate()
		{
			return new MemoryStream();
		}
	}
}
