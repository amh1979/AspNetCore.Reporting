using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class PromoteLocalitySpaceManager : ISpaceManager
	{
		private long m_blockSize;

		private long m_position;

		private long m_streamEnd;

		private List<FileBlock> m_blocks;

		public long StreamEnd
		{
			get
			{
				return this.m_streamEnd;
			}
			set
			{
				this.m_streamEnd = value;
				this.m_position = value;
			}
		}

		internal PromoteLocalitySpaceManager(long blockSize)
		{
			this.m_blockSize = blockSize;
			this.m_position = 0L;
			this.m_streamEnd = 0L;
			this.m_blocks = new List<FileBlock>(10);
		}

		public void Seek(long offset, SeekOrigin origin)
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
				this.m_position = this.m_streamEnd + offset;
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
		}

		private FileBlock GetBlock(long offset)
		{
			FileBlock result = null;
			int num = (int)(offset / this.m_blockSize);
			if (num < this.m_blocks.Count)
			{
				result = this.m_blocks[num];
			}
			return result;
		}

		private FileBlock GetOrCreateBlock(long offset)
		{
			int num = (int)(offset / this.m_blockSize);
			if (num >= this.m_blocks.Count)
			{
				for (int i = this.m_blocks.Count - 1; i < num; i++)
				{
					this.m_blocks.Add(null);
				}
				this.m_blocks.Add(new FileBlock());
			}
			FileBlock fileBlock = this.m_blocks[num];
			if (fileBlock == null)
			{
				fileBlock = new FileBlock();
				this.m_blocks[num] = fileBlock;
			}
			return fileBlock;
		}

		public void Free(long offset, long size)
		{
			FileBlock orCreateBlock = this.GetOrCreateBlock(offset);
			orCreateBlock.Free(offset, size);
		}

		public long AllocateSpace(long size)
		{
			long num = -1L;
			num = this.SearchBlock(this.m_position, size);
			long num2 = this.m_position - this.m_blockSize;
			long num3 = this.m_position + this.m_blockSize;
			while (true)
			{
				if (num != -1)
				{
					break;
				}
				if (num2 < 0 && num3 >= this.m_streamEnd)
				{
					break;
				}
				if (num2 >= 0)
				{
					num = this.SearchBlock(num2, size);
				}
				if (num3 < this.m_streamEnd && num == -1)
				{
					num = this.SearchBlock(num3, size);
				}
				num2 -= this.m_blockSize;
				num3 += this.m_blockSize;
			}
			if (num == -1)
			{
				num = this.m_streamEnd;
				this.m_streamEnd += size;
			}
			return num;
		}

		private long SearchBlock(long offset, long size)
		{
			long result = -1L;
			FileBlock block = this.GetBlock(offset);
			if (block != null)
			{
				result = block.AllocateSpace(size);
			}
			return result;
		}

		public long Resize(long offset, long oldSize, long newSize)
		{
			this.Free(offset, oldSize);
			return this.AllocateSpace(newSize);
		}

		public void TraceStats()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "LocalitySpaceManager Stats. StreamSize: {0} MB. FileBlocks:", this.m_streamEnd / 1048576);
			for (int i = 0; i < this.m_blocks.Count; i++)
			{
				FileBlock fileBlock = this.m_blocks[i];
				if (fileBlock != null)
				{
					fileBlock.TraceStats((i * this.m_blockSize / 1048576).ToString(CultureInfo.InvariantCulture));
				}
			}
		}
	}
}
