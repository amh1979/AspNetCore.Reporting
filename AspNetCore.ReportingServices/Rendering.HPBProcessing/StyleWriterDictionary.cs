using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterDictionary : StyleWriter
	{
		private Dictionary<byte, object> m_styles;

		public StyleWriterDictionary(Dictionary<byte, object> styles)
		{
			this.m_styles = styles;
		}

		public void Write(byte rplId, string value)
		{
			this.m_styles.Add(rplId, value);
		}

		public void Write(byte rplId, byte value)
		{
			this.m_styles.Add(rplId, value);
		}

		public void Write(byte rplId, int value)
		{
			this.m_styles.Add(rplId, value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				this.m_styles.Add(style.Key, style.Value);
			}
		}
	}
}
