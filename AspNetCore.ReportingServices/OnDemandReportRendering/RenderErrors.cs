using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[CompilerGenerated]
	internal class RenderErrors
	{
		[CompilerGenerated]
		internal class Keys
		{
			public const string rrInvalidPageNumber = "rrInvalidPageNumber";

			public const string rrRenderStyleError = "rrRenderStyleError";

			public const string rrRenderSectionInstanceError = "rrRenderSectionInstanceError";

			public const string rrRenderResultNull = "rrRenderResultNull";

			public const string rrRenderStreamNull = "rrRenderStreamNull";

			public const string rrRenderDeviceNull = "rrRenderDeviceNull";

			public const string rrRenderReportNull = "rrRenderReportNull";

			public const string rrRenderReportNameNull = "rrRenderReportNameNull";

			public const string rrRenderUnknownReportItem = "rrRenderUnknownReportItem";

			public const string rrRenderStyleName = "rrRenderStyleName";

			public const string rrRenderTextBox = "rrRenderTextBox";

			public const string rrRenderingError = "rrRenderingError";

			public const string rrUnexpectedError = "rrUnexpectedError";

			public const string rrControlInvalidTag = "rrControlInvalidTag";

			public const string rrPageNamespaceInvalid = "rrPageNamespaceInvalid";

			public const string rrInvalidAttribute = "rrInvalidAttribute";

			public const string rrInvalidProperty = "rrInvalidProperty";

			public const string rrInvalidStyleName = "rrInvalidStyleName";

			public const string rrInvalidControl = "rrInvalidControl";

			public const string rrParameterExpected = "rrParameterExpected";

			public const string rrExpectedTopLevelElement = "rrExpectedTopLevelElement";

			public const string rrInvalidDeviceInfo = "rrInvalidDeviceInfo";

			public const string rrInvalidParamValue = "rrInvalidParamValue";

			public const string rrExpectedEndElement = "rrExpectedEndElement";

			public const string rrReportNameNull = "rrReportNameNull";

			public const string rrReportParamsNull = "rrReportParamsNull";

			public const string rrRendererParamsNull = "rrRendererParamsNull";

			public const string rrMeasurementUnitError = "rrMeasurementUnitError";

			public const string rrInvalidOWCRequest = "rrInvalidOWCRequest";

			public const string rrInvalidColor = "rrInvalidColor";

			public const string rrInvalidSize = "rrInvalidSize";

			public const string rrInvalidMeasurementUnit = "rrInvalidMeasurementUnit";

			public const string rrNegativeSize = "rrNegativeSize";

			public const string rrOutOfRange = "rrOutOfRange";

			public const string rrInvalidStyleArgumentType = "rrInvalidStyleArgumentType";

			public const string rrInvalidBorderStyle = "rrInvalidBorderStyle";

			public const string rrInvalidUniqueName = "rrInvalidUniqueName";

			public const string rrInvalidActionLabel = "rrInvalidActionLabel";

			public const string rrInvalidMimeType = "rrInvalidMimeType";

			private static ResourceManager resourceManager = new ResourceManager(typeof(RenderErrors).FullName, typeof(RenderErrors).Module.Assembly);

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

		public static string rrRenderStyleError
		{
			get
			{
				return Keys.GetString("rrRenderStyleError");
			}
		}

		public static string rrRenderSectionInstanceError
		{
			get
			{
				return Keys.GetString("rrRenderSectionInstanceError");
			}
		}

		public static string rrRenderResultNull
		{
			get
			{
				return Keys.GetString("rrRenderResultNull");
			}
		}

		public static string rrRenderStreamNull
		{
			get
			{
				return Keys.GetString("rrRenderStreamNull");
			}
		}

		public static string rrRenderDeviceNull
		{
			get
			{
				return Keys.GetString("rrRenderDeviceNull");
			}
		}

		public static string rrRenderReportNull
		{
			get
			{
				return Keys.GetString("rrRenderReportNull");
			}
		}

		public static string rrRenderReportNameNull
		{
			get
			{
				return Keys.GetString("rrRenderReportNameNull");
			}
		}

		public static string rrRenderUnknownReportItem
		{
			get
			{
				return Keys.GetString("rrRenderUnknownReportItem");
			}
		}

		public static string rrRenderStyleName
		{
			get
			{
				return Keys.GetString("rrRenderStyleName");
			}
		}

		public static string rrRenderTextBox
		{
			get
			{
				return Keys.GetString("rrRenderTextBox");
			}
		}

		public static string rrRenderingError
		{
			get
			{
				return Keys.GetString("rrRenderingError");
			}
		}

		public static string rrUnexpectedError
		{
			get
			{
				return Keys.GetString("rrUnexpectedError");
			}
		}

		public static string rrControlInvalidTag
		{
			get
			{
				return Keys.GetString("rrControlInvalidTag");
			}
		}

		public static string rrPageNamespaceInvalid
		{
			get
			{
				return Keys.GetString("rrPageNamespaceInvalid");
			}
		}

		public static string rrInvalidAttribute
		{
			get
			{
				return Keys.GetString("rrInvalidAttribute");
			}
		}

		public static string rrInvalidProperty
		{
			get
			{
				return Keys.GetString("rrInvalidProperty");
			}
		}

		public static string rrInvalidStyleName
		{
			get
			{
				return Keys.GetString("rrInvalidStyleName");
			}
		}

		public static string rrInvalidControl
		{
			get
			{
				return Keys.GetString("rrInvalidControl");
			}
		}

		public static string rrParameterExpected
		{
			get
			{
				return Keys.GetString("rrParameterExpected");
			}
		}

		public static string rrReportNameNull
		{
			get
			{
				return Keys.GetString("rrReportNameNull");
			}
		}

		public static string rrReportParamsNull
		{
			get
			{
				return Keys.GetString("rrReportParamsNull");
			}
		}

		public static string rrRendererParamsNull
		{
			get
			{
				return Keys.GetString("rrRendererParamsNull");
			}
		}

		public static string rrMeasurementUnitError
		{
			get
			{
				return Keys.GetString("rrMeasurementUnitError");
			}
		}

		public static string rrInvalidOWCRequest
		{
			get
			{
				return Keys.GetString("rrInvalidOWCRequest");
			}
		}

		public static string rrInvalidUniqueName
		{
			get
			{
				return Keys.GetString("rrInvalidUniqueName");
			}
		}

		public static string rrInvalidActionLabel
		{
			get
			{
				return Keys.GetString("rrInvalidActionLabel");
			}
		}

		protected RenderErrors()
		{
		}

		public static string rrInvalidPageNumber(int totalNumPages)
		{
			return Keys.GetString("rrInvalidPageNumber", totalNumPages);
		}

		public static string rrExpectedTopLevelElement(string elementName)
		{
			return Keys.GetString("rrExpectedTopLevelElement", elementName);
		}

		public static string rrInvalidDeviceInfo(string detail)
		{
			return Keys.GetString("rrInvalidDeviceInfo", detail);
		}

		public static string rrInvalidParamValue(string paramName)
		{
			return Keys.GetString("rrInvalidParamValue", paramName);
		}

		public static string rrExpectedEndElement(string elementName)
		{
			return Keys.GetString("rrExpectedEndElement", elementName);
		}

		public static string rrInvalidColor(string color)
		{
			return Keys.GetString("rrInvalidColor", color);
		}

		public static string rrInvalidSize(string size)
		{
			return Keys.GetString("rrInvalidSize", size);
		}

		public static string rrInvalidMeasurementUnit(string size)
		{
			return Keys.GetString("rrInvalidMeasurementUnit", size);
		}

		public static string rrNegativeSize(string size)
		{
			return Keys.GetString("rrNegativeSize", size);
		}

		public static string rrOutOfRange(string size)
		{
			return Keys.GetString("rrOutOfRange", size);
		}

		public static string rrInvalidStyleArgumentType(string argumentType)
		{
			return Keys.GetString("rrInvalidStyleArgumentType", argumentType);
		}

		public static string rrInvalidBorderStyle(string style)
		{
			return Keys.GetString("rrInvalidBorderStyle", style);
		}

		public static string rrInvalidMimeType(string mimeType)
		{
			return Keys.GetString("rrInvalidMimeType", mimeType);
		}
	}
}
