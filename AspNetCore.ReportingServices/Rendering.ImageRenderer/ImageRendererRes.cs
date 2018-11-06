using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	[CompilerGenerated]
	internal class ImageRendererRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string IMAGELocalizedName = "IMAGELocalizedName";

			public const string PDFLocalizedName = "PDFLocalizedName";

			public const string RGDILocalizedName = "RGDILocalizedName";

			public const string Win32ErrorInfo = "Win32ErrorInfo";

			private static ResourceManager resourceManager = new ResourceManager(typeof(ImageRendererRes).FullName, typeof(ImageRendererRes).Module.Assembly);

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

		public static string IMAGELocalizedName
		{
			get
			{
				return Keys.GetString("IMAGELocalizedName");
			}
		}

		public static string PDFLocalizedName
		{
			get
			{
				return Keys.GetString("PDFLocalizedName");
			}
		}

		public static string RGDILocalizedName
		{
			get
			{
				return Keys.GetString("RGDILocalizedName");
			}
		}

		public static string Win32ErrorInfo
		{
			get
			{
				return Keys.GetString("Win32ErrorInfo");
			}
		}

		protected ImageRendererRes()
		{
		}
	}
}
