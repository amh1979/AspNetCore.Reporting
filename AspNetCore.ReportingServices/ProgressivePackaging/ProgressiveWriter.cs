using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class ProgressiveWriter : IMessageWriter, IDisposable
	{
		internal const string Format = "Progressive";

		internal const int MajorVersion = 1;

		internal const int MinorVersion = 0;

		private BinaryWriter m_writer;

		private bool m_disposed;

		private LengthEncodedWritableStream m_lastStream;

		internal ProgressiveWriter(BinaryWriter writer)
		{
			this.m_writer = writer;
		}

		public void WriteMessage(string name, object value)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			if (StringComparer.OrdinalIgnoreCase.Compare(name, ".") == 0)
			{
				this.m_writer.Write(name);
			}
			else
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Type type = this.VerifyType(name, value.GetType());
				this.VerifyLastStream();
				this.m_writer.Write(name);
				this.InternalWriteValue(value, type);
			}
		}

		public Stream CreateWritableStream(string name)
		{
			this.VerifyType(name, typeof(Stream));
			this.VerifyLastStream();
			this.m_lastStream = new LengthEncodedWritableStream(this.m_writer, name);
			return this.m_lastStream;
		}

		private Type VerifyType(string name, Type type)
		{
			Type type2 = ProgressiveTypeDictionary.GetType(name);
			if (type2 == null)
			{
				throw new NotImplementedException();
			}
			if (!type2.IsAssignableFrom(type))
			{
				throw new ArgumentException("wrong type", "value");
			}
			return type2;
		}

		private void InternalWriteValue(object value, Type type)
		{
			if (type == typeof(string))
			{
				this.m_writer.Write(value as string);
				return;
			}
			if (type == typeof(string[]))
			{
				MessageUtil.WriteStringArray(this.m_writer, value as string[]);
				return;
			}
			if (type == typeof(bool))
			{
				this.m_writer.Write((bool)value);
				return;
			}
			if (type == typeof(int))
			{
				this.m_writer.Write((int)value);
				return;
			}
			if (type == typeof(Stream))
			{
				throw new InvalidOperationException("stream");
			}
			if (type == typeof(Dictionary<string, object>))
			{
				this.Write((Dictionary<string, object>)value);
				return;
			}
			throw new NotImplementedException();
		}

		private void Write(Dictionary<string, object> value)
		{
			this.m_writer.Write(value.Count);
			foreach (KeyValuePair<string, object> item in value)
			{
				if (item.Value is bool)
				{
					this.m_writer.Write(3);
					this.m_writer.Write(item.Key);
					this.m_writer.Write((bool)item.Value);
					continue;
				}
				throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.m_writer.Write(string.Empty);
				this.m_disposed = true;
				if (this.m_lastStream != null)
				{
					this.m_lastStream.Dispose();
					this.m_lastStream = null;
				}
			}
		}

		private void VerifyLastStream()
		{
			if (this.m_lastStream == null)
			{
				return;
			}
			if (this.m_lastStream.Closed)
			{
				this.m_lastStream = null;
				return;
			}
			throw new InvalidOperationException("last stream");
		}
	}
}
