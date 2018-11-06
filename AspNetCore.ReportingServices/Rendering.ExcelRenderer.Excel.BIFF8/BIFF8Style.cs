using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Style : ICloneable
	{
		private const int BLOCK1_Offset = 4;

		private const short BLOCK1_fLocked = 1;

		private const short BLOCK1_fHidden = 2;

		private const short BLOCK1_fStyle = 4;

		private const short BLOCK1_f123Prefix = 8;

		private const short BLOCK1_ixfParent = -16;

		private const int BLOCK2_Offset = 6;

		private const short BLOCK2_alc = 7;

		private const short BLOCK2_fWrap = 8;

		private const short BLOCK2_alcV = 112;

		private const short BLOCK2_fJustLast = 128;

		private const short BLOCK2_trot = -256;

		private const int BLOCK3_Offset = 8;

		private const short BLOCK3_cIndent = 15;

		private const short BLOCK3_fShrinkToFit = 16;

		private const short BLOCK3_fMergeCell = 32;

		private const short BLOCK3_iReadingOrder = 192;

		private const short BLOCK3_fAtrNum = 1024;

		private const short BLOCK3_fAtrFnt = 2048;

		private const short BLOCK3_fAtrAlc = 4096;

		private const short BLOCK3_fAtrBdr = 8192;

		private const short BLOCK3_fAtrPat = 16384;

		private const short BLOCK3_fAtrProt = -32768;

		private const int BLOCK4_Offset = 10;

		private const short BLOCK4_dgLeft = 15;

		private const short BLOCK4_dgRight = 240;

		private const short BLOCK4_dgTop = 3840;

		private const short BLOCK4_dgBottom = -4096;

		private const int BLOCK5_Offset = 12;

		private const short BLOCK5_icvLeft = 127;

		private const short BLOCK5_icvRight = 16256;

		private const short BLOCK5_grbitDiag = -16384;

		private const int BLOCK6_Offset = 14;

		private const int BLOCK6_icvTop = 127;

		private const int BLOCK6_icvBottom = 16256;

		private const int BLOCK6_icvDiag = 2080768;

		private const int BLOCK6_dgDiag = 31457280;

		private const int BLOCK6_fls = -67108864;

		private const int BLOCK7_Offset = 18;

		private const short BLOCK7_icvFore = 127;

		private const short BLOCK7_icvBack = 16256;

		private const short BLOCK7_fSxButton = 16384;

		private byte[] m_xfData = new byte[20];

		private int m_hash;

		internal byte[] RecordData
		{
			get
			{
				return this.m_xfData;
			}
		}

		internal int Ifnt
		{
			get
			{
				return LittleEndianHelper.ReadShortU(this.m_xfData, 0);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, this.m_xfData, 0);
			}
		}

		internal int Ifmt
		{
			get
			{
				return LittleEndianHelper.ReadShortU(this.m_xfData, 2);
			}
			set
			{
				LittleEndianHelper.WriteShortU(value, this.m_xfData, 2);
			}
		}

		internal ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				this.SetValue16(10, 15, (int)value);
			}
		}

		internal ExcelBorderStyle BorderRightStyle
		{
			set
			{
				this.SetValue16(10, 240, (int)value);
			}
		}

		internal ExcelBorderStyle BorderTopStyle
		{
			set
			{
				this.SetValue16(10, 3840, (int)value);
			}
		}

		internal ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				this.SetValue16(10, -4096, (int)value);
			}
		}

		internal ExcelBorderStyle BorderOutlineStyle
		{
			set
			{
				this.BorderLeftStyle = value;
				this.BorderRightStyle = value;
				this.BorderTopStyle = value;
				this.BorderBottomStyle = value;
			}
		}

		internal ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				this.SetValue32(14, 31457280, (int)value);
			}
		}

		internal IColor BorderLeftColor
		{
			set
			{
				this.SetValue16(12, 127, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderRightColor
		{
			set
			{
				this.SetValue16(12, 16256, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderTopColor
		{
			set
			{
				this.SetValue32(14, 127, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderBottomColor
		{
			set
			{
				this.SetValue32(14, 16256, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal IColor BorderOutlineColor
		{
			set
			{
				this.BorderLeftColor = value;
				this.BorderRightColor = value;
				this.BorderBottomColor = value;
				this.BorderTopColor = value;
			}
		}

		internal IColor BorderDiagColor
		{
			set
			{
				this.SetValue32(14, 2080768, ((BIFF8Color)value).PaletteIndex);
			}
		}

		internal ExcelBorderPart BorderDiagPart
		{
			set
			{
				switch (value)
				{
				case ExcelBorderPart.DiagonalBoth:
					this.SetValue16(12, -16384, 0);
					break;
				case ExcelBorderPart.DiagonalDown:
					this.SetValue16(12, -16384, 1);
					break;
				case ExcelBorderPart.DiagonalUp:
					this.SetValue16(12, -16384, 2);
					break;
				}
			}
		}

		internal IColor BackgroundColor
		{
			set
			{
				int paletteIndex = ((BIFF8Color)value).PaletteIndex;
				this.SetValue32(14, -67108864, 1);
				this.SetValue16(18, 127, (short)paletteIndex);
			}
		}

		internal int IndentLevel
		{
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > 15)
				{
					value = 15;
				}
				this.SetValue16(8, 15, (short)value);
			}
		}

		internal bool WrapText
		{
			set
			{
				this.SetValue16(6, 8, (short)(value ? 1 : 0));
			}
		}

		internal int Orientation
		{
			set
			{
				if (value > 90)
				{
					if (value != 255)
					{
						value = 90;
					}
				}
				else if (value < 0)
				{
					if (value < -90)
					{
						value = -90;
					}
					value = 90 + Math.Abs(value);
					if (value > 180)
					{
						value = 180;
					}
				}
				this.SetValue16(6, -256, (short)value);
			}
		}

		internal HorizontalAlignment HorizontalAlignment
		{
			set
			{
				this.SetValue16(6, 7, (short)value);
			}
		}

		internal VerticalAlignment VerticalAlignment
		{
			set
			{
				this.SetValue16(6, 112, (short)value);
			}
		}

		internal TextDirection TextDirection
		{
			set
			{
				short value2 = 0;
				if (value == TextDirection.LeftToRight)
				{
					value2 = 1;
				}
				if (value == TextDirection.RightToLeft)
				{
					value2 = 2;
				}
				this.SetValue16(8, 192, value2);
			}
		}

		internal BIFF8Style()
		{
			this.WrapText = true;
			this.VerticalAlignment = VerticalAlignment.Top;
		}

		internal BIFF8Style(StyleProperties props)
			: this()
		{
			if (props.BackgroundColor != null)
			{
				this.BackgroundColor = props.BackgroundColor;
			}
			if (props.BorderBottomColor != null)
			{
				this.BorderBottomColor = props.BorderBottomColor;
			}
			this.BorderBottomStyle = props.BorderBottomStyle;
			if (props.BorderDiagColor != null)
			{
				this.BorderDiagColor = props.BorderDiagColor;
			}
			this.BorderDiagStyle = props.BorderDiagStyle;
			this.BorderDiagPart = props.BorderDiagPart;
			if (props.BorderLeftColor != null)
			{
				this.BorderLeftColor = props.BorderLeftColor;
			}
			this.BorderLeftStyle = props.BorderLeftStyle;
			if (props.BorderRightColor != null)
			{
				this.BorderRightColor = props.BorderRightColor;
			}
			this.BorderRightStyle = props.BorderRightStyle;
			if (props.BorderTopColor != null)
			{
				this.BorderTopColor = props.BorderTopColor;
			}
			this.BorderTopStyle = props.BorderTopStyle;
			this.HorizontalAlignment = props.HorizontalAlignment;
			this.IndentLevel = props.IndentLevel;
			this.Orientation = props.Orientation;
			this.TextDirection = props.TextDirection;
			this.VerticalAlignment = props.VerticalAlignment;
			this.WrapText = props.WrapText;
		}

		private void SetValue16(int offset, short mask, int value)
		{
			short aVal = BitField16.PutValue(LittleEndianHelper.ReadShort(this.m_xfData, offset), mask, (short)value);
			LittleEndianHelper.WriteShort(aVal, this.m_xfData, offset);
		}

		private void SetValue32(int offset, int mask, int value)
		{
			int aVal = BitField32.PutValue(LittleEndianHelper.ReadInt(this.m_xfData, offset), mask, value);
			LittleEndianHelper.WriteInt(aVal, this.m_xfData, offset);
		}

		public override bool Equals(object target)
		{
			BIFF8Style bIFF8Style = (BIFF8Style)target;
			for (int i = 0; i < this.m_xfData.Length; i++)
			{
				byte b = this.m_xfData[i];
				byte b2 = bIFF8Style.m_xfData[i];
				if (b != b2)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.m_hash == 0)
			{
				for (int i = 0; i < this.m_xfData.Length; i++)
				{
					this.m_hash ^= this.m_xfData[i] << i;
				}
			}
			return this.m_hash;
		}

		public object Clone()
		{
			BIFF8Style bIFF8Style = (BIFF8Style)base.MemberwiseClone();
			bIFF8Style.m_xfData = (byte[])this.m_xfData.Clone();
			bIFF8Style.m_hash = 0;
			return bIFF8Style;
		}
	}
}
