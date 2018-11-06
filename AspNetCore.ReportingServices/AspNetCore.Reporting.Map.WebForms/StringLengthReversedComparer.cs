using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class StringLengthReversedComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			if (x is string && y is string)
			{
				string text = (string)x;
				string text2 = (string)y;
				if (text.Length > text2.Length)
				{
					return -1;
				}
				if (text.Length < text2.Length)
				{
					return 1;
				}
				Comparer comparer = new Comparer(CultureInfo.InvariantCulture);
				return comparer.Compare(x, y);
			}
			return 0;
		}
	}
}
