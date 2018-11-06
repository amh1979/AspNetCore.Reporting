using System;

namespace AspNetCore.Reporting
{
	internal sealed class SPIntlCal
	{
		public const int DaysInWeek = 7;

		public const int MaxJDay = 2666269;

		private static SPGregorianCalendar _GregorianCalendar = new SPGregorianCalendar();

		internal static SPGregorianCalendar GregorianCalendar
		{
			get
			{
				return SPIntlCal._GregorianCalendar;
			}
		}

		private SPIntlCal()
		{
		}

		public static bool IsCalendarSupported(SPCalendarType calType)
		{
			try
			{
				SPIntlCal.GetLocalCalendar(calType);
				return true;
			}
			catch (Exception)
			{
			}
			return false;
		}

		public static ISPCalendar GetLocalCalendar(SPCalendarType calType)
		{
			return SPIntlCal.GregorianCalendar;
		}

		public static bool IsSupportedLocalYear(SPCalendarType calType, int year)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsSupportedYear(year);
		}

		public static bool IsSupportedLocalMonth(SPCalendarType calType, int year, int month)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsSupportedMonth(year, month);
		}

		public static bool IsSupportedLocalDate(SPCalendarType calType, ref SimpleDate di)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsSupportedDate(ref di);
		}

		public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, 0, 0);
		}

		public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di, int iAdvance)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, 0);
		}

		public static bool IsLocalDateValid(SPCalendarType calType, ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsDateValid(ref di, iAdvance, jDayCurrent);
		}

		public static bool IsSupportedLocalJulianDay(SPCalendarType calType, int jDay)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsSupportedJulianDay(jDay);
		}

		public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di)
		{
			return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, 0, 0);
		}

		public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di, int iAdvance)
		{
			return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, 0);
		}

		public static int LocalToJulianDay(SPCalendarType calType, ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref di, iAdvance, jDayCurrent);
		}

		public static int LocalToJulianDay(SPCalendarType calType, IntlDate id)
		{
			SimpleDate simpleDate = new SimpleDate(id.Year, id.Month, id.Day, id.Era);
			return SPIntlCal.GetLocalCalendar(calType).DateToJulianDay(ref simpleDate, 0, 0);
		}

		public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di)
		{
			SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, 0, 0);
		}

		public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di, int iAdvance)
		{
			SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, 0);
		}

		public static void JulianDayToLocal(SPCalendarType calType, int jDay, ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref di, iAdvance, jDayCurrent);
		}

		public static void JulianDayToLocal(SPCalendarType calType, int jDay, IntlDate id)
		{
			SimpleDate simpleDate = new SimpleDate(0, 0, 0);
			SPIntlCal.GetLocalCalendar(calType).JulianDayToDate(jDay, ref simpleDate, 0, 0);
			id.Init(simpleDate.Year, simpleDate.Month, simpleDate.Day, simpleDate.Era, calType);
		}

		public static int EraOffset(SPCalendarType calType, int era)
		{
			return SPIntlCal.GetLocalCalendar(calType).GetEraOffset(era);
		}

		public static int GetEraJulianDay(SPCalendarType calType, int era)
		{
			return SPIntlCal.GetLocalCalendar(calType).GetEraJulianDay(era);
		}

		public static bool IsLocalYearLeap(SPCalendarType calType, int year)
		{
			return SPIntlCal.GetLocalCalendar(calType).IsYearLeap(year);
		}

		public static int MonthsInLocalYear(SPCalendarType calType, ref SimpleDate di)
		{
			return SPIntlCal.GetLocalCalendar(calType).MonthsInYear(ref di);
		}

		public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di)
		{
			return SPIntlCal.GetLocalCalendar(calType).DaysInMonth(ref di);
		}

		public static int DaysInLocalMonth(SPCalendarType calType, ref SimpleDate di, int iAdvance)
		{
			return SPIntlCal.GetLocalCalendar(calType).DaysInMonth(ref di, iAdvance);
		}

		public static int GetWeekNumber(SPCalendarType calType, SimpleDate di, int FirstDayOfWeek, short FirstWeekOfYear)
		{
			int num = di.Day;
			int num2 = di.Month;
			while (--num2 > 0)
			{
				SimpleDate simpleDate = new SimpleDate(di.Year, num2, 1);
				num += SPIntlCal.DaysInLocalMonth(calType, ref simpleDate);
			}
			SimpleDate simpleDate2 = new SimpleDate(di.Year, 1, 1, di.Era);
			int num3 = SPIntlCal.LocalToJulianDay(calType, ref simpleDate2);
			int num4 = (num3 + 1) % 7;
			int num5 = (num - 1) / 7 + 1;
			if (num4 < FirstDayOfWeek)
			{
				num4 += 7;
			}
			if (FirstDayOfWeek < 7 && FirstDayOfWeek >= 0)
			{
				if (FirstWeekOfYear == 2 && num4 > FirstDayOfWeek + 3)
				{
					goto IL_0097;
				}
				if (FirstWeekOfYear == 1 && num4 != FirstDayOfWeek)
				{
					goto IL_0097;
				}
			}
			goto IL_009b;
			IL_009b:
			return num5;
			IL_0097:
			num5--;
			goto IL_009b;
		}
	}
}
