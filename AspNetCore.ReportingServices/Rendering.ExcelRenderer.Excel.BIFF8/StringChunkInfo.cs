namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class StringChunkInfo
	{
		private bool mCompressed;

		private byte[] mData = StringChunkInfo.EMPTYARRAY;

		private int mCharPos;

		private int mCharsTotal;

		private static byte[] EMPTYARRAY = new byte[0];

		internal byte[] Bytes
		{
			get
			{
				return this.mData;
			}
		}

		internal int CharPos
		{
			get
			{
				return this.mCharPos;
			}
			set
			{
				this.mCharPos = value;
			}
		}

		internal int CharsTotal
		{
			get
			{
				return this.mCharsTotal;
			}
			set
			{
				this.mData = StringChunkInfo.EMPTYARRAY;
				this.mCharPos = 0;
				this.mCharsTotal = value;
			}
		}

		internal byte[] Data
		{
			get
			{
				return this.mData;
			}
			set
			{
				if (value == null)
				{
					this.mData = StringChunkInfo.EMPTYARRAY;
				}
				else
				{
					this.mData = value;
				}
			}
		}

		internal bool Compressed
		{
			get
			{
				return this.mCompressed;
			}
			set
			{
				this.mCompressed = value;
			}
		}

		internal bool HasMore
		{
			get
			{
				return this.mCharPos < this.mCharsTotal;
			}
		}
	}
}
