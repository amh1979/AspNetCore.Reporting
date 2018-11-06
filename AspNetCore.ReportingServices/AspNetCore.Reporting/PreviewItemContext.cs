using AspNetCore.ReportingServices.Diagnostics;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal class PreviewItemContext : CatalogItemContextBase<string>
	{
		private sealed class ControlRequestParameters : RSRequestParameters
		{
			public ControlRequestParameters()
			{
				base.SetCatalogParameters(null);
				base.PaginationModeValue = "Estimate";
			}

			protected override void ApplyDefaultRenderingParameters()
			{
			}
		}

		private DefinitionSource m_definitionSource;

		private string m_previewStorePath;

		private Assembly m_embeddedResourceAssembly;

		public override string ItemPathAsString
		{
			get
			{
				return base.ItemPath;
			}
		}

		public DefinitionSource DefinitionSource
		{
			get
			{
				return this.m_definitionSource;
			}
		}

		public string PreviewStorePath
		{
			get
			{
				return this.m_previewStorePath;
			}
		}

		protected virtual string RdlExtension
		{
			get
			{
				return ".rdlc";
			}
		}

		public override string HostRootUri
		{
			get
			{
				return null;
			}
		}

		public Assembly EmbeddedResourceAssembly
		{
			get
			{
				return this.m_embeddedResourceAssembly;
			}
		}

		public PreviewItemContext()
		{
			base.m_primaryContext = this;
		}

		public void SetPath(string pathForFileDefinitionSource, string fullyQualifiedPath, DefinitionSource definitionSource, Assembly embeddedResourceAssembly)
		{
			this.m_definitionSource = definitionSource;
			if (this.m_definitionSource == DefinitionSource.EmbeddedResource)
			{
				this.m_embeddedResourceAssembly = embeddedResourceAssembly;
			}
			if (definitionSource == DefinitionSource.File)
			{
				this.SetPath(pathForFileDefinitionSource, ItemPathOptions.None);
			}
			else
			{
				this.SetPath(string.Empty, ItemPathOptions.None);
			}
			this.m_previewStorePath = fullyQualifiedPath;
			base.m_ItemName = PreviewItemContext.CalculateDisplayName(fullyQualifiedPath, definitionSource);
		}

		public override bool SetPath(string path, ItemPathOptions pathOptions)
		{
			base.m_ItemPath = path;
			base.m_OriginalItemPath = path;
			return true;
		}

		protected override CatalogItemContextBase<string> CreateContext(IPathTranslator pathTranslator)
		{
			return new PreviewItemContext();
		}

		public override ICatalogItemContext GetSubreportContext(string subreportPath)
		{
			PreviewItemContext previewItemContext = (PreviewItemContext)base.GetSubreportContext(subreportPath);
			string text = this.MapUserProvidedPath(subreportPath);
			previewItemContext.SetPath(text, text, this.DefinitionSource, this.m_embeddedResourceAssembly);
			previewItemContext.OriginalItemPath = subreportPath;
			return previewItemContext;
		}

		public virtual ICatalogItemContext GetDataSetContext(string dataSetPath)
		{
			PreviewItemContext previewItemContext = new PreviewItemContext();
			previewItemContext.SetPath(dataSetPath, ItemPathOptions.None);
			return previewItemContext;
		}

		public override string MapUserProvidedPath(string path)
		{
			switch (this.m_definitionSource)
			{
			case DefinitionSource.File:
			{
				if (Path.IsPathRooted(path))
				{
					return path;
				}
				string directoryName = Path.GetDirectoryName(this.PreviewStorePath);
				string str = Path.Combine(directoryName, path);
				return str + this.RdlExtension;
			}
			case DefinitionSource.EmbeddedResource:
				return this.FindEmbeddedResource(path);
			default:
				return path;
			}
		}

		public override bool IsReportServerPathOrUrl(string pathOrUrl, bool checkProtocol, out bool isRelative)
		{
			isRelative = false;
			return false;
		}

		public override bool IsSupportedProtocol(string path, bool protocolRestriction)
		{
			bool flag = default(bool);
			return ((CatalogItemContextBase<string>)this).IsSupportedProtocol(path, protocolRestriction, out flag);
		}

		public override bool IsSupportedProtocol(string path, bool protocolRestriction, out bool isRelative)
		{
			IPathManager instance = RSPathUtil.Instance;
			return instance.IsSupportedUrl(path, protocolRestriction, out isRelative);
		}

		private string FindEmbeddedResource(string path)
		{
			if (this.m_embeddedResourceAssembly == null)
			{
				return path;
			}
			string[] manifestResourceNames = this.m_embeddedResourceAssembly.GetManifestResourceNames();
			StringDictionary stringDictionary = new StringDictionary();
			string[] array = manifestResourceNames;
			foreach (string key in array)
			{
				stringDictionary.Add(key, null);
			}
			if (stringDictionary.ContainsKey(path))
			{
				return path;
			}
			if (this.PreviewStorePath != null)
			{
				string text = this.PreviewStorePath;
				while (text.Length > 0)
				{
					int num = text.LastIndexOf('.');
					if (num == -1)
					{
						num = 0;
					}
					text = text.Substring(0, num);
					string text2 = text;
					if (text2.Length > 0)
					{
						text2 += ".";
					}
					string text3 = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", text2, path, this.RdlExtension);
					if (stringDictionary.ContainsKey(text3))
					{
						return text3;
					}
				}
			}
			return path;
		}

		private static string CalculateDisplayName(string fullyQualifiedPath, DefinitionSource definitionSource)
		{
			switch (definitionSource)
			{
			case DefinitionSource.File:
				return Path.GetFileNameWithoutExtension(fullyQualifiedPath);
			case DefinitionSource.EmbeddedResource:
			{
				string[] array = fullyQualifiedPath.Split('.');
				if (array.Length >= 2)
				{
					return array[array.Length - 2];
				}
				return array[0];
			}
			case DefinitionSource.Direct:
				return fullyQualifiedPath;
			default:
				return string.Empty;
			}
		}

		protected override RSRequestParameters CreateRequestParametersInstance()
		{
			return new ControlRequestParameters();
		}
	}
}
