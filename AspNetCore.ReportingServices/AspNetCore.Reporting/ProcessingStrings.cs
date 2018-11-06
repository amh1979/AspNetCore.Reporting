using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.Reporting
{
	[CompilerGenerated]
	internal class ProcessingStrings
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string MissingDefinition = "MissingDefinition";

			public const string pvInvalidDefinition = "pvInvalidDefinition";

			public const string MainReport = "MainReport";

			public const string RdlCompile_CouldNotWriteStateFile = "RdlCompile_CouldNotWriteStateFile";

			public const string RdlCompile_CouldNotOpenFile = "RdlCompile_CouldNotOpenFile";

			public const string DataSetExtensionName = "DataSetExtensionName";

			public const string MissingDataReader = "MissingDataReader";

			public const string CasPolicyUnavailableForCurrentAppDomain = "CasPolicyUnavailableForCurrentAppDomain";

			public const string MapTileServerConfiguration_MaxConnectionsOutOfRange = "MapTileServerConfiguration_MaxConnectionsOutOfRange";

			public const string MapTileServerConfiguration_TimeoutOutOfRange = "MapTileServerConfiguration_TimeoutOutOfRange";

			private static ResourceManager resourceManager = new ResourceManager(typeof(ProcessingStrings).FullName, typeof(ProcessingStrings).Module.Assembly);

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

			public static string GetString(string key, object arg0, object arg1)
			{
				return string.Format(CultureInfo.CurrentCulture, Keys.resourceManager.GetString(key, Keys._culture), arg0, arg1);
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

		public static string MainReport
		{
			get
			{
				return Keys.GetString("MainReport");
			}
		}

		public static string RdlCompile_CouldNotWriteStateFile
		{
			get
			{
				return Keys.GetString("RdlCompile_CouldNotWriteStateFile");
			}
		}

		public static string RdlCompile_CouldNotOpenFile
		{
			get
			{
				return Keys.GetString("RdlCompile_CouldNotOpenFile");
			}
		}

		public static string DataSetExtensionName
		{
			get
			{
				return Keys.GetString("DataSetExtensionName");
			}
		}

		public static string MissingDataReader
		{
			get
			{
				return Keys.GetString("MissingDataReader");
			}
		}

		public static string CasPolicyUnavailableForCurrentAppDomain
		{
			get
			{
				return Keys.GetString("CasPolicyUnavailableForCurrentAppDomain");
			}
		}

		protected ProcessingStrings()
		{
		}

		public static string MissingDefinition(string reportName)
		{
			return Keys.GetString("MissingDefinition", reportName);
		}

		public static string pvInvalidDefinition(string reportPath)
		{
			return Keys.GetString("pvInvalidDefinition", reportPath);
		}

		public static string MapTileServerConfiguration_MaxConnectionsOutOfRange(int minValue, int maxValue)
		{
			return Keys.GetString("MapTileServerConfiguration_MaxConnectionsOutOfRange", minValue, maxValue);
		}

		public static string MapTileServerConfiguration_TimeoutOutOfRange(int minValue, int maxValue)
		{
			return Keys.GetString("MapTileServerConfiguration_TimeoutOutOfRange", minValue, maxValue);
		}
	}
}
