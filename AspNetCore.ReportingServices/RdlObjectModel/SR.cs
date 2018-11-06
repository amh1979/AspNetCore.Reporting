using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	[CompilerGenerated]
	internal class SR
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string Language_bn = "Language_bn";

			public const string Language_or = "Language_or";

			public const string Language_lo = "Language_lo";

			public const string Language_bo = "Language_bo";

			private static ResourceManager resourceManager = new ResourceManager(typeof(SR).FullName, typeof(SR).Module.Assembly);

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

		public static string Language_bn
		{
			get
			{
				return Keys.GetString("Language_bn");
			}
		}

		public static string Language_or
		{
			get
			{
				return Keys.GetString("Language_or");
			}
		}

		public static string Language_lo
		{
			get
			{
				return Keys.GetString("Language_lo");
			}
		}

		public static string Language_bo
		{
			get
			{
				return Keys.GetString("Language_bo");
			}
		}

		protected SR()
		{
		}
	}
}
