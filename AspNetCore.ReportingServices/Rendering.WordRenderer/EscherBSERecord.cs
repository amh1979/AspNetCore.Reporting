using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBSERecord : EscherRecord
	{
		internal const string RECORD_DESCRIPTION = "MsofbtBSE";

		internal const byte BT_ERROR = 0;

		internal const byte BT_UNKNOWN = 1;

		internal const byte BT_EMF = 2;

		internal const byte BT_WMF = 3;

		internal const byte BT_PICT = 4;

		internal const byte BT_JPEG = 5;

		internal const byte BT_PNG = 6;

		internal const byte BT_DIB = 7;

		internal static short RECORD_ID = -4089;

		private byte field_1_blipTypeWin32;

		private byte field_2_blipTypeMacOS;

		private byte[] field_3_uid;

		private ushort field_4_tag;

		private int field_5_size;

		private int field_6_ref;

		private int field_7_offset;

		private byte field_8_usage;

		private byte field_9_name;

		private byte field_10_unused2;

		private byte field_11_unused3;

		private EscherBSESubRecord field_12_sub;

		private bool _hideSub;

		internal override int RecordSize
		{
			get
			{
				return 44 + ((this.field_12_sub != null && !this._hideSub) ? this.field_12_sub.RecordSize : 0);
			}
		}

		internal override string RecordName
		{
			get
			{
				return "BSE";
			}
		}

		internal virtual byte BlipTypeWin32
		{
			get
			{
				return this.field_1_blipTypeWin32;
			}
			set
			{
				this.field_1_blipTypeWin32 = value;
			}
		}

		internal virtual byte BlipTypeMacOS
		{
			get
			{
				return this.field_2_blipTypeMacOS;
			}
			set
			{
				this.field_2_blipTypeMacOS = value;
			}
		}

		internal virtual byte[] Uid
		{
			get
			{
				return this.field_3_uid;
			}
			set
			{
				this.field_3_uid = value;
			}
		}

		internal virtual ushort Tag
		{
			get
			{
				return this.field_4_tag;
			}
			set
			{
				this.field_4_tag = value;
			}
		}

		internal virtual int Size
		{
			get
			{
				return this.field_5_size;
			}
			set
			{
				this.field_5_size = value;
			}
		}

		internal virtual int Ref
		{
			get
			{
				return this.field_6_ref;
			}
			set
			{
				this.field_6_ref = value;
			}
		}

		internal virtual int Offset
		{
			get
			{
				return this.field_7_offset;
			}
			set
			{
				this.field_7_offset = value;
			}
		}

		internal virtual byte Usage
		{
			get
			{
				return this.field_8_usage;
			}
			set
			{
				this.field_8_usage = value;
			}
		}

		internal virtual byte Name
		{
			get
			{
				return this.field_9_name;
			}
			set
			{
				this.field_9_name = value;
			}
		}

		internal virtual byte Unused2
		{
			get
			{
				return this.field_10_unused2;
			}
			set
			{
				this.field_10_unused2 = value;
			}
		}

		internal virtual byte Unused3
		{
			get
			{
				return this.field_11_unused3;
			}
			set
			{
				this.field_11_unused3 = value;
			}
		}

		internal virtual EscherBSESubRecord SubRecord
		{
			get
			{
				return this.field_12_sub;
			}
			set
			{
				this.field_12_sub = value;
			}
		}

		internal EscherBSERecord()
		{
		}

		internal override int Serialize(BinaryWriter dataWriter)
		{
			dataWriter.Write(this.getOptions());
			dataWriter.Write(this.GetRecordId());
			int value = ((this.field_12_sub != null && !this._hideSub) ? this.field_12_sub.RecordSize : 0) + 36;
			dataWriter.Write(value);
			dataWriter.Write(this.field_1_blipTypeWin32);
			dataWriter.Write(this.field_2_blipTypeMacOS);
			dataWriter.Write(this.field_3_uid);
			dataWriter.Write(this.field_4_tag);
			dataWriter.Write(this.field_5_size);
			dataWriter.Write(this.field_6_ref);
			dataWriter.Write(this.field_7_offset);
			dataWriter.Write(this.field_8_usage);
			dataWriter.Write(this.field_9_name);
			dataWriter.Write(this.field_10_unused2);
			dataWriter.Write(this.field_11_unused3);
			if (this.field_12_sub != null && !this._hideSub)
			{
				this.field_12_sub.Serialize(dataWriter);
			}
			return 44 + ((this.field_12_sub != null && !this._hideSub) ? this.field_12_sub.RecordSize : 0);
		}

		internal virtual string GetBlipType(byte b)
		{
			switch (b)
			{
			case 0:
				return " ERROR";
			case 1:
				return " UNKNOWN";
			case 2:
				return " EMF";
			case 3:
				return " WMF";
			case 4:
				return " PICT";
			case 5:
				return " JPEG";
			case 6:
				return " PNG";
			case 7:
				return " DIB";
			default:
				if (b < 32)
				{
					return " NotKnown";
				}
				return " Client";
			}
		}

		internal virtual void hideSub()
		{
			this._hideSub = true;
		}
	}
}
