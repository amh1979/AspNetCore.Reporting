using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class StyleWriterStream : StyleWriter
	{
		private BinaryWriter m_writer;

		public StyleWriterStream(BinaryWriter writer)
		{
			this.m_writer = writer;
		}

		public void WriteNotNull(byte rplId, string value)
		{
			if (value != null)
			{
				this.Write(rplId, value);
			}
		}

		public void WriteNotNull(byte rplId, byte? value)
		{
			int? nullable = value;
			if (nullable.HasValue)
			{
				this.Write(rplId, value.Value);
			}
		}

		public void Write(byte rplId, string value)
		{
			this.m_writer.Write(rplId);
			this.m_writer.Write(value);
		}

		public void Write(byte rplId, byte value)
		{
			this.m_writer.Write(rplId);
			this.m_writer.Write(value);
		}

		public void Write(byte rplId, int value)
		{
			this.m_writer.Write(rplId);
			this.m_writer.Write(value);
		}

		public void Write(byte rplId, float value)
		{
			this.m_writer.Write(rplId);
			this.m_writer.Write(value);
		}

		public void Write(byte rplId, bool value)
		{
			this.m_writer.Write(rplId);
			this.m_writer.Write(value);
		}

		public void WriteAll(Dictionary<byte, object> styles)
		{
			foreach (KeyValuePair<byte, object> style in styles)
			{
				string text = style.Value as string;
				if (text != null)
				{
					this.Write(style.Key, text);
				}
				else
				{
					byte? nullable = (byte?)(object)(style.Value as byte?);
					int? nullable2 = nullable;
					if (nullable2.HasValue)
					{
						this.Write(style.Key, nullable.Value);
					}
					else
					{
						int? nullable3 = (int?)(object)(style.Value as int?);
						if (nullable3.HasValue)
						{
							this.Write(style.Key, nullable3.Value);
						}
					}
				}
			}
		}

		public void WriteSharedProperty(byte rplId, ReportStringProperty prop)
		{
			if (prop != null && !prop.IsExpression && prop.Value != null)
			{
				this.Write(rplId, prop.Value);
			}
		}

		public void WriteSharedProperty(byte rplId, ReportSizeProperty prop)
		{
			if (prop != null && !prop.IsExpression && prop.Value != null)
			{
				this.Write(rplId, prop.Value.ToString());
			}
		}

		public void WriteSharedProperty(byte rplId, ReportIntProperty prop)
		{
			if (prop != null && !prop.IsExpression)
			{
				this.Write(rplId, prop.Value);
			}
		}
	}
}
