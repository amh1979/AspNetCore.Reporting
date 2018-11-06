using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal abstract class EscherRecord : ICloneable
	{
		internal class EscherRecordHeader
		{
			private ushort options;

			private ushort recordId;

			private int remainingBytes;

			internal virtual ushort Options
			{
				get
				{
					return this.options;
				}
			}

			internal virtual ushort RecordId
			{
				get
				{
					return this.recordId;
				}
			}

			internal virtual int RemainingBytes
			{
				get
				{
					return this.remainingBytes;
				}
			}

			internal EscherRecordHeader()
			{
			}

			internal static EscherRecordHeader readHeader(byte[] data, int offset)
			{
				EscherRecordHeader escherRecordHeader = new EscherRecordHeader();
				escherRecordHeader.options = LittleEndian.getUShort(data, offset);
				escherRecordHeader.recordId = LittleEndian.getUShort(data, offset + 2);
				escherRecordHeader.remainingBytes = LittleEndian.getInt(data, offset + 4);
				return escherRecordHeader;
			}

			public override string ToString()
			{
				return "EscherRecordHeader{options=" + this.options + ", recordId=" + this.recordId + ", remainingBytes=" + this.remainingBytes + "}";
			}
		}

		internal const int HEADER_SIZE = 8;

		private ushort options;

		private ushort recordId;

		internal virtual bool ContainerRecord
		{
			get
			{
				return (this.options & 0xF) == 15;
			}
		}

		internal abstract int RecordSize
		{
			get;
		}

		internal virtual IList ChildRecords
		{
			get
			{
				return ArrayList.ReadOnly(new ArrayList());
			}
			set
			{
				throw new ArgumentException("This record does not support child records.");
			}
		}

		internal abstract string RecordName
		{
			get;
		}

		internal virtual short Instance
		{
			get
			{
				return (short)(this.options >> 4);
			}
		}

		internal EscherRecord()
		{
		}

		protected internal virtual int readHeader(byte[] data, int offset)
		{
			EscherRecordHeader escherRecordHeader = EscherRecordHeader.readHeader(data, offset);
			this.options = escherRecordHeader.Options;
			this.recordId = escherRecordHeader.RecordId;
			return escherRecordHeader.RemainingBytes;
		}

		internal virtual ushort getOptions()
		{
			return this.options;
		}

		internal virtual void setOptions(ushort options)
		{
			this.options = options;
		}

		internal abstract int Serialize(BinaryWriter dataWriter);

		internal virtual ushort GetRecordId()
		{
			return this.recordId;
		}

		internal virtual void SetRecordId(ushort recordId)
		{
			this.recordId = recordId;
		}

		public virtual object Clone()
		{
			throw new SystemException("The class " + base.GetType().FullName + " needs to define a clone method");
		}

		internal virtual EscherRecord GetChild(int index)
		{
			return (EscherRecord)this.ChildRecords[index];
		}

		internal virtual void Display(StreamWriter w, int indent)
		{
			for (int i = 0; i < indent * 4; i++)
			{
				w.Write(' ');
			}
			w.WriteLine(this.RecordName);
		}
	}
}
