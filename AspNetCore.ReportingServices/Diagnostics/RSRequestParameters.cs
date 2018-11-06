using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal abstract class RSRequestParameters : IReportParameterLookup
	{
		internal enum CacheDeviceInfoTags
		{
			Parameters,
			ReplacementRoot,
			StreamRoot
		}

		private enum HttpMethod
		{
			GET,
			POST
		}

		private const string ParametersXmlElement = "Parameters";

		private const string ParameterXmlElement = "Parameter";

		private const string NameXmlElement = "Name";

		private const string ValueXmlElement = "Value";

		private const string BrowserCapabilitiesXmlElement = "BrowserCapabilities";

		private const string DeviceInfoXmlElement = "DeviceInfo";

		public const string ReportParameterPrefix = "";

		public const string CatalogParameterPrefix = "rs:";

		public const string RenderingParameterPrefix = "rc:";

		public const string UserNameParameterPrefix = "dsu:";

		public const string PasswordParameterPrefix = "dsp:";

		public const string ParameterNullSuffix = ":isnull";

		public const string PowerViewParameterPrefix = "pv:";

		public const char PrefixTerminatorChar = ':';

		public const string FormatParamName = "Format";

		public const string EncodingParamName = "Encoding";

		public const string FullFeatureFormat = "HTML5";

		public const string OWCFormat = "HTMLOWC";

		public const string StreamRoot = "StreamRoot";

		public const string PrimaryVersion = "2015-02";

		public const string Version201411 = "2014-11";

		public const string Version201409iOSDev = "2014-09-iOSDev";

		public const string Version201409iOS = "2014-09-iOS";

		public const string Version201403 = "2014-03";

		public const string iOSDevVersion = "2014-09-iOSDev";

		public const string iOSVersion = "2014-09-iOS";

		public const string UpgradeVersion = "2014-11";

		public const string HelixVersion = "2014-03";

		public const string ShowHideToggleParamName = "ShowHideToggle";

		public const string SortIdParamName = "SortId";

		public const string ClearSortParamName = "ClearSort";

		public const string SortDirectionParamName = "SortDirection";

		public const string AllowNewSessionsParamName = "AllowNewSessions";

		public const string ResetSessionParamName = "ResetSession";

		public const string CommandParamName = "Command";

		public const string SessionIDParamName = "SessionID";

		public const string PowerViewSessionId = "ocp-sqlrs-session-id";

		public const string PowerViewRoutingToken = "ocp-sqlrs-rtoken";

		public const string ServiceApiVersion = "api-version";

		public const string ResponseApiVersion = "accept-api-version";

		public const string ResponseGroupVersion = "accept-api-version-group";

		public const string ReportId = "ReportId";

		public const string TileId = "TileId";

		public const string InstantiationMode = "InstantiationMode";

		public const string DashboardId = "DashboardId";

		public const string SaveType = "SaveType";

		public const string ReportName = "ReportName";

		public const string ImageIDParamName = "ImageID";

		public const string SnapshotParamName = "Snapshot";

		public const string ClearSessionParamName = "ClearSession";

		public const string ErrorResponseAsXml = "ErrorResponseAsXml";

		public const string StoredParametersID = "StoredParametersID";

		public const string ProgressiveSessionId = "ProgressiveSessionId";

		public const string ParamLanguage = "ParameterLanguage";

		public const string ReturnUrlParamName = "ReturnUrl";

		public const string RendererAccessCommand = "Get";

		public const string RunReportCommand = "Render";

		public const string ListChildrenCommand = "ListChildren";

		public const string GetResourceContentsCommand = "GetResourceContents";

		public const string GetDataSourceContentsCommand = "GetDataSourceContents";

		public const string GetModelDefinitionCommand = "GetModelDefinition";

		public const string DrillthroughCommand = "Drillthrough";

		public const string ExecuteQueryCommand = "ExecuteQuery";

		public const string BlankCommand = "Blank";

		public const string SortCommand = "Sort";

		public const string StyleSheet = "StyleSheet";

		public const string StyleSheetImage = "StyleSheetImage";

		public const string GetComponentDefinitionCommand = "GetComponentDefinition";

		public const string GetDataSetDefinitionCommand = "GetDataSetDefinition";

		public const string Ascending = "Ascending";

		public const string Descending = "Descending";

		public const string DBUserParamName = "DBUser";

		public const string DBPasswordParamName = "DBPassword";

		public const string PersistStreams = "PersistStreams";

		public const string GetNextStream = "GetNextStream";

		public const string EntityID = "EntityID";

		public const string DrillType = "DrillType";

		public const string DataSourceName = "DataSourceName";

		public const string CommandText = "CommandText";

		public const string Timeout = "Timeout";

		public const string GetUserModel = "GetUserModel";

		public const string PerspectiveID = "PerspectiveID";

		public const string OmitModelDefinitions = "OmitModelDefinitions";

		public const string ModelMetadataVersion = "ModelMetadataVersion";

		public const string ItemPath = "ItemPath";

		public const string SourceReportUri = "SourceReportUri";

		public const string ReturnRawDataParamName = "ReturnRawData";

		public const string StyleSheetName = "Name";

		public const string StyleSheetImageName = "Name";

		public const string PaginationMode = "PageCountMode";

		public const string ActualPageMode = "Actual";

		public const string EstimatePageMode = "Estimate";

		internal const string IsCancellable = "IsCancellable";

		public const string RenderEditCommand = "RenderEdit";

		public const string GetModelCommand = "GetModel";

		public const string GetReportAndModelsCommand = "GetReportAndModels";

		public const string GetExternalImagesCommand = "GetExternalImages";

		public const string ExecuteQueriesCommand = "ExecuteQueries";

		public const string LogClientTraceEventsCommand = "LogClientTraceEvents";

		public const string CloseSessionCommand = "CloseSession";

		public const string CancelProgressiveSessionJobsCommand = "CancelProgressiveSessionJobs";

		public const string PowerViewCloseSession = "CloseSession";

		public const string PowerViewOpenSession = "OpenSession";

		public const string PowerViewLoadReport = "LoadReport";

		public const string PowerViewLogClientActivities = "LogClientActivities";

		public const string PowerViewLogClientTraces = "LogClientTraces";

		public const string LoadDocument = "LoadDocument";

		public const string SaveReport = "SaveReport";

		public const string ExecuteCommands = "ExecuteCommands";

		public const string ExecuteSemanticQuery = "ExecuteSemanticQuery";

		public const string GetDocument = "GetDocument";

		public const string GetEntityDataModel = "GetEntityDataModel";

		public const string GetVisual = "GetVisual";

		public const string GetSemanticQuery = "GetSemanticQuery";

		public const string Height = "Height";

		public const string Width = "Width";

		public const string VisualName = "VisualName";

		public const string SheetName = "SheetName";

		public const string IsNew = "IsNew";

		public const string SheetReportSectionMapping = "SheetReportSectionMapping";

		public const string ReportContentType = "ReportContentType";

		public const string Mode = "Mode";

		public const string ModelId = "ModelId";

		public const string IsCloudRlsEnabled = "IsCloudRlsEnabled";

		public const string UserPrincipalName = "UserPrincipalName";

		public const string IsCloudRoleLevelSecurityMembershipEnabled = "IsCloudRoleLevelSecurityMembershipEnabled";

		public const string ImpersonatedUserPrincipalName = "ImpersonatedUserPrincipalName";

		public const string ImpersonatedRoles = "ImpersonatedRoles";

		public const string TenantId = "TenantId";

		public const string ExecuteDaxQuery = "ExecuteDaxQuery";

		private Hashtable m_reverseLookupParameters;

		protected NameValueCollection m_renderingParameters;

		private NameValueCollection m_reportParameters;

		private NameValueCollection m_catalogParameters;

		private DatasourceCredentialsCollection m_datasourcesCred;

		private NameValueCollection m_browserCapabilities;

		private string m_ServerVirtualRoot;

		private string m_SessionId;

		public static string PBIDeviceInfoStringFormat;

		private static readonly Dictionary<string, bool> m_crescentCommands;

		private static readonly Dictionary<string, HttpMethod> m_powerViewCommands;

		private static readonly List<string> m_powerViewServerVrmCommands;

		public NameValueCollection RenderingParameters
		{
			get
			{
				return this.m_renderingParameters;
			}
		}

		public NameValueCollection ReportParameters
		{
			get
			{
				return this.m_reportParameters;
			}
		}

		public string ReportParametersXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartElement("Parameters");
				for (int i = 0; i < this.m_reportParameters.Count; i++)
				{
					xmlTextWriter.WriteStartElement("Parameter");
					string key = this.m_reportParameters.GetKey(i);
					if (key != null)
					{
						xmlTextWriter.WriteElementString("Name", key);
					}
					string[] values = this.m_reportParameters.GetValues(i);
					xmlTextWriter.WriteStartElement("Values");
					if (values != null)
					{
						for (int j = 0; j < values.Length; j++)
						{
							xmlTextWriter.WriteElementString("Value", values[j]);
						}
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				return stringWriter.ToString();
			}
		}

		public NameValueCollection CatalogParameters
		{
			get
			{
				return this.m_catalogParameters;
			}
		}

		public string FormatParamValue
		{
			get
			{
				if (this.CatalogParameters != null)
				{
					return this.CatalogParameters["Format"];
				}
				return null;
			}
			set
			{
				this.CatalogParameters["Format"] = value;
			}
		}

		public string SessionIDParamValue
		{
			get
			{
				return this.CatalogParameters["SessionID"];
			}
			set
			{
				this.CatalogParameters["SessionID"] = value;
			}
		}

		public string ImageIDParamValue
		{
			get
			{
				return this.CatalogParameters["ImageID"];
			}
			set
			{
				this.CatalogParameters["ImageID"] = value;
			}
		}

		public string ReturnUrlValue
		{
			get
			{
				return this.CatalogParameters["ReturnUrl"];
			}
			set
			{
				this.CatalogParameters["ReturnUrl"] = value;
			}
		}

		public string SortIdParamValue
		{
			get
			{
				return this.CatalogParameters["SortId"];
			}
			set
			{
				this.CatalogParameters["SortId"] = value;
			}
		}

		public string ShowHideToggleParamValue
		{
			get
			{
				return this.CatalogParameters["ShowHideToggle"];
			}
			set
			{
				this.CatalogParameters["ShowHideToggle"] = value;
			}
		}

		public string SnapshotParamValue
		{
			get
			{
				return this.CatalogParameters["Snapshot"];
			}
			set
			{
				this.CatalogParameters["Snapshot"] = value;
			}
		}

		public string ClearSessionParamValue
		{
			get
			{
				return this.CatalogParameters["ClearSession"];
			}
			set
			{
				this.CatalogParameters["ClearSession"] = value;
			}
		}

		public string AllowNewSessionsParamValue
		{
			get
			{
				return this.CatalogParameters["AllowNewSessions"];
			}
			set
			{
				this.CatalogParameters["AllowNewSessions"] = value;
			}
		}

		public string CommandParamValue
		{
			get
			{
				return this.CatalogParameters["Command"];
			}
			set
			{
				this.CatalogParameters["Command"] = value;
			}
		}

		public string EntityIdValue
		{
			get
			{
				return this.CatalogParameters["EntityID"];
			}
		}

		public string DrillTypeValue
		{
			get
			{
				return this.CatalogParameters["DrillType"];
			}
		}

		public string PaginationModeValue
		{
			get
			{
				return this.CatalogParameters["PageCountMode"];
			}
			set
			{
				this.CatalogParameters["PageCountMode"] = value;
			}
		}

		public DatasourceCredentialsCollection DatasourcesCred
		{
			get
			{
				return this.m_datasourcesCred;
			}
			set
			{
				this.m_datasourcesCred = value;
			}
		}

		public NameValueCollection BrowserCapabilities
		{
			get
			{
				return this.m_browserCapabilities;
			}
		}

		public string ServerVirtualRoot
		{
			get
			{
				return this.m_ServerVirtualRoot;
			}
			set
			{
				this.m_ServerVirtualRoot = value;
			}
		}

		public string SessionId
		{
			get
			{
				return this.m_SessionId;
			}
			set
			{
				this.m_SessionId = value;
			}
		}

		string IReportParameterLookup.GetReportParamsInstanceId(NameValueCollection reportParameters)
		{
			if (this.m_reverseLookupParameters == null)
			{
				return null;
			}
			ReportParameterCollection key = new ReportParameterCollection(reportParameters);
			return (string)this.m_reverseLookupParameters[key];
		}

		public static string GetFallbackFormat()
		{
			return "HTML5";
		}

		public void ParseQueryString(NameValueCollection allParametersCollection, IParametersTranslator paramsTranslator, ExternalItemPath itemPath)
		{
			RSRequestParameters.ParseQueryString(itemPath, paramsTranslator, allParametersCollection, out this.m_catalogParameters, out this.m_renderingParameters, out this.m_reportParameters, out this.m_datasourcesCred, out this.m_reverseLookupParameters);
			this.ApplyDefaultRenderingParameters();
		}

		public string GetImageUrl(bool useSessionId, string imageId, ICatalogItemContext ctx)
		{
			string text = null;
			if (this.m_renderingParameters != null)
			{
				text = this.m_renderingParameters["StreamRoot"];
			}
			if (text != null && text != string.Empty)
			{
				StringBuilder stringBuilder = new StringBuilder(text);
				if (imageId != null)
				{
					stringBuilder.Append(imageId);
				}
				return stringBuilder.ToString();
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(ctx);
			string snapshotParamValue = this.SnapshotParamValue;
			if (snapshotParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("Snapshot", snapshotParamValue);
			}
			string sessionIDParamValue = this.SessionIDParamValue;
			if (sessionIDParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SessionID", sessionIDParamValue);
			}
			else if (useSessionId && this.m_SessionId != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("SessionID", this.m_SessionId);
			}
			string formatParamValue = this.FormatParamValue;
			if (formatParamValue != null)
			{
				catalogItemUrlBuilder.AppendCatalogParameter("Format", formatParamValue);
			}
			catalogItemUrlBuilder.AppendCatalogParameter("ImageID", imageId);
			return catalogItemUrlBuilder.ToString();
		}

		public static NameValueCollection ExtractReportParameters(NameValueCollection allParametersCollection, ref bool[] whichParamsAreShared, out NameValueCollection otherParameters)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			otherParameters = new NameValueCollection();
			List<bool> list = new List<bool>();
			for (int i = 0; i < allParametersCollection.Count; i++)
			{
				string text = allParametersCollection.GetKey(i);
				string[] array = allParametersCollection.GetValues(i);
				if (array != null && text != null)
				{
					if (StringSupport.EndsWith(text, ":isnull", true, CultureInfo.InvariantCulture))
					{
						text = text.Substring(0, text.Length - ":isnull".Length);
						string[] array2 = new string[1];
						array = array2;
					}
					if (StringSupport.EndsWith(text, ":isnull", true, CultureInfo.InvariantCulture))
					{
						text = text.Substring(0, text.Length - ":isnull".Length);
						string[] array3 = new string[1];
						array = array3;
					}
					if (StringSupport.StartsWith(text, "rs:", true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "rc:", true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "dsu:", true, CultureInfo.InvariantCulture) || StringSupport.StartsWith(text, "dsp:", true, CultureInfo.InvariantCulture))
					{
						if (!RSRequestParameters.TryToAddToCollection(text, array, null, false, otherParameters))
						{
							throw new InternalCatalogException("expected to add parameter to collection" + text.MarkAsUserContent());
						}
					}
					else
					{
						if (!RSRequestParameters.TryToAddToCollection(text, array, "", true, nameValueCollection))
						{
							throw new InternalCatalogException("expected to add parameter to collection" + text.MarkAsUserContent());
						}
						if (whichParamsAreShared != null && whichParamsAreShared.Length > 0)
						{
							list.Add(whichParamsAreShared[i]);
						}
					}
				}
			}
			if (whichParamsAreShared != null && whichParamsAreShared.Length > 0)
			{
				whichParamsAreShared = list.ToArray();
			}
			return nameValueCollection;
		}

		public static NameValueCollection ShallowXmlToNameValueCollection(string xml, string topElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (xml != null && !(xml == string.Empty))
			{
				XmlTextReader xmlTextReader = XmlUtil.SafeCreateXmlTextReader(xml);
				try
				{
					xmlTextReader.MoveToContent();
					if (xmlTextReader.NodeType == XmlNodeType.Element && string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) == 0)
					{
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.IsStartElement())
							{
								bool isEmptyElement = xmlTextReader.IsEmptyElement;
								string name = xmlTextReader.Name;
								name = XmlUtil.DecodePropertyName(name);
								string value = xmlTextReader.ReadString();
								if (nameValueCollection.GetValues(name) != null)
								{
									throw new InvalidXmlException();
								}
								nameValueCollection[name] = value;
								if (!isEmptyElement && xmlTextReader.IsStartElement())
								{
									throw new InvalidXmlException();
								}
							}
						}
						return nameValueCollection;
					}
					throw new InvalidXmlException();
				}
				catch (XmlException ex)
				{
					throw new MalformedXmlException(ex);
				}
			}
			return nameValueCollection;
		}

		public static NameValueCollection DeepXmlToNameValueCollection(string xml, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(StringComparer.InvariantCulture);
			if (xml != null && !(xml == string.Empty))
			{
				XmlTextReader xmlTextReader = XmlUtil.SafeCreateXmlTextReader(xml);
				try
				{
					xmlTextReader.MoveToContent();
					if (xmlTextReader.NodeType == XmlNodeType.Element && string.Compare(xmlTextReader.Name, topElementTag, StringComparison.Ordinal) == 0)
					{
						while (xmlTextReader.Read())
						{
							if (xmlTextReader.IsStartElement())
							{
								if (xmlTextReader.IsEmptyElement || string.Compare(xmlTextReader.Name, eachElementTag, StringComparison.Ordinal) != 0)
								{
									throw new InvalidXmlException();
								}
								xmlTextReader.Read();
								string text = null;
								string value = null;
								while (true)
								{
									if (!xmlTextReader.IsStartElement())
									{
										break;
									}
									bool isEmptyElement = xmlTextReader.IsEmptyElement;
									string name = xmlTextReader.Name;
									string text2 = xmlTextReader.ReadString();
									if (string.Compare(name, nameElementTag, StringComparison.Ordinal) == 0)
									{
										text = text2;
									}
									else if (string.Compare(name, valueElementTag, StringComparison.Ordinal) == 0)
									{
										value = text2;
									}
									if (!isEmptyElement)
									{
										xmlTextReader.ReadEndElement();
									}
									else
									{
										xmlTextReader.Read();
									}
								}
								if (text == null)
								{
									throw new InvalidXmlException();
								}
								nameValueCollection.Add(text, value);
							}
						}
						return nameValueCollection;
					}
					throw new InvalidXmlException();
				}
				catch (XmlException ex)
				{
					throw new MalformedXmlException(ex);
				}
			}
			return nameValueCollection;
		}

		public static string NameValueCollectionToShallowXml(NameValueCollection parameters, string topElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string text = parameters.Get(i);
				if (key != null && text != null)
				{
					if (string.IsNullOrEmpty(key))
					{
						throw new InternalCatalogException("Empty Property Name");
					}
					string text2 = XmlUtil.EncodePropertyName(key);
					RSTrace.CatalogTrace.Assert(!string.IsNullOrEmpty(text2), "encodedName");
					xmlTextWriter.WriteStartElement(text2);
					xmlTextWriter.WriteString(text);
					xmlTextWriter.WriteEndElement();
				}
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public static string NameValueCollectionToDeepXml(NameValueCollection parameters, string topElementTag, string eachElementTag, string nameElementTag, string valueElementTag)
		{
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartElement(topElementTag);
			for (int i = 0; i < parameters.Count; i++)
			{
				xmlTextWriter.WriteStartElement(eachElementTag);
				string key = parameters.GetKey(i);
				if (key != null)
				{
					xmlTextWriter.WriteElementString(nameElementTag, key);
				}
				string text = parameters.Get(i);
				if (text != null)
				{
					xmlTextWriter.WriteElementString(valueElementTag, text);
				}
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			return stringWriter.ToString();
		}

		public void SetRenderingParameters(string renderingParametersXml)
		{
			this.m_renderingParameters = RSRequestParameters.ShallowXmlToNameValueCollection(renderingParametersXml, "DeviceInfo");
			this.ApplyDefaultRenderingParameters();
		}

		public void SetRenderingParameters(NameValueCollection otherParams)
		{
			if (otherParams != null)
			{
				this.m_renderingParameters = new NameValueCollection(otherParams);
			}
			else
			{
				this.m_renderingParameters = new NameValueCollection();
			}
		}

		protected abstract void ApplyDefaultRenderingParameters();

		public void SetReportParameters(string reportParametersXml)
		{
			this.m_reportParameters = RSRequestParameters.DeepXmlToNameValueCollection(reportParametersXml, "Parameters", "Parameter", "Name", "Value");
		}

		public void SetReportParameters(NameValueCollection allReportParameters)
		{
			this.m_reportParameters = allReportParameters;
			if (this.m_reportParameters == null)
			{
				this.m_reportParameters = new NameValueCollection();
			}
		}

		public void SetReportParameters(NameValueCollection allReportParameters, IParametersTranslator paramsTranslator)
		{
			if (allReportParameters != null)
			{
				string text = allReportParameters["rs:StoredParametersID"];
				if (text != null)
				{
					ExternalItemPath externalItemPath = default(ExternalItemPath);
					NameValueCollection nameValueCollection = default(NameValueCollection);
					paramsTranslator.GetParamsInstance(text, out externalItemPath, out nameValueCollection);
					if (nameValueCollection == null)
					{
						throw new StoredParameterNotFoundException(text.MarkAsPrivate());
					}
					NameValueCollection nameValueCollection2 = new NameValueCollection();
					foreach (string item in nameValueCollection)
					{
						string[] values = nameValueCollection.GetValues(item);
						RSRequestParameters.TryToAddToCollection(item, values, "", true, nameValueCollection2);
					}
					this.m_reportParameters = nameValueCollection2;
					return;
				}
			}
			this.SetReportParameters(allReportParameters);
		}

		public void SetCatalogParameters(string catalogParametersXml)
		{
			this.m_catalogParameters = RSRequestParameters.ShallowXmlToNameValueCollection(catalogParametersXml, "Parameters");
		}

		public void DetectFormatIfNotPresent()
		{
			RSRequestParameters.GuessFormatIfNotPresent(this.m_catalogParameters);
		}

		public void SetBrowserCapabilities(NameValueCollection browserCapabilities)
		{
			this.m_browserCapabilities = browserCapabilities;
		}

		public static bool HasPrefix(string name, string prefix, out string unprefixedName)
		{
			unprefixedName = name;
			if (prefix != null)
			{
				if (prefix.Length != 0)
				{
					if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					unprefixedName = name.Substring(prefix.Length);
				}
				else if (name.IndexOf(':') >= 0)
				{
					return false;
				}
			}
			return true;
		}

		public NameValueCollection GetAllParameters()
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (this.m_catalogParameters != null)
			{
				foreach (string catalogParameter in this.m_catalogParameters)
				{
					nameValueCollection["rs:" + catalogParameter] = this.m_catalogParameters[catalogParameter];
				}
			}
			if (this.m_reportParameters != null)
			{
				foreach (string reportParameter in this.m_reportParameters)
				{
					nameValueCollection[reportParameter] = this.m_reportParameters[reportParameter];
				}
			}
			if (this.m_renderingParameters != null)
			{
				{
					foreach (string renderingParameter in this.m_renderingParameters)
					{
						nameValueCollection["rc:" + renderingParameter] = this.m_renderingParameters[renderingParameter];
					}
					return nameValueCollection;
				}
			}
			return nameValueCollection;
		}

		private static void ResolveServerParameters(IParametersTranslator paramsTranslator, NameValueCollection allParametersCollection, NameValueCollection rsParameters, NameValueCollection rcParameters, NameValueCollection dsuParameters, NameValueCollection dspParameters, NameValueCollection reportParameters, out Hashtable reverseLookup, out ExternalItemPath itemPath)
		{
			reverseLookup = new Hashtable();
			itemPath = null;
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < allParametersCollection.Count; i++)
			{
				string key = allParametersCollection.GetKey(i);
				if (key != null && StringComparer.OrdinalIgnoreCase.Compare(key, "rs:StoredParametersID") == 0)
				{
					string text = allParametersCollection[i];
					NameValueCollection nameValueCollection = default(NameValueCollection);
					paramsTranslator.GetParamsInstance(text, out itemPath, out nameValueCollection);
					if (nameValueCollection == null)
					{
						throw new StoredParameterNotFoundException(text.MarkAsPrivate());
					}
					reverseLookup.Add(new ReportParameterCollection(nameValueCollection), text);
					stringCollection.Add(key);
					foreach (string item in nameValueCollection)
					{
						string[] values = nameValueCollection.GetValues(item);
						if (!RSRequestParameters.TryToAddToCollection(item, values, "rs:", false, rsParameters) && !RSRequestParameters.TryToAddToCollection(item, values, "rc:", false, rcParameters) && !RSRequestParameters.TryToAddToCollection(item, values, "dsu:", false, dsuParameters) && !RSRequestParameters.TryToAddToCollection(item, values, "dsp:", false, dspParameters))
						{
							RSRequestParameters.TryToAddToCollection(item, values, "", true, reportParameters);
						}
					}
				}
			}
			StringEnumerator enumerator2 = stringCollection.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current = enumerator2.Current;
					allParametersCollection.Remove(current);
				}
			}
			finally
			{
				IDisposable disposable = enumerator2 as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static void ParseQueryString(ExternalItemPath itemPath, IParametersTranslator paramsTranslator, NameValueCollection allParametersCollection, out NameValueCollection rsParameters, out NameValueCollection rcParameters, out NameValueCollection reportParameters, out DatasourceCredentialsCollection dsParameters, out Hashtable reverseLookup)
		{
			dsParameters = null;
			reverseLookup = null;
			rsParameters = new NameValueCollection();
			rcParameters = new NameValueCollection();
			reportParameters = new NameValueCollection();
			NameValueCollection nameValueCollection = new NameValueCollection();
			NameValueCollection nameValueCollection2 = new NameValueCollection();
			ExternalItemPath externalItemPath = null;
			if (allParametersCollection != null)
			{
				RSRequestParameters.ResolveServerParameters(paramsTranslator, allParametersCollection, rsParameters, rcParameters, nameValueCollection, nameValueCollection2, reportParameters, out reverseLookup, out externalItemPath);
				if (externalItemPath != null && Localization.CatalogCultureCompare(itemPath.Value, externalItemPath.Value) != 0)
				{
					rsParameters = new NameValueCollection();
					rcParameters = new NameValueCollection();
					nameValueCollection = new NameValueCollection();
					nameValueCollection2 = new NameValueCollection();
					reportParameters = new NameValueCollection();
					reverseLookup = null;
					if (RSTrace.CatalogTrace.TraceInfo)
					{
						string message = string.Format(CultureInfo.InvariantCulture, "Requested item path '{0}' doesn't match stored parameters path '{1}'.", itemPath.Value.MarkAsPrivate(), externalItemPath.Value.MarkAsPrivate());
						RSTrace.CatalogTrace.Trace(TraceLevel.Info, message);
					}
				}
				for (int i = 0; i < allParametersCollection.Count; i++)
				{
					string text = allParametersCollection.GetKey(i);
					string[] array = allParametersCollection.GetValues(i);
					if (array != null && text != null)
					{
						if (StringSupport.EndsWith(text, ":isnull", true, CultureInfo.InvariantCulture))
						{
							text = text.Substring(0, text.Length - ":isnull".Length);
							string[] array2 = new string[1];
							array = array2;
						}
						if (!RSRequestParameters.TryToAddToCollection(text, array, "rs:", false, rsParameters) && !RSRequestParameters.TryToAddToCollection(text, array, "rc:", false, rcParameters) && !RSRequestParameters.TryToAddToCollection(text, array, "dsu:", false, nameValueCollection) && !RSRequestParameters.TryToAddToCollection(text, array, "dsp:", false, nameValueCollection2))
						{
							RSRequestParameters.TryToAddToCollection(text, array, "", true, reportParameters);
						}
					}
				}
				dsParameters = new DatasourceCredentialsCollection(nameValueCollection, nameValueCollection2);
			}
		}

		private static bool TryToAddToCollection(string name, string[] val, string prefix, bool allowMultiValue, NameValueCollection collection)
		{
			string name2 = default(string);
			if (!RSRequestParameters.HasPrefix(name, prefix, out name2))
			{
				return false;
			}
			if (!allowMultiValue)
			{
				if (val.Length > 1)
				{
					return true;
				}
				string[] values = collection.GetValues(name2);
				if (values == null)
				{
					collection[name2] = val[0];
					return true;
				}
				if (val[0] == null)
				{
					collection[name2] = null;
					return true;
				}
				return true;
			}
			foreach (string value in val)
			{
				collection.Add(name2, value);
			}
			return true;
		}

		private static void GuessFormatIfNotPresent(NameValueCollection catalogParameters)
		{
			if (catalogParameters["Format"] == null)
			{
				catalogParameters["Format"] = RSRequestParameters.GetFallbackFormat();
			}
		}

		static RSRequestParameters()
		{
			RSRequestParameters.PBIDeviceInfoStringFormat = "<DeviceInfo>\r\n                        <UseFullUrls>True</UseFullUrls>\r\n                        <PageHeight>3.125in</PageHeight>\r\n                        <PageWidth>5.3125in</PageWidth>\r\n                        <ActiveXControls>False</ActiveXControls>\r\n                        <OutputFormat>PNG</OutputFormat>                   \r\n                        <ReportItemPath>{0}</ReportItemPath>\r\n                    </DeviceInfo>";
			RSRequestParameters.m_crescentCommands = new Dictionary<string, bool>(8);
			RSRequestParameters.m_powerViewCommands = new Dictionary<string, HttpMethod>(6);
			RSRequestParameters.m_powerViewServerVrmCommands = new List<string>();
			RSRequestParameters.m_crescentCommands.Add("RenderEdit", true);
			RSRequestParameters.m_crescentCommands.Add("GetModel", true);
			RSRequestParameters.m_crescentCommands.Add("ExecuteQueries", true);
			RSRequestParameters.m_crescentCommands.Add("LogClientTraceEvents", true);
			RSRequestParameters.m_crescentCommands.Add("CloseSession", true);
			RSRequestParameters.m_crescentCommands.Add("CancelProgressiveSessionJobs", true);
			RSRequestParameters.m_crescentCommands.Add("GetExternalImages", true);
			RSRequestParameters.m_crescentCommands.Add("GetReportAndModels", true);
			RSRequestParameters.m_powerViewCommands.Add("CloseSession", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("OpenSession", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("LogClientActivities", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("LogClientTraces", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("ExecuteCommands", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("LoadDocument", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("LoadReport", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("GetVisual", HttpMethod.GET);
			RSRequestParameters.m_powerViewCommands.Add("SaveReport", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("ExecuteSemanticQuery", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("GetEntityDataModel", HttpMethod.GET);
			RSRequestParameters.m_powerViewCommands.Add("GetSemanticQuery", HttpMethod.POST);
			RSRequestParameters.m_powerViewCommands.Add("GetDocument", HttpMethod.GET);
			RSRequestParameters.m_powerViewServerVrmCommands.Add("ExecuteCommands");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("LoadDocument");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("LoadReport");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("GetVisual");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("ExecuteSemanticQuery");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("SaveReport");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("GetSemanticQuery");
			RSRequestParameters.m_powerViewServerVrmCommands.Add("GetDocument");
		}

		internal static bool IsCrescentCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return RSRequestParameters.m_crescentCommands.ContainsKey(command);
		}

		internal static bool IsPowerViewCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return RSRequestParameters.m_powerViewCommands.ContainsKey(command);
		}

		internal static bool IsPowerViewVrmCommand(string command)
		{
			if (string.IsNullOrEmpty(command))
			{
				return false;
			}
			return RSRequestParameters.m_powerViewServerVrmCommands.Contains(command);
		}

		internal static bool IsPOSTOnlyCommand(string command)
		{
			if (!RSRequestParameters.IsPowerViewCommand(command))
			{
				return false;
			}
			return RSRequestParameters.m_powerViewCommands[command] == HttpMethod.POST;
		}
	}
}
