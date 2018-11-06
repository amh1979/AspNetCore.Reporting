using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal abstract class DataTypeUtility
	{
		internal static bool IsSpatial(DataAggregate.DataTypeCode typeCode)
		{
			if (typeCode != DataAggregate.DataTypeCode.SqlGeography)
			{
				return typeCode == DataAggregate.DataTypeCode.SqlGeometry;
			}
			return true;
		}

		internal static bool IsNumeric(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.Int64:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.UInt64:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
			case DataAggregate.DataTypeCode.TimeSpan:
			case DataAggregate.DataTypeCode.Single:
			case DataAggregate.DataTypeCode.Double:
			case DataAggregate.DataTypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsFloat(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Single:
			case DataAggregate.DataTypeCode.Double:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsSigned(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.Int64:
			case DataAggregate.DataTypeCode.SByte:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsUnsigned(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.UInt64:
			case DataAggregate.DataTypeCode.Byte:
				return true;
			default:
				return false;
			}
		}

		internal static bool Is32BitOrLess(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
				return true;
			default:
				return false;
			}
		}

		internal static bool Is64BitOrLess(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
			case DataAggregate.DataTypeCode.Int32:
			case DataAggregate.DataTypeCode.Int64:
			case DataAggregate.DataTypeCode.UInt16:
			case DataAggregate.DataTypeCode.UInt32:
			case DataAggregate.DataTypeCode.UInt64:
			case DataAggregate.DataTypeCode.Byte:
			case DataAggregate.DataTypeCode.SByte:
				return true;
			default:
				return false;
			}
		}

		internal static double ConvertToDouble(DataAggregate.DataTypeCode typeCode, object data)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Double:
				return (double)data;
			case DataAggregate.DataTypeCode.Int16:
				return (double)(short)data;
			case DataAggregate.DataTypeCode.Int32:
				return (double)(int)data;
			case DataAggregate.DataTypeCode.Int64:
				return (double)(long)data;
			case DataAggregate.DataTypeCode.UInt16:
				return (double)(int)(ushort)data;
			case DataAggregate.DataTypeCode.UInt32:
				return (double)(uint)data;
			case DataAggregate.DataTypeCode.UInt64:
				return (double)(ulong)data;
			case DataAggregate.DataTypeCode.Byte:
				return (double)(int)(byte)data;
			case DataAggregate.DataTypeCode.SByte:
				return (double)(sbyte)data;
			case DataAggregate.DataTypeCode.TimeSpan:
				return (double)((TimeSpan)data).Ticks;
			case DataAggregate.DataTypeCode.Single:
				return (double)(float)data;
			case DataAggregate.DataTypeCode.Decimal:
				return Convert.ToDouble((decimal)data);
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal static int ConvertToInt32(DataAggregate.DataTypeCode typeCode, object data, out bool valid)
		{
			valid = true;
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int16:
				return (short)data;
			case DataAggregate.DataTypeCode.Int32:
				return (int)data;
			case DataAggregate.DataTypeCode.UInt16:
				return (ushort)data;
			case DataAggregate.DataTypeCode.UInt32:
				if ((uint)data > 2147483647)
				{
					break;
				}
				return (int)data;
			case DataAggregate.DataTypeCode.UInt64:
				if ((ulong)data > 2147483647)
				{
					break;
				}
				return (int)data;
			case DataAggregate.DataTypeCode.Int64:
				if ((long)data > 2147483647)
				{
					break;
				}
				if ((long)data < -2147483648)
				{
					break;
				}
				return (int)data;
			case DataAggregate.DataTypeCode.Byte:
				return (byte)data;
			case DataAggregate.DataTypeCode.SByte:
				return (sbyte)data;
			}
			valid = false;
			return 0;
		}

		internal static string ConvertToInvariantString(object o)
		{
			if (o == null)
			{
				return null;
			}
			string text = null;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			try
			{
				return o.ToString();
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		internal static Type GetNumericTypeFromDataTypeCode(DataAggregate.DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataAggregate.DataTypeCode.Int32:
				return typeof(int);
			case DataAggregate.DataTypeCode.Int64:
				return typeof(long);
			case DataAggregate.DataTypeCode.UInt32:
				return typeof(uint);
			case DataAggregate.DataTypeCode.UInt64:
				return typeof(ulong);
			case DataAggregate.DataTypeCode.Single:
				return typeof(float);
			case DataAggregate.DataTypeCode.Double:
				return typeof(double);
			case DataAggregate.DataTypeCode.Decimal:
				return typeof(decimal);
			case DataAggregate.DataTypeCode.Int16:
				return typeof(short);
			case DataAggregate.DataTypeCode.UInt16:
				return typeof(ushort);
			default:
				return null;
			}
		}

		internal static DataAggregate.DataTypeCode CommonNumericDenominator(DataAggregate.DataTypeCode x, DataAggregate.DataTypeCode y)
		{
			if (DataTypeUtility.IsNumeric(x) && DataTypeUtility.IsNumeric(y))
			{
				if (x == y)
				{
					return x;
				}
				if (DataTypeUtility.IsSigned(x) && DataTypeUtility.IsSigned(y))
				{
					if (DataAggregate.DataTypeCode.Int64 != x && DataAggregate.DataTypeCode.Int64 != y)
					{
						return DataAggregate.DataTypeCode.Int32;
					}
					return DataAggregate.DataTypeCode.Int64;
				}
				if (DataTypeUtility.IsUnsigned(x) && DataTypeUtility.IsUnsigned(y))
				{
					if (DataAggregate.DataTypeCode.UInt64 != x && DataAggregate.DataTypeCode.UInt64 != y)
					{
						return DataAggregate.DataTypeCode.UInt32;
					}
					return DataAggregate.DataTypeCode.UInt64;
				}
				if (DataTypeUtility.IsFloat(x) && DataTypeUtility.IsFloat(y))
				{
					return DataAggregate.DataTypeCode.Double;
				}
				if (DataTypeUtility.IsSigned(x) && DataTypeUtility.IsUnsigned(y))
				{
					return DataTypeUtility.CommonDataTypeSignedUnsigned(x, y);
				}
				if (DataTypeUtility.IsUnsigned(x) && DataTypeUtility.IsSigned(y))
				{
					return DataTypeUtility.CommonDataTypeSignedUnsigned(y, x);
				}
				if (DataTypeUtility.Is32BitOrLess(x) && DataTypeUtility.IsFloat(y))
				{
					goto IL_00b6;
				}
				if (DataTypeUtility.Is32BitOrLess(y) && DataTypeUtility.IsFloat(x))
				{
					goto IL_00b6;
				}
				if (DataTypeUtility.Is64BitOrLess(x) && DataAggregate.DataTypeCode.Decimal == y)
				{
					goto IL_00d3;
				}
				if (DataTypeUtility.Is64BitOrLess(y) && DataAggregate.DataTypeCode.Decimal == x)
				{
					goto IL_00d3;
				}
				return DataAggregate.DataTypeCode.Null;
			}
			return DataAggregate.DataTypeCode.Null;
			IL_00b6:
			return DataAggregate.DataTypeCode.Double;
			IL_00d3:
			return DataAggregate.DataTypeCode.Decimal;
		}

		internal static bool IsNumericLessThanZero(object value, DataAggregate.DataTypeCode dataType)
		{
			switch (dataType)
			{
			case DataAggregate.DataTypeCode.Int32:
				return (int)value < 0;
			case DataAggregate.DataTypeCode.Double:
				return (double)value < 0.0;
			case DataAggregate.DataTypeCode.Single:
				return (float)value < 0.0;
			case DataAggregate.DataTypeCode.Decimal:
				return (decimal)value < 0m;
			case DataAggregate.DataTypeCode.Int16:
				return (short)value < 0;
			case DataAggregate.DataTypeCode.Int64:
				return (long)value < 0;
			case DataAggregate.DataTypeCode.UInt16:
				return (ushort)value < 0;
			case DataAggregate.DataTypeCode.UInt32:
				return (uint)value < 0;
			case DataAggregate.DataTypeCode.UInt64:
				return (ulong)value < 0;
			case DataAggregate.DataTypeCode.Byte:
				return (byte)value < 0;
			case DataAggregate.DataTypeCode.SByte:
				return (sbyte)value < 0;
			default:
				return false;
			}
		}

		private static DataAggregate.DataTypeCode CommonDataTypeSignedUnsigned(DataAggregate.DataTypeCode signed, DataAggregate.DataTypeCode unsigned)
		{
			Global.Tracer.Assert(DataTypeUtility.IsSigned(signed) && DataTypeUtility.IsUnsigned(unsigned), "(IsSigned(signed) && IsUnsigned(unsigned))");
			if (DataAggregate.DataTypeCode.UInt64 == unsigned)
			{
				return DataAggregate.DataTypeCode.Null;
			}
			if (DataAggregate.DataTypeCode.UInt32 == unsigned)
			{
				return DataAggregate.DataTypeCode.Int64;
			}
			if (DataAggregate.DataTypeCode.Int64 == signed)
			{
				return DataAggregate.DataTypeCode.Int64;
			}
			return DataAggregate.DataTypeCode.Int32;
		}
	}
}
