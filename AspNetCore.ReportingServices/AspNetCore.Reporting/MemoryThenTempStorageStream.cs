using System.IO;

namespace AspNetCore.Reporting
{
	internal sealed class MemoryThenTempStorageStream : Stream
	{
		private const int m_threshold = 65536;

		private ITemporaryStorage m_tempStorage;

		private Stream m_storageStream = new MemoryStream();

		private bool m_thresholdReached;

		private bool m_isClosed;

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
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				return this.m_storageStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.m_storageStream.Position;
			}
			set
			{
				this.m_storageStream.Position = value;
			}
		}

		public MemoryThenTempStorageStream(ITemporaryStorage storage)
		{
			this.m_tempStorage = storage;
		}

		public override void Flush()
		{
			this.m_storageStream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && !this.m_isClosed)
			{
				this.m_storageStream.Close();
				this.m_isClosed = true;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.m_storageStream.Read(buffer, offset, count);
		}

		public override int ReadByte()
		{
			return this.m_storageStream.ReadByte();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.m_storageStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (!this.m_thresholdReached && value > 65536)
			{
				this.ThresholdReached();
			}
			if (this.m_storageStream != null)
			{
				this.m_storageStream.SetLength(value);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!this.m_thresholdReached && this.Position + count > 65536)
			{
				this.ThresholdReached();
			}
			if (this.m_storageStream != null)
			{
				this.m_storageStream.Write(buffer, offset, count);
			}
		}

		public override void WriteByte(byte value)
		{
			if (!this.m_thresholdReached && this.Position >= 65536)
			{
				this.ThresholdReached();
			}
			if (this.m_storageStream != null)
			{
				this.m_storageStream.WriteByte(value);
			}
		}

		private void ThresholdReached()
		{
			Stream stream = this.m_tempStorage.CreateTemporaryStream();
			if (stream != null)
			{
				if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
				{
					throw new System.Exception(" InvalidTemporaryStorageStreamException");
				}
				this.m_storageStream.Position = 0L;
				byte[] array = new byte[this.m_storageStream.Length];
				this.m_storageStream.Read(array, 0, array.Length);
				stream.Write(array, 0, array.Length);
				this.m_storageStream.Close();
				this.m_storageStream = stream;
				this.m_thresholdReached = true;
			}
		}
	}
}
