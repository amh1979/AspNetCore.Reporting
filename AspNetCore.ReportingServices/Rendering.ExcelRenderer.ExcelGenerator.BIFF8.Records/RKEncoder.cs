using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records
{
	internal static class RKEncoder
	{
		private static readonly double MAXRKVALUE = 1.7976931348623156E+306;

		internal static uint? EncodeRK(double aRKValue)
		{
			uint? nullable = null;
			if (aRKValue == 0.0)
			{
				aRKValue = 0.0;
			}
			uint? nullable2 = nullable = RKEncoder.double2RK(aRKValue, 0u);
			if (nullable2.HasValue)
			{
				return nullable;
			}
			uint? nullable3 = nullable = RKEncoder.longint2RK(aRKValue, 2L);
			if (nullable3.HasValue)
			{
				return nullable;
			}
			if (Math.Abs(aRKValue) > RKEncoder.MAXRKVALUE)
			{
				return null;
			}
			double aRKValue2 = aRKValue * 100.0;
			uint? nullable4 = nullable = RKEncoder.double2RK(aRKValue2, 1u);
			if (nullable4.HasValue)
			{
				return nullable;
			}
			return RKEncoder.longint2RK(aRKValue2, 3L);
		}

		internal static double DecodeRK(int aBytes)
		{
			double num = 0.0;
			num = (((aBytes & 2) == 0) ? BitConverter.Int64BitsToDouble((long)(aBytes >> 2) << 34) : ((double)(aBytes >> 2)));
			if ((aBytes & 1) != 0)
			{
				num /= 100.0;
			}
			return num;
		}

		private static uint? double2RK(double aRKValue, uint aTypeMask)
		{
			ulong num = (ulong)BitConverter.DoubleToInt64Bits(aRKValue);
			ulong num2 = num & 4294967295u;
			ulong num3 = num >> 32;
			if (num2 == 0 && (num3 & 3) == 0)
			{
				ulong num4 = (num3 & 4294967292u) | aTypeMask;
				return (uint)num4;
			}
			return null;
		}

		private static uint? longint2RK(double aRKValue, long aTypeMask)
		{
			long num = (long)Math.Round(aRKValue);
			double num2 = aRKValue - (double)num;
			if (num2 == 0.0 && Math.Abs(num) <= 536870911 && (Math.Abs(num) & 3758096384u) == 0)
			{
				long num3 = (num << 2 & 4294967292u) | aTypeMask;
				return (uint)num3;
			}
			return null;
		}
	}
}
