using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.Collections.Specialized;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Interactivity
	{
		internal enum EventType
		{
			UserSortEvent,
			FindStringEvent,
			BookmarkNavigationEvent,
			DocumentMapNavigationEvent,
			GetDocumentMap,
			FindChart,
			FindGaugePanel,
			Collect,
			FindImage,
			FindMap,
			ImageConsolidation,
			DrillthroughEvent
		}

		internal class DrillthroughInfo
		{
			private string m_reportName;

			private NameValueCollection m_parameters;

			internal string ReportName
			{
				get
				{
					return this.m_reportName;
				}
				set
				{
					this.m_reportName = value;
				}
			}

			internal NameValueCollection Parameters
			{
				get
				{
					return this.m_parameters;
				}
				set
				{
					this.m_parameters = value;
				}
			}
		}

		private string m_itemInfo;

		private string m_bookmarkId;

		private bool m_itemFound;

		private string m_streamName;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private EventType m_eventType = EventType.Collect;

		private DrillthroughInfo m_drillthroughResult;

		internal bool Done
		{
			get
			{
				return this.m_itemFound;
			}
		}

		internal string ItemInfo
		{
			get
			{
				return this.m_itemInfo;
			}
		}

		internal bool NeedPageHeaderFooter
		{
			get
			{
				if (this.m_eventType != 0 && this.m_eventType != EventType.DocumentMapNavigationEvent && this.m_eventType != EventType.GetDocumentMap && this.m_eventType != EventType.FindChart && this.m_eventType != EventType.FindGaugePanel && this.m_eventType != EventType.FindMap)
				{
					return true;
				}
				return false;
			}
		}

		internal bool RegisterHiddenItems
		{
			get
			{
				if (this.m_eventType != 0 && this.m_eventType != EventType.FindStringEvent && this.m_eventType != EventType.FindChart && this.m_eventType != EventType.FindGaugePanel && this.m_eventType != EventType.FindMap && this.m_eventType != EventType.FindImage && this.m_eventType != EventType.ImageConsolidation)
				{
					return true;
				}
				return false;
			}
		}

		internal EventType InteractivityEventType
		{
			get
			{
				return this.m_eventType;
			}
		}

		internal DrillthroughInfo DrillthroughResult
		{
			get
			{
				return this.m_drillthroughResult;
			}
		}

		internal Interactivity()
		{
		}

		internal Interactivity(string itemInfo, EventType eventType, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			this.m_itemInfo = itemInfo;
			this.m_eventType = eventType;
			this.m_streamName = streamName;
			this.m_createAndRegisterStream = createAndRegisterStream;
		}

		internal Interactivity(string itemInfo, EventType eventType)
		{
			this.m_itemInfo = itemInfo;
			this.m_eventType = eventType;
		}

		internal Interactivity(EventType eventType)
		{
			this.m_eventType = eventType;
		}

		internal Interactivity(string bookmarkId)
		{
			this.m_bookmarkId = bookmarkId;
			this.m_eventType = EventType.BookmarkNavigationEvent;
		}

		internal bool RegisterItem(PageItem pageItem, PageContext pageContext)
		{
			if (!this.m_itemFound && pageItem != null)
			{
				switch (this.m_eventType)
				{
				case EventType.Collect:
				{
					ReportItemInstance instance2 = pageItem.Source.Instance;
					if (pageContext.Labels != null)
					{
						pageContext.Labels.WriteDocMapLabel(instance2);
					}
					if (pageContext.Bookmarks != null)
					{
						pageContext.Bookmarks.WriteBookmark(instance2);
					}
					if (pageContext.PageBookmarks != null)
					{
						pageContext.RegisterPageBookmark(instance2);
					}
					if (pageItem.ItemState == PageItem.State.OnPageHidden)
					{
						break;
					}
					return false;
				}
				case EventType.BookmarkNavigationEvent:
				{
					ReportItemInstance instance = pageItem.Source.Instance;
					if (instance.Bookmark != null && SPBProcessing.CompareWithOrdinalComparison(this.m_bookmarkId, instance.Bookmark, false) == 0)
					{
						this.m_itemFound = true;
						this.m_itemInfo = instance.UniqueName;
						return false;
					}
					if (pageItem.ItemState == PageItem.State.OnPageHidden)
					{
						break;
					}
					return false;
				}
				case EventType.DrillthroughEvent:
				{
					ReportItemInstance instance5 = pageItem.Source.Instance;
					TextBoxInstance textBoxInstance = instance5 as TextBoxInstance;
					if (textBoxInstance != null)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = (AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)pageItem.Source;
						if (!this.HasMatchingDrillthrough(textBox.ActionInfo))
						{
							foreach (ParagraphInstance paragraphInstance in textBoxInstance.ParagraphInstances)
							{
								foreach (TextRunInstance textRunInstance in paragraphInstance.TextRunInstances)
								{
									AspNetCore.ReportingServices.OnDemandReportRendering.TextRun definition = textRunInstance.Definition;
									if (this.HasMatchingDrillthrough(definition.ActionInfo))
									{
										return false;
									}
								}
							}
						}
					}
					else
					{
						ImageInstance imageInstance2 = instance5 as ImageInstance;
						if (imageInstance2 != null)
						{
							if (!this.HasMatchingDrillthrough(imageInstance2.ActionInfoWithDynamicImageMapAreas))
							{
								AspNetCore.ReportingServices.OnDemandReportRendering.Image image = (AspNetCore.ReportingServices.OnDemandReportRendering.Image)pageItem.Source;
								this.HasMatchingDrillthrough(image.ActionInfo);
							}
						}
						else
						{
							IDynamicImageInstance dynamicImageInstance = instance5 as IDynamicImageInstance;
							if (dynamicImageInstance != null)
							{
								ActionInfoWithDynamicImageMapCollection imageMaps = default(ActionInfoWithDynamicImageMapCollection);
								using (dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.PNG, out imageMaps))
								{
								}
								this.HasMatchingDrillthrough(imageMaps);
							}
						}
					}
					if (this.m_itemFound)
					{
						return false;
					}
					if (pageItem.ItemState == PageItem.State.OnPageHidden)
					{
						break;
					}
					return false;
				}
				case EventType.DocumentMapNavigationEvent:
				{
					ReportItemInstance instance3 = pageItem.Source.Instance;
					if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, instance3.UniqueName, true) == 0)
					{
						this.m_itemFound = true;
						return false;
					}
					if (pageItem.ItemState == PageItem.State.OnPageHidden)
					{
						break;
					}
					return false;
				}
				case EventType.GetDocumentMap:
				{
					ReportItemInstance instance4 = pageItem.Source.Instance;
					if (pageContext.Labels != null)
					{
						pageContext.Labels.WriteDocMapLabel(instance4);
					}
					if (pageItem.ItemState == PageItem.State.OnPageHidden)
					{
						break;
					}
					return false;
				}
				case EventType.FindChart:
					if (pageItem.ItemState != PageItem.State.OnPageHidden)
					{
						ReportItem source2 = pageItem.Source;
						if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, source2.Instance.UniqueName, true) == 0)
						{
							this.m_itemFound = true;
							ChartInstance chartInstance2 = source2.Instance as ChartInstance;
							if (chartInstance2 != null)
							{
								this.WriteDynamicImageStream(chartInstance2.GetImage());
							}
						}
					}
					break;
				case EventType.FindGaugePanel:
					if (pageItem.ItemState != PageItem.State.OnPageHidden)
					{
						ReportItem source3 = pageItem.Source;
						if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, source3.Instance.UniqueName, true) == 0)
						{
							this.m_itemFound = true;
							GaugePanelInstance gaugePanelInstance2 = source3.Instance as GaugePanelInstance;
							if (gaugePanelInstance2 != null)
							{
								this.WriteDynamicImageStream(gaugePanelInstance2.GetImage());
							}
						}
					}
					break;
				case EventType.FindMap:
					if (pageItem.ItemState != PageItem.State.OnPageHidden)
					{
						ReportItem source5 = pageItem.Source;
						if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, source5.Instance.UniqueName, true) == 0)
						{
							this.m_itemFound = true;
							MapInstance mapInstance2 = source5.Instance as MapInstance;
							if (mapInstance2 != null)
							{
								this.WriteDynamicImageStream(mapInstance2.GetImage());
							}
						}
					}
					break;
				case EventType.FindImage:
					if (pageItem.ItemState != PageItem.State.OnPageHidden)
					{
						ReportItem source4 = pageItem.Source;
						if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, source4.Instance.UniqueName, true) == 0)
						{
							this.m_itemFound = true;
							ImageInstance imageInstance = source4.Instance as ImageInstance;
							if (imageInstance != null)
							{
								Stream stream2 = this.m_createAndRegisterStream(this.m_streamName, string.Empty, null, imageInstance.MIMEType, false, StreamOper.CreateAndRegister);
								byte[] imageData = imageInstance.ImageData;
								if (stream2 != null && imageData != null && imageData.Length != 0)
								{
									stream2.Write(imageData, 0, imageData.Length);
								}
							}
						}
					}
					break;
				case EventType.ImageConsolidation:
					if (pageItem.ItemState != PageItem.State.OnPageHidden)
					{
						ReportItem source = pageItem.Source;
						GaugePanelInstance gaugePanelInstance = source.Instance as GaugePanelInstance;
						Stream stream = null;
						if (gaugePanelInstance != null)
						{
							stream = gaugePanelInstance.GetImage();
						}
						else
						{
							ChartInstance chartInstance = source.Instance as ChartInstance;
							if (chartInstance != null)
							{
								stream = chartInstance.GetImage();
							}
							else
							{
								MapInstance mapInstance = source.Instance as MapInstance;
								if (mapInstance != null)
								{
									stream = mapInstance.GetImage();
								}
							}
						}
						if (stream != null)
						{
							ImageConsolidation imageConsolidation = pageContext.ImageConsolidation;
							imageConsolidation.AppendImage(stream);
							if (imageConsolidation.CurrentOffset >= imageConsolidation.IgnoreOffsetTill + 1 && imageConsolidation.ImageInfos.Count > 0)
							{
								this.m_itemFound = true;
							}
						}
					}
					break;
				default:
					this.FindTextBox(pageItem as TextBox, pageContext);
					break;
				}
				return true;
			}
			return false;
		}

		private bool HasMatchingDrillthrough(ActionInfoWithDynamicImageMapCollection imageMaps)
		{
			if (imageMaps == null)
			{
				return false;
			}
			foreach (ActionInfoWithDynamicImageMap imageMap in imageMaps)
			{
				if (this.HasMatchingDrillthrough(imageMap))
				{
					return true;
				}
			}
			return false;
		}

		private bool HasMatchingDrillthrough(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return false;
			}
			foreach (Action action in actionInfo.Actions)
			{
				ActionDrillthrough drillthrough = action.Drillthrough;
				if (drillthrough != null)
				{
					ActionDrillthroughInstance instance = drillthrough.Instance;
					if (instance != null && SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, instance.DrillthroughID, false) == 0)
					{
						this.m_drillthroughResult = new DrillthroughInfo();
						this.m_drillthroughResult.ReportName = instance.ReportName;
						if (drillthrough.Parameters != null)
						{
							this.m_drillthroughResult.Parameters = drillthrough.Parameters.ToNameValueCollectionForDrillthroughEvent();
						}
						this.m_itemFound = true;
						return true;
					}
				}
			}
			return false;
		}

		private void WriteDynamicImageStream(Stream startStream)
		{
			Stream stream = this.m_createAndRegisterStream(this.m_streamName, "png", null, PageContext.PNG_MIME_TYPE, false, StreamOper.CreateAndRegister);
			if (startStream != null && stream != null)
			{
				startStream.Position = 0L;
				byte[] array = new byte[4096];
				for (int num = startStream.Read(array, 0, array.Length); num != 0; num = startStream.Read(array, 0, array.Length))
				{
					stream.Write(array, 0, num);
				}
			}
		}

		private void FindTextBox(TextBox textbox, PageContext pageContext)
		{
			if (!this.m_itemFound && textbox != null && textbox.ItemState != PageItem.State.OnPageHidden)
			{
				if (this.m_eventType == EventType.FindStringEvent)
				{
					this.m_itemFound = textbox.SearchTextBox(this.m_itemInfo, pageContext);
				}
				else if (this.m_eventType == EventType.UserSortEvent)
				{
					ReportItem source = textbox.Source;
					if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, source.Instance.UniqueName, true) == 0)
					{
						this.m_itemFound = true;
					}
				}
			}
		}

		internal bool RegisterHiddenItem(ReportItem reportItem, PageContext pageContext)
		{
			if (this.m_eventType == EventType.Collect)
			{
				ReportItemInstance instance = reportItem.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance);
				}
				if (pageContext.Bookmarks != null)
				{
					pageContext.Bookmarks.WriteBookmark(instance);
				}
				if (pageContext.PageBookmarks != null)
				{
					pageContext.RegisterPageBookmark(instance);
				}
				goto IL_00e5;
			}
			if (this.m_eventType == EventType.GetDocumentMap)
			{
				ReportItemInstance instance2 = reportItem.Instance;
				if (pageContext.Labels != null)
				{
					pageContext.Labels.WriteDocMapLabel(instance2);
				}
				goto IL_00e5;
			}
			if (this.m_eventType == EventType.BookmarkNavigationEvent)
			{
				ReportItemInstance instance3 = reportItem.Instance;
				if (instance3.Bookmark != null && SPBProcessing.CompareWithOrdinalComparison(this.m_bookmarkId, instance3.Bookmark, false) == 0)
				{
					this.m_itemFound = true;
					this.m_itemInfo = instance3.UniqueName;
					return false;
				}
				goto IL_00e5;
			}
			if (this.m_eventType == EventType.DocumentMapNavigationEvent)
			{
				ReportItemInstance instance4 = reportItem.Instance;
				if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, instance4.UniqueName, true) == 0)
				{
					this.m_itemFound = true;
					return false;
				}
				goto IL_00e5;
			}
			return false;
			IL_00e5:
			return true;
		}

		internal void RegisterGroupLabel(Group group, PageContext pageContext)
		{
			if (!this.m_itemFound && group != null)
			{
				if (this.m_eventType == EventType.Collect || this.m_eventType == EventType.GetDocumentMap)
				{
					GroupInstance instance = group.Instance;
					if (pageContext.Labels != null)
					{
						pageContext.Labels.WriteDocMapLabel(instance);
					}
				}
				else if (this.m_eventType == EventType.DocumentMapNavigationEvent)
				{
					GroupInstance instance2 = group.Instance;
					if (SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, instance2.UniqueName, true) == 0)
					{
						this.m_itemFound = true;
					}
				}
			}
		}

		internal void RegisterDocMapRootLabel(string rootLabelUniqueName, PageContext pageContext)
		{
			if (!this.m_itemFound && rootLabelUniqueName != null && pageContext.PageNumber == 1)
			{
				if (this.m_eventType == EventType.Collect || this.m_eventType == EventType.GetDocumentMap)
				{
					if (pageContext.Labels != null)
					{
						pageContext.Labels.WriteDocMapRootLabel(rootLabelUniqueName);
					}
				}
				else if (this.m_eventType == EventType.DocumentMapNavigationEvent && SPBProcessing.CompareWithOrdinalComparison(this.m_itemInfo, rootLabelUniqueName, true) == 0)
				{
					this.m_itemFound = true;
				}
			}
		}
	}
}
