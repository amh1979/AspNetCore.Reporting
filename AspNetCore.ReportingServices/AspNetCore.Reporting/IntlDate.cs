using System;

namespace AspNetCore.Reporting
{
	internal sealed class IntlDate
	{
		private int m_Year;

		private int m_Month;

		private int m_Day;

		private int m_Era;

		private int m_Jday;

		private SPCalendarType m_CalendarType;

		public int Year
		{
			get
			{
				return this.m_Year;
			}
			set
			{
				this.Init(value, this.m_Month, this.m_Day, this.m_CalendarType);
			}
		}

		public int Month
		{
			get
			{
				return this.m_Month;
			}
			set
			{
				this.Init(this.m_Year, value, this.m_Day, this.m_CalendarType);
			}
		}

		public int Day
		{
			get
			{
				return this.m_Day;
			}
			set
			{
				this.Init(this.m_Year, this.m_Month, value, this.m_CalendarType);
			}
		}

		public int Era
		{
			get
			{
				return this.m_Era;
			}
			set
			{
				this.Init(this.m_Year, this.m_Month, this.m_Day, value, this.m_CalendarType);
			}
		}

		public int JDay
		{
			get
			{
				return this.m_Jday;
			}
			set
			{
				this.Init(value, this.m_CalendarType);
			}
		}

		public SPCalendarType CalendarType
		{
			get
			{
				return this.m_CalendarType;
			}
			set
			{
				this.Init(this.m_Jday, value);
			}
		}

		public int DayOfWeek
		{
			get
			{
				return (this.m_Jday + 1) % 7;
			}
		}

		public IntlDate(int year, int month, int day)
		{
			this.Init(year, month, day, SPCalendarType.Gregorian);
		}

		public IntlDate(int year, int month, int day, SPCalendarType calendarType)
		{
			this.Init(year, month, day, calendarType);
		}

		public IntlDate(int year, int month, int day, int era, SPCalendarType calendarType)
		{
			this.Init(year, month, day, era, calendarType);
		}

		public IntlDate(int julianDay)
		{
			this.Init(julianDay, SPCalendarType.Gregorian);
		}

		public IntlDate(int julianDay, SPCalendarType calendarType)
		{
			this.Init(julianDay, calendarType);
		}

		public bool IsYearLeap()
		{
			return SPIntlCal.IsLocalYearLeap(this.m_CalendarType, this.m_Year);
		}

		public int MonthsInYear()
		{
			SimpleDate simpleDate = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
			return SPIntlCal.MonthsInLocalYear(this.m_CalendarType, ref simpleDate);
		}

		public int DaysInMonth()
		{
			SimpleDate simpleDate = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
			return SPIntlCal.DaysInLocalMonth(this.m_CalendarType, ref simpleDate);
		}

		public void AddDays(int days)
		{
			this.JDay += days;
		}

		internal void Init(int year, int month, int day, SPCalendarType calendarType)
		{
			this.Init(year, month, day, 1, calendarType);
		}

		internal void Init(int year, int month, int day, int era, SPCalendarType calendarType)
		{
			this.m_Year = year;
			this.m_Month = month;
			this.m_Day = day;
			this.m_Era = era;
			this.m_CalendarType = calendarType;
			SimpleDate simpleDate = new SimpleDate(this.m_Year, this.m_Month, this.m_Day, this.m_Era);
			if (!SPIntlCal.IsLocalDateValid(calendarType, ref simpleDate))
			{
				throw new ArgumentOutOfRangeException("calendarType");
			}
			this.m_Jday = SPIntlCal.LocalToJulianDay(this.m_CalendarType, ref simpleDate);
		}

		internal void Init(int julianDay, SPCalendarType calendarType)
		{
			SimpleDate simpleDate = new SimpleDate(0, 0, 0);
			SPIntlCal.JulianDayToLocal(calendarType, julianDay, ref simpleDate);
			this.m_Year = simpleDate.Year;
			this.m_Month = simpleDate.Month;
			this.m_Day = simpleDate.Day;
			this.m_Era = simpleDate.Era;
			this.m_CalendarType = calendarType;
			this.m_Jday = julianDay;
		}
	}
}
