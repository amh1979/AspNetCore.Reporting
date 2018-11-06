using System;
using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal sealed class LengthEncodedWritableStream : Stream
	{
		private MemoryStream m_bufferStream;

		private BinaryWriter m_writer;

		private string m_name;

		private bool m_closed;

		private long m_length;

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.m_writer.BaseStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.m_length;
			}
		}

		public override long Position
		{
			get
			{
				return this.m_length;
			}
			set
			{
				throw new NotSupportedException("LengthEncodedWritableStream.set_Position");
			}
		}

		internal bool Closed
		{
			get
			{
				return this.m_closed;
			}
		}

		internal LengthEncodedWritableStream(BinaryWriter writer, string name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			this.m_writer = writer;
			this.m_name = name;
			this.m_bufferStream = new MemoryStream();
			this.m_length = 0L;
		}

		public override void Flush()
		{
			this.m_bufferStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.Read");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.Seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("LengthEncodedWritableStream.SetLength");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.m_bufferStream.Write(buffer, offset, count);
			this.m_length += count;
		}

		public override void Close()
		{
			if (!this.m_closed)
			{
				this.m_closed = true;
				try
				{
					this.m_writer.Write(this.m_name);
					MessageUtil.WriteByteArray(this.m_writer, this.m_bufferStream.GetBuffer(), 0, checked((int)this.m_bufferStream.Length));
				}
				finally
				{
					this.m_bufferStream.Close();
					base.Close();
				}
			}
		}
	}
}
