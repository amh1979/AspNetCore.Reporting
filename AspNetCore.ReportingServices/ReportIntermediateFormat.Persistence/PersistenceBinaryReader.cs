using AspNetCore.ReportingServices.ReportProcessing;
//
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal class PersistenceBinaryReader : BinaryReader
	{
		internal long StreamPosition
		{
			get
			{
				return this.BaseStream.Position;
			}
			set
			{
				this.BaseStream.Position = value;
			}
		}

		internal bool EOS
		{
			get
			{
				return base.BaseStream.Position == base.BaseStream.Length;
			}
		}

		internal PersistenceBinaryReader(Stream str)
			: base(str)
		{
		}

		internal bool ReadReference(out int refID, out ObjectType declaredRefType)
		{
			declaredRefType = this.ReadObjectType();
			if (declaredRefType != 0)
			{
				refID = this.ReadInt32();
				return true;
			}
			refID = -1;
			return false;
		}

		internal bool ReadListStart(ObjectType objectType, out int listSize)
		{
			if (this.ReadObjectType() == ObjectType.Null)
			{
				listSize = -1;
				return false;
			}
			listSize = base.Read7BitEncodedInt();
			return true;
		}

		internal bool ReadDictionaryStart(ObjectType objectType, out int dictionarySize)
		{
			if (this.ReadObjectType() == ObjectType.Null)
			{
				dictionarySize = -1;
				return false;
			}
			dictionarySize = base.Read7BitEncodedInt();
			return true;
		}

		internal bool ReadArrayStart(ObjectType objectType, out int arraySize)
		{
			if (this.ReadObjectType() == ObjectType.Null)
			{
				arraySize = -1;
				return false;
			}
			arraySize = base.Read7BitEncodedInt();
			return true;
		}

		internal bool Read2DArrayStart(ObjectType objectType, out int arrayXLength, out int arrayYLength)
		{
			if (this.ReadObjectType() == ObjectType.Null)
			{
				arrayXLength = -1;
				arrayYLength = -1;
				return false;
			}
			arrayXLength = base.Read7BitEncodedInt();
			arrayYLength = base.Read7BitEncodedInt();
			return true;
		}

		internal bool[] ReadBooleanArray()
		{
			bool[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadBoolean();
				}
			}
			return array;
		}

		internal byte[] ReadByteArray()
		{
			byte[] result = null;
			int count = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out count))
			{
				result = this.ReadBytes(count);
			}
			return result;
		}

		internal float[] ReadFloatArray()
		{
			float[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new float[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadSingle();
				}
			}
			return array;
		}

		internal double[] ReadDoubleArray()
		{
			double[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new double[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadDouble();
				}
			}
			return array;
		}

		internal char[] ReadCharArray()
		{
			char[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new char[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadChar();
				}
			}
			return array;
		}

		internal int[] ReadInt32Array()
		{
			int[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadInt32();
				}
			}
			return array;
		}

		internal long[] ReadInt64Array()
		{
			long[] array = null;
			int num = default(int);
			if (this.ReadArrayStart(ObjectType.PrimitiveTypedArray, out num) && num > 0)
			{
				array = new long[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = this.ReadInt64();
				}
			}
			return array;
		}

		public override bool ReadBoolean()
		{
			return this.ReadByte() != 0;
		}

		internal Guid ReadGuid()
		{
			byte[] b = base.ReadBytes(16);
			return new Guid(b);
		}

		public override decimal ReadDecimal()
		{
			byte b = 0;
			int[] array = new int[4];
			b = base.ReadByte();
			for (int i = 0; i < 3; i++)
			{
				if ((b & 1 << i * 2) != 0)
				{
					if ((b & 2 << i * 2) != 0)
					{
						array[i] = base.Read7BitEncodedInt();
					}
					else
					{
						array[i] = base.ReadInt32();
					}
				}
			}
			if ((b & 0x40) != 0)
			{
				array[3] = (base.ReadByte() & 0xFF);
				array[3] <<= 16;
			}
			if ((b & 0x80) != 0)
			{
				array[3] |= -2147483648;
			}
			return new decimal(array);
		}

		public override string ReadString()
		{
			return this.ReadString(true);
		}

		internal string ReadString(bool checkforNull)
		{
			if (checkforNull && this.ReadObjectType() == ObjectType.Null)
			{
				return null;
			}
			return base.ReadString();
		}

		internal DateTime ReadDateTime()
		{
			return new DateTime(this.ReadInt64());
		}

		internal DateTime ReadDateTimeWithKind()
		{
			DateTimeKind kind = (DateTimeKind)this.ReadByte();
			return DateTime.SpecifyKind(new DateTime(this.ReadInt64()), kind);
		}

		internal DateTimeOffset ReadDateTimeOffset()
		{
			DateTime dateTime = this.ReadDateTime();
			TimeSpan offset = this.ReadTimeSpan();
			return new DateTimeOffset(dateTime, offset);
		}

		internal TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(this.ReadInt64());
		}

		internal int ReadEnum()
		{
			return base.Read7BitEncodedInt();
		}

		internal Token ReadToken()
		{
			return (Token)this.ReadByte();
		}

		internal ObjectType ReadObjectType()
		{
			return (ObjectType)base.Read7BitEncodedInt();
		}

		private MemberName ReadMemberName()
		{
			return (MemberName)base.Read7BitEncodedInt();
		}

		internal Declaration ReadDeclaration()
		{
			ObjectType type = this.ReadObjectType();
			ObjectType baseType = this.ReadObjectType();
			int num = base.Read7BitEncodedInt();
			List<MemberInfo> list = new List<MemberInfo>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new MemberInfo(this.ReadMemberName(), this.ReadObjectType(), this.ReadToken(), this.ReadObjectType()));
			}
			return new Declaration(type, baseType, list);
		}

		internal void SkipString()
		{
			if (this.ReadObjectType() != 0)
			{
				this.SkipBytes(base.Read7BitEncodedInt());
			}
		}

		internal void SkipBytes(int bytesToSkip)
		{
			if (bytesToSkip > 0)
			{
				Stream baseStream = this.BaseStream;
				if (baseStream.CanSeek)
				{
					baseStream.Seek(bytesToSkip, SeekOrigin.Current);
				}
				else
				{
					this.ReadBytes(bytesToSkip);
				}
			}
		}

		internal void SkipMultiByteInt()
		{
			base.Read7BitEncodedInt();
		}

		internal void SkipTypedArray(int elementSize)
		{
			int num = base.Read7BitEncodedInt();
			this.SkipBytes(num * elementSize);
		}

		internal void Seek(long newPosition, SeekOrigin seekOrigin)
		{
			Stream baseStream = this.BaseStream;
			if (baseStream.CanSeek)
			{
				baseStream.Seek(newPosition, seekOrigin);
			}
			else
			{
				Global.Tracer.Assert(false, "Seek not supported for this stream.");
			}
		} 
        /*
		internal SqlGeography ReadSqlGeography()
		{
			SqlGeography sqlGeography = new SqlGeography();
			sqlGeography.Read(this);
			return sqlGeography;
		}

		internal SqlGeometry ReadSqlGeometry()
		{
			SqlGeometry sqlGeometry = new SqlGeometry();
			sqlGeometry.Read(this);
			return sqlGeometry;
		} 
        */
	}
}
