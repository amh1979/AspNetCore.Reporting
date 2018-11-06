using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Diagnostics;
using System.IO;
//using System.Runtime.Remoting;
using System.Text;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal static class StreamSupport
	{
		public delegate int StreamRead(byte[] buffer, int offset, int count);

		internal class MonitoredInputStream : Stream
		{
			public const int InputBufferSize = 1024;

			private Stream m_stream;

			private Action<int, string, bool> m_readCompleteAction;

			private byte[] m_inputBuffer;

			private int m_readByteCount;

			private bool m_actionFired;

			public override bool CanRead
			{
				get
				{
					return this.m_stream.CanRead;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return this.m_stream.CanSeek;
				}
			}

			public override bool CanTimeout
			{
				get
				{
					return this.m_stream.CanTimeout;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return this.m_stream.CanWrite;
				}
			}

			public override long Length
			{
				get
				{
					return this.m_stream.Length;
				}
			}

			public override long Position
			{
				get
				{
					return this.m_stream.Position;
				}
				set
				{
					this.m_stream.Position = value;
				}
			}

			public override int ReadTimeout
			{
				get
				{
					return this.m_stream.ReadTimeout;
				}
				set
				{
					this.m_stream.ReadTimeout = value;
				}
			}

			public override int WriteTimeout
			{
				get
				{
					return this.m_stream.WriteTimeout;
				}
				set
				{
					this.m_stream.WriteTimeout = value;
				}
			}

			public MonitoredInputStream(Stream s, Action<int, string, bool> readCompleteAction)
			{
				this.m_stream = s;
				this.m_readCompleteAction = readCompleteAction;
				this.m_inputBuffer = new byte[1024];
			}

			public void FireReadCompleteAction()
			{
				if (!this.m_actionFired)
				{
					this.m_actionFired = true;
					string @string = Encoding.UTF8.GetString(this.m_inputBuffer, 0, Math.Min(this.m_readByteCount, this.m_inputBuffer.Length));
					this.m_readCompleteAction(this.m_readByteCount, @string, this.m_readByteCount >= this.m_inputBuffer.Length);
				}
			}

			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return this.m_stream.BeginRead(buffer, offset, count, callback, state);
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				return this.m_stream.BeginWrite(buffer, offset, count, callback, state);
			}

			public override void Close()
			{
				this.m_stream.Close();
				this.FireReadCompleteAction();
			}

            //public override ObjRef CreateObjRef(Type requestedType)
            //{
            //    return this.m_stream.CreateObjRef(requestedType);
            //}

            public override int EndRead(IAsyncResult asyncResult)
			{
				return this.m_stream.EndRead(asyncResult);
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				this.m_stream.EndWrite(asyncResult);
			}

			public override void Flush()
			{
				this.m_stream.Flush();
			}

			public override object InitializeLifetimeService()
			{
				return this.m_stream.InitializeLifetimeService();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				int num = this.m_stream.Read(buffer, offset, count);
				if (num == 0)
				{
					this.FireReadCompleteAction();
				}
				else
				{
					if (this.m_readByteCount < this.m_inputBuffer.Length)
					{
						int length = Math.Min(num, this.m_inputBuffer.Length - this.m_readByteCount);
						Array.Copy(buffer, offset, this.m_inputBuffer, this.m_readByteCount, length);
					}
					this.m_readByteCount += num;
				}
				return num;
			}

			public override int ReadByte()
			{
				int num = this.m_stream.ReadByte();
				if (num == -1)
				{
					this.FireReadCompleteAction();
				}
				else
				{
					if (this.m_readByteCount < this.m_inputBuffer.Length)
					{
						this.m_inputBuffer[this.m_readByteCount] = (byte)num;
					}
					this.m_readByteCount++;
				}
				return num;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return this.m_stream.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				this.m_stream.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.m_stream.Write(buffer, offset, count);
			}

			public override void WriteByte(byte value)
			{
				this.m_stream.WriteByte(value);
			}

			public override string ToString()
			{
				return this.m_stream.ToString();
			}
		}

		private static readonly int __MemoryBufferLimit = 81920;

		private static readonly int __MaxAllowedBytesUnlimited = 2147483647;

		public static int MemoryBufferLimit
		{
			get
			{
				return StreamSupport.__MemoryBufferLimit;
			}
		}

		public static byte[] ReadToEndNotUsingLength(Stream s, int initialBufferSize)
		{
			bool flag = default(bool);
			return StreamSupport.ReadToEndNotUsingLength(s, initialBufferSize, StreamSupport.__MaxAllowedBytesUnlimited, out flag);
		}

		public static byte[] ReadToEndNotUsingLength(Stream s, int initialBufferSize, int maxAllowedBytes, out bool exceededMaxSize)
		{
			exceededMaxSize = false;
			if (s == null)
			{
				return null;
			}
			if (initialBufferSize <= 0)
			{
				initialBufferSize = 1;
			}
			byte[] array = new byte[initialBufferSize];
			int num = 0;
			int num2 = 0;
			do
			{
				num2 = s.Read(array, num, array.Length - num);
				num += num2;
				if (num > maxAllowedBytes)
				{
					exceededMaxSize = true;
					return null;
				}
				if (num2 != 0 && num >= array.Length)
				{
					byte[] array2 = new byte[array.Length * 2];
					Array.Copy(array, 0, array2, 0, num);
					array = array2;
				}
			}
			while (num2 != 0);
			if (num == array.Length)
			{
				return array;
			}
			byte[] array3 = new byte[num];
			Array.Copy(array, 0, array3, 0, num);
			return array3;
		}

		public static byte[] ReadToEndUsingLength(Stream s)
		{
			if (s == null)
			{
				return null;
			}
			byte[] array = new byte[s.Length];
			int num = 0;
			int num2 = 0;
			do
			{
				num2 = s.Read(array, num, array.Length - num);
				num += num2;
			}
			while (num2 != 0);
			if (num != array.Length)
			{
				throw new NotSupportedException("Stream must be at position 0 when calling this function!");
			}
			return array;
		}

		public static long CopyStreamUsingBuffer(Stream from, Stream to, int bufferSize)
		{
			if (bufferSize <= 0)
			{
				bufferSize = 1024;
			}
			if (bufferSize > StreamSupport.MemoryBufferLimit)
			{
				RSTrace.CatalogTrace.Assert(false, "Buffers size is non optimal size for copying streams");
			}
			byte[] array = new byte[bufferSize];
			int num = 0;
			long num2 = 0L;
			do
			{
				num = from.Read(array, 0, array.Length);
				if (num > 0)
				{
					to.Write(array, 0, num);
					num2 += num;
				}
			}
			while (num > 0);
			return num2;
		}

		public static long CopyFromStreamUsingBuffer(Stream from, Stream to, long bytesToCopy, int bufferSize)
		{
			long num = bytesToCopy;
			if (bufferSize <= 0)
			{
				bufferSize = 1024;
			}
			if (bufferSize > StreamSupport.MemoryBufferLimit)
			{
				RSTrace.CatalogTrace.Assert(false, "Buffers size is non optimal size for copying streams");
			}
			byte[] array = new byte[bufferSize];
			int num2 = 0;
			int num3 = array.Length;
			long num4 = 0L;
			do
			{
				if (num3 > num)
				{
					num3 = (int)num;
				}
				num2 = from.Read(array, 0, num3);
				if (num2 > 0)
				{
					to.Write(array, 0, num2);
					num4 += num2;
					num -= num2;
				}
			}
			while (num2 > 0);
			if (num != 0)
			{
				RSTrace.ChunkTracer.Assert(false, "unexpected number of bytes to copy ({0})", num);
			}
			return num4;
		}

		public static int ReadToCountOrEnd(byte[] buffer, int offset, int count, StreamRead streamReadDelegate)
		{
			int num = 0;
			int num2 = offset;
			int num3 = count;
			while (true)
			{
				int num4 = streamReadDelegate(buffer, num2, num3);
				if (num4 == -1)
				{
					break;
				}
				num += num4;
				if (num >= count)
				{
					break;
				}
				if (num4 <= 0)
				{
					break;
				}
				num2 += num4;
				num3 -= num4;
			}
			return num;
		}

		public static Stream CreateTraceableInputStream(Stream inputStream, RSTrace trace, string description)
		{
			return new MonitoredInputStream(inputStream, delegate(int byteCount, string content, bool capacityReached)
			{
				trace.Trace(TraceLevel.Info, "{0}: bytes={1}, content={2}{3}", description, byteCount, content.MarkAsPrivate(), capacityReached ? "..." : string.Empty);
			});
		}
	}
}
