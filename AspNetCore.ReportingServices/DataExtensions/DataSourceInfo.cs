using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourceInfo
	{
		internal enum CredentialsRetrievalOption
		{
			Unknown,
			Prompt,
			Store,
			Integrated,
			None,
			ServiceAccount,
			SecureStore
		}

		[Flags]
		private enum DataSourceFlags
		{
			Enabled = 1,
			ReferenceIsValid = 2,
			ImpersonateUser = 4,
			WindowsCredentials = 8,
			IsModel = 0x10,
			ConnectionStringUseridReference = 0x20
		}

		private const DataSourceFlags DefaultFlags = DataSourceFlags.Enabled | DataSourceFlags.ReferenceIsValid;

		internal const string ExtensionXmlTag = "Extension";

		internal const string ConnectionStringXmlTag = "ConnectString";

		internal const string UseOriginalConnectStringXmlTag = "UseOriginalConnectString";

		internal const string OriginalConnectStringExpressionBasedXmlTag = "OriginalConnectStringExpressionBased";

		internal const string CredentialRetrievalXmlTag = "CredentialRetrieval";

		internal const string ImpersonateUserXmlTag = "ImpersonateUser";

		internal const string PromptXmlTag = "Prompt";

		internal const string WindowsCredentialsXmlTag = "WindowsCredentials";

		internal const string UserNameXmlTag = "UserName";

		internal const string PasswordXmlTag = "Password";

		internal const string EnabledXmlTag = "Enabled";

		internal const string NameXmlTag = "Name";

		internal const string SecureStoreLookupXmlTag = "SecureStoreLookup";

		internal const string TargetApplicationIdXmlTag = "TargetApplicationId";

		internal const string DataSourcesXmlTag = "DataSources";

		internal const string DataSourceXmlTag = "DataSource";

		internal const string DataSourceDefinitionXmlTag = "DataSourceDefinition";

		internal const string m_dataSourceReferenceXmlTag = "DataSourceReference";

		internal const string InvalidDataSourceReferenceXmlTag = "InvalidDataSourceReference";

		internal const string XmlNameSpace = "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource";

		[NonSerialized]
		private const RegexOptions CompiledRegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;

		[NonSerialized]
		private const string UseridPattern = "{{[\\s]*[uU][sS][eE][rR][iI][dD][\\s]*}}";

		private Guid m_id;

		private string m_name;

		private string m_originalName;

		private string m_extension;

		private byte[] m_connectionStringEncrypted;

		private byte[] m_originalConnectionStringEncrypted;

		private bool m_originalConnectStringExpressionBased;

		private string m_dataSourceReference;

		private Guid m_linkID;

		private Guid m_DataSourceWithCredentialsId;

		private byte[] m_secDesc;

		private CredentialsRetrievalOption m_credentialsRetrieval;

		private string m_prompt;

		private byte[] m_userNameEncrypted;

		private byte[] m_passwordEncrypted;

		private DataSourceFlags m_flags;

		private Guid m_modelID = Guid.Empty;

		private DateTime? m_modelLastUpdatedTime;

		private bool m_isEmbeddedInModel;

		private bool m_isModelSecurityUsed;

		private string m_tenantName;

		[NonSerialized]
		private bool m_isExternalDataSource;

		[NonSerialized]
		private bool m_isFullyFormedExternalDataSource;

		[NonSerialized]
		private bool m_isMultidimensional;

		[NonSerialized]
		private IServiceEndpoint m_serviceEndpoint;

		[NonSerialized]
		private SecureStringWrapper m_passwordSecureString;

		[NonSerialized]
		private SecureStoreLookup m_secureStoreLookup;

		[NonSerialized]
		private DataSourceFaultContext m_dataSourceFaultContext;

		[NonSerialized]
		private string m_modelPerspectiveName;

		[NonSerialized]
		private static Regex m_useridDetectionRegex;

		public Guid ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public Guid DataSourceWithCredentialsID
		{
			get
			{
				return this.m_DataSourceWithCredentialsId;
			}
			set
			{
				this.m_DataSourceWithCredentialsId = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public string PromptIdentifier
		{
			get
			{
				return this.OriginalName;
			}
		}

		public string OriginalName
		{
			get
			{
				return this.m_originalName;
			}
			set
			{
				this.m_originalName = value;
			}
		}

		public string Extension
		{
			get
			{
				return this.m_extension;
			}
			set
			{
				this.m_extension = value;
			}
		}

		public byte[] ConnectionStringEncrypted
		{
			get
			{
				return this.m_connectionStringEncrypted;
			}
		}

		public bool UseOriginalConnectionString
		{
			get
			{
				return this.m_connectionStringEncrypted == null;
			}
		}

		public byte[] OriginalConnectionStringEncrypted
		{
			get
			{
				return this.m_originalConnectionStringEncrypted;
			}
		}

		public bool OriginalConnectStringExpressionBased
		{
			get
			{
				return this.m_originalConnectStringExpressionBased;
			}
		}

		internal bool IsExternalDataSource
		{
			get
			{
				return this.m_isExternalDataSource;
			}
			set
			{
				this.m_isExternalDataSource = value;
			}
		}

		internal bool IsFullyFormedExternalDataSource
		{
			get
			{
				return this.m_isFullyFormedExternalDataSource;
			}
			set
			{
				this.m_isFullyFormedExternalDataSource = value;
			}
		}

		internal bool IsMultiDimensional
		{
			get
			{
				return this.m_isMultidimensional;
			}
			set
			{
				this.m_isMultidimensional = value;
			}
		}

		public string DataSourceReference
		{
			get
			{
				return this.m_dataSourceReference;
			}
			set
			{
				this.m_dataSourceReference = value;
			}
		}

		public Guid LinkID
		{
			get
			{
				return this.m_linkID;
			}
		}

		public bool ReferenceByPath
		{
			get
			{
				if (this.DataSourceReference != null && this.LinkID == Guid.Empty)
				{
					return this.ReferenceIsValid;
				}
				return false;
			}
		}

		public bool IsReference
		{
			get
			{
				if (this.DataSourceReference == null && !(this.LinkID != Guid.Empty))
				{
					return !this.ReferenceIsValid;
				}
				return true;
			}
		}

		public byte[] SecurityDescriptor
		{
			get
			{
				return this.m_secDesc;
			}
		}

		public CredentialsRetrievalOption CredentialsRetrieval
		{
			get
			{
				return this.m_credentialsRetrieval;
			}
			set
			{
				this.m_credentialsRetrieval = value;
			}
		}

		public bool ImpersonateUser
		{
			get
			{
				return (this.m_flags & DataSourceFlags.ImpersonateUser) != (DataSourceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= DataSourceFlags.ImpersonateUser;
				}
				else
				{
					this.m_flags &= ~DataSourceFlags.ImpersonateUser;
				}
			}
		}

		public string Prompt
		{
			get
			{
				return this.m_prompt;
			}
			set
			{
				this.m_prompt = value;
			}
		}

		public bool WindowsCredentials
		{
			get
			{
				return (this.m_flags & DataSourceFlags.WindowsCredentials) != (DataSourceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= DataSourceFlags.WindowsCredentials;
				}
				else
				{
					this.m_flags &= ~DataSourceFlags.WindowsCredentials;
				}
			}
		}

		public byte[] UserNameEncrypted
		{
			get
			{
				return this.m_userNameEncrypted;
			}
		}

		public byte[] PasswordEncrypted
		{
			get
			{
				return this.m_passwordEncrypted;
			}
		}

		public SecureStoreLookup SecureStoreLookup
		{
			get
			{
				return this.m_secureStoreLookup;
			}
		}

		public DataSourceFaultContext DataSourceFaultContext
		{
			get
			{
				return this.m_dataSourceFaultContext;
			}
		}

		public bool IsCredentialSet
		{
			get;
			set;
		}

		public bool Enabled
		{
			get
			{
				return (this.m_flags & DataSourceFlags.Enabled) != (DataSourceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= DataSourceFlags.Enabled;
				}
				else
				{
					this.m_flags &= ~DataSourceFlags.Enabled;
				}
			}
		}

		public bool IsModel
		{
			get
			{
				return DataSourceInfo.StaticIsModel((int)this.m_flags);
			}
			set
			{
				if (value)
				{
					this.m_flags |= DataSourceFlags.IsModel;
				}
				else
				{
					this.m_flags &= ~DataSourceFlags.IsModel;
				}
			}
		}

		public bool IsModelSecurityUsed
		{
			get
			{
				return this.m_isModelSecurityUsed;
			}
		}

		public IServiceEndpoint ServiceEndpoint
		{
			get
			{
				return this.m_serviceEndpoint;
			}
			set
			{
				this.m_serviceEndpoint = value;
			}
		}

		public string TenantName
		{
			get
			{
				return this.m_tenantName;
			}
			set
			{
				this.m_tenantName = value;
			}
		}

		public Guid ModelID
		{
			get
			{
				return this.m_modelID;
			}
			set
			{
				this.m_modelID = value;
			}
		}

		public DateTime? ModelLastUpdatedTime
		{
			get
			{
				return this.m_modelLastUpdatedTime;
			}
			set
			{
				this.m_modelLastUpdatedTime = value;
			}
		}

		public string ModelPerspectiveName
		{
			get
			{
				return this.m_modelPerspectiveName;
			}
			set
			{
				this.m_modelPerspectiveName = value;
			}
		}

		public bool ReferenceIsValid
		{
			get
			{
				return (this.m_flags & DataSourceFlags.ReferenceIsValid) != (DataSourceFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= DataSourceFlags.ReferenceIsValid;
				}
				else
				{
					this.m_flags &= ~DataSourceFlags.ReferenceIsValid;
				}
			}
		}

		public bool NeedPrompt
		{
			get
			{
				if (this.CredentialsRetrieval == CredentialsRetrievalOption.Prompt && this.m_userNameEncrypted == null)
				{
					return true;
				}
				return false;
			}
		}

		public int FlagsForCatalogSerialization
		{
			get
			{
				DataSourceFlags dataSourceFlags = this.m_flags;
				if (!this.m_isEmbeddedInModel)
				{
					dataSourceFlags &= ~DataSourceFlags.IsModel;
				}
				return (int)dataSourceFlags;
			}
		}

		public bool HasConnectionStringUseridReference
		{
			get
			{
				return (this.m_flags & DataSourceFlags.ConnectionStringUseridReference) != (DataSourceFlags)0;
			}
		}

		private static Regex UseridDetectionRegex
		{
			get
			{
				if (DataSourceInfo.m_useridDetectionRegex == null)
				{
					DataSourceInfo.m_useridDetectionRegex = new Regex("{{[\\s]*[uU][sS][eE][rR][iI][dD][\\s]*}}", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);
				}
				return DataSourceInfo.m_useridDetectionRegex;
			}
		}

		public void LinkToStandAlone(DataSourceInfo standAlone, string standAlonePath, Guid standAloneCatalogItemId)
		{
			this.m_name = standAlone.m_name;
			this.m_extension = standAlone.m_extension;
			this.m_connectionStringEncrypted = standAlone.m_connectionStringEncrypted;
			this.m_dataSourceReference = standAlonePath;
			this.m_linkID = standAloneCatalogItemId;
			this.m_secDesc = standAlone.m_secDesc;
			this.m_credentialsRetrieval = standAlone.CredentialsRetrieval;
			this.m_prompt = standAlone.m_prompt;
			this.m_userNameEncrypted = standAlone.m_userNameEncrypted;
			this.m_passwordEncrypted = standAlone.m_passwordEncrypted;
			this.Enabled = standAlone.Enabled;
			this.ImpersonateUser = standAlone.ImpersonateUser;
			this.WindowsCredentials = standAlone.WindowsCredentials;
		}

		public void LinkModelToDataSource(DataSourceInfo standAlone, Guid modelID)
		{
			this.m_DataSourceWithCredentialsId = standAlone.m_DataSourceWithCredentialsId;
			this.m_extension = standAlone.m_extension;
			this.m_connectionStringEncrypted = standAlone.m_connectionStringEncrypted;
			this.m_credentialsRetrieval = standAlone.CredentialsRetrieval;
			this.m_prompt = standAlone.Prompt;
			this.m_userNameEncrypted = standAlone.m_userNameEncrypted;
			this.m_passwordEncrypted = standAlone.m_passwordEncrypted;
			this.Enabled = standAlone.Enabled;
			this.ImpersonateUser = standAlone.ImpersonateUser;
			this.m_flags = standAlone.m_flags;
			this.m_modelID = modelID;
			this.m_isEmbeddedInModel = false;
			this.IsModel = true;
		}

		public void InitializeAsEmbeddedInModel(Guid modelID)
		{
			this.m_modelID = modelID;
			this.m_isEmbeddedInModel = true;
			this.IsModel = true;
		}

		public void CopyFrom(DataSourceInfo copy, string referencePath, Guid linkToCatalogItemId, bool isEmbeddedInModel)
		{
			this.LinkToStandAlone(copy, referencePath, linkToCatalogItemId);
			this.m_flags = copy.m_flags;
			this.m_modelID = copy.ModelID;
			this.m_modelLastUpdatedTime = copy.ModelLastUpdatedTime;
			this.m_isEmbeddedInModel = isEmbeddedInModel;
			if (isEmbeddedInModel)
			{
				this.IsModel = true;
			}
		}

		public DataSourceInfo(SerializationInfo info, StreamingContext context)
		{
			this.m_id = (Guid)info.GetValue("id", typeof(Guid));
			this.m_DataSourceWithCredentialsId = (Guid)info.GetValue("originalid", typeof(Guid));
			this.m_name = (string)info.GetValue("name", typeof(string));
			this.m_originalName = (string)info.GetValue("originalname", typeof(string));
			this.m_extension = (string)info.GetValue("extension", typeof(string));
			this.m_connectionStringEncrypted = (byte[])info.GetValue("connectionstringencrypted", typeof(byte[]));
			this.m_originalConnectionStringEncrypted = (byte[])info.GetValue("originalconnectionstringencrypted", typeof(byte[]));
			this.m_originalConnectStringExpressionBased = (bool)info.GetValue("originalConnectStringExpressionBased", typeof(bool));
			this.m_dataSourceReference = (string)info.GetValue("datasourcereference", typeof(string));
			this.m_linkID = (Guid)info.GetValue("linkid", typeof(Guid));
			this.m_secDesc = (byte[])info.GetValue("secdesc", typeof(byte[]));
			this.m_credentialsRetrieval = (CredentialsRetrievalOption)info.GetValue("credentialsretrieval", typeof(CredentialsRetrievalOption));
			this.m_prompt = (string)info.GetValue("prompt", typeof(string));
			this.m_userNameEncrypted = (byte[])info.GetValue("usernameencrypted", typeof(byte[]));
			this.m_passwordEncrypted = (byte[])info.GetValue("passwordencrypted", typeof(byte[]));
			this.m_flags = (DataSourceFlags)info.GetValue("datasourceflags", typeof(DataSourceFlags));
			this.m_modelID = (Guid)info.GetValue("modelid", typeof(Guid));
			this.m_modelLastUpdatedTime = (DateTime?)info.GetValue("modellastupdatedtime", typeof(DateTime?));
			this.m_isEmbeddedInModel = (bool)info.GetValue("isembeddedinmodel", typeof(bool));
		}

		public static DataSourceInfo ParseDataSourceNode(XmlNode node, bool clientLoad, IDataProtection dataProtection)
		{
			return DataSourceInfo.ParseDataSourceNode(node, clientLoad, false, dataProtection);
		}

		public static DataSourceInfo ParseDataSourceNode(XmlNode node, bool clientLoad, bool allowNoName, IDataProtection dataProtection)
		{
            if (node.Name != "DataSource")
            {
                throw new InvalidXmlException();
            }
            XmlNode xmlNode = node.SelectSingleNode("Name");
            node.SelectSingleNode("Extension");
            XmlNode xmlNode2 = node.SelectSingleNode("DataSourceDefinition");
            XmlNode xmlNode3 = node.SelectSingleNode("DataSourceReference");
            DataSourceInfo result = null;
            if ((!allowNoName && xmlNode == null) || xmlNode2 == null == (xmlNode3 == null))
            {
                bool flag = true;
                if (clientLoad)
                {
                    XmlNode xmlNode4 = node.SelectSingleNode("InvalidDataSourceReference");
                    if (xmlNode4 != null)
                    {
                        flag = false;
                        result = new DataSourceInfo((xmlNode == null) ? "" : xmlNode.InnerText);
                    }
                }
                if (flag)
                {
                    throw new InvalidXmlException();
                }
            }
            string text = (xmlNode == null) ? "" : xmlNode.InnerText;
            if (xmlNode2 != null)
            {
                result = new DataSourceInfo(text, text, xmlNode2.OuterXml, dataProtection);
            }
            else if (xmlNode3 != null)
            {
                result = new DataSourceInfo(text, xmlNode3.InnerText, Guid.Empty);
            }
            return result;
        }

		public DataSourceInfo(string name, string originalName, string dataSourceDefinition, IDataProtection dataProtection)
			: this(name, originalName)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode xmlNode = null;
			try
			{
				XmlUtil.SafeOpenXmlDocumentString(xmlDocument, dataSourceDefinition);
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
			try
			{
				xmlNode = xmlDocument.SelectSingleNode("/DataSourceDefinition");
				if (xmlNode == null)
				{
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
					xmlNamespaceManager.AddNamespace("rds", DataSourceInfo.GetXmlNamespace());
					xmlNode = xmlDocument.SelectSingleNode("/rds:" + DataSourceInfo.GetDataSourceDefinitionXmlTag(), xmlNamespaceManager);
				}
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
			this.ParseAndValidate(xmlNode, dataProtection);
		}

		public DataSourceInfo(string name, string originalName, XmlNode root, IDataProtection dataProtection)
			: this(name, originalName)
		{
			this.ParseAndValidate(root, dataProtection);
		}

		public DataSourceInfo(string name, string linkPath, Guid linkId, DataSourceInfo standAloneDatasource)
		{
			this.m_id = Guid.NewGuid();
			this.m_name = name;
			this.m_originalName = name;
			this.m_DataSourceWithCredentialsId = standAloneDatasource.m_DataSourceWithCredentialsId;
			this.InitDefaultsOnCreation();
			this.LinkToStandAlone(standAloneDatasource, linkPath, linkId);
			if (standAloneDatasource.IsModel)
			{
				this.IsModel = true;
				this.m_modelID = standAloneDatasource.ModelID;
			}
		}

		public DataSourceInfo(string name, string originalName)
		{
			this.m_id = Guid.NewGuid();
			this.m_name = name;
			this.m_originalName = originalName;
			this.InitDefaultsOnCreation();
		}

		public void ValidateDefinition(bool useOriginalConnectString)
		{
			if (this.Extension == null)
			{
				throw new MissingElementException("Extension");
			}
			if (this.CredentialsRetrieval != CredentialsRetrievalOption.Store && this.CredentialsRetrieval != CredentialsRetrievalOption.Prompt)
			{
				this.WindowsCredentials = false;
			}
			if (this.CredentialsRetrieval != CredentialsRetrievalOption.Store)
			{
				if (this.m_userNameEncrypted != null)
				{
					throw new InvalidElementCombinationException("UserName", "CredentialRetrieval");
				}
				if (this.m_passwordEncrypted != null)
				{
					throw new InvalidElementCombinationException("Password", "CredentialRetrieval");
				}
			}
			else if (this.m_userNameEncrypted == null)
			{
				throw new MissingElementException("UserName");
			}
			if (this.ImpersonateUser && this.CredentialsRetrieval != CredentialsRetrievalOption.Store && this.CredentialsRetrieval != CredentialsRetrievalOption.ServiceAccount)
			{
				throw new InvalidElementCombinationException("ImpersonateUser", "CredentialRetrieval");
			}
			if (!useOriginalConnectString && this.ConnectionStringEncrypted == null)
			{
				throw new MissingElementException("ConnectString");
			}
			if (this.CredentialsRetrieval == CredentialsRetrievalOption.Unknown)
			{
				throw new MissingElementException("CredentialRetrieval");
			}
			if (this.CredentialsRetrieval == CredentialsRetrievalOption.ServiceAccount && !this.ImpersonateUser)
			{
				throw new InvalidElementCombinationException("CredentialRetrieval", "ImpersonateUser");
			}
			if (this.CredentialsRetrieval == CredentialsRetrievalOption.SecureStore)
			{
				if (this.SecureStoreLookup == null)
				{
					throw new MissingElementException("SecureStoreLookup");
				}
				if (this.SecureStoreLookup.TargetApplicationId == null)
				{
					throw new MissingElementException("TargetApplicationId");
				}
			}
			if (this.SecureStoreLookup == null)
			{
				return;
			}
			if (this.CredentialsRetrieval == CredentialsRetrievalOption.SecureStore)
			{
				return;
			}
			throw new InvalidElementCombinationException("SecureStoreLookup", "CredentialRetrieval");
		}

		public DataSourceInfo(string originalName, string extension, string connectionString, bool integratedSecurity, string prompt, IDataProtection dataProtection)
		{
			this.m_id = Guid.NewGuid();
			this.m_name = originalName;
			this.m_originalName = originalName;
			this.InitDefaultsOnCreation();
			this.m_prompt = prompt;
			if (integratedSecurity)
			{
				this.m_credentialsRetrieval = CredentialsRetrievalOption.Integrated;
			}
			else
			{
				this.m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			}
			this.m_extension = extension;
			this.SetConnectionString(connectionString, dataProtection);
		}

		public DataSourceInfo(string originalName, string extension, string connectionString, bool originalConnectStringExpressionBased, bool integratedSecurity, string prompt, IDataProtection dataProtection)
		{
			this.m_id = Guid.NewGuid();
			this.m_name = originalName;
			this.m_originalName = originalName;
			this.InitDefaultsOnCreation();
			this.m_prompt = prompt;
			if (integratedSecurity)
			{
				this.m_credentialsRetrieval = CredentialsRetrievalOption.Integrated;
			}
			else
			{
				this.m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			}
			this.m_extension = extension;
			this.SetOriginalConnectionString(connectionString, dataProtection);
			this.m_originalConnectStringExpressionBased = originalConnectStringExpressionBased;
		}

		public DataSourceInfo(string originalName, string referenceName, Guid linkID)
		{
			this.m_id = Guid.NewGuid();
			this.m_name = originalName;
			this.m_originalName = originalName;
			this.InitDefaultsOnCreation();
			this.m_credentialsRetrieval = CredentialsRetrievalOption.Prompt;
			this.m_dataSourceReference = referenceName;
			this.m_linkID = linkID;
		}

		public DataSourceInfo(string originalName, string referenceName, Guid linkID, bool isEmbeddedInModel)
			: this(originalName, referenceName, linkID)
		{
			this.m_isEmbeddedInModel = isEmbeddedInModel;
			this.IsModel = true;
		}

		public DataSourceInfo(string originalName)
		{
			this.m_id = Guid.NewGuid();
			this.InitDefaultsOnCreation();
			this.OriginalName = originalName;
			this.ReferenceIsValid = false;
		}

		public DataSourceInfo(string originalName, bool isEmbeddedInModel)
			: this(originalName)
		{
			this.m_isEmbeddedInModel = isEmbeddedInModel;
			this.IsModel = true;
		}

		public static string GetDataSourceReferenceXmlTag()
		{
			return "DataSourceReference";
		}

		public static string GetUserNameXmlTag()
		{
			return "UserName";
		}

		public static string GetDataSourceDefinitionXmlTag()
		{
			return "DataSourceDefinition";
		}

		public static string GetXmlNamespace()
		{
			return "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource";
		}

		public static string GetEnabledXmlTag()
		{
			return "Enabled";
		}

		public string GetConnectionString(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(this.m_connectionStringEncrypted, "ConnectionString");
		}

		public string GetOriginalConnectionString(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(this.m_originalConnectionStringEncrypted, "OriginalConnectionString");
		}

		public void SetConnectionString(string connectionString, IDataProtection dataProtection)
		{
			this.SetConnectionStringUseridReference(DataSourceInfo.HasUseridReference(connectionString));
			this.m_connectionStringEncrypted = dataProtection.ProtectData(connectionString, "ConnectionString");
		}

		private void SetOriginalConnectionString(string connectionString, IDataProtection dataProtection)
		{
			this.SetConnectionStringUseridReference(DataSourceInfo.HasUseridReference(connectionString));
			this.m_originalConnectionStringEncrypted = dataProtection.ProtectData(connectionString, "OriginalConnectionString");
		}

		internal void SetOriginalConnectionString(byte[] connectionStringEncrypted)
		{
			this.m_originalConnectionStringEncrypted = connectionStringEncrypted;
		}

		internal void SetOriginalConnectStringExpressionBased(bool expressionBased)
		{
			this.m_originalConnectStringExpressionBased = expressionBased;
		}

		public string GetUserName(IDataProtection dataProtection)
		{
			return dataProtection.UnprotectDataToString(this.m_userNameEncrypted, "UserName");
		}

		public void SetUserName(string userName, IDataProtection dataProtection)
		{
			this.m_userNameEncrypted = dataProtection.ProtectData(userName, "UserName");
		}

		public string GetUserNameOnly(IDataProtection dataProtection)
		{
			string userName = this.GetUserName(dataProtection);
			return DataSourceInfo.GetUserNameOnly(userName);
		}

		public static string GetUserNameOnly(string domainAndUserName)
		{
			if (domainAndUserName == null)
			{
				return null;
			}
			int num = domainAndUserName.IndexOf("\\", StringComparison.Ordinal);
			if (num < 0)
			{
				return domainAndUserName;
			}
			return domainAndUserName.Substring(num + 1);
		}

		public string GetDomainOnly(IDataProtection dataProtection)
		{
			string userName = this.GetUserName(dataProtection);
			return DataSourceInfo.GetDomainOnly(userName);
		}

		public static string GetDomainOnly(string domainAndUserName)
		{
			if (domainAndUserName == null)
			{
				return null;
			}
			int num = domainAndUserName.IndexOf("\\", StringComparison.Ordinal);
			if (num < 0)
			{
				return null;
			}
			return domainAndUserName.Substring(0, num);
		}

		public SecureStringWrapper GetPassword(IDataProtection dataProtection)
		{
			if (this.m_passwordSecureString == null && this.m_passwordEncrypted != null)
			{
				this.m_passwordSecureString = new SecureStringWrapper(dataProtection.UnprotectDataToString(this.m_passwordEncrypted, "Password"));
			}
			return this.m_passwordSecureString;
		}

		public string GetPasswordDecrypted(IDataProtection dataProtection)
		{
			SecureStringWrapper password = this.GetPassword(dataProtection);
			if (password != null)
			{
				return password.ToString();
			}
			return null;
		}

		public void SetPassword(string password, IDataProtection dataProtection)
		{
			this.m_passwordEncrypted = dataProtection.ProtectData(password, "Password");
		}

		public void SetPassword(SecureString password, IDataProtection dataProtection)
		{
			this.m_passwordSecureString = new SecureStringWrapper(password);
		}

		public void SetPasswordFromDataSourceInfo(DataSourceInfo dsInfo)
		{
			this.m_passwordEncrypted = dsInfo.m_passwordEncrypted;
			if (dsInfo.m_passwordSecureString != null)
			{
				this.m_passwordSecureString = new SecureStringWrapper(dsInfo.m_passwordSecureString);
			}
		}

		public void ResetPassword()
		{
			this.m_passwordEncrypted = null;
			this.m_passwordSecureString = null;
		}

		public void SetSecureStoreLookupContext(SecureStoreLookup.LookupContextOptions lookupContext, string targetAppId)
		{
			this.m_secureStoreLookup = new SecureStoreLookup(lookupContext, targetAppId);
		}

		public void SetDataSourceFaultContext(ErrorCode errorCode, string errorString)
		{
			this.m_dataSourceFaultContext = new DataSourceFaultContext(errorCode, errorString);
		}

		public static bool StaticIsModel(int flags)
		{
			return (flags & 0x10) != 0;
		}

		public void ThrowIfNotUsable(ServerDataSourceSettings serverDatasourceSetting)
		{
			if (!this.Enabled)
			{
				throw new DataSourceDisabledException();
			}
			if (!this.ReferenceIsValid)
			{
				throw new InvalidDataSourceReferenceException(this.OriginalName);
			}
			if (!this.GoodForLiveExecution(serverDatasourceSetting.IsSurrogatePresent))
			{
				throw new InvalidDataSourceCredentialSettingException();
			}
			if (this.m_credentialsRetrieval != CredentialsRetrievalOption.Integrated)
			{
				return;
			}
			if (serverDatasourceSetting.AllowIntegratedSecurity)
			{
				return;
			}
			throw new WindowsIntegratedSecurityDisabledException();
		}

		public bool GoodForLiveExecution(bool isSurrogatePresent)
		{
			if (this.ReferenceIsValid && this.Enabled)
			{
				if (!isSurrogatePresent && this.CredentialsRetrieval == CredentialsRetrievalOption.None)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public byte[] GetXmlBytes(IDataProtection dataProtection)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
			using (xmlWriter)
			{
				xmlWriter.WriteStartElement("DataSourceDefinition", "http://schemas.microsoft.com/sqlserver/reporting/2006/03/reportdatasource");
				xmlWriter.WriteElementString("Extension", this.Extension);
				xmlWriter.WriteElementString("ConnectString", this.GetConnectionString(dataProtection));
				xmlWriter.WriteElementString("CredentialRetrieval", this.CredentialsRetrieval.ToString());
				if (this.CredentialsRetrieval == CredentialsRetrievalOption.Prompt || this.CredentialsRetrieval == CredentialsRetrievalOption.Store)
				{
					xmlWriter.WriteElementString("WindowsCredentials", this.WindowsCredentials.ToString());
				}
				if (this.CredentialsRetrieval == CredentialsRetrievalOption.Prompt)
				{
					xmlWriter.WriteElementString("Prompt", string.IsNullOrEmpty(this.Prompt) ? "" : this.Prompt);
				}
				if (this.CredentialsRetrieval == CredentialsRetrievalOption.Store)
				{
					xmlWriter.WriteElementString("ImpersonateUser", this.ImpersonateUser.ToString());
				}
				xmlWriter.WriteElementString("Enabled", this.Enabled.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.Flush();
				return memoryStream.ToArray();
			}
		}

		private bool ParseDefinitionXml(XmlNode root, IDataProtection dataProtection)
		{
			try
			{
				if (root == null)
				{
					throw new InvalidXmlException();
				}
				string connectionString = null;
				bool flag = false;
				foreach (XmlNode childNode in root.ChildNodes)
				{
					string name = childNode.Name;
					string innerText = childNode.InnerText;
					switch (name)
					{
					case "Extension":
						this.Extension = innerText;
						break;
					case "ConnectString":
						connectionString = innerText;
						break;
					case "UseOriginalConnectString":
						try
						{
							flag = bool.Parse(innerText);
						}
						catch (ArgumentException)
						{
							throw new ElementTypeMismatchException("UseOriginalConnectString");
						}
						break;
					case "CredentialRetrieval":
						try
						{
							this.m_credentialsRetrieval = (CredentialsRetrievalOption)Enum.Parse(typeof(CredentialsRetrievalOption), innerText, true);
						}
						catch (ArgumentException)
						{
							throw new ElementTypeMismatchException("CredentialRetrieval");
						}
						break;
					case "WindowsCredentials":
						try
						{
							this.WindowsCredentials = bool.Parse(innerText);
						}
						catch (Exception)
						{
							throw new ElementTypeMismatchException("WindowsCredentials");
						}
						break;
					case "ImpersonateUser":
						try
						{
							this.ImpersonateUser = bool.Parse(innerText);
						}
						catch (FormatException)
						{
							throw new ElementTypeMismatchException("ImpersonateUser");
						}
						break;
					case "Prompt":
						this.m_prompt = innerText;
						break;
					case "UserName":
						this.SetUserName(innerText, dataProtection);
						break;
					case "Password":
						this.SetPassword(innerText, dataProtection);
						break;
					case "Enabled":
						try
						{
							this.Enabled = bool.Parse(innerText);
						}
						catch (FormatException)
						{
							throw new ElementTypeMismatchException("Enabled");
						}
						break;
					default:
						throw new InvalidXmlException();
					case "OriginalConnectStringExpressionBased":
						break;
					}
				}
				if (flag)
				{
					this.SetConnectionString(null, dataProtection);
				}
				else
				{
					this.SetConnectionString(connectionString, dataProtection);
				}
				return flag;
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
		}

		private void ParseAndValidate(XmlNode root, IDataProtection dataProtection)
		{
			bool useOriginalConnectString = this.ParseDefinitionXml(root, dataProtection);
			this.ValidateDefinition(useOriginalConnectString);
		}

		private void SetConnectionStringUseridReference(bool hasUseridReference)
		{
			if (hasUseridReference)
			{
				this.m_flags |= DataSourceFlags.ConnectionStringUseridReference;
			}
			else
			{
				this.m_flags &= ~DataSourceFlags.ConnectionStringUseridReference;
			}
		}

		internal static bool HasUseridReference(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				return false;
			}
			return DataSourceInfo.UseridDetectionRegex.Matches(connectionString).Count > 0;
		}

		internal static string ReplaceAllUseridReferences(string originalConnectionString, string useridReplacementString)
		{
			if (string.IsNullOrEmpty(originalConnectionString))
			{
				return originalConnectionString;
			}
			return DataSourceInfo.UseridDetectionRegex.Replace(originalConnectionString, useridReplacementString);
		}

		private void InitDefaultsOnCreation()
		{
			this.m_extension = null;
			this.m_dataSourceReference = null;
			this.m_linkID = Guid.Empty;
			this.m_credentialsRetrieval = CredentialsRetrievalOption.Unknown;
			this.m_prompt = string.Format(CultureInfo.CurrentCulture, RPRes.rsDataSourcePrompt);
			this.m_userNameEncrypted = null;
			this.m_passwordEncrypted = null;
			this.m_flags = (DataSourceFlags.Enabled | DataSourceFlags.ReferenceIsValid);
			this.m_originalConnectStringExpressionBased = false;
		}

		public DataSourceInfo(Guid id, Guid originalId, string name, string originalName, string extension, byte[] connectionStringEncrypted, byte[] originalConnectionStringEncypted, bool originalConnectStringExpressionBased, string dataSourceReference, Guid linkID, byte[] secDesc, CredentialsRetrievalOption credentialsRetrieval, string prompt, byte[] userNameEncrypted, byte[] passwordEncrypted, int flags, bool isModelSecurityUsed)
		{
			this.m_id = id;
			this.m_DataSourceWithCredentialsId = originalId;
			this.m_name = name;
			this.m_originalName = originalName;
			this.m_extension = extension;
			this.m_connectionStringEncrypted = connectionStringEncrypted;
			this.m_originalConnectionStringEncrypted = originalConnectionStringEncypted;
			this.m_originalConnectStringExpressionBased = originalConnectStringExpressionBased;
			this.m_dataSourceReference = dataSourceReference;
			this.m_linkID = linkID;
			this.m_secDesc = secDesc;
			this.m_credentialsRetrieval = credentialsRetrieval;
			this.m_prompt = prompt;
			if (this.m_credentialsRetrieval != CredentialsRetrievalOption.Store && (userNameEncrypted != null || passwordEncrypted != null))
			{
				throw new InternalCatalogException("unexpected data source type");
			}
			this.m_userNameEncrypted = userNameEncrypted;
			this.m_passwordEncrypted = passwordEncrypted;
			this.m_flags = (DataSourceFlags)flags;
			this.m_isModelSecurityUsed = isModelSecurityUsed;
		}
	}
}
