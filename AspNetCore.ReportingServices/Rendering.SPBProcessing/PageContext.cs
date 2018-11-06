using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class PageContext
	{
		internal enum RPLReportSectionArea : byte
		{
			Header,
			Footer,
			Body
		}

		[Flags]
		internal enum PageContextFlags : ushort
		{
			Clear = 0,
			IgnorePageBreak = 1,
			FullOnPage = 2,
			KeepTogether = 4,
			HideDuplicates = 8,
			TypeCodeNonString = 0x10,
			StretchPage = 0x20
		}

		internal enum IgnorePBReasonFlag
		{
			None,
			Toggled,
			Repeated,
			TablixParent,
			HeaderFooter
		}

		internal class PageContextCommon
		{
			private double m_pageHeight = 1.7976931348623157E+308;

			private double m_originalPageHeight = 1.7976931348623157E+308;

			private bool m_registerEvents;

			private bool m_measureItems;

			private bool m_emfDynamicImage;

			private bool m_consumeWhitespace;

			private bool m_cancelPage;

			private bool m_initCancelPage;

			private SecondaryStreams m_secondaryStreams;

			private CreateAndRegisterStream m_createAndRegisterStream;

			private bool m_addToggledItems;

			private bool m_addSecondaryStreamNames;

			private bool m_addOriginalValue;

			private float m_dpiX = 96f;

			private float m_dpiY = 96f;

			private Bitmap m_hdcBits;

			private Graphics m_bitsGraphics;

			private FontCache m_fontCache;

			private bool m_evaluatePageHeaderFooter;

			private bool m_addFirstPageHeaderFooter;

			private int m_pageNumber;

			private string m_pageName;

			private PageBreakInfo m_pageBreakInfo;

			private PageTotalInfo m_pageTotalInfo;

			private RPLReportSectionArea m_rplSectionArea = RPLReportSectionArea.Body;

			private RPLVersionEnum m_versionPicker;

			private Hashtable m_textboxSharedInfo;

			private Hashtable m_itemPropsStart;

			private Hashtable m_sharedImages;

			private Hashtable m_registeredStreamNames;

			private Hashtable m_autosizeSharedImages;

			private Hashtable m_sharedItemSizes;

			private Hashtable m_sharedRenderItemSizes;

			private Hashtable m_sharedEdgeItemSizes;

			private Hashtable m_sharedRenderEdgeItemSizes;

			private Hashtable m_sharedRenderRepeatItemSizes;

			private DocumentMapLabels m_labels;

			private Bookmarks m_bookmarks;

			private Dictionary<string, string> m_pageBookmarks;

			private IScalabilityCache m_scalabilityCache;

			private long m_totalScaleTimeMs;

			private long m_peakMemoryUsageKB;

			private MemoryPressureListener m_memoryPressure;

			private double m_pageHeightForMemory = 420.0;

			private bool m_useEmSquare;

			private bool m_canTracePagination;

			private ImageConsolidation m_imageConsolidation;

			private bool m_convertImages;

			private Stream m_delayTextBoxCache;

			private BinaryReader m_propertyCacheReader;

			private BinaryWriter m_propertyCacheWriter;

			private Hashtable m_registeredPBIgnored;

			internal double PageHeight
			{
				get
				{
					return this.m_pageHeight;
				}
				set
				{
					this.m_pageHeight = value;
				}
			}

			internal double OriginalPageHeight
			{
				get
				{
					return this.m_originalPageHeight;
				}
				set
				{
					this.m_originalPageHeight = value;
				}
			}

			internal bool RegisterEvents
			{
				get
				{
					return this.m_registerEvents;
				}
			}

			internal bool MeasureItems
			{
				get
				{
					return this.m_measureItems;
				}
			}

			internal bool EmfDynamicImage
			{
				get
				{
					return this.m_emfDynamicImage;
				}
			}

			internal bool ConsumeContainerWhitespace
			{
				get
				{
					return this.m_consumeWhitespace;
				}
			}

			internal CreateAndRegisterStream CreateAndRegisterStream
			{
				get
				{
					return this.m_createAndRegisterStream;
				}
			}

			internal SecondaryStreams SecondaryStreams
			{
				get
				{
					return this.m_secondaryStreams;
				}
			}

			internal bool AddToggledItems
			{
				get
				{
					return this.m_addToggledItems;
				}
			}

			internal bool AddSecondaryStreamNames
			{
				get
				{
					return this.m_addSecondaryStreamNames;
				}
			}

			internal bool AddOriginalValue
			{
				get
				{
					return this.m_addOriginalValue;
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

			internal bool AddFirstPageHeaderFooter
			{
				get
				{
					return this.m_addFirstPageHeaderFooter;
				}
				set
				{
					this.m_addFirstPageHeaderFooter = value;
				}
			}

			internal float DpiX
			{
				get
				{
					return this.m_dpiX;
				}
				set
				{
					this.m_dpiX = value;
				}
			}

			internal float DpiY
			{
				get
				{
					return this.m_dpiY;
				}
				set
				{
					this.m_dpiY = value;
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
				set
				{
					this.m_pageName = value;
				}
			}

			internal RPLReportSectionArea RPLSectionArea
			{
				get
				{
					return this.m_rplSectionArea;
				}
				set
				{
					this.m_rplSectionArea = value;
				}
			}

			internal Hashtable TextBoxSharedInfo
			{
				get
				{
					return this.m_textboxSharedInfo;
				}
				set
				{
					this.m_textboxSharedInfo = value;
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

			internal Hashtable RegisteredStreamNames
			{
				get
				{
					return this.m_registeredStreamNames;
				}
				set
				{
					this.m_registeredStreamNames = value;
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

			internal DocumentMapLabels Labels
			{
				get
				{
					return this.m_labels;
				}
				set
				{
					this.m_labels = value;
				}
			}

			internal Bookmarks Bookmarks
			{
				get
				{
					return this.m_bookmarks;
				}
				set
				{
					this.m_bookmarks = value;
				}
			}

			internal Dictionary<string, string> PageBookmarks
			{
				get
				{
					return this.m_pageBookmarks;
				}
				set
				{
					this.m_pageBookmarks = value;
				}
			}

			internal PageBreakInfo PageBreakInfo
			{
				get
				{
					return this.m_pageBreakInfo;
				}
				set
				{
					this.m_pageBreakInfo = value;
				}
			}

			internal PageTotalInfo PageTotalInfo
			{
				get
				{
					return this.m_pageTotalInfo;
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

			internal bool CancelMode
			{
				get
				{
					return this.m_initCancelPage;
				}
			}

			internal bool CancelPage
			{
				get
				{
					if (this.m_memoryPressure == null)
					{
						return false;
					}
					if (!this.m_cancelPage)
					{
						this.m_cancelPage = this.m_memoryPressure.CheckAndResetNotification();
					}
					return this.m_cancelPage;
				}
				set
				{
					this.m_cancelPage = value;
				}
			}

			internal FontCache FontCache
			{
				get
				{
					if (this.m_fontCache == null)
					{
						this.m_fontCache = new FontCache(96f, this.m_useEmSquare);
					}
					return this.m_fontCache;
				}
			}

			internal bool EmSquare
			{
				get
				{
					return this.m_useEmSquare;
				}
				set
				{
					this.m_useEmSquare = value;
				}
			}

			internal bool CanTracePagination
			{
				get
				{
					return this.m_canTracePagination;
				}
				set
				{
					this.m_canTracePagination = value;
				}
			}

			internal ImageConsolidation ImageConsolidation
			{
				get
				{
					return this.m_imageConsolidation;
				}
				set
				{
					this.m_imageConsolidation = value;
				}
			}

			internal RPLVersionEnum VersionPicker
			{
				get
				{
					return this.m_versionPicker;
				}
				set
				{
					this.m_versionPicker = value;
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

			internal bool ConvertImages
			{
				get
				{
					return this.m_convertImages;
				}
			}

			internal Hashtable RegisteredPBIgnored
			{
				get
				{
					return this.m_registeredPBIgnored;
				}
			}

			internal PageContextCommon(string pageName, double pageHeight, bool registerEvents, bool consumeWhitespace, CreateAndRegisterStream createAndRegisterStream)
			{
				this.m_pageHeight = pageHeight;
				this.m_originalPageHeight = pageHeight;
				this.m_registerEvents = registerEvents;
				this.m_consumeWhitespace = consumeWhitespace;
				this.m_sharedItemSizes = new Hashtable();
				this.m_sharedEdgeItemSizes = new Hashtable();
				this.m_sharedRenderItemSizes = new Hashtable();
				this.m_sharedRenderEdgeItemSizes = new Hashtable();
				this.m_sharedRenderRepeatItemSizes = new Hashtable();
				this.m_registeredStreamNames = new Hashtable();
				this.m_registeredPBIgnored = new Hashtable();
				this.m_createAndRegisterStream = createAndRegisterStream;
				this.m_pageTotalInfo = new PageTotalInfo(pageName);
			}

			internal void SetContext(bool measureItems, bool emfDynamicImage, SecondaryStreams secondaryStreams, bool addSecondaryStreamNames, bool addToggledItems, bool addOriginalValue, bool addFirstPageHeaderFooter, bool convertImages)
			{
				this.m_measureItems = measureItems;
				this.m_emfDynamicImage = emfDynamicImage;
				this.m_secondaryStreams = secondaryStreams;
				this.m_addSecondaryStreamNames = addSecondaryStreamNames;
				this.m_addToggledItems = addToggledItems;
				this.m_addOriginalValue = addOriginalValue;
				this.m_addFirstPageHeaderFooter = addFirstPageHeaderFooter;
				this.m_evaluatePageHeaderFooter = false;
				this.m_itemPropsStart = null;
				this.m_sharedImages = null;
				this.m_convertImages = convertImages;
				if (!measureItems)
				{
					this.DisposeResources();
				}
			}

			internal void DisposeResources()
			{
				this.DisposeGraphics();
				this.DisposeTextboxSharedInfo();
				this.DisposeDelayTextBox();
				this.DisposeScalabilityCache();
				this.DisposeMemoryPressureListener();
			}

			internal SizeF MeasureStringGDI(string text, CanvasFont font, SizeF layoutArea, out int charactersFitted, out int linesFilled)
			{
				if (this.m_bitsGraphics == null)
				{
					this.CreateGraphics();
				}
				StringFormat trimStringFormat = font.TrimStringFormat;
				return this.m_bitsGraphics.MeasureString(text, font.GDIFont, layoutArea, trimStringFormat, out charactersFitted, out linesFilled);
			}

			internal float MeasureFullTextBoxHeight(AspNetCore.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
			{
				if (this.m_bitsGraphics == null)
				{
					this.CreateGraphics();
				}
				return AspNetCore.ReportingServices.Rendering.RichText.TextBox.MeasureFullHeight(textBox, this.m_bitsGraphics, this.FontCache, flowContext, out contentHeight);
			}

			internal ItemSizes GetSharedItemSizesElement(ReportItem reportItem, bool isPadded)
			{
				if (reportItem == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (this.m_sharedItemSizes.ContainsKey(reportItem.ID))
				{
					itemSizes = (ItemSizes)this.m_sharedItemSizes[reportItem.ID];
					itemSizes.Update(reportItem);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(reportItem) : new PaddItemSizes(reportItem));
					this.m_sharedItemSizes.Add(reportItem.ID, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedItemSizesElement(ReportSize width, ReportSize height, string id, bool isPadded)
			{
				if (id == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (this.m_sharedItemSizes.ContainsKey(id))
				{
					itemSizes = (ItemSizes)this.m_sharedItemSizes[id];
					itemSizes.Update(width, height);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(width, height, id) : new PaddItemSizes(width, height, id));
					this.m_sharedItemSizes.Add(id, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID != null)
				{
					if (this.m_sharedRenderItemSizes.ContainsKey(itemSizes.ID))
					{
						itemSizes2 = (ItemSizes)this.m_sharedRenderItemSizes[itemSizes.ID];
						itemSizes2.Update(itemSizes, returnPaddings);
					}
					else
					{
						itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
						this.m_sharedRenderItemSizes.Add(itemSizes.ID, itemSizes2);
					}
				}
				else
				{
					itemSizes2 = ((!isPadded || !returnPaddings) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedEdgeItemSizesElement(double top, double left, string id)
			{
				if (id == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				if (this.m_sharedEdgeItemSizes.ContainsKey(id))
				{
					itemSizes = (ItemSizes)this.m_sharedEdgeItemSizes[id];
					itemSizes.Update(top, left);
				}
				else
				{
					itemSizes = new ItemSizes(top, left, id);
					this.m_sharedEdgeItemSizes.Add(id, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderEdgeItemSizesElement(ItemSizes itemSizes)
			{
				if (itemSizes == null)
				{
					return null;
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID != null)
				{
					if (this.m_sharedRenderEdgeItemSizes.ContainsKey(itemSizes.ID))
					{
						itemSizes2 = (ItemSizes)this.m_sharedRenderEdgeItemSizes[itemSizes.ID];
						itemSizes2.Update(itemSizes, false);
					}
					else
					{
						itemSizes2 = new ItemSizes(itemSizes);
						this.m_sharedRenderEdgeItemSizes.Add(itemSizes.ID, itemSizes2);
					}
				}
				else
				{
					itemSizes2 = new ItemSizes(itemSizes);
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedFromRepeatItemSizesElement(ReportItem reportItem, bool isPadded)
			{
				if (reportItem == null)
				{
					return null;
				}
				ItemSizes itemSizes = null;
				string key = reportItem.ID + "_REPEAT";
				if (this.m_sharedItemSizes.ContainsKey(key))
				{
					itemSizes = (ItemSizes)this.m_sharedItemSizes[key];
					itemSizes.Update(reportItem);
				}
				else
				{
					itemSizes = ((!isPadded) ? new ItemSizes(reportItem) : new PaddItemSizes(reportItem));
					this.m_sharedItemSizes.Add(key, itemSizes);
				}
				return itemSizes;
			}

			internal ItemSizes GetSharedRenderFromRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID != null)
				{
					string key = itemSizes.ID + "_REPEAT";
					if (this.m_sharedRenderItemSizes.ContainsKey(key))
					{
						itemSizes2 = (ItemSizes)this.m_sharedRenderItemSizes[key];
						itemSizes2.Update(itemSizes, returnPaddings);
					}
					else
					{
						itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
						this.m_sharedRenderItemSizes.Add(key, itemSizes2);
					}
				}
				else
				{
					itemSizes2 = ((!isPadded || !returnPaddings) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				return itemSizes2;
			}

			internal ItemSizes GetSharedRenderRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
			{
				if (itemSizes == null)
				{
					return null;
				}
				if (isPadded)
				{
					RSTrace.RenderingTracer.Assert(itemSizes is PaddItemSizes, "The ItemSizes object is not a PaddItemSizes object.");
				}
				ItemSizes itemSizes2 = null;
				if (itemSizes.ID != null)
				{
					if (this.m_sharedRenderRepeatItemSizes.ContainsKey(itemSizes.ID))
					{
						itemSizes2 = (ItemSizes)this.m_sharedRenderRepeatItemSizes[itemSizes.ID];
						itemSizes2.Update(itemSizes, returnPaddings);
					}
					else
					{
						itemSizes2 = ((!isPadded) ? new ItemSizes(itemSizes) : ((!returnPaddings) ? new PaddItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes)));
						this.m_sharedRenderRepeatItemSizes.Add(itemSizes.ID, itemSizes2);
					}
				}
				else
				{
					itemSizes2 = ((!isPadded || !returnPaddings) ? new ItemSizes(itemSizes) : new PaddItemSizes(itemSizes as PaddItemSizes));
				}
				return itemSizes2;
			}

			internal void RegisterPageBookmark(ReportItemInstance reportItemInstance)
			{
				if (reportItemInstance != null && reportItemInstance.Bookmark != null && !this.m_pageBookmarks.ContainsKey(reportItemInstance.Bookmark))
				{
					this.m_pageBookmarks.Add(reportItemInstance.Bookmark, reportItemInstance.UniqueName);
				}
			}

			internal void InitCache()
			{
				if (this.m_scalabilityCache == null)
				{
					this.m_scalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(this.m_createAndRegisterStream, "SPB", StorageObjectCreator.Instance, SPBReferenceCreator.Instance, ComponentType.Pagination, 1);
				}
			}

			internal void InitCancelPage(double pageHeight)
			{
				this.m_pageHeightForMemory = Math.Min(pageHeight, this.m_pageHeightForMemory);
				this.m_initCancelPage = true;
			}

			internal void ResetCancelPage()
			{
				this.m_initCancelPage = false;
				this.DisposeMemoryPressureListener();
				if (this.m_cancelPage)
				{
					this.m_itemPropsStart = null;
					this.m_sharedImages = null;
					this.m_pageBreakInfo = null;
					this.DisposeTextboxSharedInfo();
					if (this.m_imageConsolidation != null)
					{
						this.m_imageConsolidation.ResetCancelPage();
					}
				}
			}

			internal void CheckPageSize(RoundedDouble pageHeight)
			{
				if (this.m_initCancelPage && this.m_memoryPressure == null && pageHeight > this.m_pageHeightForMemory)
				{
					this.m_memoryPressure = new MemoryPressureListener();
				}
			}

			private void CreateGraphics()
			{
				this.m_hdcBits = new Bitmap(2, 2);
				this.m_hdcBits.SetResolution(96f, 96f);
				this.m_bitsGraphics = Graphics.FromImage(this.m_hdcBits);
				this.m_bitsGraphics.CompositingMode = CompositingMode.SourceOver;
				this.m_bitsGraphics.PageUnit = GraphicsUnit.Millimeter;
				this.m_bitsGraphics.PixelOffsetMode = PixelOffsetMode.Default;
				this.m_bitsGraphics.SmoothingMode = SmoothingMode.Default;
				this.m_bitsGraphics.TextRenderingHint = TextRenderingHint.SystemDefault;
			}

			private void DisposeGraphics()
			{
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
				if (this.m_fontCache != null)
				{
					this.m_fontCache.Dispose();
					this.m_fontCache = null;
				}
			}

			private void DisposeScalabilityCache()
			{
				if (this.m_scalabilityCache != null)
				{
					this.m_totalScaleTimeMs += this.m_scalabilityCache.ScalabilityDurationMs;
					this.m_peakMemoryUsageKB = Math.Max(this.m_peakMemoryUsageKB, this.m_scalabilityCache.PeakMemoryUsageKBytes);
					this.m_scalabilityCache.Dispose();
					this.m_scalabilityCache = null;
				}
			}

			private void DisposeMemoryPressureListener()
			{
				if (this.m_memoryPressure != null)
				{
					this.m_memoryPressure.Dispose();
					this.m_memoryPressure = null;
				}
			}

			private void DisposeTextboxSharedInfo()
			{
				if (this.m_textboxSharedInfo != null)
				{
					foreach (object value in this.m_textboxSharedInfo.Values)
					{
						TextBoxSharedInfo textBoxSharedInfo = (TextBoxSharedInfo)value;
						if (textBoxSharedInfo != null && textBoxSharedInfo.SharedFont != null)
						{
							textBoxSharedInfo.Dispose();
						}
					}
					this.m_textboxSharedInfo = null;
				}
			}

			internal void DisposeDelayTextBox()
			{
				if (this.m_delayTextBoxCache != null)
				{
					this.m_propertyCacheReader = null;
					this.m_propertyCacheWriter = null;
					this.m_delayTextBoxCache.Close();
					this.m_delayTextBoxCache.Dispose();
					this.m_delayTextBoxCache = null;
				}
			}

			internal void CreateCacheStream(long offsetStart)
			{
				if (this.m_delayTextBoxCache == null)
				{
					this.m_delayTextBoxCache = this.m_createAndRegisterStream("SPBNonSharedCache", "rpl", null, null, true, StreamOper.CreateOnly);
					BufferedStream bufferedStream = new BufferedStream(this.m_delayTextBoxCache);
					this.m_propertyCacheReader = new BinaryReader(bufferedStream, Encoding.Unicode);
					this.m_propertyCacheWriter = new BinaryWriter(bufferedStream, Encoding.Unicode);
				}
				this.m_delayTextBoxCache.Position = offsetStart;
			}
		}

		internal class MemoryPressureListener : IDisposable
		{
			private int m_notificationCount;

			public bool ReceivedPressureNotification
			{
				get
				{
					return this.m_notificationCount > 0;
				}
			}

			public void ResetNotificationState()
			{
				this.m_notificationCount = 0;
			}

			public bool CheckAndResetNotification()
			{
				bool receivedPressureNotification = this.ReceivedPressureNotification;
				this.ResetNotificationState();
				return receivedPressureNotification;
			}

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}
		}

		internal const double RoundDelta = 0.01;

		internal const double RoundOverlapDelta = 0.0001;

		internal const string InvalidImage = "InvalidImage";

		internal const char StreamNameSeparator = '_';

		internal const string PageHeaderSuffix = "H";

		internal const string PageFooterSuffix = "F";

		internal const string ImageStreamNamePrefix = "I";

		internal const string BkGndImageStreamNamePrefix = "B";

		internal const string ChartStreamNamePrefix = "C";

		internal const string GaugeStreamNamePrefix = "G";

		internal const string MapStreamNamePrefix = "M";

		internal const double InvalidImageWidth = 3.8;

		internal const double InvalidImageHeight = 4.0;

		internal const double MinFontSize = 0.3528;

		internal const char RPLVersionSeparator = '.';

		internal static readonly string PNG_MIME_TYPE = "image/png";

		private PageContextFlags m_flags;

		private PageContextCommon m_common;

		private IgnorePBReasonFlag m_ignorePBReason;

		internal ImageConsolidation ImageConsolidation
		{
			get
			{
				return this.m_common.ImageConsolidation;
			}
			set
			{
				this.m_common.ImageConsolidation = value;
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

		internal bool CancelMode
		{
			get
			{
				return this.m_common.CancelMode;
			}
		}

		internal bool CancelPage
		{
			get
			{
				return this.m_common.CancelPage;
			}
			set
			{
				this.m_common.CancelPage = value;
			}
		}

		internal double PageHeight
		{
			get
			{
				if (this.KeepTogether)
				{
					return 1.7976931348623157E+308;
				}
				return this.m_common.PageHeight;
			}
			set
			{
				this.m_common.PageHeight = value;
			}
		}

		internal double OriginalPageHeight
		{
			get
			{
				return this.m_common.OriginalPageHeight;
			}
			set
			{
				this.m_common.OriginalPageHeight = value;
			}
		}

		internal PageContextFlags Flags
		{
			get
			{
				return this.m_flags;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				if (this.FullOnPage)
				{
					return true;
				}
				return (int)(this.m_flags & PageContextFlags.IgnorePageBreak) > 0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= PageContextFlags.IgnorePageBreak;
				}
				else
				{
					this.m_flags &= ~PageContextFlags.IgnorePageBreak;
				}
			}
		}

		internal bool FullOnPage
		{
			get
			{
				return (int)(this.m_flags & PageContextFlags.FullOnPage) > 0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= PageContextFlags.FullOnPage;
				}
				else
				{
					this.m_flags &= ~PageContextFlags.FullOnPage;
				}
			}
		}

		internal bool StretchPage
		{
			get
			{
				return (int)(this.m_flags & PageContextFlags.StretchPage) > 0;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return (int)(this.m_flags & PageContextFlags.KeepTogether) > 0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= PageContextFlags.KeepTogether;
				}
				else
				{
					this.m_flags &= ~PageContextFlags.KeepTogether;
				}
			}
		}

		internal bool HideDuplicates
		{
			get
			{
				return (int)(this.m_flags & PageContextFlags.HideDuplicates) > 0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= PageContextFlags.HideDuplicates;
				}
				else
				{
					this.m_flags &= ~PageContextFlags.HideDuplicates;
				}
			}
		}

		internal bool TypeCodeNonString
		{
			get
			{
				return (int)(this.m_flags & PageContextFlags.TypeCodeNonString) > 0;
			}
			set
			{
				if (value)
				{
					this.m_flags |= PageContextFlags.TypeCodeNonString;
				}
				else
				{
					this.m_flags &= ~PageContextFlags.TypeCodeNonString;
				}
			}
		}

		internal bool RegisterEvents
		{
			get
			{
				return this.m_common.RegisterEvents;
			}
		}

		internal bool MeasureItems
		{
			get
			{
				return this.m_common.MeasureItems;
			}
		}

		internal bool EmfDynamicImage
		{
			get
			{
				return this.m_common.EmfDynamicImage;
			}
		}

		internal bool ConsumeContainerWhitespace
		{
			get
			{
				return this.m_common.ConsumeContainerWhitespace;
			}
		}

		internal CreateAndRegisterStream CreateAndRegisterStream
		{
			get
			{
				return this.m_common.CreateAndRegisterStream;
			}
		}

		internal SecondaryStreams SecondaryStreams
		{
			get
			{
				return this.m_common.SecondaryStreams;
			}
		}

		internal bool AddToggledItems
		{
			get
			{
				return this.m_common.AddToggledItems;
			}
		}

		internal bool AddOriginalValue
		{
			get
			{
				return this.m_common.AddOriginalValue;
			}
		}

		internal bool AddSecondaryStreamNames
		{
			get
			{
				return this.m_common.AddSecondaryStreamNames;
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

		internal bool AddFirstPageHeaderFooter
		{
			get
			{
				return this.m_common.AddFirstPageHeaderFooter;
			}
			set
			{
				this.m_common.AddFirstPageHeaderFooter = value;
			}
		}

		internal float DpiX
		{
			get
			{
				return this.m_common.DpiX;
			}
			set
			{
				this.m_common.DpiY = value;
			}
		}

		internal float DpiY
		{
			get
			{
				return this.m_common.DpiY;
			}
			set
			{
				this.m_common.DpiY = value;
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

		internal RPLReportSectionArea RPLSectionArea
		{
			get
			{
				return this.m_common.RPLSectionArea;
			}
			set
			{
				this.m_common.RPLSectionArea = value;
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

		internal Hashtable RegisteredStreamNames
		{
			get
			{
				return this.m_common.RegisteredStreamNames;
			}
			set
			{
				this.m_common.RegisteredStreamNames = value;
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

		internal Hashtable TextBoxSharedInfo
		{
			get
			{
				return this.m_common.TextBoxSharedInfo;
			}
			set
			{
				this.m_common.TextBoxSharedInfo = value;
			}
		}

		internal DocumentMapLabels Labels
		{
			get
			{
				return this.m_common.Labels;
			}
			set
			{
				this.m_common.Labels = value;
			}
		}

		internal Bookmarks Bookmarks
		{
			get
			{
				return this.m_common.Bookmarks;
			}
			set
			{
				this.m_common.Bookmarks = value;
			}
		}

		internal Dictionary<string, string> PageBookmarks
		{
			get
			{
				return this.m_common.PageBookmarks;
			}
			set
			{
				this.m_common.PageBookmarks = value;
			}
		}

		internal bool IsPageBreakRegistered
		{
			get
			{
				return this.m_common.PageBreakInfo != null;
			}
		}

		internal bool IsPageNameRegistered
		{
			get
			{
				return this.m_common.PageName != null;
			}
		}

		internal PageTotalInfo PageTotalInfo
		{
			get
			{
				return this.m_common.PageTotalInfo;
			}
		}

		internal bool CanTracePagination
		{
			get
			{
				return this.m_common.CanTracePagination;
			}
			set
			{
				this.m_common.CanTracePagination = value;
			}
		}

		internal RPLVersionEnum VersionPicker
		{
			get
			{
				return this.m_common.VersionPicker;
			}
			set
			{
				this.m_common.VersionPicker = value;
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

		internal PageContextCommon Common
		{
			get
			{
				return this.m_common;
			}
		}

		internal bool ConvertImages
		{
			get
			{
				return this.m_common.ConvertImages;
			}
		}

		internal IgnorePBReasonFlag IgnorePBReason
		{
			get
			{
				return this.m_ignorePBReason;
			}
		}

		internal bool TracingEnabled
		{
			get
			{
				if (RenderingDiagnostics.Enabled)
				{
					return this.CanTracePagination;
				}
				return false;
			}
		}

		internal PageContext(string pageName, double pageHeight, bool registerEvents, bool consumeWhiteSpace, CreateAndRegisterStream createAndRegisterStream)
		{
			this.m_common = new PageContextCommon(pageName, pageHeight, registerEvents, consumeWhiteSpace, createAndRegisterStream);
		}

		internal PageContext(PageContext pageContext)
		{
			this.m_common = pageContext.Common;
			this.m_flags = pageContext.Flags;
			this.m_ignorePBReason = pageContext.IgnorePBReason;
		}

		internal PageContext(PageContext pageContext, PageContextFlags flags, IgnorePBReasonFlag ignorePBReason)
		{
			this.m_common = pageContext.Common;
			this.m_flags = flags;
			if (pageContext.IgnorePBReason == IgnorePBReasonFlag.None)
			{
				this.m_ignorePBReason = ignorePBReason;
			}
			else
			{
				this.m_ignorePBReason = pageContext.IgnorePBReason;
			}
			this.KeepTogether = pageContext.KeepTogether;
		}

		internal void SetContext(bool measureItems, bool emfDynamicImage, SecondaryStreams secondaryStreams, bool addSecondaryStreamNames, bool addToggledItems, bool addOriginalValue, bool addFirstPageHeaderFooter, bool convertImages)
		{
			this.m_common.SetContext(measureItems, emfDynamicImage, secondaryStreams, addSecondaryStreamNames, addToggledItems, addOriginalValue, addFirstPageHeaderFooter, convertImages);
			this.m_flags = PageContextFlags.Clear;
		}

		internal void InitCache()
		{
			this.m_common.InitCache();
		}

		internal void InitCancelPage(double pageHeight)
		{
			this.m_common.InitCancelPage(pageHeight);
		}

		internal void ResetCancelPage()
		{
			this.m_common.ResetCancelPage();
		}

		internal void RegisterPageBreak(PageBreakInfo pageBreakInfo)
		{
			this.RegisterPageBreak(pageBreakInfo, false);
		}

		internal void RegisterPageBreak(PageBreakInfo pageBreakInfo, bool overrideChild)
		{
			if (this.m_common.PageBreakInfo == null || overrideChild)
			{
				this.m_common.PageBreakInfo = pageBreakInfo;
			}
			else if (this.TracingEnabled)
			{
				this.TracePageBreakIgnoredBecauseOfPeerItem(pageBreakInfo);
			}
		}

		internal void RegisterPageName(string pageName)
		{
			this.RegisterPageName(pageName, false);
		}

		internal void RegisterPageName(string pageName, bool overrideChild)
		{
			if (pageName != null && (this.m_common.PageName == null || overrideChild))
			{
				this.m_common.PageName = pageName;
			}
		}

		internal void ApplyPageBreak(int currentPageNumber)
		{
			if (this.m_common.PageBreakInfo != null && !this.m_common.PageBreakInfo.Disabled)
			{
				if (this.m_common.PageBreakInfo.ResetPageNumber)
				{
					this.m_common.PageTotalInfo.FinalizePageNumberForTotal();
				}
				this.m_common.PageBreakInfo = null;
			}
		}

		internal void ApplyPageName(int currentPageNumber)
		{
			if (this.TracingEnabled)
			{
				this.TracePageNameChanged();
			}
			this.m_common.PageTotalInfo.SetPageName(currentPageNumber, this.m_common.PageName);
			this.m_common.PageName = null;
		}

		internal void CheckPageSize(RoundedDouble pageHeight)
		{
			this.m_common.CheckPageSize(pageHeight);
		}

		internal void DisposeResources()
		{
			this.m_common.DisposeResources();
		}

		internal float MeasureFullTextBoxHeight(AspNetCore.ReportingServices.Rendering.RichText.TextBox textBox, FlowContext flowContext, out float contentHeight)
		{
			return this.m_common.MeasureFullTextBoxHeight(textBox, flowContext, out contentHeight);
		}

		internal ItemSizes GetSharedItemSizesElement(ReportItem reportItem, bool isPadded)
		{
			return this.m_common.GetSharedItemSizesElement(reportItem, isPadded);
		}

		internal ItemSizes GetSharedItemSizesElement(ReportSize width, ReportSize height, string id, bool isPadded)
		{
			return this.m_common.GetSharedItemSizesElement(width, height, id, isPadded);
		}

		internal ItemSizes GetSharedRenderItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return this.m_common.GetSharedRenderItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal ItemSizes GetSharedEdgeItemSizesElement(double top, double left, string id)
		{
			return this.m_common.GetSharedEdgeItemSizesElement(top, left, id);
		}

		internal ItemSizes GetSharedRenderEdgeItemSizesElement(ItemSizes itemSizes)
		{
			return this.m_common.GetSharedRenderEdgeItemSizesElement(itemSizes);
		}

		internal ItemSizes GetSharedFromRepeatItemSizesElement(ReportItem reportItem, bool isPadded)
		{
			return this.m_common.GetSharedFromRepeatItemSizesElement(reportItem, isPadded);
		}

		internal ItemSizes GetSharedRenderFromRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return this.m_common.GetSharedRenderFromRepeatItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal ItemSizes GetSharedRenderRepeatItemSizesElement(ItemSizes itemSizes, bool isPadded, bool returnPaddings)
		{
			return this.m_common.GetSharedRenderRepeatItemSizesElement(itemSizes, isPadded, returnPaddings);
		}

		internal SizeF MeasureStringGDI(string text, CanvasFont font, SizeF layoutArea, out int charactersFitted, out int linesFilled)
		{
			return this.m_common.MeasureStringGDI(text, font, layoutArea, out charactersFitted, out linesFilled);
		}

		internal double ConvertToMillimeters(int coordinate, float dpi)
		{
			if (0.0 == dpi)
			{
				return 1.7976931348623157E+308;
			}
			return 1.0 / (double)dpi * (double)coordinate * 25.399999618530273;
		}

		internal string GenerateStreamName(IImageInstance imageInstance, string ownerUniqueName)
		{
			StringBuilder stringBuilder = null;
			stringBuilder = ((!(imageInstance is BackgroundImageInstance)) ? new StringBuilder("I") : new StringBuilder("B"));
			stringBuilder.Append('_');
			stringBuilder.Append(ownerUniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(this.PageNumber);
			if (this.RPLSectionArea == RPLReportSectionArea.Header)
			{
				stringBuilder.Append('_');
				stringBuilder.Append("H");
			}
			else if (this.RPLSectionArea == RPLReportSectionArea.Footer)
			{
				stringBuilder.Append('_');
				stringBuilder.Append("F");
			}
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(ChartInstance chartIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("C");
			stringBuilder.Append('_');
			stringBuilder.Append(chartIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(this.PageNumber);
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(GaugePanelInstance gaugeIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("G");
			stringBuilder.Append('_');
			stringBuilder.Append(gaugeIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(this.PageNumber);
			return stringBuilder.ToString();
		}

		internal string GenerateStreamName(MapInstance mapIntance)
		{
			StringBuilder stringBuilder = new StringBuilder("M");
			stringBuilder.Append('_');
			stringBuilder.Append(mapIntance.UniqueName);
			stringBuilder.Append('_');
			stringBuilder.Append(this.PageNumber);
			return stringBuilder.ToString();
		}

		internal void RegisterPageBookmark(ReportItemInstance reportItemInstance)
		{
			this.m_common.RegisterPageBookmark(reportItemInstance);
		}

		internal void CreateCacheStream(long value)
		{
			this.m_common.CreateCacheStream(value);
		}

		private void TracePageBreakIgnoredBecauseOfPeerItem(PageBreakInfo ignoredPageBreak)
		{
			if (ignoredPageBreak != null && !ignoredPageBreak.Disabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – peer item precedence", this.PageNumber, ignoredPageBreak.ReportItemName);
			}
		}

		private void TracePageNameChanged()
		{
			if (this.m_common.PageName != null && this.m_common.PageTotalInfo != null && string.CompareOrdinal(this.m_common.PageTotalInfo.GetPageName(this.PageNumber - 1), this.m_common.PageName) != 0)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page name changed", this.PageNumber);
			}
		}
	}
}
