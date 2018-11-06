using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageData
	{
		private byte[] m_data;

		private string m_MIMEType;

		internal string MIMEType
		{
			get
			{
				return this.m_MIMEType;
			}
		}

		internal byte[] Data
		{
			get
			{
				return this.m_data;
			}
		}

		internal ImageData(byte[] data, string mimeType)
		{
			this.m_data = data;
			this.m_MIMEType = mimeType;
		}
	}
}
