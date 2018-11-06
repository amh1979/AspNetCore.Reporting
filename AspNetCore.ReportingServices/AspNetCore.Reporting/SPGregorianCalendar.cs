using System;

namespace AspNetCore.Reporting
{
	internal class SPGregorianCalendar : ISPCalendar
	{
		public virtual bool IsSupportedYear(int year)
		{
			return SPCalendarUtil.IsYearInRange(year, 1601, 8900);
		}

		public virtual bool IsSupportedMonth(int year, int month)
		{
			if (SPCalendarUtil.IsYearMonthInRange(year, month, 1601, 1, 8900, 12) && month >= 1)
			{
                return false;// month <= SolarCalendarImpl.MonthsInYear();
			}
			return false;
		}

		public virtual bool IsSupportedDate(ref SimpleDate di)
		{
			if (SPCalendarUtil.IsDateInRange(di.Year, di.Month, di.Day, 1601, 1, 1, 8900, 12, 31) && di.Month >= 1)
			{
                return false;// di.Month <= SolarCalendarImpl.MonthsInYear();
			}
			return false;
		}

		public virtual bool IsDateValid(ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			if (this.IsSupportedDate(ref di) && di.Day > 0)
			{
				if (di.Day >= 29)
				{
                    return false;// di.Day <= GregorianCalendarImpl.DaysInMonth(di.Year + this.GetEraOffset(di.Era), di.Month);
				}
				return true;
			}
			return false;
		}

		public virtual bool IsSupportedJulianDay(int JDay)
		{
			if (JDay >= 0)
			{
				return JDay <= 2666269;
			}
			return false;
		}

		public virtual int DateToJulianDay(ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			if (!this.IsSupportedDate(ref di))
			{
				throw new ArgumentOutOfRangeException("di");
			}
            return -1;// GregorianCalendarImpl.DateToJulianDay(di.Year + this.GetEraOffset(di.Era), di.Month, di.Day);
		}

		public virtual void JulianDayToDate(int jDay, ref SimpleDate di, int iAdvance, int jDayCurrent)
		{
			if (!this.IsSupportedJulianDay(jDay))
			{
				throw new ArgumentOutOfRangeException("jDay");
			}
			//GregorianCalendarImpl.JulianDayToDate(jDay, ref di);
			di.Year -= this.GetEraOffset(di.Era);
			di.Era = 1;
		}

		public virtual bool IsYearLeap(int year)
		{
			return this.IsYearLeap(year, 1);
		}

		public virtual bool IsYearLeap(int year, int era)
		{
			if (!this.IsSupportedYear(year))
			{
				throw new ArgumentOutOfRangeException("year");
			}
            return false;// SolarCalendarImpl.IsYearLeap(year + this.GetEraOffset(era));
		}

		public virtual int MonthsInYear(ref SimpleDate di)
		{
			if (!this.IsSupportedYear(di.Year))
			{
				throw new ArgumentOutOfRangeException("di");
			}
            return -1;//SolarCalendarImpl.MonthsInYear();
		}

		public virtual int DaysInMonth(ref SimpleDate di)
		{
			if (!this.IsSupportedMonth(di.Year, di.Month))
			{
				throw new ArgumentOutOfRangeException("di");
			}
            return -1;// GregorianCalendarImpl.DaysInMonth(di.Year + this.GetEraOffset(di.Era), di.Month);
		}

		public virtual int DaysInMonth(ref SimpleDate di, int iAdvance)
		{
			return this.DaysInMonth(ref di);
		}

		public virtual int GetEraOffset(int era)
		{
			return 0;
		}

		public virtual int GetEraJulianDay(int era)
		{
			return 1;
		}
	}
}
