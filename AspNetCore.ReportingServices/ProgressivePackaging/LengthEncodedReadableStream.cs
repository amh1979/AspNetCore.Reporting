using System;
using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class LengthEncodedReadableStream : Stream
	{
		private BinaryReader m_reader;

		private int m_length;

		private int m_position;

		private bool m_closed;

		public override bool CanRead
		{
			get
			{
				return this.m_reader.BaseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
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
				return this.m_length;
			}
		}

		public override long Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal bool Closed
		{
			get
			{
				return this.m_closed;
			}
		}

		internal LengthEncodedReadableStream(BinaryReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.m_reader = reader;
			this.m_length = this.m_reader.ReadInt32();
			this.m_position = 0;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.m_length == this.m_position)
			{
				return 0;
			}
			int count2 = Math.Min(count, this.m_length - this.m_position);
			int num = this.m_reader.Read(buffer, offset, count2);
			this.m_position += num;
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			if (!this.m_closed)
			{
				this.m_closed = true;
				base.Close();
			}
		}
	}
}
