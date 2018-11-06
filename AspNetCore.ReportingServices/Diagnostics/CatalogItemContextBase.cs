using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Diagnostics
{
	[Serializable]
	internal abstract class CatalogItemContextBase<TPathStorageType> : ICatalogItemContext
	{
		protected TPathStorageType m_reportDefinitionPath;

		protected TPathStorageType m_OriginalItemPath;

		protected TPathStorageType m_ItemPath;

		protected string m_ItemName;

		protected string m_ParentPath;

		protected CatalogItemContextBase<TPathStorageType> m_primaryContext;

		[NonSerialized]
		private RSRequestParameters m_parsedParameters;

		[NonSerialized]
		protected IPathManager m_pathManager;

		[NonSerialized]
		protected IPathTranslator m_PathTranslator;

		public RSRequestParameters RSRequestParameters
		{
			get
			{
				if (this.m_parsedParameters == null)
				{
					this.m_parsedParameters = this.CreateRequestParametersInstance();
				}
				return this.m_parsedParameters;
			}
		}

		public TPathStorageType ItemPath
		{
			get
			{
				return this.m_ItemPath;
			}
		}

		public abstract string ItemPathAsString
		{
			get;
		}

		public virtual string HostSpecificItemPath
		{
			get
			{
				return this.ItemPathAsString;
			}
		}

		public virtual string StableItemPath
		{
			get
			{
				return this.ItemPathAsString;
			}
		}

		public string ItemName
		{
			get
			{
				if (this.m_ItemName == null)
				{
					string parentPath = default(string);
					CatalogItemNameUtility.SplitPath(this.ItemPathAsString, out this.m_ItemName, out parentPath);
					this.m_ParentPath = parentPath;
				}
				return this.m_ItemName;
			}
		}

		public string ParentPath
		{
			get
			{
				if (this.m_ItemName == null)
				{
					string parentPath = default(string);
					CatalogItemNameUtility.SplitPath(this.ItemPathAsString, out this.m_ItemName, out parentPath);
					this.m_ParentPath = parentPath;
				}
				return this.m_ParentPath;
			}
		}

		public abstract string HostRootUri
		{
			get;
		}

		public virtual IPathManager PathManager
		{
			get
			{
				if (this.m_pathManager == null)
				{
					this.m_pathManager = RSPathUtil.Instance;
				}
				return this.m_pathManager;
			}
		}

		protected TPathStorageType StoredItemPath
		{
			get
			{
				return this.m_ItemPath;
			}
		}

		public TPathStorageType OriginalItemPath
		{
			get
			{
				return this.m_OriginalItemPath;
			}
			protected set
			{
				this.m_OriginalItemPath = value;
			}
		}

		public IPathTranslator PathTranslator
		{
			get
			{
				return this.m_PathTranslator;
			}
		}

		public virtual string ReportDefinitionPath
		{
			get
			{
				return this.ItemPathAsString;
			}
		}

		public abstract string MapUserProvidedPath(string path);

		public abstract bool IsReportServerPathOrUrl(string pathOrUrl, bool checkProtocol, out bool isRelative);

		public abstract bool IsSupportedProtocol(string path, bool checkProtocol);

		public abstract bool IsSupportedProtocol(string path, bool checkProtocol, out bool isRelative);

		public virtual string AdjustSubreportOrDrillthroughReportPath(string reportPath)
		{
			string text;
			try
			{
				text = this.MapUserProvidedPath(reportPath);
			}
			catch (UriFormatException)
			{
				return null;
			}
			CatalogItemContextBase<TPathStorageType> catalogItemContextBase = this.CreateContext(this.m_PathTranslator);
			if (!catalogItemContextBase.SetPath(text, ItemPathOptions.Default))
			{
				return null;
			}
			if (Localization.CatalogCultureCompare(text, catalogItemContextBase.ItemPathAsString) == 0)
			{
				return reportPath;
			}
			return catalogItemContextBase.ItemPathAsString;
		}

		public virtual ICatalogItemContext GetSubreportContext(string subreportPath)
		{
			CatalogItemContextBase<TPathStorageType> catalogItemContextBase = this.CreateContext(this.m_PathTranslator);
			this.InitSubreportContext(catalogItemContextBase, subreportPath);
			catalogItemContextBase.m_primaryContext = this.m_primaryContext;
			return catalogItemContextBase;
		}

		private void InitSubreportContext(CatalogItemContextBase<TPathStorageType> subreportContext, string subreportPath)
		{
			string path = this.MapUserProvidedPath(subreportPath);
			subreportContext.SetPath(path, ItemPathOptions.Validate);
			subreportContext.RSRequestParameters.SetCatalogParameters(null);
		}

		public virtual string CombineUrl(string url, bool checkProtocol, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = this;
			string text = new CatalogItemUrlBuilder(this).ToString();
			if (url == null)
			{
				return text;
			}
			if (string.Compare(url, text, StringComparison.Ordinal) == 0)
			{
				return text;
			}
			newContext = this.Combine(url, checkProtocol, true);
			if (newContext == null)
			{
				newContext = null;
				return url;
			}
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(newContext);
			if (unparsedParameters != null)
			{
				catalogItemUrlBuilder.AppendUnparsedParameters(unparsedParameters);
			}
			return catalogItemUrlBuilder.ToString();
		}

		protected abstract RSRequestParameters CreateRequestParametersInstance();

		public ICatalogItemContext Combine(string url)
		{
			return this.Combine(url, false);
		}

		public CatalogItemContextBase<TPathStorageType> Combine(string url, bool externalFormat)
		{
			return this.Combine(url, true, externalFormat);
		}

		public CatalogItemContextBase<TPathStorageType> Combine(string url, bool checkProtocol, bool externalFormat)
		{
			bool flag = default(bool);
			if (!this.IsReportServerPathOrUrl(url, checkProtocol, out flag))
			{
				return null;
			}
			if (flag)
			{
				string text = this.MapUserProvidedPath(url);
				if (externalFormat && this.m_PathTranslator != null)
				{
					string text2 = this.m_PathTranslator.PathToExternal(text);
					if (text2 != null)
					{
						text = text2;
					}
				}
				CatalogItemContextBase<TPathStorageType> catalogItemContextBase = this.CreateContext(this.m_PathTranslator);
				ItemPathOptions itemPathOptions = ItemPathOptions.Validate;
				itemPathOptions = (ItemPathOptions)((int)itemPathOptions | (externalFormat ? 4 : 2));
				if (!catalogItemContextBase.SetPath(text, itemPathOptions))
				{
					throw new ItemNotFoundException(text.MarkAsPrivate());
				}
				catalogItemContextBase.RSRequestParameters.SetCatalogParameters(null);
				return catalogItemContextBase;
			}
			return null;
		}

		public abstract bool SetPath(string path, ItemPathOptions pathOptions);

		protected abstract CatalogItemContextBase<TPathStorageType> CreateContext(IPathTranslator pathTranslator);
	}
}
