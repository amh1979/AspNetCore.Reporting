using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class TableData
	{
		internal enum Positions
		{
			Top,
			Left,
			Bottom,
			Right
		}

		private const int DefaultOffset = 14;

		private const int BrcSize = 8;

		private const int Brc97Size = 4;

		private const int MaxTapxSprmSize = 2048;

		internal static byte TC_ROTATEFONT = 16;

		internal static byte TC_BACKWARD = 8;

		internal static byte TC_VERTICAL = 4;

		private CellBorderColor m_borderColors;

		private CellShading m_cellShading;

		private CellPadding m_cellPadding;

		private SprmBuffer m_tapx;

		private int m_numColumns;

		private float[] m_columnWidths;

		private BorderCode[] m_tableBorders;

		private BorderCode[] m_cellBorders;

		private byte[] m_tableShd;

		private int m_startOffset;

		private int m_rowHeight;

		private bool m_writeRowHeight = true;

		private bool m_writeExactRowHeight;

		private bool m_layoutTable;

		internal bool WriteRowHeight
		{
			get
			{
				return this.m_writeRowHeight;
			}
			set
			{
				this.m_writeRowHeight = value;
			}
		}

		internal bool WriteExactRowHeight
		{
			get
			{
				return this.m_writeExactRowHeight;
			}
			set
			{
				this.m_writeExactRowHeight = value;
			}
		}

		internal byte[] Tapx
		{
			get
			{
				this.m_cellPadding.Finish();
				this.m_rowHeight -= this.m_cellPadding.HeightAdjustment;
				this.m_rowHeight = Math.Min(this.m_rowHeight, 31680);
				if (this.WriteRowHeight && this.m_rowHeight > 0)
				{
					short num = (short)this.m_rowHeight;
					if (this.WriteExactRowHeight && num > 0)
					{
						num = (short)(num * -1);
					}
					this.m_tapx.AddSprm(37895, num, null);
				}
				int num2 = this.m_tapx.Offset + this.m_borderColors.SprmSize + this.m_cellShading.SprmSize + this.m_cellPadding.SprmSize;
				byte[] array = new byte[num2];
				Array.Copy(this.m_tapx.Buf, array, this.m_tapx.Offset);
				int offset = this.m_tapx.Offset;
				byte[] array2 = this.m_borderColors.ToByteArray();
				Array.Copy(array2, 0, array, offset, array2.Length);
				offset += array2.Length;
				byte[] array3 = this.m_cellShading.ToByteArray();
				Array.Copy(array3, 0, array, offset, array3.Length);
				offset += array3.Length;
				byte[] array4 = this.m_cellPadding.ToByteArray();
				Array.Copy(array4, 0, array, offset, array4.Length);
				return array;
			}
		}

		internal TableData(int tableLevel, bool layoutTable)
		{
			this.m_tapx = new SprmBuffer(2048, 2);
			this.m_tapx.AddSprm(9238, 1, null);
			this.m_tapx.AddSprm((ushort)((tableLevel > 1) ? 9292 : 9239), 1, null);
			this.m_tapx.AddSprm(26185, tableLevel, null);
			this.m_tableBorders = new BorderCode[4];
			this.m_tableBorders[0] = new BorderCode();
			this.m_tableBorders[1] = new BorderCode();
			this.m_tableBorders[2] = new BorderCode();
			this.m_tableBorders[3] = new BorderCode();
			this.m_cellBorders = new BorderCode[4];
			this.m_cellBorders[0] = new BorderCode();
			this.m_cellBorders[1] = new BorderCode();
			this.m_cellBorders[2] = new BorderCode();
			this.m_cellBorders[3] = new BorderCode();
			this.m_layoutTable = layoutTable;
		}

		internal void InitTableRow(float leftStart, float rowHeight, float[] columnWidths, AutoFit autoFit)
		{
			if (this.m_tapx.Offset == 14)
			{
				float num = 0f;
				foreach (float num2 in columnWidths)
				{
					num += num2;
				}
				if (num / 25.399999618530273 > 22.0)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page width of the report exceeds 22 inches (55.88 centimeters).");
				}
				else if (columnWidths.Length > 63)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The rendered report contains a table that has more than 63 columns.");
				}
				this.m_columnWidths = columnWidths;
				this.m_numColumns = columnWidths.Length;
				this.m_borderColors = new CellBorderColor(this.m_numColumns);
				this.m_cellShading = new CellShading(this.m_numColumns, this.m_tableShd);
				this.m_cellPadding = new CellPadding(this.m_numColumns);
				this.CreateTableDefSprm(leftStart);
				if (autoFit != AutoFit.Never)
				{
					this.m_tapx.AddSprm(13845, (int)autoFit, null);
				}
				if (this.m_tableShd != null)
				{
					this.m_tapx.AddSprm(54880, 0, this.m_tableShd);
				}
				this.m_startOffset = this.m_tapx.Offset;
			}
			else
			{
				this.m_tapx.Clear(this.m_startOffset, this.m_tapx.Buf.Length - this.m_startOffset);
				int tcLocation = this.GetTcLocation(this.m_numColumns, 0);
				this.m_tapx.Clear(tcLocation, this.m_numColumns * 20);
				this.m_borderColors.Reset();
				this.m_cellShading.Reset();
				this.m_cellPadding.Reset();
				this.m_tapx.Reset(this.m_startOffset);
			}
			this.m_rowHeight = Word97Writer.ToTwips(rowHeight);
			this.WriteExactRowHeight = false;
			this.WriteRowHeight = true;
		}

		private void CreateTableDefSprm(float leftStart)
		{
			int num = 1 + 2 * (this.m_numColumns + 1) + 20 * this.m_numColumns;
			byte[] array = new byte[num + 4];
			int num2 = 0;
			LittleEndian.PutUShort(array, num2, 54792);
			num2 += 2;
			LittleEndian.PutUShort(array, num2, (ushort)(num + 1));
			num2 += 2;
			array[num2++] = (byte)this.m_numColumns;
			ushort num4 = Word97Writer.ToTwips(leftStart);
			if (num4 > 31680)
			{
				num4 = 31680;
			}
			LittleEndian.PutUShort(array, num2, num4);
			num2 += 2;
			for (int i = 0; i < this.m_numColumns; i++)
			{
				ushort num5 = Word97Writer.ToTwips(this.m_columnWidths[i]);
				if (num5 == 0)
				{
					num5 = 1;
				}
				num4 = (ushort)(num4 + num5);
				if (num4 > 31680)
				{
					num4 = 31680;
				}
				LittleEndian.PutUShort(array, num2, num4);
				num2 += 2;
			}
			this.m_tapx.AddRawSprmData(array);
		}

		internal void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			int tcLocation = this.GetTcLocation(numColumns, cellIndex);
			this.m_tapx.Buf[tcLocation] |= (byte)(firstVertMerge ? 96 : 0);
			this.m_tapx.Buf[tcLocation] |= (byte)(vertMerge ? 32 : 0);
			this.m_tapx.Buf[tcLocation] |= (byte)(firstHorzMerge ? 1 : 0);
			this.m_tapx.Buf[tcLocation] |= (byte)(horzMerge ? 2 : 0);
		}

		internal void AddCellStyleProp(int cellIndex, byte code, object value)
		{
			switch (code)
			{
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 27:
			case 28:
			case 29:
			case 31:
			case 32:
			case 33:
			case 35:
				break;
			case 0:
				this.SetCellBorderColor(cellIndex, (string)value);
				break;
			case 1:
				this.SetCellBorderColor(cellIndex, (string)value, Positions.Left);
				break;
			case 2:
				this.SetCellBorderColor(cellIndex, (string)value, Positions.Right);
				break;
			case 3:
				this.SetCellBorderColor(cellIndex, (string)value, Positions.Top);
				break;
			case 4:
				this.SetCellBorderColor(cellIndex, (string)value, Positions.Bottom);
				break;
			case 5:
				this.SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value);
				break;
			case 6:
				this.SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Left);
				break;
			case 7:
				this.SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Right);
				break;
			case 8:
				this.SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Top);
				break;
			case 9:
				this.SetCellBorderStyle(cellIndex, (RPLFormat.BorderStyles)value, Positions.Bottom);
				break;
			case 10:
				this.SetCellBorderWidth(cellIndex, (string)value);
				break;
			case 11:
				this.SetCellBorderWidth(cellIndex, (string)value, Positions.Left);
				break;
			case 12:
				this.SetCellBorderWidth(cellIndex, (string)value, Positions.Right);
				break;
			case 13:
				this.SetCellBorderWidth(cellIndex, (string)value, Positions.Top);
				break;
			case 14:
				this.SetCellBorderWidth(cellIndex, (string)value, Positions.Bottom);
				break;
			case 26:
				this.RenderVerticalAlign(cellIndex, (RPLFormat.VerticalAlignments)value);
				break;
			case 30:
				this.RenderWritingMode(cellIndex, (RPLFormat.WritingModes)value);
				break;
			case 34:
				this.SetTableCellShading(cellIndex, (string)value);
				break;
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			int twips = defaultValue;
			if (value != null)
			{
				twips = Word97Writer.ToTwips((string)value);
			}
			switch (code)
			{
			case 15:
				this.m_cellPadding.SetPaddingLeft(cellIndex, twips);
				break;
			case 16:
				this.m_cellPadding.SetPaddingRight(cellIndex, twips);
				break;
			case 17:
				this.m_cellPadding.SetPaddingTop(cellIndex, twips);
				break;
			case 18:
				this.m_cellPadding.SetPaddingBottom(cellIndex, twips);
				break;
			}
		}

		internal void AddCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			BorderCode borderCode = new BorderCode();
			byte b = (byte)(slantUp ? 32 : 16);
			borderCode.Ico24 = Word97Writer.ToIco24(color);
			double num = Word97Writer.ToPoints(width);
			borderCode.LineWidth = (byte)(num * 8.0);
			borderCode.Style = this.ConvertBorderStyle(style);
			byte[] array = new byte[11]
			{
				(byte)cellIndex,
				(byte)(cellIndex + 1),
				b,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			borderCode.Serialize2K3(array, 3);
			this.m_tapx.AddSprm(54831, 0, array);
		}

		private void SetCellBorderWidth(int cellIndex, string width, Positions position)
		{
			double num = Word97Writer.ToPoints(width);
			this.m_cellBorders[(int)position].LineWidth = (int)(num * 8.0);
		}

		private void SetCellBorderWidth(int cellIndex, string width)
		{
			double num = Word97Writer.ToPoints(width);
			byte lineWidth = (byte)(num * 8.0);
			for (int i = 0; i < this.m_cellBorders.Length; i++)
			{
				this.m_cellBorders[i].LineWidth = lineWidth;
			}
		}

		private void SetCellBorderStyle(int cellIndex, RPLFormat.BorderStyles borderStyle, Positions position)
		{
			this.m_cellBorders[(int)position].Style = this.ConvertBorderStyle(borderStyle);
		}

		private void SetCellBorderStyle(int cellIndex, RPLFormat.BorderStyles borderStyle)
		{
			LineStyle style = this.ConvertBorderStyle(borderStyle);
			for (int i = 0; i < this.m_cellBorders.Length; i++)
			{
				this.m_cellBorders[i].Style = style;
			}
		}

		private void SetCellBorderColor(int cellIndex, string color)
		{
			int ico = Word97Writer.ToIco24(color);
			for (int i = 0; i < this.m_cellBorders.Length; i++)
			{
				this.m_cellBorders[i].Ico24 = ico;
			}
		}

		private void SetCellBorderColor(int cellIndex, string color, Positions position)
		{
			int ico = Word97Writer.ToIco24(color);
			this.m_cellBorders[(int)position].Ico24 = ico;
		}

		private void SetTableCellShading(int index, string color)
		{
			if (!color.Equals("Transparent"))
			{
				int ico = Word97Writer.ToIco24(color);
				this.m_cellShading.SetCellShading(index, ico);
			}
		}

		internal void AddTableStyleProp(byte code, object value)
		{
			switch (code)
			{
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 35:
			case 36:
			case 37:
				break;
			case 0:
				this.SetDefaultBorderColor((string)value);
				break;
			case 1:
				this.SetBorderColor((string)value, Positions.Left);
				break;
			case 2:
				this.SetBorderColor((string)value, Positions.Right);
				break;
			case 3:
				this.SetBorderColor((string)value, Positions.Top);
				break;
			case 4:
				this.SetBorderColor((string)value, Positions.Bottom);
				break;
			case 5:
				this.SetDefaultBorderStyle((RPLFormat.BorderStyles)value);
				break;
			case 6:
				this.SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Left);
				break;
			case 7:
				this.SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Right);
				break;
			case 8:
				this.SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Top);
				break;
			case 9:
				this.SetBorderStyle((RPLFormat.BorderStyles)value, Positions.Bottom);
				break;
			case 10:
				this.SetDefaultBorderWidth((string)value);
				break;
			case 11:
				this.SetBorderWidth((string)value, Positions.Left);
				break;
			case 12:
				this.SetBorderWidth((string)value, Positions.Right);
				break;
			case 13:
				this.SetBorderWidth((string)value, Positions.Top);
				break;
			case 14:
				this.SetBorderWidth((string)value, Positions.Bottom);
				break;
			case 34:
				this.SetTableShading((string)value);
				break;
			}
		}

		internal void SetTableContext(BorderContext borderContext)
		{
			if (borderContext.Top)
			{
				this.m_tableBorders[0] = new BorderCode();
			}
			if (borderContext.Left)
			{
				this.m_tableBorders[1] = new BorderCode();
			}
			if (borderContext.Bottom)
			{
				this.m_tableBorders[2] = new BorderCode();
			}
			if (borderContext.Right)
			{
				this.m_tableBorders[3] = new BorderCode();
			}
		}

		private void SetDefaultBorderColor(string color)
		{
			int color2 = Word97Writer.ToIco24(color);
			for (int i = 0; i < this.m_tableBorders.Length; i++)
			{
				this.m_tableBorders[i].SetColor(color2);
			}
		}

		private void SetTableShading(string color)
		{
			if (!color.Equals("Transparent"))
			{
				int val = Word97Writer.ToIco24(color);
				this.m_tableShd = new byte[10];
				LittleEndian.PutInt(this.m_tableShd, 4, val);
			}
		}

		private void SetBorderWidth(string width, Positions position)
		{
			this.m_tableBorders[(int)position].LineWidth = (int)(Word97Writer.ToPoints(width) * 8.0);
		}

		private void SetDefaultBorderWidth(string width)
		{
			for (int i = 0; i < this.m_tableBorders.Length; i++)
			{
				this.m_tableBorders[i].LineWidth = (int)(Word97Writer.ToPoints(width) * 8.0);
			}
		}

		private void SetBorderStyle(RPLFormat.BorderStyles style, Positions position)
		{
			this.m_tableBorders[(int)position].Style = this.ConvertBorderStyle(style);
		}

		private void SetDefaultBorderStyle(RPLFormat.BorderStyles style)
		{
			for (int i = 0; i < this.m_tableBorders.Length; i++)
			{
				this.m_tableBorders[i].Style = this.ConvertBorderStyle(style);
			}
		}

		private void SetBorderColor(string color, Positions position)
		{
			int color2 = Word97Writer.ToIco24(color);
			this.m_tableBorders[(int)position].SetColor(color2);
		}

		private void RenderVerticalAlign(int cellIndex, RPLFormat.VerticalAlignments vertAlign)
		{
			int num = 0;
			switch (vertAlign)
			{
			case RPLFormat.VerticalAlignments.Bottom:
				num = 2;
				break;
			case RPLFormat.VerticalAlignments.Middle:
				num = 1;
				break;
			}
			int tcLocation = this.GetTcLocation(this.m_numColumns, cellIndex);
			ushort uShort = LittleEndian.getUShort(this.m_tapx.Buf, tcLocation);
			uShort = (ushort)(uShort | (ushort)(num << 7));
			LittleEndian.PutUShort(this.m_tapx.Buf, tcLocation, uShort);
		}

		private void RenderWritingMode(int cellIndex, RPLFormat.WritingModes writingModes)
		{
			if (writingModes != RPLFormat.WritingModes.Vertical && writingModes != RPLFormat.WritingModes.Rotate270)
			{
				return;
			}
			int tcLocation = this.GetTcLocation(this.m_numColumns, cellIndex);
			ushort uShort = LittleEndian.getUShort(this.m_tapx.Buf, tcLocation);
			uShort = (ushort)(uShort | (byte)((writingModes == RPLFormat.WritingModes.Vertical) ? (TableData.TC_ROTATEFONT | TableData.TC_VERTICAL) : (TableData.TC_BACKWARD | TableData.TC_VERTICAL)));
			LittleEndian.PutUShort(this.m_tapx.Buf, tcLocation, uShort);
		}

		internal void WriteBrc97(byte[] grpprl, int offset, BorderCode brc)
		{
			brc.Serialize97(grpprl, offset);
		}

		private int GetTcLocation(int numColumns, int cellIndex)
		{
			return 19 + 2 * (numColumns + 1) + 20 * cellIndex;
		}

		private LineStyle ConvertBorderStyle(RPLFormat.BorderStyles style)
		{
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				return LineStyle.DashSmallGap;
			case RPLFormat.BorderStyles.Dotted:
				return LineStyle.Dot;
			case RPLFormat.BorderStyles.Double:
				return LineStyle.Double;
			case RPLFormat.BorderStyles.None:
				return LineStyle.None;
			case RPLFormat.BorderStyles.Solid:
				return LineStyle.Single;
			default:
				return LineStyle.None;
			}
		}

		internal void WriteTableCellEnd(int cellIndex, BorderContext borderContext)
		{
			int offset = this.GetTcLocation(this.m_numColumns, cellIndex) + 4;
			this.UpdateBorderColor(Positions.Top, offset, cellIndex, borderContext.Top);
			this.UpdateBorderColor(Positions.Left, offset, cellIndex, borderContext.Left);
			this.UpdateBorderColor(Positions.Bottom, offset, cellIndex, borderContext.Bottom);
			this.UpdateBorderColor(Positions.Right, offset, cellIndex, borderContext.Right);
		}

		private void UpdateBorderColor(Positions position, int offset, int cellIndex, bool borderContext)
		{
			if (!borderContext)
			{
				if (!this.m_cellBorders[(int)position].Empty)
				{
					this.m_cellBorders[(int)position].Serialize97(this.m_tapx.Buf, offset + (int)position * 4);
					this.m_borderColors.SetColor(position, cellIndex, this.m_cellBorders[(int)position].Ico24);
				}
			}
			else if (!this.m_tableBorders[(int)position].Empty)
			{
				this.m_tableBorders[(int)position].Serialize97(this.m_tapx.Buf, offset + (int)position * 4);
				this.m_borderColors.SetColor(position, cellIndex, this.m_tableBorders[(int)position].Ico24);
			}
			this.m_cellBorders[(int)position] = new BorderCode();
		}

		internal void ClearCellBorder(Positions position)
		{
			this.m_cellBorders[(int)position] = new BorderCode();
		}
	}
}
