using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[CompilerGenerated]
	internal class RDLUpgradeStrings
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string rdlInvalidTargetNamespace = "rdlInvalidTargetNamespace";

			public const string rdlInvalidXmlContents = "rdlInvalidXmlContents";

			private static ResourceManager resourceManager = new ResourceManager(typeof(RDLUpgradeStrings).FullName, typeof(RDLUpgradeStrings).Module.Assembly);

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

		protected RDLUpgradeStrings()
		{
		}

		public static string rdlInvalidTargetNamespace(string @namespace)
		{
			return Keys.GetString("rdlInvalidTargetNamespace", @namespace);
		}

		public static string rdlInvalidXmlContents(string innerMessage)
		{
			return Keys.GetString("rdlInvalidXmlContents", innerMessage);
		}
	}
}
