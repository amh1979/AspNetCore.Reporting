using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Report : IDefinitionPath, IReportScope
	{
		internal enum DataElementStyles
		{
			Attribute,
			Element
		}

		internal enum SnapshotPageSize
		{
			Unknown,
			Small,
			Large
		}

		internal enum ChunkTypes
		{
			Interactivity = 6,
			Pagination,
			Rendering
		}

		private const int m_bytesPerPage = 1000000;

		private bool m_isOldSnapshot;

		private bool m_subreportInSubtotal;

		private IDefinitionPath m_parentDefinitionPath;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_reportDef;

		private AspNetCore.ReportingServices.ReportRendering.Report m_renderReport;

		private RenderingContext m_renderingContext;

		private string m_name;

		private string m_description;

		private ReportSectionCollection m_reportSections;

		private DataSetCollection m_dataSets;

		private ReportParameterCollection m_parameters;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance m_reportInstance;

		private ReportInstance m_instance;

		private CustomPropertyCollection m_customProperties;

		private PageEvaluation m_pageEvaluation;

		private ReportUrl m_location;

		private ReportStringProperty m_language;

		private ReportStringProperty m_initialPageName;

		private DocumentMap m_cachedDocumentMap;

		private bool m_cachedNeedsOverallTotalPages;

		private bool m_cachedNeedsPageBreakTotalPages;

		private bool m_cachedNeedsReportItemsOnPage;

		private bool m_hasCachedHeaderFooterFlags;

		private RenderingContext m_headerFooterRenderingContext;

		public string DefinitionPath
		{
			get
			{
				if (this.m_parentDefinitionPath != null)
				{
					return this.m_parentDefinitionPath.DefinitionPath + "xS";
				}
				return "xA";
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_parentDefinitionPath;
			}
		}

		public string ID
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.Body.ID + "xB";
				}
				return this.ReportDef.RenderingModelID;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		public string Author
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.Author;
				}
				return this.m_reportDef.Author;
			}
		}

		public string DefaultFontFamily
		{
			get
			{
				return this.m_reportDef.DefaultFontFamily;
			}
		}

		public string DataSetName
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.DataSetName;
				}
				return this.m_reportDef.OneDataSetName;
			}
		}

		[Obsolete("Use ReportSection.NeedsHeaderFooterEvaluation instead.")]
		public bool NeedsHeaderFooterEvaluation
		{
			get
			{
				ReportSection firstSection = this.FirstSection;
				if (!firstSection.NeedsTotalPages)
				{
					return firstSection.NeedsReportItemsOnPage;
				}
				return true;
			}
		}

		public bool NeedsTotalPages
		{
			get
			{
				if (!this.NeedsPageBreakTotalPages)
				{
					return this.NeedsOverallTotalPages;
				}
				return true;
			}
		}

		public bool NeedsPageBreakTotalPages
		{
			get
			{
				this.CacheHeaderFooterFlags();
				return this.m_cachedNeedsPageBreakTotalPages;
			}
		}

		public bool NeedsOverallTotalPages
		{
			get
			{
				this.CacheHeaderFooterFlags();
				return this.m_cachedNeedsOverallTotalPages;
			}
		}

		public bool NeedsReportItemsOnPage
		{
			get
			{
				this.CacheHeaderFooterFlags();
				return this.m_cachedNeedsReportItemsOnPage;
			}
		}

		public int AutoRefresh
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.AutoRefresh;
				}
				return this.Instance.AutoRefresh;
			}
		}

		[Obsolete("Use ReportSection.Width instead.")]
		public ReportSize Width
		{
			get
			{
				return this.FirstSection.Width;
			}
		}

		internal DataSetCollection DataSets
		{
			get
			{
				if (this.m_dataSets == null)
				{
					if (this.m_isOldSnapshot)
					{
						return null;
					}
					this.m_dataSets = new DataSetCollection(this.m_reportDef, this.m_renderingContext);
				}
				return this.m_dataSets;
			}
		}

		[Obsolete("Use ReportSection.Body instead.")]
		public Body Body
		{
			get
			{
				return this.FirstSection.Body;
			}
		}

		[Obsolete("Use ReportSection.Page instead.")]
		public Page Page
		{
			get
			{
				return this.FirstSection.Page;
			}
		}

		public ReportSectionCollection ReportSections
		{
			get
			{
				if (this.m_reportSections == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_reportSections = new ReportSectionCollection(this, this.m_renderReport);
					}
					else
					{
						this.m_reportSections = new ReportSectionCollection(this);
					}
				}
				return this.m_reportSections;
			}
		}

		public ReportParameterCollection Parameters
		{
			get
			{
				if (this.m_parameters == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderReport.ReportDef.Parameters != null)
						{
							this.m_parameters = new ReportParameterCollection(this.m_renderReport.ReportDef.Parameters, this.m_renderReport.Parameters);
						}
					}
					else if (this.m_reportDef.Parameters != null)
					{
						this.m_parameters = new ReportParameterCollection(this.m_renderingContext.OdpContext, this.m_reportDef.Parameters, this.m_reportInstance != null || this.m_renderingContext.OdpContext.ContextMode == OnDemandProcessingContext.Mode.DefinitionOnly);
					}
				}
				return this.m_parameters;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (this.m_customProperties == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_customProperties = new CustomPropertyCollection(this.m_renderingContext, this.m_renderReport.CustomProperties);
					}
					else
					{
						this.m_customProperties = new CustomPropertyCollection(this.Instance, this.m_renderingContext, null, this.m_reportDef, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report, this.m_reportDef.Name);
					}
				}
				return this.m_customProperties;
			}
		}

		public string DataTransform
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.DataTransform;
				}
				return this.m_reportDef.DataTransform;
			}
		}

		public string DataSchema
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.DataSchema;
				}
				return this.m_reportDef.DataSchema;
			}
		}

		public string DataElementName
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.DataElementName;
				}
				return this.m_reportDef.DataElementName;
			}
		}

		public DataElementStyles DataElementStyle
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return (DataElementStyles)this.m_renderReport.DataElementStyle;
				}
				if (!this.m_reportDef.DataElementStyleAttribute)
				{
					return DataElementStyles.Element;
				}
				return DataElementStyles.Attribute;
			}
		}

		public bool HasDocumentMap
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.RenderingContext.ReportSnapshot.HasDocumentMap;
				}
				return this.m_renderingContext.ReportSnapshot.HasDocumentMap;
			}
		}

		public DocumentMap DocumentMap
		{
			get
			{
				if (!this.HasDocumentMap)
				{
					return null;
				}
				if (this.m_cachedDocumentMap != null && !this.m_cachedDocumentMap.IsClosed)
				{
					this.m_cachedDocumentMap.Reset();
				}
				else
				{
					this.m_cachedDocumentMap = null;
					if (this.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode documentMap = this.m_renderReport.RenderingContext.ReportSnapshot.GetDocumentMap(this.m_renderReport.RenderingContext.ChunkManager);
						if (documentMap == null)
						{
							return null;
						}
						this.m_cachedDocumentMap = new ShimDocumentMap(documentMap);
					}
					else
					{
						OnDemandProcessingContext odpContext = this.RenderingContext.OdpContext;
						Stream stream = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.OpenExistingDocumentMapStream(odpContext.OdpMetadata, odpContext.TopLevelContext.ReportContext, odpContext.ChunkFactory);
						if (stream == null)
						{
							NullRenderer nullRenderer = new NullRenderer();
							nullRenderer.Process(this, this.RenderingContext.OdpContext, true, false);
							stream = nullRenderer.DocumentMapStream;
							if (stream == null)
							{
								this.RenderingContext.ReportSnapshot.HasDocumentMap = false;
								this.m_cachedDocumentMap = null;
							}
							else
							{
								stream.Seek(0L, SeekOrigin.Begin);
								DocumentMapReader aReader = new DocumentMapReader(stream);
								this.m_cachedDocumentMap = new InternalDocumentMap(aReader);
							}
						}
						else
						{
							DocumentMapReader aReader2 = new DocumentMapReader(stream);
							this.m_cachedDocumentMap = new InternalDocumentMap(aReader2);
						}
					}
				}
				return this.m_cachedDocumentMap;
			}
		}

		public bool HasBookmarks
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.RenderingContext.ReportSnapshot.HasBookmarks;
				}
				return this.RenderingContext.OdpContext.HasBookmarks;
			}
		}

		public ReportUrl Location
		{
			get
			{
				if (this.m_location == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderReport.Location != null)
						{
							this.m_location = new ReportUrl(this.m_renderReport.Location);
						}
					}
					else
					{
						this.m_location = new ReportUrl(this.m_renderingContext.OdpContext.ReportContext, null);
					}
				}
				return this.m_location;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.RenderingContext.ExecutionTime;
				}
				return this.m_renderingContext.OdpContext.ExecutionTime;
			}
		}

		public ReportStringProperty Language
		{
			get
			{
				if (this.m_language == null)
				{
					if (this.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo language = this.m_renderReport.ReportDef.Language;
						if (language == null)
						{
							this.m_language = new ReportStringProperty(false, this.m_renderReport.ReportLanguage, this.m_renderReport.ReportLanguage);
						}
						else
						{
							this.m_language = new ReportStringProperty(language);
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo language2 = this.m_reportDef.Language;
						if (language2 == null)
						{
							string text = Localization.DefaultReportServerSpecificCulture.ToString();
							this.m_language = new ReportStringProperty(false, text, text);
						}
						else
						{
							this.m_language = new ReportStringProperty(language2);
						}
					}
				}
				return this.m_language;
			}
		}

		public bool ConsumeContainerWhitespace
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return true;
				}
				return this.m_reportDef.ConsumeContainerWhitespace;
			}
		}

		public string ShowHideToggle
		{
			get
			{
				RSRequestParameters rSRequestParameters = this.GetRSRequestParameters();
				if (rSRequestParameters != null)
				{
					return rSRequestParameters.ShowHideToggleParamValue;
				}
				return null;
			}
		}

		public string SortItem
		{
			get
			{
				RSRequestParameters rSRequestParameters = this.GetRSRequestParameters();
				if (rSRequestParameters != null)
				{
					return rSRequestParameters.SortIdParamValue;
				}
				return null;
			}
		}

		public SnapshotPageSize SnapshotPageSizeInfo
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderReport.ReportDef.MainChunkSize > 0 && this.m_renderReport.ReportInstance.NumberOfPages > 0)
					{
						if (1000000 < this.m_renderReport.ReportDef.MainChunkSize / this.m_renderReport.ReportInstance.NumberOfPages)
						{
							return SnapshotPageSize.Large;
						}
						return SnapshotPageSize.Small;
					}
					return SnapshotPageSize.Unknown;
				}
				return SnapshotPageSize.Unknown;
			}
		}

		internal PageEvaluation PageEvaluation
		{
			get
			{
				return this.m_pageEvaluation;
			}
		}

		internal RenderingContext HeaderFooterRenderingContext
		{
			get
			{
				if (this.m_headerFooterRenderingContext == null)
				{
					this.m_headerFooterRenderingContext = new RenderingContext(this.m_renderingContext, this.NeedsReportItemsOnPage);
				}
				return this.m_headerFooterRenderingContext;
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					return this.m_renderReport.RenderingContext.JobContext;
				}
				return this.RenderingContext.OdpContext.JobContext;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Report ReportDef
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_reportDef;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				return this.m_renderingContext;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_isOldSnapshot;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Report RenderReport
		{
			get
			{
				if (!this.m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_renderReport;
			}
		}

		internal bool SubreportInSubtotal
		{
			get
			{
				return this.m_subreportInSubtotal;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return this.Instance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return this.m_reportDef;
			}
		}

		internal ReportSection FirstSection
		{
			get
			{
				return ((ReportElementCollectionBase<ReportSection>)this.ReportSections)[0];
			}
		}

		public ReportStringProperty InitialPageName
		{
			get
			{
				if (this.m_initialPageName == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_initialPageName = new ReportStringProperty();
					}
					else
					{
						this.m_initialPageName = new ReportStringProperty(this.m_reportDef.InitialPageName);
					}
				}
				return this.m_initialPageName;
			}
		}

		public ReportInstance Instance
		{
			get
			{
				if (this.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_instance = new ReportInstance(this);
					}
					else
					{
						this.m_instance = new ReportInstance(this, this.m_reportInstance);
					}
				}
				return this.m_instance;
			}
		}

		internal Report(AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description)
		{
			this.m_parentDefinitionPath = null;
			this.m_isOldSnapshot = false;
			this.m_reportDef = reportDef;
			this.m_reportInstance = reportInstance;
			this.m_renderingContext = renderingContext;
			this.m_name = reportName;
			this.m_description = description;
			if (reportDef.HasHeadersOrFooters)
			{
				this.m_pageEvaluation = new OnDemandPageEvaluation(this);
				this.m_renderingContext.SetPageEvaluation(this.m_pageEvaluation);
			}
		}

		internal Report(AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef, RenderingContext renderingContext, string reportName, string description)
		{
			this.m_parentDefinitionPath = null;
			this.m_isOldSnapshot = false;
			this.m_reportDef = reportDef;
			this.m_reportInstance = null;
			this.m_renderingContext = renderingContext;
			this.m_name = reportName;
			this.m_description = description;
		}

		internal Report(AspNetCore.ReportingServices.ReportProcessing.Report reportDef, AspNetCore.ReportingServices.ReportProcessing.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportRendering.RenderingContext oldRenderingContext, RenderingContext renderingContext, string reportName, string description)
		{
			this.m_renderReport = new AspNetCore.ReportingServices.ReportRendering.Report(reportDef, reportInstance, oldRenderingContext, reportName, description, Localization.DefaultReportServerSpecificCulture);
			this.m_parentDefinitionPath = null;
			this.m_isOldSnapshot = true;
			this.m_subreportInSubtotal = false;
			this.m_renderingContext = renderingContext;
			this.m_name = reportName;
			this.m_description = description;
			if (this.m_renderReport.NeedsHeaderFooterEvaluation)
			{
				this.m_pageEvaluation = new ShimPageEvaluation(this);
				this.m_renderingContext.SetPageEvaluation(this.m_pageEvaluation);
			}
		}

		internal Report(IDefinitionPath parentDefinitionPath, AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description, bool subreportInSubtotal)
		{
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_isOldSnapshot = false;
			this.m_reportDef = reportDef;
			this.m_reportInstance = reportInstance;
			this.m_isOldSnapshot = false;
			this.m_subreportInSubtotal = subreportInSubtotal;
			this.m_renderingContext = renderingContext;
			this.m_name = reportName;
			this.m_description = description;
			this.m_pageEvaluation = null;
		}

		internal Report(IDefinitionPath parentDefinitionPath, bool subreportInSubtotal, AspNetCore.ReportingServices.ReportRendering.SubReport subReport, RenderingContext renderingContext)
		{
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_renderReport = subReport.Report;
			this.m_isOldSnapshot = true;
			this.m_subreportInSubtotal = subreportInSubtotal;
			if (this.m_renderReport != null)
			{
				this.m_name = this.m_renderReport.Name;
				this.m_description = this.m_renderReport.Description;
			}
			this.m_renderingContext = new RenderingContext(renderingContext);
			this.m_pageEvaluation = null;
		}

		[Obsolete("Use ReportSection.SetPage(Int32, Int32) instead.")]
		public void SetPage(int pageNumber, int totalPages)
		{
			this.FirstSection.SetPage(pageNumber, totalPages);
		}

		[Obsolete("Use ReportSection.GetPageSections() instead.")]
		public void GetPageSections()
		{
			this.FirstSection.GetPageSections();
		}

		public void AddToCurrentPage(string textboxDefinitionName, object textboxInstanceOriginalValue)
		{
			if (this.m_pageEvaluation != null)
			{
				this.m_pageEvaluation.Add(textboxDefinitionName, textboxInstanceOriginalValue);
			}
		}

		public void EnableNativeCustomReportItem()
		{
			if (this.IsOldSnapshot)
			{
				this.m_renderReport.RenderingContext.NativeCRITypes = null;
				this.m_renderReport.RenderingContext.NativeAllCRITypes = true;
			}
			else
			{
				this.m_renderingContext.NativeCRITypes = null;
				this.m_renderingContext.NativeAllCRITypes = true;
			}
		}

		public void EnableNativeCustomReportItem(string type)
		{
			if (type == null)
			{
				this.EnableNativeCustomReportItem();
			}
			else if (this.IsOldSnapshot)
			{
				if (this.m_renderReport.RenderingContext.NativeCRITypes == null)
				{
					this.m_renderReport.RenderingContext.NativeCRITypes = new Hashtable();
				}
				if (!this.m_renderReport.RenderingContext.NativeCRITypes.ContainsKey(type))
				{
					this.m_renderReport.RenderingContext.NativeCRITypes.Add(type, null);
				}
			}
			else
			{
				if (this.m_renderingContext.NativeCRITypes == null)
				{
					this.m_renderingContext.NativeCRITypes = new Hashtable();
				}
				if (!this.m_renderingContext.NativeCRITypes.ContainsKey(type))
				{
					this.m_renderingContext.NativeCRITypes.Add(type, null);
				}
			}
		}

		public Stream GetOrCreateChunk(ChunkTypes type, string chunkName, out bool isNewChunk)
		{
			return this.m_renderingContext.GetOrCreateChunk((AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName, true, out isNewChunk);
		}

		public Stream CreateChunk(ChunkTypes type, string chunkName)
		{
			return this.m_renderingContext.CreateChunk((AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName);
		}

		public Stream GetChunk(ChunkTypes type, string chunkName)
		{
			bool flag = default(bool);
			return this.m_renderingContext.GetOrCreateChunk((AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes)type, chunkName, false, out flag);
		}

		private void CacheHeaderFooterFlags()
		{
			if (!this.m_hasCachedHeaderFooterFlags)
			{
				this.m_cachedNeedsPageBreakTotalPages = false;
				this.m_cachedNeedsReportItemsOnPage = false;
				this.m_cachedNeedsOverallTotalPages = false;
				foreach (ReportSection reportSection in this.ReportSections)
				{
					this.m_cachedNeedsReportItemsOnPage |= reportSection.NeedsReportItemsOnPage;
					this.m_cachedNeedsOverallTotalPages |= reportSection.NeedsOverallTotalPages;
					this.m_cachedNeedsPageBreakTotalPages |= reportSection.NeedsPageBreakTotalPages;
				}
				this.m_hasCachedHeaderFooterFlags = true;
			}
		}

		public string GetReportUrl(bool addReportParameters)
		{
			if (this.m_isOldSnapshot)
			{
				return new ReportUrlBuilder(this.m_renderReport.RenderingContext, null, true, addReportParameters).ToString();
			}
			ICatalogItemContext reportContext = this.m_renderingContext.OdpContext.ReportContext;
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(reportContext);
			if (addReportParameters && this.Parameters != null)
			{
				NameValueCollection toNameValueCollection = this.Parameters.ToNameValueCollection;
				catalogItemUrlBuilder.AppendReportParameters(toNameValueCollection);
			}
			return catalogItemUrlBuilder.ToString();
		}

		public string GetStreamUrl(bool useSessionId, string streamName)
		{
			ICatalogItemContext catalogItemContext = this.m_isOldSnapshot ? this.m_renderReport.RenderingContext.TopLevelReportContext : this.m_renderingContext.OdpContext.ReportContext;
			string hostSpecificItemPath = catalogItemContext.HostSpecificItemPath;
			string hostRootUri = catalogItemContext.HostRootUri;
			return catalogItemContext.RSRequestParameters.GetImageUrl(useSessionId, streamName, catalogItemContext);
		}

		public bool GetResource(string resourcePath, out byte[] resource, out string mimeType)
		{
			resource = null;
			mimeType = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_renderReport.RenderingContext.GetResourceCallback != null)
				{
					bool flag = default(bool);
					bool flag2 = default(bool);
					this.m_renderReport.RenderingContext.GetResourceCallback.GetResource(this.m_renderReport.RenderingContext.CurrentReportContext, resourcePath, out resource, out mimeType, out flag, out flag2);
					return true;
				}
				return false;
			}
			bool flag3 = default(bool);
			return this.m_renderingContext.OdpContext.GetResource(resourcePath, out resource, out mimeType, out flag3);
		}

		private RSRequestParameters GetRSRequestParameters()
		{
			if (this.m_isOldSnapshot)
			{
				return this.m_renderReport.RenderingContext.TopLevelReportContext.RSRequestParameters;
			}
			return this.RenderingContext.OdpContext.TopLevelContext.ReportContext.RSRequestParameters;
		}

		internal void UpdateSubReportContents(SubReport subReport, AspNetCore.ReportingServices.ReportRendering.SubReport renderSubreport)
		{
			if (renderSubreport != null)
			{
				this.m_renderReport = renderSubreport.Report;
			}
			if (this.m_reportSections != null)
			{
				((ReportElementCollectionBase<ReportSection>)this.m_reportSections)[0].UpdateSubReportContents(this.m_renderReport);
			}
			if (this.m_parameters != null)
			{
				this.m_parameters.UpdateRenderReportItem(this.m_renderReport.Parameters);
			}
		}

		internal void SetNewContext(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			this.m_reportInstance = reportInstance;
			this.SetNewContext();
		}

		internal void SetNewContext()
		{
			if (this.m_reportSections != null)
			{
				this.m_reportSections.SetNewContext();
			}
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_parameters != null)
			{
				this.m_parameters.SetNewContext(this.m_reportInstance != null);
			}
			if (this.m_dataSets != null)
			{
				this.m_dataSets.SetNewContext();
			}
		}
	}
}
