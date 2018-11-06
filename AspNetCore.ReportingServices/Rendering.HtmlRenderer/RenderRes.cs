using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	[CompilerGenerated]
	internal class RenderRes
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string AccessibleChartLabel = "AccessibleChartLabel";

			public const string AccessibleChartNavigationGroupLabel = "AccessibleChartNavigationGroupLabel";

			public const string AccessibleImageLabel = "AccessibleImageLabel";

			public const string AccessibleImageNavigationGroupLabel = "AccessibleImageNavigationGroupLabel";

			public const string AccessibleTableBoxLabel = "AccessibleTableBoxLabel";

			public const string AccessibleTextBoxLabel = "AccessibleTextBoxLabel";

			public const string DocumentMap = "DocumentMap";

			public const string DefaultDocMapLabel = "DefaultDocMapLabel";

			public const string HideDocMapTooltip = "HideDocMapTooltip";

			public const string HTML40LocalizedName = "HTML40LocalizedName";

			public const string HTML5LocalizedName = "HTML5LocalizedName";

			public const string MHTMLLocalizedName = "MHTMLLocalizedName";

			public const string rrInvalidSectionError = "rrInvalidSectionError";

			public const string rrInvalidDeviceInfo = "rrInvalidDeviceInfo";

			public const string ToggleStateCollapse = "ToggleStateCollapse";

			public const string ToggleStateExpand = "ToggleStateExpand";

			public const string SortAscAltText = "SortAscAltText";

			public const string SortDescAltText = "SortDescAltText";

			public const string UnsortedAltText = "UnsortedAltText";

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

		public static string AccessibleChartLabel
		{
			get
			{
				return Keys.GetString("AccessibleChartLabel");
			}
		}

		public static string AccessibleImageLabel
		{
			get
			{
				return Keys.GetString("AccessibleImageLabel");
			}
		}

		public static string AccessibleTableBoxLabel
		{
			get
			{
				return Keys.GetString("AccessibleTableBoxLabel");
			}
		}

		public static string AccessibleTextBoxLabel
		{
			get
			{
				return Keys.GetString("AccessibleTextBoxLabel");
			}
		}

		public static string DocumentMap
		{
			get
			{
				return Keys.GetString("DocumentMap");
			}
		}

		public static string DefaultDocMapLabel
		{
			get
			{
				return Keys.GetString("DefaultDocMapLabel");
			}
		}

		public static string HideDocMapTooltip
		{
			get
			{
				return Keys.GetString("HideDocMapTooltip");
			}
		}

		public static string HTML40LocalizedName
		{
			get
			{
				return Keys.GetString("HTML40LocalizedName");
			}
		}

		public static string HTML5LocalizedName
		{
			get
			{
				return Keys.GetString("HTML5LocalizedName");
			}
		}

		public static string MHTMLLocalizedName
		{
			get
			{
				return Keys.GetString("MHTMLLocalizedName");
			}
		}

		public static string rrInvalidSectionError
		{
			get
			{
				return Keys.GetString("rrInvalidSectionError");
			}
		}

		public static string rrInvalidDeviceInfo
		{
			get
			{
				return Keys.GetString("rrInvalidDeviceInfo");
			}
		}

		public static string ToggleStateCollapse
		{
			get
			{
				return Keys.GetString("ToggleStateCollapse");
			}
		}

		public static string ToggleStateExpand
		{
			get
			{
				return Keys.GetString("ToggleStateExpand");
			}
		}

		public static string SortAscAltText
		{
			get
			{
				return Keys.GetString("SortAscAltText");
			}
		}

		public static string SortDescAltText
		{
			get
			{
				return Keys.GetString("SortDescAltText");
			}
		}

		public static string UnsortedAltText
		{
			get
			{
				return Keys.GetString("UnsortedAltText");
			}
		}

		protected RenderRes()
		{
		}

		public static string AccessibleChartNavigationGroupLabel(string name)
		{
			return Keys.GetString("AccessibleChartNavigationGroupLabel", name);
		}

		public static string AccessibleImageNavigationGroupLabel(string name)
		{
			return Keys.GetString("AccessibleImageNavigationGroupLabel", name);
		}
	}
}
