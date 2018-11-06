using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PageContext
	{
		internal enum CacheState : byte
		{
			RPLStream,
			RPLObjectModel,
			CountPages
		}

		internal enum IgnorePageBreakReason
		{
			Unknown,
			ToggleableItem,
			InsideTablixCell,
			InsideHeaderFooter
		}

		internal class PageContextCommon
		{
			private PaginationSettings m_pagination;

			private bool m_consumeWhitespace;

			private Bitmap m_hdcBits;

			private Graphics m_bitsGraphics;

			private Hashtable m_textBoxDuplicates;

			private Hashtable m_itemPaddingsStyle;

			private bool m_evaluatePageHeaderFooter;

			private int m_verticalPageNumber;

			private int m_pageNumber;

			private bool m_paginatingHorizontally;

			private string m_pageName;

			private string m_previousPageName;

			private bool m_canSetPageName = true;

			private int m_pageNumberRegion;

			private List<int> m_totalPagesRegion;

			private PageBreakProperties m_pageBreakProperties;

			private List<PageBreakProperties> m_registeredPageBreakProperties;

			private Hashtable m_tracedPageBreakIgnored;

			private Hashtable m_itemPropsStart;

			private Hashtable m_sharedImages;

			private Hashtable m_autosizeSharedImages;

			private Stream m_propertyCache;

			private CacheState m_cacheState;

			private BinaryReader m_propertyCacheReader;

			private BinaryWriter m_propertyCacheWriter;

			private AddTextBoxDelegate m_addTextBox;

			private Hashtable m_cacheSharedImages;

			internal CachedSharedImageInfo m_itemCacheSharedImageInfo;

			private IScalabilityCache m_scalabilityCache;

			private long m_totalScaleTimeMs;

			private long m_peakMemoryUsageKB;

			private CreateAndRegisterStream m_createAndRegisterStream;

			private FontCache m_fontCache;

			private bool m_inHeaderFooter;

			private bool m_inSubReport;

			private bool m_isInSelectiveRendering;

			private bool m_outputDiagnostics = true;

			internal PaginationSettings Pagination
			{
				get
				{
					return this.m_pagination;
				}
			}

			internal bool ConsumeWhitespace
			{
				get
				{
					return this.m_consumeWhitespace;
				}
			}

			internal bool EvaluatePageHeaderFooter
			{
				get
				{
					return this.m_evaluatePageHeaderFooter;
				}
				set
				{
					this.m_evaluatePageHeaderFooter = value;
				}
			}

			internal int VerticalPageNumber
			{
				get
				{
					return this.m_verticalPageNumber;
				}
				set
				{
					this.m_verticalPageNumber = value;
				}
			}

			internal int PageNumber
			{
				get
				{
					return this.m_pageNumber;
				}
				set
				{
					this.m_pageNumber = value;
				}
			}

			internal string PageName
			{
				get
				{
					return this.m_pageName;
				}
			}

			internal int PageNumberRegion
			{
				get
				{
					return this.m_pageNumberRegion;
				}
				set
				{
					this.m_pageNumberRegion = value;
				}
			}

			public bool PaginatingHorizontally
			{
				get
				{
					return this.m_paginatingHorizontally;
				}
				set
				{
					this.m_paginatingHorizontally = value;
				}
			}

			public PageBreakProperties RegisteredPageBreakProperties
			{
				get
				{
					return this.m_pageBreakProperties;
				}
			}

			public bool CanOverwritePageBreak
			{
				get
				{
					return null == this.RegisteredPageBreakProperties;
				}
			}

			public bool CanSetPageName
			{
				get
				{
					return this.m_canSetPageName;
				}
			}

			internal Hashtable ItemPropsStart
			{
				get
				{
					return this.m_itemPropsStart;
				}
				set
				{
					this.m_itemPropsStart = value;
				}
			}

			internal Hashtable SharedImages
			{
				get
				{
					return this.m_sharedImages;
				}
				set
				{
					this.m_sharedImages = value;
				}
			}

			internal Hashtable AutoSizeSharedImages
			{
				get
				{
					return this.m_autosizeSharedImages;
				}
				set
				{
					this.m_autosizeSharedImages = value;
				}
			}

			internal Hashtable TextBoxDuplicates
			{
				get
				{
					return this.m_textBoxDuplicates;
				}
				set
				{
					this.m_textBoxDuplicates = value;
				}
			}

			internal Hashtable ItemPaddingsStyle
			{
				get
				{
					return this.m_itemPaddingsStyle;
				}
				set
				{
					this.m_itemPaddingsStyle = value;
				}
			}

			internal Stream PropertyCache
			{
				get
				{
					return this.m_propertyCache;
				}
				set
				{
					this.m_propertyCache = value;
					if (this.m_propertyCache != null)
					{
						BufferedStream bufferedStream = new BufferedStream(this.m_propertyCache);
						this.m_propertyCacheReader = new BinaryReader(bufferedStream, Encoding.Unicode);
						this.m_propertyCacheWriter = new BinaryWriter(bufferedStream, Encoding.Unicode);
					}
				}
			}

			internal CacheState CacheState
			{
				get
				{
					return this.m_cacheState;
				}
				set
				{
					this.m_cacheState = value;
				}
			}

			internal Hashtable CacheSharedImages
			{
				get
				{
					return this.m_cacheSharedImages;
				}
				set
				{
					this.m_cacheSharedImages = value;
				}
			}

			internal CachedSharedImageInfo ItemCacheSharedImageInfo
			{
				get
				{
					return this.m_itemCacheSharedImageInfo;
				}
				set
				{
					this.m_itemCacheSharedImageInfo = value;
				}
			}

			internal BinaryReader PropertyCacheReader
			{
				get
				{
					return this.m_propertyCacheReader;
				}
			}

			internal BinaryWriter PropertyCacheWriter
			{
				get
				{
					return this.m_propertyCacheWriter;
				}
			}

			internal AddTextBoxDelegate AddTextBox
			{
				get
				{
					return this.m_addTextBox;
				}
			}

			internal IScalabilityCache ScalabilityCache
			{
				get
				{
					return this.m_scalabilityCache;
				}
			}

			internal long TotalScaleTimeMs
			{
				get
				{
					return this.m_totalScaleTimeMs;
				}
			}

			internal long PeakMemoryUsageKB
			{
				get
				{
					return this.m_peakMemoryUsageKB;
				}
			}

			internal FontCache FontCache
			{
				get
				{
					if (this.m_fontCache == null)
					{
						this.m_fontCache = new FontCache((float)this.Pagination.MeasureTextDpi, this.Pagination.UseEmSquare);
					}
					return this.m_fontCache;
				}
			}

			internal bool InHeaderFooter
			{
				get
				{
					return this.m_inHeaderFooter;
				}
				set
				{
					this.m_inHeaderFooter = value;
				}
			}

			internal bool InSubReport
			{
				get
				{
					return this.m_inSubReport;
				}
				set
				{
					this.m_inSubReport = value;
				}
			}

			internal bool IsInSelectiveRendering
			{
				get
				{
					return this.m_isInSelectiveRendering;
				}
				set
				{
					this.m_isInSelectiveRendering = value;
				}
			}

			internal bool DiagnosticsEnabled
			{
				get
				{
					if (RenderingDiagnostics.Enabled)
					{
						return this.m_outputDiagnostics;
					}
					return false;
				}
			}

			internal PageContextCommon(PaginationSettings pagination, AddTextBoxDelegate aAddTextBoxDelegate, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
			{
				this.m_pagination = pagination;
				this.m_addTextBox = aAddTextBoxDelegate;
				this.m_consumeWhitespace = consumeWhitespace;
				this.m_createAndRegisterStream = createAndRegisterStream;
			}

			internal void CreateGraphics()
			{
				this.m_hdcBits = new Bitmap(2, 2);
				this.m_hdcBits.SetResolution((float)this.m_pagination.MeasureTextDpi, (float)this.m_pagination.MeasureTextDpi);
				this.m_bitsGraphics = Graphics.FromImage(this.m_hdcBits);
				this.m_bitsGraphics.CompositingMode = CompositingMode.SourceOver;
				this.m_bitsGraphics.PageUnit = GraphicsUnit.Millimeter;
				this.m_bitsGraphics.PixelOffsetMode = PixelOffsetMode.Default;
				this.m_bitsGraphics.SmoothingMode = SmoothingMode.Default;
				this.m_bitsGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
			}

			internal void DisposeGraphics()
			{
				this.m_textBoxDuplicates = null;
				this.m_itemPaddingsStyle = null;
				this.m_itemPropsStart = null;
				this.m_sharedImages = null;
				this.m_autosizeSharedImages = null;
				if (this.m_propertyCache != null)
				{
					this.m_propertyCacheReader = null;
					this.m_propertyCacheWriter = null;
					this.m_propertyCache.Close();
					this.m_propertyCache.Dispose();
				}
				if (this.m_bitsGraphics != null)
				{
					this.m_bitsGraphics.Dispose();
					this.m_bitsGraphics = null;
				}
				if (this.m_hdcBits != null)
				{
					this.m_hdcBits.Dispose();
					this.m_hdcBits = null;
				}
				if (this.m_scalabilityCache != null)
				{
					this.m_totalScaleTimeMs += this.m_scalabilityCache.ScalabilityDurationMs;
					this.m_peakMemoryUsageKB = Math.Max(this.m_peakMemoryUsageKB, this.m_scalabilityCache.PeakMemoryUsageKBytes);
					this.m_scalabilityCache.Dispose();
					this.m_scalabilityCache = null;
				}
				if (this.m_fontCache != null)
				{
					this.m_fontCache.Dispose();
					this.m_fontCache = null;
				}
			}

			internal float MeasureFullTextBoxHeight(AspNetCore.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
			{
				if (this.m_bitsGraphics == null)
				{
					this.CreateGraphics();
				}
				return AspNetCore.ReportingServices.Rendering.RichText.TextBox.MeasureFullHeight(textBox, this.m_bitsGraphics, this.FontCache, flowContext, out contentHeight);
			}

			internal float MeasureTextBoxHeight(AspNetCore.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext)
			{
				if (this.m_bitsGraphics == null)
				{
					this.CreateGraphics();
				}
				float result = 0f;
				LineBreaker.Flow(textBox, this.m_bitsGraphics, this.FontCache, flowContext, false, out result);
				return result;
			}

			internal void InitCache()
			{
				if (this.m_scalabilityCache == null)
				{
					this.m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(this.m_createAndRegisterStream, "HPB", StorageObjectCreator.Instance, HPBReferenceCreator.Instance, ComponentType.Pagination, 1);
				}
			}

			internal T GetFromCache<T>(string id, out Hashtable itemPropsStart) where T : class
			{
				T result = null;
				itemPropsStart = this.ItemPropsStart;
				if (itemPropsStart != null)
				{
					return (T)(itemPropsStart[id] as T);
				}
				itemPropsStart = new Hashtable();
				this.ItemPropsStart = itemPropsStart;
				return result;
			}

			internal T GetPrimitiveFromCache<T>(string id, out Hashtable itemPropsStart) where T : struct
			{
				T result = default(T);
				itemPropsStart = this.ItemPropsStart;
				if (itemPropsStart != null)
				{
					object obj = itemPropsStart[id];
					if (obj is T)
					{
						return (T)obj;
					}
					return default(T);
				}
				itemPropsStart = new Hashtable();
				this.ItemPropsStart = itemPropsStart;
				return result;
			}

			internal int GetTotalPagesRegion(int pageNumber)
			{
				if (this.m_cacheState != CacheState.CountPages && this.m_totalPagesRegion != null)
				{
					int num = 0;
					foreach (int item in this.m_totalPagesRegion)
					{
						num += item;
						if (pageNumber <= num)
						{
							return item;
						}
					}
				}
				return 0;
			}

			internal void UpdateTotalPagesRegionMapping()
			{
				if (this.m_cacheState == CacheState.CountPages)
				{
					if (this.m_totalPagesRegion == null)
					{
						this.m_totalPagesRegion = new List<int>();
					}
					this.m_totalPagesRegion.Add(this.m_pageNumberRegion);
				}
			}

			internal void RegisterPageBreakProperties(PageBreakProperties pageBreakProperties, bool overwrite)
			{
				if (this.DiagnosticsEnabled && pageBreakProperties != null && !this.m_registeredPageBreakProperties.Contains(pageBreakProperties))
				{
					this.m_registeredPageBreakProperties.Add(pageBreakProperties);
				}
				if (this.m_pageBreakProperties != null && !overwrite)
				{
					return;
				}
				this.m_pageBreakProperties = pageBreakProperties;
			}

			internal void ProcessPageBreakProperties()
			{
				this.TracePageCreated();
				if (this.m_pageBreakProperties != null && !this.m_paginatingHorizontally)
				{
					if (this.m_pageBreakProperties.ResetPageNumber)
					{
						this.UpdateTotalPagesRegionMapping();
						this.m_pageNumberRegion = 0;
					}
					this.ResetPageBreakProcessing();
				}
			}

			internal void ResetPageBreakProcessing()
			{
				this.m_pageBreakProperties = null;
				if (this.DiagnosticsEnabled)
				{
					if (this.m_registeredPageBreakProperties == null)
					{
						this.m_registeredPageBreakProperties = new List<PageBreakProperties>();
					}
					else
					{
						this.m_registeredPageBreakProperties.Clear();
					}
				}
			}

			internal void SetPageName(string pageName, bool overwrite)
			{
				if (pageName != null)
				{
					if (!this.m_canSetPageName && !overwrite)
					{
						return;
					}
					this.m_pageName = pageName;
					this.m_canSetPageName = false;
				}
			}

			internal void OverwritePageName(string pageName)
			{
				this.m_pageName = pageName;
			}

			internal void ResetPageNameProcessing()
			{
				this.m_canSetPageName = true;
			}

			internal void PauseDiagnostics()
			{
				this.m_outputDiagnostics = false;
			}

			internal void ResumeDiagnostics()
			{
				this.m_outputDiagnostics = true;
			}

			private void GetItemIdAndName(object item, out string id, out string name)
			{
				if (item is PageItem)
				{
					PageItem pageItem = (PageItem)item;
					id = pageItem.Source.ID;
					name = pageItem.Source.Name;
				}
				else if (item is TablixMember)
				{
					TablixMember tablixMember = (TablixMember)item;
					id = tablixMember.ID;
					name = tablixMember.Group.Name;
				}
				else
				{
					id = "";
					name = "ItemTypeNotFound";
				}
			}

			private string GetItemName(object item)
			{
				string text = default(string);
				string result = default(string);
				this.GetItemIdAndName(item, out text, out result);
				return result;
			}

			private void TracePageCreated(PageCreationType pageCreationType, bool resetPageNumber)
			{
				string text = "PR-DIAG [Page {0}] Page created by {1} page break";
				if (resetPageNumber)
				{
					text += ". Page number reset";
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, text, this.m_pageNumber + 1, pageCreationType.ToString());
			}

			private void TracePageCreated(PageCreationType pageCreationType)
			{
				this.TracePageCreated(pageCreationType, false);
			}

			public void TracePageCreated()
			{
				if (this.DiagnosticsEnabled && this.m_pageNumber != 0)
				{
					foreach (PageBreakProperties registeredPageBreakProperty in this.m_registeredPageBreakProperties)
					{
						if (registeredPageBreakProperty != this.m_pageBreakProperties)
						{
							string itemName = this.GetItemName(registeredPageBreakProperty.Source);
							RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – peer item precedence", this.PageNumber, itemName);
						}
					}
					if (this.m_pageBreakProperties != null)
					{
						if (this.m_paginatingHorizontally)
						{
							this.TracePageCreated(PageCreationType.Horizontal);
						}
						else
						{
							this.TracePageCreated(PageCreationType.Logical, this.m_pageBreakProperties.ResetPageNumber);
						}
					}
					else if (this.m_paginatingHorizontally)
					{
						this.TracePageCreated(PageCreationType.Horizontal);
					}
					else
					{
						this.TracePageCreated(PageCreationType.Vertical);
					}
				}
			}

			public void ResetPageNameTracing()
			{
				this.m_previousPageName = null;
			}

			public void CheckPageNameChanged()
			{
				if (this.DiagnosticsEnabled && this.m_pageName != this.m_previousPageName)
				{
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page name changed", this.m_pageNumber);
					this.m_previousPageName = this.m_pageName;
				}
			}

			public void TracePageBreakIgnored(object item, IgnorePageBreakReason ignorePageBreakReason)
			{
				if (this.DiagnosticsEnabled)
				{
					string key = default(string);
					string text = default(string);
					this.GetItemIdAndName(item, out key, out text);
					if (this.m_tracedPageBreakIgnored != null && this.m_tracedPageBreakIgnored.ContainsKey(key))
					{
						return;
					}
					string text2 = "PR-DIAG [Page {0}] Page break on '{1}' ignored";
					switch (ignorePageBreakReason)
					{
					case IgnorePageBreakReason.InsideTablixCell:
						text2 += " - inside TablixCell";
						break;
					case IgnorePageBreakReason.ToggleableItem:
						text2 += " - part of toggleable region";
						break;
					case IgnorePageBreakReason.InsideHeaderFooter:
						text2 += " - inside header or footer";
						break;
					}
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, text2, this.m_pageNumber, text);
					if (this.m_tracedPageBreakIgnored == null)
					{
						this.m_tracedPageBreakIgnored = new Hashtable();
					}
					this.m_tracedPageBreakIgnored.Add(key, null);
				}
			}

			public void TracePageBreakIgnoredDisabled(object item)
			{
				if (this.DiagnosticsEnabled)
				{
					string itemName = this.GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – Disable is True", this.m_pageNumber, itemName);
				}
			}

			public void TracePageBreakIgnoredAtTopOfPage(object item)
			{
				if (this.DiagnosticsEnabled)
				{
					string itemName = this.GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – at top of page", this.m_pageNumber, itemName);
				}
			}

			public void TracePageBreakIgnoredAtBottomOfPage(object item)
			{
				if (this.DiagnosticsEnabled)
				{
					string itemName = this.GetItemName(item);
					RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Info, "PR-DIAG [Page {0}] Page break on '{1}' ignored – bottom of page", this.m_pageNumber, itemName);
				}
			}
		}

		internal const double RoundDelta = 0.01;

		internal const string InvalidImage = "InvalidImage";

		private bool m_ignorePageBreak;

		private IgnorePageBreakReason m_ignorePageBreakReason;

		private bool m_fullOnPage;

		private bool m_cacheNonSharedProps;

		private bool m_resetHorizontal;

		private PageContextCommon m_common;

		private List<TextRunItemizedData> m_paragraphItemizedData;

		internal bool ConsumeWhitespace
		{
			get
			{
				return this.m_common.ConsumeWhitespace;
			}
		}

		internal IScalabilityCache ScalabilityCache
		{
			get
			{
				return this.m_common.ScalabilityCache;
			}
		}

		internal long TotalScaleTimeMs
		{
			get
			{
				return this.m_common.TotalScaleTimeMs;
			}
		}

		internal long PeakMemoryUsageKB
		{
			get
			{
				return this.m_common.PeakMemoryUsageKB;
			}
		}

		internal bool UseGenericDefault
		{
			get
			{
				return this.m_common.Pagination.UseGenericDefault;
			}
		}

		internal double ColumnHeight
		{
			get
			{
				return this.m_common.Pagination.CurrentColumnHeight;
			}
		}

		internal double ColumnWidth
		{
			get
			{
				return this.m_common.Pagination.CurrentColumnWidth;
			}
		}

		internal double UsablePageHeight
		{
			get
			{
				return this.m_common.Pagination.UsablePageHeight;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				return this.m_ignorePageBreak;
			}
			set
			{
				this.m_ignorePageBreak = value;
			}
		}

		internal IgnorePageBreakReason IgnorePageBreaksReason
		{
			get
			{
				return this.m_ignorePageBreakReason;
			}
			set
			{
				this.m_ignorePageBreakReason = value;
			}
		}

		internal bool FullOnPage
		{
			get
			{
				return this.m_fullOnPage;
			}
			set
			{
				this.m_fullOnPage = value;
			}
		}

		internal bool ResetHorizontal
		{
			get
			{
				return this.m_resetHorizontal;
			}
			set
			{
				this.m_resetHorizontal = value;
			}
		}

		internal bool EvaluatePageHeaderFooter
		{
			get
			{
				return this.m_common.EvaluatePageHeaderFooter;
			}
			set
			{
				this.m_common.EvaluatePageHeaderFooter = value;
			}
		}

		public int DpiX
		{
			get
			{
				return this.m_common.Pagination.DpiX;
			}
		}

		public int DpiY
		{
			get
			{
				return this.m_common.Pagination.DpiY;
			}
		}

		public int DynamicImageDpiX
		{
			get
			{
				return this.m_common.Pagination.DynamicImageDpiX;
			}
		}

		public int DynamicImageDpiY
		{
			get
			{
				return this.m_common.Pagination.DynamicImageDpiY;
			}
		}

		internal bool EMFDynamicImages
		{
			get
			{
				return this.m_common.Pagination.EMFOutputFormat;
			}
		}

		public PaginationSettings.FormatEncoding OutputFormat
		{
			get
			{
				return this.m_common.Pagination.OutputFormat;
			}
		}

		internal int VerticalPageNumber
		{
			get
			{
				return this.m_common.VerticalPageNumber;
			}
			set
			{
				this.m_common.VerticalPageNumber = value;
			}
		}

		internal int PageNumber
		{
			get
			{
				return this.m_common.PageNumber;
			}
			set
			{
				this.m_common.PageNumber = value;
			}
		}

		internal string PageName
		{
			get
			{
				return this.m_common.PageName;
			}
		}

		internal int PageNumberRegion
		{
			get
			{
				return this.m_common.PageNumberRegion;
			}
			set
			{
				this.m_common.PageNumberRegion = value;
			}
		}

		internal Hashtable ItemPropsStart
		{
			get
			{
				return this.m_common.ItemPropsStart;
			}
			set
			{
				this.m_common.ItemPropsStart = value;
			}
		}

		internal Hashtable SharedImages
		{
			get
			{
				return this.m_common.SharedImages;
			}
			set
			{
				this.m_common.SharedImages = value;
			}
		}

		internal Hashtable AutoSizeSharedImages
		{
			get
			{
				return this.m_common.AutoSizeSharedImages;
			}
			set
			{
				this.m_common.AutoSizeSharedImages = value;
			}
		}

		internal Hashtable TextBoxDuplicates
		{
			get
			{
				return this.m_common.TextBoxDuplicates;
			}
			set
			{
				this.m_common.TextBoxDuplicates = value;
			}
		}

		internal Hashtable ItemPaddingsStyle
		{
			get
			{
				return this.m_common.ItemPaddingsStyle;
			}
			set
			{
				this.m_common.ItemPaddingsStyle = value;
			}
		}

		internal bool CacheNonSharedProps
		{
			get
			{
				return this.m_cacheNonSharedProps;
			}
		}

		internal Stream PropertyCache
		{
			get
			{
				return this.m_common.PropertyCache;
			}
			set
			{
				this.m_common.PropertyCache = value;
			}
		}

		internal CacheState PropertyCacheState
		{
			get
			{
				return this.m_common.CacheState;
			}
			set
			{
				this.m_common.CacheState = value;
			}
		}

		internal BinaryReader PropertyCacheReader
		{
			get
			{
				return this.m_common.PropertyCacheReader;
			}
		}

		internal BinaryWriter PropertyCacheWriter
		{
			get
			{
				return this.m_common.PropertyCacheWriter;
			}
		}

		internal Hashtable CacheSharedImages
		{
			get
			{
				return this.m_common.CacheSharedImages;
			}
			set
			{
				this.m_common.CacheSharedImages = value;
			}
		}

		internal CachedSharedImageInfo ItemCacheSharedImageInfo
		{
			get
			{
				return this.m_common.ItemCacheSharedImageInfo;
			}
			set
			{
				this.m_common.ItemCacheSharedImageInfo = value;
			}
		}

		internal AddTextBoxDelegate AddTextBox
		{
			get
			{
				return this.m_common.AddTextBox;
			}
		}

		internal PageContextCommon Common
		{
			get
			{
				return this.m_common;
			}
		}

		internal List<TextRunItemizedData> ParagraphItemizedData
		{
			get
			{
				return this.m_paragraphItemizedData;
			}
			set
			{
				this.m_paragraphItemizedData = value;
			}
		}

		internal bool IsInSelectiveRendering
		{
			get
			{
				return this.Common.IsInSelectiveRendering;
			}
		}

		internal PageContext(PaginationSettings pagination, AddTextBoxDelegate aAddTextBox, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
		{
			this.m_common = new PageContextCommon(pagination, aAddTextBox, consumeWhitespace, createAndRegisterStream);
		}

		internal PageContext(PageContext pageContext)
		{
			this.m_common = pageContext.Common;
		}

		internal PageContext(PageContext pageContext, bool fullOnPage, bool ignorePageBreaks, IgnorePageBreakReason ignorePageBreakReason, bool cacheNonSharedProps)
		{
			this.m_common = pageContext.Common;
			this.m_fullOnPage = fullOnPage;
			this.m_ignorePageBreak = ignorePageBreaks;
			this.m_ignorePageBreakReason = ignorePageBreakReason;
			this.m_cacheNonSharedProps = cacheNonSharedProps;
		}

		internal PageContext(PageContext pageContext, bool cacheNonSharedProps)
		{
			this.m_common = pageContext.Common;
			this.m_fullOnPage = pageContext.FullOnPage;
			this.m_ignorePageBreak = pageContext.IgnorePageBreaks;
			this.m_ignorePageBreakReason = pageContext.IgnorePageBreaksReason;
			this.m_cacheNonSharedProps = cacheNonSharedProps;
		}

		internal void InitCache()
		{
			this.m_common.InitCache();
		}

		internal void DisposeGraphics()
		{
			this.m_common.DisposeGraphics();
		}

		internal double ConvertToMillimeters(int coordinate, float dpi)
		{
			if (0.0 == dpi)
			{
				return 1.7976931348623157E+308;
			}
			return 1.0 / (double)dpi * (double)coordinate * 25.399999618530273;
		}

		internal void RegisterTextRunData(TextRunItemizedData runItemizedData)
		{
			if (this.m_paragraphItemizedData != null)
			{
				this.m_paragraphItemizedData.Add(runItemizedData);
			}
		}
	}
}
