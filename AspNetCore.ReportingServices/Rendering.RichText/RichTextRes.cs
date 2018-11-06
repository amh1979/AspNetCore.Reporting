using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	[CompilerGenerated]
	internal class RichTextRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string Win32ErrorInfo = "Win32ErrorInfo";

			private static ResourceManager resourceManager = new ResourceManager(typeof(RichTextRes).FullName, typeof(RichTextRes).Module.Assembly);

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

		public static string Win32ErrorInfo
		{
			get
			{
				return Keys.GetString("Win32ErrorInfo");
			}
		}

		protected RichTextRes()
		{
		}
	}
}
