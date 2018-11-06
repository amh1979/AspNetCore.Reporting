using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	[CompilerGenerated]
	internal class HPBRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string RenderSubreportError = "RenderSubreportError";

			public const string ReportItemCannotBeFound = "ReportItemCannotBeFound";

			private static ResourceManager resourceManager = new ResourceManager(typeof(HPBRes).FullName, typeof(HPBRes).Module.Assembly);

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

		protected HPBRes()
		{
		}

		public static string ReportItemCannotBeFound(string name)
		{
			return Keys.GetString("ReportItemCannotBeFound", name);
		}
	}
}
