using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherSpRecord : EscherRecord
	{
		internal const string RECORD_DESCRIPTION = "MsofbtSp";

		internal const int FLAG_GROUP = 1;

		internal const int FLAG_CHILD = 2;

		internal const int FLAG_PATRIARCH = 4;

		internal const int FLAG_DELETED = 8;

		internal const int FLAG_OLESHAPE = 16;

		internal const int FLAG_HAVEMASTER = 32;

		internal const int FLAG_FLIPHORIZ = 64;

		internal const int FLAG_FLIPVERT = 128;

		internal const int FLAG_CONNECTOR = 256;

		internal const int FLAG_HAVEANCHOR = 512;

		internal const int FLAG_BACKGROUND = 1024;

		internal const int FLAG_HASSHAPETYPE = 2048;

		internal static ushort RECORD_ID = 61450;

		private int field_1_shapeId;

		private int field_2_flags;

		internal override int RecordSize
		{
			get
			{
				return 16;
			}
		}

		internal override string RecordName
		{
			get
			{
				return "Sp";
			}
		}

		internal virtual int ShapeId
		{
			get
			{
				return this.field_1_shapeId;
			}
			set
			{
				this.field_1_shapeId = value;
			}
		}

		internal virtual int Flags
		{
			get
			{
				return this.field_2_flags;
			}
			set
			{
				this.field_2_flags = value;
			}
		}

		internal EscherSpRecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.getOptions());
			dataWriter.Write(this.GetRecordId());
			int value = 8;
			dataWriter.Write(value);
			dataWriter.Write(this.field_1_shapeId);
			dataWriter.Write(this.field_2_flags);
			return 16;
		}

		internal override ushort GetRecordId()
		{
			return EscherSpRecord.RECORD_ID;
		}
	}
}
