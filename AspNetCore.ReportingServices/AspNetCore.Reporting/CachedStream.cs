using System;
using System.IO;
using System.Text;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class CachedStream : IDisposable
	{
		private Stream m_stream;

		private Encoding m_encoding;

		private string m_mimeType;

		private string m_fileExtension;

		public Stream Stream
		{
			get
			{
				this.m_stream.Seek(0L, SeekOrigin.Begin);
				return this.m_stream;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.m_encoding;
			}
		}

		public string MimeType
		{
			get
			{
				return this.m_mimeType;
			}
		}

		public string FileExtension
		{
			get
			{
				return this.m_fileExtension;
			}
		}

		public CachedStream(Stream stream, Encoding encoding, string mimeType, string fileExtension)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_stream = stream;
			this.m_encoding = encoding;
			this.m_mimeType = mimeType;
			this.m_fileExtension = fileExtension;
		}

		public static Stream Extract(CachedStream cachedStream, out Encoding encoding, out string mimeType, out string fileExtension)
		{
			if (cachedStream != null)
			{
				if (cachedStream.Encoding == null)
				{
                    encoding = Encoding.UTF8;
				}
				else
				{
					encoding = cachedStream.Encoding;
				}
				mimeType = cachedStream.MimeType;
				fileExtension = cachedStream.FileExtension;
				return cachedStream.Stream;
			}
			encoding = null;
			mimeType = null;
			fileExtension = null;
			return null;
		}

		public void Dispose()
		{
			this.m_stream.Dispose();
		}
	}
}
