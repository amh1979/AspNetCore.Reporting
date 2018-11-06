using System;
using System.IO;

namespace AspNetCore.ReportingServices.ReportProcessing.Utils
{
	internal class ReadOnlyStream : Stream
	{
		private readonly Stream m_underlyingStream;

		private readonly bool m_canCloseUnderlyingStream;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.m_underlyingStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this.m_underlyingStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.m_underlyingStream.Position;
			}
			set
			{
				throw new InvalidOperationException("This Stream does not support this operation.");
			}
		}

		public ReadOnlyStream(Stream underlyingStream, bool canCloseUnderlyingStream)
		{
			if (underlyingStream == null)
			{
				throw new ArgumentNullException("underlyingStream");
			}
			this.m_underlyingStream = underlyingStream;
			this.m_canCloseUnderlyingStream = canCloseUnderlyingStream;
		}

		public override void Flush()
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override int ReadByte()
		{
			return this.m_underlyingStream.ReadByte();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.m_underlyingStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.m_underlyingStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("This Stream does not support this operation.");
		}

		public override void Close()
		{
			if (this.m_canCloseUnderlyingStream)
			{
				this.m_underlyingStream.Close();
			}
		}
	}
}
