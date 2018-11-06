using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.Common
{
	internal static class DateTimeUtil
	{
		internal static bool TryParseDateTime(string strDateTime, CultureInfo formatProvider, out DateTimeOffset dateTimeOffset, out bool hasTimeOffset)
		{
			hasTimeOffset = false;
			if (DateTimeOffset.TryParse(strDateTime, (IFormatProvider)formatProvider, DateTimeStyles.None, out dateTimeOffset))
			{
				TimeSpan timeSpan = default(TimeSpan);
				if (TimeSpan.TryParse(strDateTime, out timeSpan))
				{
					return false;
				}
				DateTimeOffset dateTimeOffset2 = default(DateTimeOffset);
				if (!DateTimeOffset.TryParse(strDateTime + " +0", (IFormatProvider)formatProvider, DateTimeStyles.None, out dateTimeOffset2))
				{
					hasTimeOffset = true;
				}
				return true;
			}
			DateTimeFormatInfo dateTimeFormatInfo = (formatProvider != null) ? formatProvider.DateTimeFormat : CultureInfo.CurrentCulture.DateTimeFormat;
			string[] allDateTimePatterns = dateTimeFormatInfo.GetAllDateTimePatterns('d');
			if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns, (IFormatProvider)formatProvider, DateTimeStyles.None, out dateTimeOffset))
			{
				string[] allDateTimePatterns2 = dateTimeFormatInfo.GetAllDateTimePatterns('G');
				if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns2, (IFormatProvider)formatProvider, DateTimeStyles.None, out dateTimeOffset))
				{
					for (int i = 0; i < allDateTimePatterns2.Length; i++)
					{
						string[] array;
						string[] array2 = array = allDateTimePatterns2;
						int num = i;
						IntPtr intPtr = (IntPtr)num;
						array2[num] = array[(long)intPtr] + " zzz";
					}
					if (!DateTimeOffset.TryParseExact(strDateTime, allDateTimePatterns2, (IFormatProvider)formatProvider, DateTimeStyles.None, out dateTimeOffset))
					{
						return false;
					}
					hasTimeOffset = true;
				}
			}
			return true;
		}
	}
}
