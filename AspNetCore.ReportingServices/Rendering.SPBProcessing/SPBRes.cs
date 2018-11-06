using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	[CompilerGenerated]
	internal class SPBRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string RenderSubreportError = "RenderSubreportError";

			public const string InvalidPaginationStream = "InvalidPaginationStream";

			public const string InvalidTokenPaginationProperties = "InvalidTokenPaginationProperties";

			public const string InvalidTokenPaginationItems = "InvalidTokenPaginationItems";

			public const string UnsupportedRPLVersion = "UnsupportedRPLVersion";

			public const string InvalidStartPageNumber = "InvalidStartPageNumber";

			public const string InvalidEndPageNumber = "InvalidEndPageNumber";

			private static ResourceManager resourceManager = new ResourceManager(typeof(SPBRes).FullName, typeof(SPBRes).Module.Assembly);

			private static CultureInfo _culture = null;

			public static CultureInfo Culture
			{
				get
				{
					return Keys._culture;
				}
				set
				{
					Keys._culture = value;
				}
			}

			private Keys()
			{
			}

			public static string GetString(string key)
			{
				return Keys.resourceManager.GetString(key, Keys._culture);
			}

			public static string GetString(string key, object arg0)
			{
				return string.Format(CultureInfo.CurrentCulture, Keys.resourceManager.GetString(key, Keys._culture), arg0);
			}
		}

		public static CultureInfo Culture
		{
			get
			{
				return Keys.Culture;
			}
			set
			{
				Keys.Culture = value;
			}
		}

		public static string RenderSubreportError
		{
			get
			{
				return Keys.GetString("RenderSubreportError");
			}
		}

		public static string InvalidPaginationStream
		{
			get
			{
				return Keys.GetString("InvalidPaginationStream");
			}
		}

		public static string InvalidStartPageNumber
		{
			get
			{
				return Keys.GetString("InvalidStartPageNumber");
			}
		}

		public static string InvalidEndPageNumber
		{
			get
			{
				return Keys.GetString("InvalidEndPageNumber");
			}
		}

		protected SPBRes()
		{
		}

		public static string InvalidTokenPaginationProperties(string hexToken)
		{
			return Keys.GetString("InvalidTokenPaginationProperties", hexToken);
		}

		public static string InvalidTokenPaginationItems(string hexToken)
		{
			return Keys.GetString("InvalidTokenPaginationItems", hexToken);
		}

		public static string UnsupportedRPLVersion(string requestedVersion)
		{
			return Keys.GetString("UnsupportedRPLVersion", requestedVersion);
		}
	}
}
