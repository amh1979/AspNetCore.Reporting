using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Report : IDocumentMapEntry
	{
		internal enum DataElementStyles
		{
			AttributeNormal,
			ElementNormal
		}

		private AspNetCore.ReportingServices.ReportProcessing.Report m_reportDef;

		private ReportInstance m_reportInstance;

		private ReportInstanceInfo m_reportInstanceInfo;

		private RenderingContext m_renderingContext;

		private Rectangle m_reportBody;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		private PageCollection m_reportPagination;

		private string m_name;

		private string m_description;

		private ReportUrl m_reportUrl;

		private DocumentMapNode m_documentMapRoot;

		private ReportParameterCollection m_reportParameters;

		private string m_reportLanguage;

		private CustomPropertyCollection m_customProperties;

		private Bookmarks m_bookmarksInfo;

		private bool? m_bodyStyleConstainsBorder = null;

		public string UniqueName
		{
			get
			{
				if (this.m_reportInstance == null)
				{
					return null;
				}
				return this.m_reportInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
			}
		}

		public string ShowHideToggle
		{
			get
			{
				return this.m_renderingContext.TopLevelReportContext.RSRequestParameters.ShowHideToggleParamValue;
			}
		}

		public string SortItem
		{
			get
			{
				return this.m_renderingContext.TopLevelReportContext.RSRequestParameters.SortIdParamValue;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				return this.m_renderingContext.ReportSnapshot.HasDocumentMap;
			}
		}

		public bool HasBookmarks
		{
			get
			{
				return this.m_renderingContext.ReportSnapshot.HasBookmarks;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		internal DocumentMapNode DocumentMap
		{
			get
			{
				DocumentMapNode documentMapNode = this.m_documentMapRoot;
				if (this.m_documentMapRoot == null && this.InDocumentMap)
				{
					AspNetCore.ReportingServices.ReportProcessing.DocumentMapNode documentMap = this.m_renderingContext.ReportSnapshot.GetDocumentMap(this.m_renderingContext.ChunkManager);
					documentMapNode = new DocumentMapNode(documentMap);
					if (this.m_renderingContext.CacheState)
					{
						this.m_documentMapRoot = documentMapNode;
					}
				}
				return documentMapNode;
			}
		}

		internal Bookmarks ReportBookmarks
		{
			get
			{
				Bookmarks bookmarks = this.m_bookmarksInfo;
				if (this.m_bookmarksInfo == null && this.HasBookmarks)
				{
					BookmarksHashtable bookmarksInfo = this.m_renderingContext.ReportSnapshot.GetBookmarksInfo(this.m_renderingContext.ChunkManager);
					bookmarks = new Bookmarks(bookmarksInfo);
					if (this.m_renderingContext.CacheState)
					{
						this.m_bookmarksInfo = bookmarks;
					}
				}
				return bookmarks;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		public ReportUrl Location
		{
			get
			{
				ReportUrl reportUrl = this.m_reportUrl;
				if (this.m_reportUrl == null)
				{
					reportUrl = new ReportUrl(this.m_renderingContext, null);
					if (this.m_renderingContext.CacheState)
					{
						this.m_reportUrl = reportUrl;
					}
				}
				return reportUrl;
			}
		}

		public string ReportLanguage
		{
			get
			{
				return this.m_reportLanguage;
			}
		}

		public bool CacheState
		{
			get
			{
				return this.m_renderingContext.CacheState;
			}
			set
			{
				this.m_renderingContext.CacheState = value;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				return this.m_renderingContext.ExecutionTime;
			}
		}

		public string Author
		{
			get
			{
				return this.m_reportDef.Author;
			}
		}

		public string DataSetName
		{
			get
			{
				return this.m_reportDef.OneDataSetName;
			}
		}

		public bool NeedsHeaderFooterEvaluation
		{
			get
			{
				if (!this.m_reportDef.PageHeaderEvaluation)
				{
					return this.m_reportDef.PageFooterEvaluation;
				}
				return true;
			}
		}

		public PageSection PageHeader
		{
			get
			{
				PageSection pageSection = this.m_pageHeader;
				if (this.m_pageHeader == null)
				{
					if (this.m_reportDef.PageHeader == null)
					{
						return null;
					}
					string text = "ph";
					RenderingContext renderingContext = new RenderingContext(this.m_renderingContext, text);
					pageSection = new PageSection(text, this.m_reportDef.PageHeader, null, this, renderingContext, this.m_reportDef.PageHeaderEvaluation);
					if (this.m_renderingContext.CacheState)
					{
						this.m_pageHeader = pageSection;
					}
				}
				return pageSection;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				PageSection pageSection = this.m_pageFooter;
				if (this.m_pageFooter == null)
				{
					if (this.m_reportDef.PageFooter == null)
					{
						return null;
					}
					string text = "pf";
					RenderingContext renderingContext = new RenderingContext(this.m_renderingContext, text);
					pageSection = new PageSection(text, this.m_reportDef.PageFooter, null, this, renderingContext, this.m_reportDef.PageFooterEvaluation);
					if (this.m_renderingContext.CacheState)
					{
						this.m_pageFooter = pageSection;
					}
				}
				return pageSection;
			}
		}

		public int AutoRefresh
		{
			get
			{
				return this.m_reportDef.AutoRefresh;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (this.m_reportDef.WidthForRendering == null)
				{
					this.m_reportDef.WidthForRendering = new ReportSize(this.m_reportDef.Width, this.m_reportDef.WidthValue);
				}
				return this.m_reportDef.WidthForRendering;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				if (this.m_reportDef.PageHeightForRendering == null)
				{
					this.m_reportDef.PageHeightForRendering = new ReportSize(this.m_reportDef.PageHeight, this.m_reportDef.PageHeightValue);
				}
				return this.m_reportDef.PageHeightForRendering;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				if (this.m_reportDef.PageWidthForRendering == null)
				{
					this.m_reportDef.PageWidthForRendering = new ReportSize(this.m_reportDef.PageWidth, this.m_reportDef.PageWidthValue);
				}
				return this.m_reportDef.PageWidthForRendering;
			}
		}

		public int Columns
		{
			get
			{
				return this.m_reportDef.Columns;
			}
		}

		public ReportSize ColumnSpacing
		{
			get
			{
				if (this.m_reportDef.ColumnSpacingForRendering == null)
				{
					this.m_reportDef.ColumnSpacingForRendering = new ReportSize(this.m_reportDef.ColumnSpacing, this.m_reportDef.ColumnSpacingValue);
				}
				return this.m_reportDef.ColumnSpacingForRendering;
			}
		}

		public PageCollection Pages
		{
			get
			{
				PageCollection pageCollection = this.m_reportPagination;
				if (this.m_reportPagination == null)
				{
					pageCollection = new PageCollection(this.m_renderingContext.RenderingInfoManager.PaginationInfo, this);
					if (this.m_renderingContext.CacheState)
					{
						this.m_reportPagination = pageCollection;
					}
				}
				return pageCollection;
			}
		}

		public ReportParameterCollection Parameters
		{
			get
			{
				ReportParameterCollection reportParameterCollection = this.m_reportParameters;
				if (this.m_reportInstance != null && this.m_reportParameters == null)
				{
					reportParameterCollection = new ReportParameterCollection(this.InstanceInfo.Parameters);
					if (this.m_renderingContext.CacheState)
					{
						this.m_reportParameters = reportParameterCollection;
					}
				}
				return reportParameterCollection;
			}
		}

		public Rectangle Body
		{
			get
			{
				Rectangle rectangle = this.m_reportBody;
				if (this.m_reportBody == null)
				{
					rectangle = new Rectangle(null, (this.m_reportInstance != null) ? this.InstanceInfo.BodyUniqueName : 0, this.m_reportDef, this.m_reportInstance, this.m_renderingContext, null);
					if (this.m_renderingContext.CacheState)
					{
						this.m_reportBody = rectangle;
					}
				}
				return rectangle;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				if (this.m_reportDef.LeftMarginForRendering == null)
				{
					this.m_reportDef.LeftMarginForRendering = new ReportSize(this.m_reportDef.LeftMargin, this.m_reportDef.LeftMarginValue);
				}
				return this.m_reportDef.LeftMarginForRendering;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (this.m_reportDef.RightMarginForRendering == null)
				{
					this.m_reportDef.RightMarginForRendering = new ReportSize(this.m_reportDef.RightMargin, this.m_reportDef.RightMarginValue);
				}
				return this.m_reportDef.RightMarginForRendering;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (this.m_reportDef.TopMarginForRendering == null)
				{
					this.m_reportDef.TopMarginForRendering = new ReportSize(this.m_reportDef.TopMargin, this.m_reportDef.TopMarginValue);
				}
				return this.m_reportDef.TopMarginForRendering;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (this.m_reportDef.BottomMarginForRendering == null)
				{
					this.m_reportDef.BottomMarginForRendering = new ReportSize(this.m_reportDef.BottomMargin, this.m_reportDef.BottomMarginValue);
				}
				return this.m_reportDef.BottomMarginForRendering;
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				return this.m_renderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_reportDef.ID];
			}
			set
			{
				this.m_renderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_reportDef.ID] = value;
			}
		}

		public object RenderingInfo
		{
			get
			{
				if (this.m_reportInstance == null)
				{
					return null;
				}
				return this.m_renderingContext.RenderingInfoManager.RenderingInfo[this.m_reportInstance.UniqueName];
			}
			set
			{
				if (this.m_reportInstance == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_renderingContext.RenderingInfoManager.RenderingInfo[this.m_reportInstance.UniqueName] = value;
			}
		}

		public string Custom
		{
			get
			{
				string text = this.m_reportDef.Custom;
				if (text == null && this.CustomProperties != null)
				{
					CustomProperty customProperty = this.CustomProperties["Custom"];
					if (customProperty != null && customProperty.Value != null)
					{
						text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
					}
				}
				return text;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = this.m_customProperties;
				if (this.m_customProperties == null && this.m_reportDef.CustomProperties != null)
				{
					customPropertyCollection = ((this.m_reportInstance == null) ? new CustomPropertyCollection(this.m_reportDef.CustomProperties, null) : new CustomPropertyCollection(this.m_reportDef.CustomProperties, this.InstanceInfo.CustomPropertyInstances));
					if (this.m_renderingContext.CacheState)
					{
						this.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public string DataTransform
		{
			get
			{
				return this.m_reportDef.DataTransform;
			}
		}

		public string DataSchema
		{
			get
			{
				return this.m_reportDef.DataSchema;
			}
		}

		public string DataElementName
		{
			get
			{
				return this.m_reportDef.DataElementName;
			}
		}

		public DataElementStyles DataElementStyle
		{
			get
			{
				if (!this.m_reportDef.DataElementStyleAttribute)
				{
					return DataElementStyles.ElementNormal;
				}
				return DataElementStyles.AttributeNormal;
			}
		}

		public bool ShowHideStateChanged
		{
			get
			{
				return this.m_renderingContext.ShowHideStateChanged;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.Report ReportDef
		{
			get
			{
				return this.m_reportDef;
			}
		}

		internal ReportInstance ReportInstance
		{
			get
			{
				return this.m_reportInstance;
			}
		}

		internal ReportInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_reportInstance == null)
				{
					return null;
				}
				if (this.m_reportInstanceInfo == null)
				{
					this.m_reportInstanceInfo = this.m_reportInstance.GetCachedReportInstanceInfo(this.m_renderingContext.ChunkManager);
				}
				return this.m_reportInstanceInfo;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				return this.m_renderingContext;
			}
		}

		public int NumberOfPages
		{
			get
			{
				if (this.m_reportInstance == null)
				{
					return 0;
				}
				return this.m_reportInstance.NumberOfPages;
			}
		}

		internal bool BodyHasBorderStyles
		{
			get
			{
				StyleAttributeHashtable styleAttributes;
				if (!this.m_bodyStyleConstainsBorder.HasValue)
				{
					this.m_bodyStyleConstainsBorder = false;
					AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_reportDef.StyleClass;
					if (styleClass != null && styleClass.StyleAttributes != null && styleClass.StyleAttributes.Count > 0)
					{
						styleAttributes = styleClass.StyleAttributes;
						if (styleAttributes.ContainsKey("BorderStyle"))
						{
							AttributeInfo attributeInfo = styleAttributes["BorderStyle"];
							if (!attributeInfo.IsExpression && Validator.CompareWithInvariantCulture(attributeInfo.Value, "None"))
							{
								goto IL_0092;
							}
							this.m_bodyStyleConstainsBorder = true;
							return true;
						}
						goto IL_0092;
					}
				}
				goto IL_019f;
				IL_00d3:
				if (styleAttributes.ContainsKey("BorderStyleRight"))
				{
					AttributeInfo attributeInfo2 = styleAttributes["BorderStyleRight"];
					if (!attributeInfo2.IsExpression && Validator.CompareWithInvariantCulture(attributeInfo2.Value, "None"))
					{
						goto IL_0117;
					}
					this.m_bodyStyleConstainsBorder = true;
					return true;
				}
				goto IL_0117;
				IL_019f:
				return this.m_bodyStyleConstainsBorder.Value;
				IL_0092:
				if (styleAttributes.ContainsKey("BorderStyleLeft"))
				{
					AttributeInfo attributeInfo3 = styleAttributes["BorderStyleLeft"];
					if (!attributeInfo3.IsExpression && Validator.CompareWithInvariantCulture(attributeInfo3.Value, "None"))
					{
						goto IL_00d3;
					}
					this.m_bodyStyleConstainsBorder = true;
					return true;
				}
				goto IL_00d3;
				IL_0117:
				if (styleAttributes.ContainsKey("BorderStyleTop"))
				{
					AttributeInfo attributeInfo4 = styleAttributes["BorderStyleTop"];
					if (!attributeInfo4.IsExpression && Validator.CompareWithInvariantCulture(attributeInfo4.Value, "None"))
					{
						goto IL_015b;
					}
					this.m_bodyStyleConstainsBorder = true;
					return true;
				}
				goto IL_015b;
				IL_015b:
				if (styleAttributes.ContainsKey("BorderStyleBottom"))
				{
					AttributeInfo attributeInfo5 = styleAttributes["BorderStyleBottom"];
					if (!attributeInfo5.IsExpression && Validator.CompareWithInvariantCulture(attributeInfo5.Value, "None"))
					{
						goto IL_019f;
					}
					this.m_bodyStyleConstainsBorder = true;
					return true;
				}
				goto IL_019f;
			}
		}

		internal Report(AspNetCore.ReportingServices.ReportProcessing.Report reportDef, ReportInstance reportInstance, RenderingContext renderingContext, string reportName, string description, CultureInfo defaultLanguage)
		{
			this.m_reportDef = reportDef;
			this.m_reportInstance = reportInstance;
			this.m_renderingContext = renderingContext;
			this.m_reportBody = null;
			this.m_pageHeader = null;
			this.m_pageFooter = null;
			this.m_reportPagination = null;
			this.m_name = reportName;
			this.m_description = description;
			this.m_reportUrl = null;
			this.m_documentMapRoot = null;
			this.m_reportParameters = null;
			if (reportDef.Language != null)
			{
				if (reportDef.Language.Type == ExpressionInfo.Types.Constant)
				{
					this.m_reportLanguage = reportDef.Language.Value;
				}
				else if (reportInstance != null)
				{
					this.m_reportLanguage = reportInstance.Language;
				}
			}
			if (this.m_reportLanguage == null && defaultLanguage != null)
			{
				this.m_reportLanguage = defaultLanguage.Name;
			}
			this.AdjustBodyWhitespace();
		}

		private void AdjustBodyWhitespace()
		{
			if (this.m_reportDef.ReportItems != null && this.m_reportDef.ReportItems.Count != 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				int count = this.m_reportDef.ReportItems.Count;
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItem = this.m_reportDef.ReportItems[i];
					num = Math.Max(num, reportItem.LeftValue + reportItem.WidthValue);
					num2 = Math.Max(num2, reportItem.TopValue + reportItem.HeightValue);
				}
				this.m_reportDef.HeightValue = Math.Min(this.m_reportDef.HeightValue, num2);
				string format = "{0:0.#####}mm";
				this.m_reportDef.Height = string.Format(CultureInfo.InvariantCulture, format, this.m_reportDef.HeightValue);
				double num3 = Math.Max(1.0, this.m_reportDef.PageWidthValue - this.m_reportDef.LeftMarginValue - this.m_reportDef.RightMarginValue);
				if (this.m_reportDef.Columns > 1)
				{
					num3 -= (double)(this.m_reportDef.Columns - 1) * this.m_reportDef.ColumnSpacingValue;
					num3 = Math.Max(1.0, num3 / (double)this.m_reportDef.Columns);
				}
				num = Math.Round(num, 1);
				num3 = Math.Round(num3, 1);
				this.m_reportDef.WidthValue = Math.Min(this.m_reportDef.WidthValue, num3 * Math.Ceiling(num / num3));
				this.m_reportDef.Width = string.Format(CultureInfo.InvariantCulture, format, this.m_reportDef.WidthValue);
			}
		}

		public string StreamURL(bool useSessionId, string streamName)
		{
			return this.m_renderingContext.TopLevelReportContext.RSRequestParameters.GetImageUrl(useSessionId, streamName, this.m_renderingContext.TopLevelReportContext);
		}

		public ReportUrlBuilder GetReportUrlBuilder(string initialUrl, bool useReplacementRoot, bool addReportParameters)
		{
			return new ReportUrlBuilder(this.m_renderingContext, initialUrl, useReplacementRoot, addReportParameters);
		}

		public bool GetResource(string resourcePath, out byte[] resource, out string mimeType)
		{
			if (this.m_renderingContext.GetResourceCallback != null)
			{
				bool flag = default(bool);
				bool flag2 = default(bool);
				this.m_renderingContext.GetResourceCallback.GetResource(this.m_renderingContext.CurrentReportContext, resourcePath, out resource, out mimeType, out flag, out flag2);
				return true;
			}
			resource = null;
			mimeType = null;
			return false;
		}

		public ReportItem Find(string uniqueName)
		{
			if (uniqueName != null && uniqueName.Length > 0)
			{
				int num = ReportItem.StringToInt(uniqueName);
				if (num < 0)
				{
					return null;
				}
				return this.m_renderingContext.FindReportItemInBody(num);
			}
			return null;
		}

		public void EnableNativeCustomReportItem()
		{
			Global.Tracer.Assert(null != this.m_renderingContext);
			this.m_renderingContext.NativeCRITypes = null;
			this.m_renderingContext.NativeAllCRITypes = true;
		}

		public void EnableNativeCustomReportItem(string type)
		{
			Global.Tracer.Assert(null != this.m_renderingContext);
			if (type == null)
			{
				this.m_renderingContext.NativeCRITypes = null;
				this.m_renderingContext.NativeAllCRITypes = true;
			}
			if (this.m_renderingContext.NativeCRITypes == null)
			{
				this.m_renderingContext.NativeCRITypes = new Hashtable();
			}
			if (!this.m_renderingContext.NativeCRITypes.ContainsKey(type))
			{
				this.m_renderingContext.NativeCRITypes.Add(type, null);
			}
		}

		internal bool Search(int searchPage, string findValue)
		{
			SearchContext searchContext = new SearchContext(searchPage, findValue, 0, this.NumberOfPages - 1);
			PageSection pageSection = this.PageHeader;
			PageSection pageSection2 = this.PageFooter;
			bool flag = false;
			bool flag2 = false;
			if (pageSection != null)
			{
				if (searchPage > 0 && searchPage < this.NumberOfPages - 1)
				{
					goto IL_0057;
				}
				if (searchPage == 0 && pageSection.PrintOnFirstPage)
				{
					goto IL_0057;
				}
				if (searchPage != 0 && searchPage == this.NumberOfPages - 1 && pageSection.PrintOnLastPage)
				{
					goto IL_0057;
				}
			}
			goto IL_0059;
			IL_0057:
			flag = true;
			goto IL_0059;
			IL_0094:
			flag2 = true;
			goto IL_0097;
			IL_0097:
			if ((flag || flag2) && this.NeedsHeaderFooterEvaluation)
			{
				PageSection pageSection3 = null;
				PageSection pageSection4 = null;
				AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.EvaluateHeaderFooterExpressions(searchPage + 1, this.NumberOfPages, this, (PageReportItems)null, out pageSection3, out pageSection4);
				if (this.m_reportDef.PageHeaderEvaluation)
				{
					pageSection = pageSection3;
				}
				if (this.m_reportDef.PageFooterEvaluation)
				{
					pageSection2 = pageSection4;
				}
			}
			bool flag3 = false;
			if (flag)
			{
				flag3 = pageSection.Search(searchContext);
			}
			if (!flag3)
			{
				flag3 = this.Body.Search(searchContext);
				if (!flag3 && flag2)
				{
					flag3 = pageSection2.Search(searchContext);
				}
			}
			return flag3;
			IL_0059:
			if (pageSection2 != null)
			{
				if (searchPage > 0 && searchPage < this.NumberOfPages - 1)
				{
					goto IL_0094;
				}
				if (searchPage != this.NumberOfPages - 1 && searchPage == 0 && pageSection2.PrintOnFirstPage)
				{
					goto IL_0094;
				}
				if (searchPage == this.NumberOfPages - 1 && pageSection2.PrintOnLastPage)
				{
					goto IL_0094;
				}
			}
			goto IL_0097;
		}
	}
}
