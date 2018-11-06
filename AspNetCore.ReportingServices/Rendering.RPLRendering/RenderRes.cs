using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.RPLRendering
{
	[CompilerGenerated]
	internal class RenderRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string RPLLocalizedName = "RPLLocalizedName";

			private static ResourceManager resourceManager = new ResourceManager(typeof(RenderRes).FullName, typeof(RenderRes).Module.Assembly);

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

		public static string RPLLocalizedName
		{
			get
			{
				return Keys.GetString("RPLLocalizedName");
			}
		}

		protected RenderRes()
		{
		}
	}
}
