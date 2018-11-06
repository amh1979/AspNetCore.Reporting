using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Font : ICloneable, IFont
	{
		private const short GRBIT_fItalic = 2;

		private const short GRBIT_fStrikeout = 8;

		private const short GRBIT_fOutline = 16;

		private const short GRBIT_fShadow = 32;

		private static readonly byte[] DEFAULT_DATA = new byte[14]
		{
			200,
			0,
			0,
			0,
			255,
			127,
			144,
			1,
			0,
			0,
			0,
			0,
			1,
			0
		};

		private int m_hash;

		private byte[] m_data = new byte[14];

		private string m_fontName = "Arial";

		internal byte[] RecordData
		{
			get
			{
				return this.m_data;
			}
		}

		public int Bold
		{
			get
			{
				return LittleEndianHelper.ReadShortU(this.m_data, 6);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, this.m_data, 6);
			}
		}

		public bool Italic
		{
			get
			{
				return BitField16.GetValue(LittleEndianHelper.ReadShort(this.m_data, 2), 2) == 1;
			}
			set
			{
				this.WriteMaskedValue(2, 2, (short)(value ? 1 : 0));
			}
		}

		public bool Strikethrough
		{
			get
			{
				return BitField16.GetValue(LittleEndianHelper.ReadShort(this.m_data, 2), 8) == 1;
			}
			set
			{
				this.WriteMaskedValue(2, 8, (short)(value ? 1 : 0));
			}
		}

		public ScriptStyle ScriptStyle
		{
			get
			{
				return (ScriptStyle)LittleEndianHelper.ReadShort(this.m_data, 8);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, this.m_data, 8);
			}
		}

		public int Color
		{
			get
			{
				return LittleEndianHelper.ReadShort(this.m_data, 4);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, this.m_data, 4);
			}
		}

		IColor IFont.Color
		{
			set
			{
				this.Color = ((BIFF8Color)value).PaletteIndex;
			}
		}

		public Underline Underline
		{
			get
			{
				return (Underline)LittleEndianHelper.ReadShort(this.m_data, 10);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, this.m_data, 10);
			}
		}

		public CharSet CharSet
		{
			get
			{
				return (CharSet)(byte)LittleEndianHelper.ReadShort(this.m_data, 12);
			}
			set
			{
				LittleEndianHelper.WriteShort((short)value, this.m_data, 12);
			}
		}

		public string Name
		{
			get
			{
				return this.m_fontName;
			}
			set
			{
				this.m_fontName = value;
			}
		}

		public double Size
		{
			get
			{
				return (double)(LittleEndianHelper.ReadShortU(this.m_data, 0) / 20);
			}
			set
			{
				LittleEndianHelper.WriteShortU((ushort)(value * 20.0), this.m_data, 0);
			}
		}

		internal BIFF8Font()
		{
			Array.Copy(BIFF8Font.DEFAULT_DATA, this.m_data, 14);
		}

		internal BIFF8Font(StyleProperties props)
			: this()
		{
			this.Bold = props.Bold;
			if (props.Color != null)
			{
				this.Color = ((BIFF8Color)props.Color).PaletteIndex;
			}
			this.Size = props.Size;
			this.Italic = props.Italic;
			if (props.Name != null)
			{
				this.Name = props.Name;
			}
			this.CharSet = props.CharSet;
			this.ScriptStyle = props.ScriptStyle;
			this.Strikethrough = props.Strikethrough;
			this.Underline = props.Underline;
		}

		private void WriteMaskedValue(int offset, short mask, short value)
		{
			short aVal = BitField16.PutValue(LittleEndianHelper.ReadShort(this.m_data, offset), mask, value);
			LittleEndianHelper.WriteShort(aVal, this.m_data, offset);
		}

		public override bool Equals(object target)
		{
			BIFF8Font bIFF8Font = (BIFF8Font)target;
			if (bIFF8Font.m_fontName.Equals(this.m_fontName))
			{
				for (int i = 0; i < this.m_data.Length; i++)
				{
					if (this.m_data[i] != bIFF8Font.m_data[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.m_hash == 0)
			{
				this.m_hash = this.m_fontName.GetHashCode();
				for (int i = 0; i < this.m_data.Length; i++)
				{
					this.m_hash ^= this.m_data[i] << i;
				}
			}
			return this.m_hash;
		}

		public object Clone()
		{
			BIFF8Font bIFF8Font = (BIFF8Font)base.MemberwiseClone();
			bIFF8Font.m_data = (byte[])this.m_data.Clone();
			bIFF8Font.m_hash = 0;
			return bIFF8Font;
		}
	}
}
