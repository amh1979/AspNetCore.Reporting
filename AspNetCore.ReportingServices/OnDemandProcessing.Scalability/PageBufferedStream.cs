using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class PageBufferedStream : Stream
	{
		private sealed class CachePage
		{
			internal byte[] Buffer;

			internal bool Dirty;

			internal CachePage NextPage;

			internal long PageNumber;

			public CachePage(int size, long pageNum)
			{
				this.Buffer = new byte[size];
				this.Dirty = false;
				this.NextPage = null;
				this.PageNumber = pageNum;
			}

			public void Read(Stream stream)
			{
				int num = 0;
				int num2 = this.Buffer.Length;
				int num3 = 0;
				do
				{
					num3 = stream.Read(this.Buffer, num, num2);
					num += num3;
					num2 -= num3;
				}
				while (num3 > 0 && num2 > 0);
				Global.Tracer.Assert(num == this.Buffer.Length, "Error filling buffer page");
				this.Dirty = false;
			}

			internal void InitBuffer()
			{
				this.Dirty = false;
			}

			public void Write(Stream stream)
			{
				stream.Write(this.Buffer, 0, this.Buffer.Length);
				this.Dirty = false;
			}
		}

		private readonly int m_bytesPerPage;

		private readonly int m_pageCacheCapacity;

		private Stream m_stream;

		private Dictionary<long, CachePage> m_pageCache;

		private CachePage m_firstPageToEvict;

		private CachePage m_lastPageToEvict;

		private long m_position;

		private long m_length;

		private bool m_freezePageAllocations;

		public bool FreezePageAllocations
		{
			get
			{
				return this.m_freezePageAllocations;
			}
			set
			{
				this.m_freezePageAllocations = value;
			}
		}

		internal int PageCount
		{
			get
			{
				return this.m_pageCache.Count;
			}
		}

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
				this.m_position = value;
			}
		}

		public PageBufferedStream(Stream stream, int bytesPerPage, int cachePageCount)
		{
			if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
			{
				Global.Tracer.Assert(false, "PageBufferedStream: Must be able to Seek, Read, and Write stream");
			}
			this.m_bytesPerPage = bytesPerPage;
			this.m_pageCacheCapacity = cachePageCount;
			this.m_stream = stream;
			this.m_length = stream.Length;
			this.m_pageCache = new Dictionary<long, CachePage>(this.m_pageCacheCapacity);
		}

		public override void Flush()
		{
			foreach (long key in this.m_pageCache.Keys)
			{
				this.FlushPage(this.m_pageCache[key], key);
			}
		}

		public override int ReadByte()
		{
			CachePage page = this.GetPage(this.m_position);
			int num = this.CalcOffsetWithinPage(this.m_position);
			this.UpdatePosition(1L);
			return page.Buffer[num];
		}

		public override void WriteByte(byte value)
		{
			CachePage page = this.GetPage(this.m_position);
			int num = this.CalcOffsetWithinPage(this.m_position);
			this.UpdatePosition(1L);
			page.Buffer[num] = value;
			page.Dirty = true;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result = count;
			long num = this.m_position + count;
			while (this.m_position < num)
			{
				CachePage page = this.GetPage(this.m_position);
				int num2 = this.CalcOffsetWithinPage(this.m_position);
				byte[] buffer2 = page.Buffer;
				int num3 = Math.Min(buffer2.Length - num2, count);
				Array.Copy(buffer2, num2, buffer, offset, num3);
				this.UpdatePosition(num3);
				count -= num3;
				offset += num3;
			}
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.m_position = offset;
				break;
			case SeekOrigin.Current:
				this.m_position += offset;
				break;
			case SeekOrigin.End:
				this.m_position = this.m_length + offset;
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
			return this.m_position;
		}

		public override void SetLength(long value)
		{
			this.m_length = value;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			long num = this.m_position + count;
			while (this.m_position < num)
			{
				CachePage page = this.GetPage(this.m_position);
				int num2 = this.CalcOffsetWithinPage(this.m_position);
				byte[] buffer2 = page.Buffer;
				int num3 = Math.Min(buffer2.Length - num2, count);
				Array.Copy(buffer, offset, buffer2, num2, num3);
				this.UpdatePosition(num3);
				count -= num3;
				offset += num3;
				page.Dirty = true;
			}
		}

		public override void Close()
		{
			this.m_stream.Close();
		}

		private void UpdatePosition(long moveBy)
		{
			this.m_position += moveBy;
			if (this.m_position > this.m_length)
			{
				this.m_length = this.m_position;
			}
		}

		private CachePage GetPage(long fileOffset)
		{
			long num = this.CalcPageNum(fileOffset);
			CachePage cachePage = null;
			if (!this.m_pageCache.TryGetValue(num, out cachePage))
			{
				bool flag = false;
				if (this.m_pageCache.Count == this.m_pageCacheCapacity || (this.m_freezePageAllocations && this.m_pageCache.Count > 0))
				{
					cachePage = this.m_firstPageToEvict;
					long pageNumber = cachePage.PageNumber;
					this.m_firstPageToEvict = cachePage.NextPage;
					this.m_pageCache.Remove(pageNumber);
					this.FlushPage(cachePage, pageNumber);
					cachePage.PageNumber = num;
				}
				else
				{
					cachePage = new CachePage(this.m_bytesPerPage, num);
					flag = true;
				}
				long num2 = this.CalcPageOffset(num);
				if (num2 < this.m_length)
				{
					this.m_stream.Seek(num2, SeekOrigin.Begin);
					cachePage.Read(this.m_stream);
				}
				else if (!flag)
				{
					cachePage.InitBuffer();
				}
				this.m_pageCache[num] = cachePage;
				if (this.m_firstPageToEvict == null)
				{
					this.m_firstPageToEvict = cachePage;
				}
				if (this.m_lastPageToEvict != null)
				{
					this.m_lastPageToEvict.NextPage = cachePage;
				}
				this.m_lastPageToEvict = cachePage;
			}
			return cachePage;
		}

		private void FlushPage(CachePage page, long pageNum)
		{
			if (page.Dirty)
			{
				long offset = this.CalcPageOffset(pageNum);
				this.m_stream.Seek(offset, SeekOrigin.Begin);
				page.Write(this.m_stream);
			}
		}

		private long CalcPageNum(long fileOffset)
		{
			return fileOffset / this.m_bytesPerPage;
		}

		private long CalcPageOffset(long pageNum)
		{
			return pageNum * this.m_bytesPerPage;
		}

		private int CalcOffsetWithinPage(long fileOffset)
		{
			return (int)(fileOffset % this.m_bytesPerPage);
		}
	}
}
