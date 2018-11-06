using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterOM : StyleWriter
	{
		private RPLStyleProps m_styleProps;

		public StyleWriterOM(RPLStyleProps styleProps)
		{
			this.m_styleProps = styleProps;
		}

		public void Write(byte rplId, string value)
		{
			this.m_styleProps.Add(rplId, value);
		}

		public void Write(byte rplId, byte value)
		{
			this.m_styleProps.Add(rplId, value);
		}

		public void Write(byte rplId, int value)
		{
			this.m_styleProps.Add(rplId, value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				this.m_styleProps.Add(style.Key, style.Value);
			}
		}
	}
}
