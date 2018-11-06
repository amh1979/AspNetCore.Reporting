using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class ProgressiveReader : IMessageReader, IEnumerable<MessageElement>, IEnumerable, IDisposable
	{
		internal const string Format = "Progressive";

		internal const int MajorVersion = 1;

		internal const int MinorVersion = 0;

		private BinaryReader m_reader;

		private LengthEncodedReadableStream m_lastStream;

		private bool m_enumeratorCreated;

		internal ProgressiveReader(BinaryReader reader)
		{
			this.m_reader = reader;
		}

		private object InternalReadValue(Type type)
		{
			if (type == typeof(string))
			{
				return this.m_reader.ReadString();
			}
			if (type == typeof(string[]))
			{
				return MessageUtil.ReadStringArray(this.m_reader);
			}
			if (type == typeof(bool))
			{
				return this.m_reader.ReadBoolean();
			}
			if (type == typeof(int))
			{
				return this.m_reader.ReadInt32();
			}
			if (type == typeof(Stream))
			{
				this.m_lastStream = new LengthEncodedReadableStream(this.m_reader);
				return this.m_lastStream;
			}
			if (type == typeof(Dictionary<string, object>))
			{
				return this.ReadDictionary();
			}
			throw new NotImplementedException();
		}

		private Dictionary<string, object> ReadDictionary()
		{
			int num = this.m_reader.ReadInt32();
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num);
			int num2 = 0;
			while (num2 < num)
			{
				TypeCode typeCode = (TypeCode)this.m_reader.ReadInt32();
				if (typeCode == TypeCode.Boolean)
				{
					dictionary.Add(this.m_reader.ReadString(), this.m_reader.ReadBoolean());
					num2++;
					continue;
				}
				throw new NotImplementedException();
			}
			return dictionary;
		}

		public IEnumerator<MessageElement> GetEnumerator()
		{
			if (this.m_enumeratorCreated)
			{
				throw new InvalidOperationException();
			}
			this.m_enumeratorCreated = true;
			while (this.m_reader.BaseStream.CanRead)
			{
				string name;
				object value;
				try
				{
					this.VerifyLastStream();
					name = this.m_reader.ReadString();
					if (string.IsNullOrEmpty(name))
					{
						yield break;
					}
					if (StringComparer.OrdinalIgnoreCase.Compare(name, ".") != 0)
					{
						Type type = ProgressiveTypeDictionary.GetType(name);
						if (type == null)
						{
							throw new NotImplementedException();
						}
						value = this.InternalReadValue(type);
						goto IL_00da;
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new IOException("end of stream", innerException);
				}
				catch (IOException)
				{
					throw;
				}
				catch (Exception innerException2)
				{
					throw new IOException("reader", innerException2);
				}
				continue;
				IL_00da:
				yield return new MessageElement(name, value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<MessageElement>)this).GetEnumerator();
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

		public void Dispose()
		{
			if (this.m_lastStream != null)
			{
				this.m_lastStream.Dispose();
				this.m_lastStream = null;
			}
		}
	}
}
