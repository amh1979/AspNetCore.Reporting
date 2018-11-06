using System;

namespace AspNetCore.ReportingServices.Common
{
	internal static class ComparerUtility
	{
		internal static Type GetNumericDateTypeFromDataTypeCode(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.DateTime:
				return typeof(DateTime);
			case DataTypeCode.Double:
				return typeof(double);
			case DataTypeCode.Decimal:
				return typeof(decimal);
			case DataTypeCode.Int64:
				return typeof(long);
			default:
				return null;
			}
		}

		internal static DataTypeCode GetCommonVariantConversionType(DataTypeCode x, DataTypeCode y)
		{
			if (y == DataTypeCode.Double && ComparerUtility.IsComparableToReal(x))
			{
				goto IL_001a;
			}
			if (x == DataTypeCode.Double && ComparerUtility.IsComparableToReal(y))
			{
				goto IL_001a;
			}
			if (y == DataTypeCode.Decimal && ComparerUtility.IsComparableToCurrency(x))
			{
				goto IL_0037;
			}
			if (x == DataTypeCode.Decimal && ComparerUtility.IsComparableToCurrency(y))
			{
				goto IL_0037;
			}
			if (y == DataTypeCode.DateTime && ComparerUtility.IsNumericVariant(x))
			{
				goto IL_0054;
			}
			if (x == DataTypeCode.DateTime && ComparerUtility.IsNumericVariant(y))
			{
				goto IL_0054;
			}
			if (y == DataTypeCode.Int64 && x == DataTypeCode.Int32)
			{
				goto IL_0067;
			}
			if (x == DataTypeCode.Int64 && y == DataTypeCode.Int32)
			{
				goto IL_0067;
			}
			return DataTypeCode.Unknown;
			IL_0054:
			return DataTypeCode.Double;
			IL_0037:
			return DataTypeCode.Decimal;
			IL_0067:
			return DataTypeCode.Int64;
			IL_001a:
			return DataTypeCode.Double;
		}

		private static bool IsComparableToReal(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
			case DataTypeCode.Decimal:
			case DataTypeCode.DateTime:
				return true;
			default:
				return false;
			}
		}

		private static bool IsComparableToCurrency(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
			case DataTypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsNumericVariant(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
			case DataTypeCode.Double:
			case DataTypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsNumericDateVariant(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Empty:
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
			case DataTypeCode.Double:
			case DataTypeCode.Decimal:
			case DataTypeCode.DateTime:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsNonNumericVariant(DataTypeCode typeCode)
		{
			DataTypeCode dataTypeCode = typeCode;
			if (dataTypeCode != DataTypeCode.Boolean && dataTypeCode != DataTypeCode.String)
			{
				return false;
			}
			return true;
		}

		internal static bool IsNumericLessThanZero(object value)
		{
			if (value is int)
			{
				return (int)value < 0;
			}
			if (value is double)
			{
				return (double)value < 0.0;
			}
			if (value is float)
			{
				return (float)value < 0.0;
			}
			if (value is decimal)
			{
				return (decimal)value < 0m;
			}
			if (value is short)
			{
				return (short)value < 0;
			}
			if (value is long)
			{
				return (long)value < 0;
			}
			if (value is ushort)
			{
				return (ushort)value < 0;
			}
			if (value is uint)
			{
				return (uint)value < 0;
			}
			if (value is ulong)
			{
				return (ulong)value < 0;
			}
			if (value is byte)
			{
				return (byte)value < 0;
			}
			if (value is sbyte)
			{
				return (sbyte)value < 0;
			}
			return false;
		}

		internal static bool IsLessThanReal(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
			case DataTypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsLessThanCurrency(DataTypeCode typeCode)
		{
			switch (typeCode)
			{
			case DataTypeCode.Int32:
			case DataTypeCode.Int64:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsLessThanInt64(DataTypeCode typeCode)
		{
			return typeCode == DataTypeCode.Int32;
		}
	}
}
