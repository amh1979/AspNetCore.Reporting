using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	[CompilerGenerated]
	internal class WordRenderRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string WordLocalizedName = "WordLocalizedName";

			public const string WordOpenXmlLocalizedName = "WordOpenXmlLocalizedName";

			public const string InvalidPNGError = "InvalidPNGError";

			public const string ColumnsErrorRectangle = "ColumnsErrorRectangle";

			public const string ColumnsErrorBody = "ColumnsErrorBody";

			public const string ColumnsErrorHeaderFooter = "ColumnsErrorHeaderFooter";

			private static ResourceManager resourceManager = new ResourceManager(typeof(WordRenderRes).FullName, typeof(WordRenderRes).Module.Assembly);

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

		public static string WordLocalizedName
		{
			get
			{
				return Keys.GetString("WordLocalizedName");
			}
		}

		public static string WordOpenXmlLocalizedName
		{
			get
			{
				return Keys.GetString("WordOpenXmlLocalizedName");
			}
		}

		public static string InvalidPNGError
		{
			get
			{
				return Keys.GetString("InvalidPNGError");
			}
		}

		public static string ColumnsErrorRectangle
		{
			get
			{
				return Keys.GetString("ColumnsErrorRectangle");
			}
		}

		public static string ColumnsErrorBody
		{
			get
			{
				return Keys.GetString("ColumnsErrorBody");
			}
		}

		public static string ColumnsErrorHeaderFooter
		{
			get
			{
				return Keys.GetString("ColumnsErrorHeaderFooter");
			}
		}

		protected WordRenderRes()
		{
		}
	}
}
