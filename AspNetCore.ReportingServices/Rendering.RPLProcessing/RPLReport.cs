using System;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReport
	{
		private RPLPageContent[] m_rplPaginatedPages;

		private string m_location;

		private string m_description;

		private string m_language;

		private string m_author;

		private int m_autoRefresh;

		private string m_reportName;

		private DateTime m_executionTime;

		private Version m_rplVersion;

		private RPLContext m_rplContext;

		private bool m_consumeContainerWhitespace;

		public Version RPLVersion
		{
			get
			{
				return this.m_rplVersion;
			}
			set
			{
				this.m_rplVersion = value;
			}
		}

		public string ReportName
		{
			get
			{
				return this.m_reportName;
			}
			set
			{
				this.m_reportName = value;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		public string Author
		{
			get
			{
				return this.m_author;
			}
			set
			{
				this.m_author = value;
			}
		}

		public int AutoRefresh
		{
			get
			{
				return this.m_autoRefresh;
			}
			set
			{
				this.m_autoRefresh = value;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
			set
			{
				this.m_executionTime = value;
			}
		}

		public string Location
		{
			get
			{
				return this.m_location;
			}
			set
			{
				this.m_location = value;
			}
		}

		public string Language
		{
			get
			{
				return this.m_language;
			}
			set
			{
				this.m_language = value;
			}
		}

		public RPLPageContent[] RPLPaginatedPages
		{
			get
			{
				return this.m_rplPaginatedPages;
			}
			set
			{
				this.m_rplPaginatedPages = value;
			}
		}

		public bool ConsumeContainerWhitespace
		{
			get
			{
				return this.m_consumeContainerWhitespace;
			}
			set
			{
				this.m_consumeContainerWhitespace = value;
			}
		}

		internal RPLReport()
		{
		}

		public RPLReport(BinaryReader reader)
		{
			this.m_rplContext = new RPLContext(reader);
			RPLReader.ReadReport(this, this.m_rplContext);
		}

		public void GetImage(long offset, Stream imageStream)
		{
			if (offset >= 0 && imageStream != null && this.m_rplContext.BinaryReader != null)
			{
				BinaryReader binaryReader = this.m_rplContext.BinaryReader;
				Stream baseStream = binaryReader.BaseStream;
				baseStream.Seek(offset, SeekOrigin.Begin);
				int num = binaryReader.ReadInt32();
				byte[] array = new byte[4096];
				while (num > 0)
				{
					int num2 = baseStream.Read(array, 0, Math.Min(array.Length, num));
					imageStream.Write(array, 0, num2);
					num -= num2;
				}
			}
		}

		public byte[] GetImage(long offset)
		{
			if (offset >= 0 && this.m_rplContext.BinaryReader != null)
			{
				BinaryReader binaryReader = this.m_rplContext.BinaryReader;
				Stream baseStream = binaryReader.BaseStream;
				baseStream.Seek(offset, SeekOrigin.Begin);
				int num = binaryReader.ReadInt32();
				byte[] array = new byte[num];
				baseStream.Read(array, 0, num);
				return array;
			}
			return null;
		}

		public RPLItemProps GetItemProps(long startOffset, out byte elementType)
		{
			elementType = 0;
			if (startOffset >= 0 && this.m_rplContext.BinaryReader != null)
			{
				return RPLReader.ReadElementProps(startOffset, this.m_rplContext, out elementType);
			}
			return null;
		}

		public RPLItemProps GetItemProps(object rplSource, out byte elementType)
		{
			elementType = 0;
			RPLItemProps rPLItemProps = rplSource as RPLItemProps;
			if (rPLItemProps != null)
			{
				if (rPLItemProps is RPLTextBoxProps)
				{
					elementType = 7;
				}
				else if (rPLItemProps is RPLChartProps)
				{
					elementType = 11;
				}
				else if (rPLItemProps is RPLGaugePanelProps)
				{
					elementType = 14;
				}
				else if (rPLItemProps is RPLMapProps)
				{
					elementType = 21;
				}
				else if (rPLItemProps is RPLImageProps)
				{
					elementType = 9;
				}
				else if (rPLItemProps is RPLLineProps)
				{
					elementType = 8;
				}
				return rPLItemProps;
			}
			long num = (long)rplSource;
			if (num >= 0 && this.m_rplContext.BinaryReader != null)
			{
				return RPLReader.ReadElementProps(num, this.m_rplContext, out elementType);
			}
			return null;
		}

		public void Release()
		{
			if (this.m_rplContext != null)
			{
				this.m_rplContext.Release();
				this.m_rplContext = null;
			}
		}
	}
}
