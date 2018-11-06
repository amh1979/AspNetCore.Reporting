using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class RowHandler
	{
		internal enum TransformResult
		{
			NotHandled,
			Handled,
			Null
		}

		private BinaryWriter m_stream;

		private int m_row;

		private byte m_counter;

		private ushort[] m_xfs = new ushort[256];

		private uint[] m_rks = new uint[256];

		private short m_recType;

		private short m_valueCol1;

		private short m_valueCol2;

		private CellRenderingDetails m_details = default(CellRenderingDetails);

		private static readonly DateTime Epoch = new DateTime(1900, 1, 1, 0, 0, 0);

		private SSTHandler m_stringHandler;

		internal int Row
		{
			get
			{
				return this.m_row;
			}
			set
			{
				this.m_row = value;
			}
		}

		internal RowHandler(BinaryWriter output, int firstRow, SSTHandler sst)
		{
			this.m_stream = output;
			this.m_counter = 0;
			this.m_recType = -1;
			this.m_row = firstRow;
			this.m_valueCol1 = -1;
			this.m_valueCol2 = -1;
			this.m_stringHandler = sst;
		}

		internal bool Add(object value, RichTextInfo richTextInfo, TypeCode type, ExcelDataType excelType, ExcelErrorCode errorCode, short column, ushort ixfe)
		{
			this.m_details.Initialize(this.m_stream, this.m_row, column, ixfe);
			TransformResult transformResult;
			if (errorCode == ExcelErrorCode.None)
			{
				switch (excelType)
				{
				case ExcelDataType.Blank:
					transformResult = this.CreateBlankRecord(this.m_details);
					break;
				case ExcelDataType.Boolean:
					transformResult = this.CreateBoolRecord(value, this.m_details);
					break;
				case ExcelDataType.Number:
				{
					if (type == TypeCode.DateTime)
					{
						value = RowHandler.DateToDays((DateTime)value);
					}
					uint? nullable = default(uint?);
					transformResult = this.CreateRKorNumberRecord((ValueType)value, this.m_details, out nullable);
					if (nullable.HasValue)
					{
						transformResult = this.CreateRKRecord(nullable.Value, this.m_details);
					}
					break;
				}
				case ExcelDataType.String:
				{
					string text = (string)value;
					transformResult = ((text.Length <= 0) ? this.CreateBlankRecord(this.m_details) : this.CreateStringRecord(text, this.m_details));
					break;
				}
				case ExcelDataType.RichString:
					transformResult = this.CreateRichStringRecord(richTextInfo, this.m_details);
					break;
				default:
					transformResult = this.CreateBlankRecord(this.m_details);
					break;
				}
			}
			else
			{
				transformResult = this.CreateErrorRecord(errorCode, this.m_details);
			}
			return TransformResult.Handled == transformResult;
		}

		private TransformResult CreateRichStringRecord(RichTextInfo richTextInfo, CellRenderingDetails details)
		{
			StringWrapperBIFF8 stringWrapperBIFF = richTextInfo.CompleteRun();
			if (stringWrapperBIFF.Cch > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(details.Row.ToString(CultureInfo.InvariantCulture), details.Column.ToString(CultureInfo.InvariantCulture)));
			}
			int isst = this.m_stringHandler.AddString(stringWrapperBIFF);
			this.OnCellBegin(253, details.Column);
			RecordFactory.LABELSST(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, (uint)isst);
			return TransformResult.Handled;
		}

		internal TransformResult CreateStringRecord(string input, CellRenderingDetails details)
		{
			if (input.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(details.Row.ToString(CultureInfo.InvariantCulture), details.Column.ToString(CultureInfo.InvariantCulture)));
			}
			if (input.Length < 256)
			{
				this.OnCellBegin(516, details.Column);
				RecordFactory.LABEL(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, input.ToString());
			}
			else
			{
				int isst = this.m_stringHandler.AddString(input.ToString());
				this.OnCellBegin(253, details.Column);
				RecordFactory.LABELSST(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, (uint)isst);
			}
			return TransformResult.Handled;
		}

		internal TransformResult CreateBoolRecord(object val, CellRenderingDetails details)
		{
			if (val == null)
			{
				return TransformResult.Null;
			}
			bool value = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
			return this.CreateBoolErrRecord(Convert.ToByte(value), false, details);
		}

		internal TransformResult CreateErrorRecord(ExcelErrorCode errorCode, CellRenderingDetails details)
		{
			return this.CreateBoolErrRecord((byte)errorCode, true, details);
		}

		private TransformResult CreateBoolErrRecord(byte val, bool isError, CellRenderingDetails details)
		{
			this.OnCellBegin(517, details.Column);
			RecordFactory.BOOLERR(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, val, isError);
			return TransformResult.Handled;
		}

		internal TransformResult CreateRKorNumberRecord(ValueType val, CellRenderingDetails details, out uint? rkValue)
		{
			if (val == null)
			{
				rkValue = null;
				return TransformResult.Null;
			}
			double num2;
			if (val is float)
			{
				float num = (float)val;
				num2 = ((num != double.PositiveInfinity) ? ((num != double.NegativeInfinity) ? ((num != double.NaN) ? Convert.ToDouble(val, CultureInfo.InvariantCulture) : double.NaN) : double.NegativeInfinity) : double.PositiveInfinity);
			}
			else
			{
				num2 = Convert.ToDouble(val, CultureInfo.InvariantCulture);
			}
			rkValue = RKEncoder.EncodeRK(num2);
			if (rkValue.HasValue)
			{
				this.OnCellBegin(638, details.Column);
			}
			else
			{
				this.OnCellBegin(515, details.Column);
				RecordFactory.NUMBER(details.Output, (ushort)details.Row, (ushort)details.Column, details.Ixfe, num2);
			}
			return TransformResult.Handled;
		}

		private TransformResult CreateBlankRecord(CellRenderingDetails details)
		{
			this.OnCellBegin(513, details.Column);
			this.m_recType = 513;
			this.m_valueCol1 = ((this.m_counter == 0) ? details.Column : this.m_valueCol1);
			this.m_valueCol2 = details.Column;
			this.m_xfs[this.m_counter] = details.Ixfe;
			this.m_counter++;
			return TransformResult.Handled;
		}

		private TransformResult CreateRKRecord(uint value, CellRenderingDetails details)
		{
			this.OnCellBegin(638, details.Column);
			this.m_recType = 638;
			this.m_valueCol1 = ((this.m_counter == 0) ? details.Column : this.m_valueCol1);
			this.m_valueCol2 = details.Column;
			this.m_xfs[this.m_counter] = details.Ixfe;
			this.m_rks[this.m_counter] = value;
			this.m_counter++;
			return TransformResult.Handled;
		}

		internal void FlushRow()
		{
			this.FlushMultiRecord();
			this.m_counter = 0;
			this.m_recType = 0;
		}

		private void OnCellBegin(short recType, int col)
		{
			if (this.m_counter > 0)
			{
				if (recType == this.m_recType && col == this.m_valueCol2 + 1)
				{
					return;
				}
				this.FlushMultiRecord();
			}
		}

		private void FlushMultiRecord()
		{
			if (this.m_counter > 1)
			{
				if (this.m_recType == 638)
				{
					RecordFactory.MULRK(this.m_stream, (ushort)this.m_row, (ushort)this.m_valueCol1, (ushort)this.m_valueCol2, this.m_xfs, this.m_rks, this.m_counter);
				}
				else
				{
					RecordFactory.MULBLANK(this.m_stream, (ushort)this.m_row, (ushort)this.m_valueCol1, (ushort)this.m_valueCol2, this.m_xfs, this.m_counter);
				}
			}
			else if (this.m_counter == 1)
			{
				if (this.m_recType == 638)
				{
					RecordFactory.RK(this.m_stream, (ushort)this.m_row, (ushort)this.m_valueCol1, this.m_xfs[0], this.m_rks[0]);
				}
				else
				{
					RecordFactory.BLANK(this.m_stream, (ushort)this.m_row, (ushort)this.m_valueCol1, this.m_xfs[0]);
				}
			}
			this.m_counter = 0;
			this.m_valueCol1 = -1;
			this.m_valueCol2 = -1;
		}

		internal static double DateToDays(DateTime dateTime)
		{
			double num = dateTime.Subtract(RowHandler.Epoch).TotalDays + 1.0;
			if (num >= 60.0)
			{
				num += 1.0;
			}
			return num;
		}
	}
}
