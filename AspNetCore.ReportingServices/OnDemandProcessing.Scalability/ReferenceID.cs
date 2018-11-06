using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal struct ReferenceID
	{
		private const long IsTemporaryMask = 4611686018427387904L;

		private const ulong HasMultiPartMask = 9223372036854775808uL;

		private const long PartitionIDMask = 4294967295L;

		public const long MinimumValidTempID = -9223372036854775808L;

		public const long MaximumValidOffset = 72057594037927935L;

		public const int SizeInBytes = 16;

		private long m_value;

		internal long Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal bool HasMultiPart
		{
			get
			{
				return this.m_value < 0;
			}
			set
			{
				ulong value2 = (ulong)this.m_value;
				value2 = (ulong)(this.m_value = ((!value) ? ((long)(value2 & 0x7FFFFFFFFFFFFFFF)) : ((long)value2 | -9223372036854775808L)));
			}
		}

		internal bool IsTemporary
		{
			get
			{
				return (this.m_value & 0x4000000000000000) != 0;
			}
			set
			{
				if (value)
				{
					this.m_value |= 4611686018427387904L;
				}
				else
				{
					this.m_value &= -4611686018427387905L;
				}
			}
		}

		internal int PartitionID
		{
			get
			{
				return (int)(this.m_value & 4294967295u);
			}
			set
			{
				long num = value;
				num &= 4294967295u;
				this.m_value &= -4294967296L;
				this.m_value |= num;
			}
		}

		internal ReferenceID(long value)
		{
			this.m_value = value;
		}

		internal ReferenceID(bool hasMultiPart, bool isTemporary, int partitionId)
		{
			this.m_value = 0L;
			this.HasMultiPart = hasMultiPart;
			this.IsTemporary = isTemporary;
			this.PartitionID = partitionId;
		}

		public static bool operator ==(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value == id2.m_value;
		}

		public static bool operator !=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value != id2.m_value;
		}

		public static bool operator <(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value < id2.m_value;
		}

		public static bool operator >(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value > id2.m_value;
		}

		public static bool operator <=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value <= id2.m_value;
		}

		public static bool operator >=(ReferenceID id1, ReferenceID id2)
		{
			return id1.m_value >= id2.m_value;
		}

		public override bool Equals(object obj)
		{
			return this.m_value == ((ReferenceID)obj).Value;
		}

		public override int GetHashCode()
		{
			return (int)this.m_value;
		}

		public override string ToString()
		{
			return this.m_value.ToString("x", CultureInfo.InvariantCulture);
		}
	}
}
