using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class IndexTable : IIndexStrategy
	{
		private const int m_valueSize = 8;

		private Dictionary<int, IndexTablePage> m_pageCache;

		private IndexTablePage m_queueFirstPage;

		private IndexTablePage m_queueLastPage;

		private long m_nextTempId;

		private int m_pageSize;

		private int m_cacheSize;

		private Stream m_stream;

		private IStreamHandler m_streamCreator;

		private long m_nextIdPageNum;

		private long m_nextIdPageSlot;

		private readonly int m_slotsPerPage;

		private readonly int m_idShift;

		public ReferenceID MaxId
		{
			get
			{
				return new ReferenceID((this.m_nextTempId + 1) * -1);
			}
		}

		public IndexTable(IStreamHandler streamCreator, int pageSize, int cacheSize)
		{
			if (pageSize % 8 != 0)
			{
				Global.Tracer.Assert(false, "Page size must be divisible by value size: {0}", 8);
			}
			this.m_streamCreator = streamCreator;
			this.m_stream = null;
			this.m_nextTempId = -1L;
			this.m_pageSize = pageSize;
			this.m_cacheSize = cacheSize;
			this.m_pageCache = new Dictionary<int, IndexTablePage>(this.m_cacheSize);
			this.m_queueFirstPage = null;
			this.m_queueLastPage = null;
			this.m_slotsPerPage = this.m_pageSize / 8;
			this.m_idShift = (int)Math.Log((double)this.m_slotsPerPage, 2.0);
		}

		public ReferenceID GenerateTempId()
		{
			long nextTempId;
			this.m_nextTempId = (nextTempId = this.m_nextTempId) - 1;
			return new ReferenceID(nextTempId);
		}

		public ReferenceID GenerateId(ReferenceID tempId)
		{
			ReferenceID result = tempId;
			if (tempId.IsTemporary)
			{
				if (this.m_nextIdPageSlot >= this.m_slotsPerPage)
				{
					this.m_nextIdPageSlot = 0L;
					this.m_nextIdPageNum += 1L;
				}
				long nextIdPageSlot = this.m_nextIdPageSlot;
				nextIdPageSlot |= this.m_nextIdPageNum << this.m_idShift;
				result = new ReferenceID(nextIdPageSlot);
				this.m_nextIdPageSlot += 1L;
			}
			return result;
		}

		public void Update(ReferenceID id, long value)
		{
			IndexTablePage page = this.GetPage(id.Value);
			this.WriteValue(id.Value, page, value);
		}

		public long Retrieve(ReferenceID id)
		{
			IndexTablePage page = this.GetPage(id.Value);
			return this.ReadValue(id.Value, page);
		}

		public void Close()
		{
			if (this.m_stream != null)
			{
				this.m_stream.Close();
				this.m_stream = null;
			}
		}

		private IndexTablePage GetPage(long id)
		{
			int num = this.CalcPageNum(id);
			IndexTablePage indexTablePage = null;
			if (!this.m_pageCache.TryGetValue(num, out indexTablePage))
			{
				if (this.m_pageCache.Count == this.m_cacheSize)
				{
					if (this.m_stream == null)
					{
						this.m_stream = this.m_streamCreator.OpenStream();
						this.m_streamCreator = null;
						if (!this.m_stream.CanSeek || !this.m_stream.CanRead || !this.m_stream.CanWrite)
						{
							Global.Tracer.Assert(false, "Must be able to Seek, Read, and Write stream");
						}
					}
					indexTablePage = this.QueueExtractFirst();
					int pageNumber = indexTablePage.PageNumber;
					this.m_pageCache.Remove(pageNumber);
					if (indexTablePage.Dirty)
					{
						long offset = this.CalcPageOffset(pageNumber);
						this.m_stream.Seek(offset, SeekOrigin.Begin);
						indexTablePage.Write(this.m_stream);
					}
					long offset2 = this.CalcPageOffset(num);
					this.m_stream.Seek(offset2, SeekOrigin.Begin);
					indexTablePage.Read(this.m_stream);
				}
				else
				{
					indexTablePage = new IndexTablePage(this.m_pageSize);
				}
				indexTablePage.PageNumber = num;
				this.m_pageCache[num] = indexTablePage;
				this.QueueAppendPage(indexTablePage);
			}
			return indexTablePage;
		}

		private long ReadValue(long id, IndexTablePage page)
		{
			long num = 0L;
			byte[] buffer = page.Buffer;
			int num2 = this.CalcValueOffset(id);
			int num3 = num2 + 8;
			for (int i = num2; i < num3; i++)
			{
				num <<= 8;
				num |= buffer[i];
			}
			return num;
		}

		private void WriteValue(long id, IndexTablePage page, long value)
		{
			byte[] buffer = page.Buffer;
			int num = this.CalcValueOffset(id);
			int num2 = num + 8;
			for (int num3 = num2 - 1; num3 >= num; num3--)
			{
				buffer[num3] = (byte)value;
				value >>= 8;
			}
			page.Dirty = true;
		}

		private int CalcPageNum(long id)
		{
			return (int)(id >> this.m_idShift);
		}

		private long CalcPageOffset(long pageNum)
		{
			return pageNum * this.m_pageSize;
		}

		private int CalcValueOffset(long id)
		{
			ulong num = (ulong)(id << 64 - this.m_idShift);
			num >>= 64 - this.m_idShift;
			return (int)num * 8;
		}

		private void QueueAppendPage(IndexTablePage page)
		{
			if (this.m_queueFirstPage == null)
			{
				this.m_queueFirstPage = page;
				this.m_queueLastPage = page;
			}
			else
			{
				page.PreviousPage = this.m_queueLastPage;
				this.m_queueLastPage.NextPage = page;
				this.m_queueLastPage = page;
			}
		}

		private IndexTablePage QueueExtractFirst()
		{
			if (this.m_queueFirstPage == null)
			{
				return null;
			}
			IndexTablePage queueFirstPage = this.m_queueFirstPage;
			this.m_queueFirstPage = queueFirstPage.NextPage;
			queueFirstPage.NextPage = null;
			if (this.m_queueFirstPage == null)
			{
				this.m_queueLastPage = null;
			}
			else
			{
				this.m_queueFirstPage.PreviousPage = null;
			}
			return queueFirstPage;
		}
	}
}
