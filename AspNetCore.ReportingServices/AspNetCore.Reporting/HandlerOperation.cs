using System.Web;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace AspNetCore.Reporting
{
	internal abstract class HandlerOperation : IDisposable
	{
		public virtual bool IsCacheable
		{
			get
			{
				return false;
			}
		}

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public abstract void PerformOperation(NameValueCollection urlQuery, HttpResponse response);

		protected static string GetAndEnsureParam(NameValueCollection urlQuery, string paramName)
		{
			string text = urlQuery[paramName];
			if (text == null)
			{
				throw new HttpHandlerInputException(Errors.MissingUrlParameter(paramName));
			}
			return text;
		}

		protected static int ParseRequiredInt(NameValueCollection urlQuery, string paramName)
		{
			string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, paramName);
			int result = default(int);
			if (int.TryParse(andEnsureParam, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			throw new HttpHandlerInputException(new FormatException());
		}

		protected static int ParseOptionalInt(string paramValueStr)
		{
			if (paramValueStr == null)
			{
				return 0;
			}
			int result = default(int);
			if (int.TryParse(paramValueStr, out result))
			{
				return result;
			}
			throw new HttpHandlerInputException(new FormatException());
		}

		protected static bool ParseRequiredBool(NameValueCollection urlQuery, string paramName)
		{
			string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, paramName);
			bool result = default(bool);
			if (bool.TryParse(andEnsureParam, out result))
			{
				return result;
			}
			throw new HttpHandlerInputException(new FormatException());
		}

		protected static object ParseRequiredEnum(NameValueCollection urlQuery, string paramName, Type enumType)
		{
			string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, paramName);
			try
			{
				return Enum.Parse(enumType, andEnsureParam);
			}
			catch (Exception e)
			{
				throw new HttpHandlerInputException(e);
			}
		}
	}
}
