using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class PageItem : PageElement
	{
		internal enum State : byte
		{
			Unknow,
			OnPage,
			Below,
			TopNextPage,
			SpanPages,
			OnPagePBEnd,
			OnPageHidden
		}

		protected new ReportItem m_source;

		protected ItemSizes m_itemPageSizes;

		protected ItemSizes m_itemRenderSizes;

		protected List<int> m_pageItemsAbove;

		protected List<int> m_pageItemsLeft;

		protected long m_offset;

		protected RPLItem m_rplElement;

		protected State m_itemState;

		protected byte m_rplItemState;

		protected double m_defLeftValue;

		public override long Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}

		internal new ReportItem Source
		{
			get
			{
				return this.m_source;
			}
		}

		internal override ReportElement OriginalSource
		{
			get
			{
				return this.m_source;
			}
		}

		internal override string SourceID
		{
			get
			{
				return this.m_source.ID;
			}
		}

		internal override string SourceUniqueName
		{
			get
			{
				return this.m_source.InstanceUniqueName;
			}
		}

		internal virtual bool StaticItem
		{
			get
			{
				return false;
			}
		}

		internal byte RplItemState
		{
			get
			{
				return this.m_rplItemState;
			}
		}

		internal bool RepeatedSibling
		{
			get
			{
				if (this.m_source == null)
				{
					return false;
				}
				return this.m_source.RepeatedSibling;
			}
		}

		internal ItemSizes ItemPageSizes
		{
			get
			{
				return this.m_itemPageSizes;
			}
		}

		internal ItemSizes ItemRenderSizes
		{
			get
			{
				return this.m_itemRenderSizes;
			}
			set
			{
				this.m_itemRenderSizes = value;
			}
		}

		internal List<int> PageItemsAbove
		{
			get
			{
				return this.m_pageItemsAbove;
			}
			set
			{
				this.m_pageItemsAbove = value;
			}
		}

		internal List<int> PageItemsLeft
		{
			get
			{
				return this.m_pageItemsLeft;
			}
			set
			{
				this.m_pageItemsLeft = value;
			}
		}

		public RPLItem RPLElement
		{
			get
			{
				return this.m_rplElement;
			}
			set
			{
				this.m_rplElement = value;
			}
		}

		internal State ItemState
		{
			get
			{
				return this.m_itemState;
			}
			set
			{
				this.m_itemState = value;
			}
		}

		internal double DefLeftValue
		{
			get
			{
				return this.m_defLeftValue;
			}
			set
			{
				this.m_defLeftValue = value;
			}
		}

		protected virtual PageBreak PageBreak
		{
			get
			{
				return null;
			}
		}

		protected virtual string PageName
		{
			get
			{
				return null;
			}
		}

		internal string ItemName
		{
			get
			{
				if (this.m_source != null)
				{
					return this.m_source.Name;
				}
				return null;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				if (this.PageBreak != null && !this.PageBreak.Instance.Disabled && !this.IgnorePageBreaks)
				{
					PageBreakLocation breakLocation = this.PageBreak.BreakLocation;
					if (breakLocation != PageBreakLocation.Start)
					{
						return breakLocation == PageBreakLocation.StartAndEnd;
					}
					return true;
				}
				return false;
			}
		}

		internal bool PageBreakAtEnd
		{
			get
			{
				if (this.PageBreak != null && !this.PageBreak.Instance.Disabled && !this.IgnorePageBreaks)
				{
					PageBreakLocation breakLocation = this.PageBreak.BreakLocation;
					if (breakLocation != PageBreakLocation.End)
					{
						return breakLocation == PageBreakLocation.StartAndEnd;
					}
					return true;
				}
				return false;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				if (this.m_source.RepeatedSibling)
				{
					return true;
				}
				if (this.m_source.Visibility == null)
				{
					return false;
				}
				if (this.m_source.Visibility.ToggleItem != null)
				{
					return true;
				}
				bool flag = false;
				if (this.m_source.Visibility.Hidden.IsExpression)
				{
					return this.m_source.Instance.Visibility.StartHidden;
				}
				return this.m_source.Visibility.Hidden.Value;
			}
		}

		internal virtual double SourceWidthInMM
		{
			get
			{
				return this.m_source.Width.ToMillimeters();
			}
		}

		protected PageItem(ReportItem source)
			: base(source)
		{
			this.m_source = source;
			if (this.m_source != null)
			{
				this.m_defLeftValue = this.m_source.Left.ToMillimeters();
			}
		}

		internal static List<int> GetNewList(List<int> source)
		{
			if (source != null && source.Count != 0)
			{
				List<int> list = new List<int>(source.Count);
				for (int i = 0; i < source.Count; i++)
				{
					list.Add(source[i]);
				}
				return list;
			}
			return null;
		}

		internal static int[] GetNewArray(int[] source)
		{
			if (source != null && source.Length != 0)
			{
				int[] array = new int[source.Length];
				Array.Copy(source, array, source.Length);
				return array;
			}
			return null;
		}

		internal static PageItem Create(ReportItem source, PageContext pageContext, bool tablixCellParent, bool createForRepeat)
		{
			if (source == null)
			{
				return null;
			}
			PageItem result = null;
			if (source is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
			{
				result = new TextBox((AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)source, pageContext, createForRepeat);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Line)
			{
				result = new Line((AspNetCore.ReportingServices.OnDemandReportRendering.Line)source, pageContext, createForRepeat);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Image)
			{
				result = new Image((AspNetCore.ReportingServices.OnDemandReportRendering.Image)source, pageContext, createForRepeat);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Chart)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Chart source2 = (AspNetCore.ReportingServices.OnDemandReportRendering.Chart)source;
				result = ((!PageItem.TransformToTextBox(source2)) ? ((PageItem)new Chart(source2, pageContext, createForRepeat)) : ((PageItem)new TextBox(source2, pageContext, createForRepeat)));
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel source3 = (AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)source;
				result = ((!PageItem.TransformToTextBox(source3)) ? ((PageItem)new GaugePanel(source3, pageContext, createForRepeat)) : ((PageItem)new TextBox(source3, pageContext, createForRepeat)));
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Map)
			{
				result = new Map((AspNetCore.ReportingServices.OnDemandReportRendering.Map)source, pageContext, createForRepeat);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
			{
				bool flag = false;
				AspNetCore.ReportingServices.OnDemandReportRendering.Tablix source4 = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)source;
				result = ((!PageItem.TransformToTextBox(source4, tablixCellParent, ref flag)) ? ((!flag) ? ((PageItem)new Tablix(source4, pageContext, createForRepeat)) : ((PageItem)new NoRowsItem(source4, pageContext, createForRepeat))) : new TextBox(source4, pageContext, createForRepeat));
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				result = new Rectangle((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)source, pageContext, createForRepeat);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)
			{
				bool flag2 = false;
				AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source5 = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)source;
				result = ((!PageItem.TransformToTextBox(source5, tablixCellParent, ref flag2)) ? ((!flag2) ? ((PageItem)new SubReport(source5, pageContext, createForRepeat)) : ((PageItem)new NoRowsItem(source5, pageContext, createForRepeat))) : new TextBox(source5, pageContext, createForRepeat));
			}
			return result;
		}

		internal static bool TransformToTextBox(DataRegion source)
		{
			bool result = false;
			DataRegionInstance dataRegionInstance = (DataRegionInstance)source.Instance;
			if (dataRegionInstance.NoRows && source.NoRowsMessage != null)
			{
				string text = null;
				text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : dataRegionInstance.NoRowsMessage);
				if (text != null)
				{
					result = true;
				}
			}
			return result;
		}

		internal static bool TransformToTextBox(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix source, bool tablixCellParent, ref bool noRows)
		{
			bool flag = false;
			TablixInstance tablixInstance = (TablixInstance)source.Instance;
			if (tablixInstance.NoRows)
			{
				noRows = true;
				if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : tablixInstance.NoRowsMessage);
					if (text != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (!source.HideStaticsIfNoRows)
					{
						noRows = false;
					}
					else if (tablixCellParent)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		internal static bool TransformToTextBox(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source, bool tablixCellParent, ref bool noRows)
		{
			bool result = false;
			SubReportInstance subReportInstance = (SubReportInstance)source.Instance;
			if (subReportInstance.NoRows)
			{
				noRows = true;
				if (tablixCellParent)
				{
					result = true;
				}
				else if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : subReportInstance.NoRowsMessage);
					if (text != null)
					{
						result = true;
					}
				}
			}
			else if (subReportInstance.ProcessedWithError)
			{
				result = true;
			}
			return result;
		}

		internal virtual void CreateItemRenderSizes(ItemSizes contentSize, PageContext pageContext, bool createForRepeat)
		{
			if (contentSize == null)
			{
				if (pageContext != null)
				{
					if (createForRepeat)
					{
						this.m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(this.m_itemPageSizes, false, false);
					}
					else
					{
						this.m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(this.m_itemPageSizes, false, false);
					}
				}
				if (this.m_itemRenderSizes == null)
				{
					this.m_itemRenderSizes = new ItemSizes(this.m_itemPageSizes);
				}
			}
			else
			{
				this.m_itemRenderSizes = contentSize;
			}
		}

		internal double ReserveSpaceForRepeatWith(RepeatWithItem[] repeatWithItems, PageContext pageContext)
		{
			if (repeatWithItems == null)
			{
				return 0.0;
			}
			DataRegion dataRegion = this.m_source as DataRegion;
			if (dataRegion == null)
			{
				return 0.0;
			}
			int[] repeatSiblings = dataRegion.GetRepeatSiblings();
			if (repeatSiblings != null && repeatSiblings.Length != 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				RepeatWithItem repeatWithItem = null;
				for (int i = 0; i < repeatSiblings.Length; i++)
				{
					repeatWithItem = repeatWithItems[repeatSiblings[i]];
					if (repeatWithItem != null)
					{
						if (repeatWithItem.RelativeTop < 0.0)
						{
							num = Math.Max(num, 0.0 - repeatWithItem.RelativeTop);
						}
						if (repeatWithItem.RelativeBottom > 0.0)
						{
							num2 = Math.Max(num2, repeatWithItem.RelativeBottom);
						}
					}
				}
				if (num + num2 >= pageContext.PageHeight)
				{
					if (num >= pageContext.PageHeight)
					{
						return 0.0;
					}
					num2 = 0.0;
				}
				return num + num2;
			}
			return 0.0;
		}

		internal void WritePageItemRenderSizes(BinaryWriter spbifWriter)
		{
			this.WritePageItemRenderSizes(spbifWriter, this.m_offset);
		}

		internal void WritePageItemRenderSizes(BinaryWriter spbifWriter, long offset)
		{
			spbifWriter.Write((float)this.m_itemRenderSizes.Left);
			spbifWriter.Write((float)this.m_itemRenderSizes.Top);
			spbifWriter.Write((float)this.m_itemRenderSizes.Width);
			spbifWriter.Write((float)this.m_itemRenderSizes.Height);
			if (this.m_source == null)
			{
				spbifWriter.Write(0);
			}
			else
			{
				spbifWriter.Write(this.m_source.ZIndex);
			}
			spbifWriter.Write(this.m_rplItemState);
			spbifWriter.Write(offset);
		}

		internal RPLItemMeasurement WritePageItemRenderSizes()
		{
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Left = (float)this.m_itemRenderSizes.Left;
			rPLItemMeasurement.Top = (float)this.m_itemRenderSizes.Top;
			rPLItemMeasurement.Width = (float)this.m_itemRenderSizes.Width;
			rPLItemMeasurement.Height = (float)this.m_itemRenderSizes.Height;
			if (this.m_source != null)
			{
				rPLItemMeasurement.ZIndex = this.m_source.ZIndex;
			}
			rPLItemMeasurement.State = this.m_rplItemState;
			rPLItemMeasurement.Element = this.m_rplElement;
			return rPLItemMeasurement;
		}

		internal void MergeRepeatSiblings(ref List<int> repeatedSiblings)
		{
			if (this.ItemState != State.OnPage && this.ItemState != State.OnPagePBEnd && this.ItemState != State.SpanPages)
			{
				return;
			}
			DataRegion dataRegion = this.m_source as DataRegion;
			if (dataRegion != null)
			{
				int[] repeatSiblings = dataRegion.GetRepeatSiblings();
				if (repeatSiblings != null && repeatSiblings.Length != 0)
				{
					if (repeatedSiblings == null)
					{
						repeatedSiblings = new List<int>(repeatSiblings);
					}
					else
					{
						int i = 0;
						for (int j = 0; j < repeatSiblings.Length; j++)
						{
							for (; repeatedSiblings[i] <= repeatSiblings[j]; i++)
							{
							}
							if (i < repeatedSiblings.Count)
							{
								repeatedSiblings.Insert(i, repeatSiblings[j]);
							}
							else
							{
								repeatedSiblings.Add(repeatSiblings[j]);
							}
							i++;
						}
					}
				}
			}
		}

		internal override void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[this.SourceID];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(this.SourceID, offset);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(this.SourceID);
			if (this.m_source.Name != null)
			{
				spbifWriter.Write((byte)2);
				spbifWriter.Write(this.m_source.Name);
			}
			if (!this.m_source.ToolTip.IsExpression && this.m_source.ToolTip.Value != null)
			{
				spbifWriter.Write((byte)5);
				spbifWriter.Write(this.m_source.ToolTip.Value);
			}
			if (!this.m_source.Bookmark.IsExpression && this.m_source.Bookmark.Value != null)
			{
				spbifWriter.Write((byte)4);
				spbifWriter.Write(this.m_source.Bookmark.Value);
			}
			if (!this.m_source.DocumentMapLabel.IsExpression && this.m_source.DocumentMapLabel.Value != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(this.m_source.DocumentMapLabel.Value);
			}
			if (this.m_source.Visibility != null && this.m_source.Visibility.ToggleItem != null)
			{
				spbifWriter.Write((byte)8);
				spbifWriter.Write(this.m_source.Visibility.ToggleItem);
			}
			this.WriteCustomSharedItemProps(spbifWriter, rplWriter, pageContext);
			this.WriteSharedStyle(spbifWriter, null, pageContext, 6);
			spbifWriter.Write((byte)255);
		}

		internal override void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.m_source.Instance.UniqueName);
			if (this.m_source.ToolTip.IsExpression && this.m_source.Instance.ToolTip != null)
			{
				spbifWriter.Write((byte)5);
				spbifWriter.Write(this.m_source.Instance.ToolTip);
			}
			if (this.m_source.Bookmark.IsExpression && this.m_source.Instance.Bookmark != null)
			{
				spbifWriter.Write((byte)4);
				spbifWriter.Write(this.m_source.Instance.Bookmark);
			}
			if (this.m_source.DocumentMapLabel.IsExpression && this.m_source.Instance.DocumentMapLabel != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(this.m_source.Instance.DocumentMapLabel);
			}
			this.WriteCustomNonSharedItemProps(spbifWriter, rplWriter, pageContext);
			this.WriteNonSharedStyle(spbifWriter, null, null, pageContext, 6, null);
			spbifWriter.Write((byte)255);
		}

		internal override void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[this.SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLElementPropsDef)obj;
					return;
				}
			}
			RPLItemProps rPLItemProps = elemProps as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(this.SourceID, rPLItemPropsDef);
			rPLItemPropsDef.ID = this.SourceID;
			rPLItemPropsDef.Name = this.m_source.Name;
			if (!this.m_source.ToolTip.IsExpression)
			{
				rPLItemPropsDef.ToolTip = this.m_source.ToolTip.Value;
			}
			if (!this.m_source.Bookmark.IsExpression)
			{
				rPLItemPropsDef.Bookmark = this.m_source.Bookmark.Value;
			}
			if (!this.m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemPropsDef.Label = this.m_source.DocumentMapLabel.Value;
			}
			if (this.m_source.Visibility != null)
			{
				rPLItemPropsDef.ToggleItem = this.m_source.Visibility.ToggleItem;
			}
			this.WriteCustomSharedItemProps(rPLItemPropsDef, rplWriter, pageContext);
			rPLItemPropsDef.SharedStyle = this.WriteSharedStyle(null, pageContext);
		}

		internal override void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			elemProps.UniqueName = this.m_source.Instance.UniqueName;
			RPLItemProps rPLItemProps = elemProps as RPLItemProps;
			if (this.m_source.ToolTip.IsExpression)
			{
				rPLItemProps.ToolTip = this.m_source.Instance.ToolTip;
			}
			if (this.m_source.Bookmark.IsExpression)
			{
				rPLItemProps.Bookmark = this.m_source.Instance.Bookmark;
			}
			if (this.m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemProps.Label = this.m_source.Instance.DocumentMapLabel;
			}
			this.WriteCustomNonSharedItemProps(elemProps, rplWriter, pageContext);
			elemProps.NonSharedStyle = this.WriteNonSharedStyle(null, null, pageContext, null);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal abstract bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentHeight, Interactivity interactivity);

		internal bool HiddenForOverlap(PageContext pageContext)
		{
			if (this.m_source.Visibility == null)
			{
				return false;
			}
			if (pageContext.AddToggledItems && this.m_source.Visibility.ToggleItem != null)
			{
				return false;
			}
			bool flag = false;
			if (this.m_source.Visibility.Hidden.IsExpression)
			{
				return this.m_source.Instance.Visibility.CurrentlyHidden;
			}
			return this.m_source.Visibility.Hidden.Value;
		}

		internal bool ResolveItemHiddenState(RPLWriter rplWriter, Interactivity interactivity, PageContext pageContext, bool createForRepeat, ref ItemSizes contentSize)
		{
			if (this.m_source.Visibility == null)
			{
				if (interactivity != null)
				{
					interactivity.RegisterItem(this, pageContext);
				}
				return false;
			}
			Visibility visibility = this.m_source.Visibility;
			bool result = false;
			if (visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				bool flag = false;
				if (visibility.ToggleItem != null)
				{
					bool flag2 = false;
					flag2 = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : this.m_source.Instance.Visibility.StartHidden);
					if (pageContext.AddToggledItems)
					{
						this.m_rplItemState |= 16;
						if (this.m_source.Instance.Visibility.CurrentlyHidden)
						{
							this.m_rplItemState |= 32;
						}
					}
					else
					{
						flag = this.m_source.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						result = true;
						this.m_itemState = State.OnPageHidden;
						if (flag2)
						{
							this.m_itemPageSizes.AdjustHeightTo(0.0);
							this.m_itemPageSizes.AdjustWidthTo(0.0);
						}
						else if (rplWriter != null)
						{
							if (pageContext != null)
							{
								PaddItemSizes paddItemSizes = this.m_itemPageSizes as PaddItemSizes;
								if (paddItemSizes != null)
								{
									if (createForRepeat)
									{
										this.m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(paddItemSizes, true, false);
									}
									else
									{
										this.m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(paddItemSizes, true, false);
									}
								}
								else if (createForRepeat)
								{
									this.m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(this.m_itemPageSizes, false, false);
								}
								else
								{
									this.m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(this.m_itemPageSizes, false, false);
								}
							}
							if (this.m_itemRenderSizes == null)
							{
								this.m_itemRenderSizes = new ItemSizes(this.m_itemPageSizes);
							}
							this.m_itemRenderSizes.AdjustHeightTo(0.0);
							this.m_itemRenderSizes.AdjustWidthTo(0.0);
						}
					}
					else
					{
						double height = this.m_itemPageSizes.Height;
						if (flag2)
						{
							this.m_itemPageSizes.AdjustHeightTo(0.0);
						}
						PaddItemSizes paddItemSizes2 = this.m_itemPageSizes as PaddItemSizes;
						if (paddItemSizes2 != null)
						{
							if (pageContext != null)
							{
								if (createForRepeat)
								{
									contentSize = pageContext.GetSharedRenderFromRepeatItemSizesElement(paddItemSizes2, true, true);
								}
								else
								{
									contentSize = pageContext.GetSharedRenderItemSizesElement(paddItemSizes2, true, true);
								}
							}
							if (contentSize == null)
							{
								contentSize = new PaddItemSizes(paddItemSizes2);
							}
						}
						else
						{
							if (pageContext != null)
							{
								if (createForRepeat)
								{
									contentSize = pageContext.GetSharedRenderFromRepeatItemSizesElement(this.m_itemPageSizes, false, false);
								}
								else
								{
									contentSize = pageContext.GetSharedRenderItemSizesElement(this.m_itemPageSizes, false, false);
								}
							}
							if (contentSize == null)
							{
								contentSize = new ItemSizes(this.m_itemPageSizes);
							}
						}
						contentSize.AdjustHeightTo(height);
					}
				}
				else if (this.m_source.Instance.Visibility.CurrentlyHidden)
				{
					this.m_itemPageSizes.AdjustHeightTo(0.0);
					this.m_itemPageSizes.AdjustWidthTo(0.0);
					result = true;
					this.m_itemState = State.OnPageHidden;
				}
			}
			else if (visibility.HiddenState == SharedHiddenState.Always)
			{
				result = true;
				this.m_itemState = State.OnPageHidden;
			}
			if (interactivity != null)
			{
				interactivity.RegisterItem(this, pageContext);
			}
			return result;
		}

		internal virtual bool HitsCurrentPage(PageContext pageContext, double parentTopInPage)
		{
			if (pageContext.CancelPage)
			{
				this.m_itemState = State.Below;
				return false;
			}
			if (pageContext.FullOnPage)
			{
				this.m_itemState = State.OnPage;
				if (pageContext.CancelMode)
				{
					RoundedDouble pageHeight = new RoundedDouble(parentTopInPage + this.m_itemPageSizes.Top);
					pageContext.CheckPageSize(pageHeight);
				}
				return true;
			}
			if (this.m_itemState == State.Unknow)
			{
				RoundedDouble roundedDouble = new RoundedDouble(parentTopInPage + this.m_itemPageSizes.Top);
				pageContext.CheckPageSize(roundedDouble);
				if (!pageContext.StretchPage && roundedDouble >= pageContext.PageHeight)
				{
					this.m_itemState = State.Below;
					return false;
				}
				if (pageContext.TracingEnabled)
				{
					if (!pageContext.IgnorePageBreaks)
					{
						this.TracePageBreakIgnoredBecauseDisabled(pageContext.PageNumber);
					}
					else
					{
						this.TracePageBreakAtStartIgnored(pageContext);
					}
				}
				if (!pageContext.IgnorePageBreaks && this.PageBreakAtStart)
				{
					if (roundedDouble > 0.0)
					{
						this.m_itemState = State.TopNextPage;
						pageContext.RegisterPageBreak(new PageBreakInfo(this.PageBreak, this.ItemName));
						return false;
					}
					if (pageContext.TracingEnabled)
					{
						this.TracePageBreakIgnoredAtTop(pageContext.PageNumber);
					}
				}
				this.m_itemState = State.OnPage;
				return true;
			}
			if (this.m_itemState != State.OnPage && this.m_itemState != State.OnPagePBEnd && this.m_itemState != State.SpanPages)
			{
				return false;
			}
			return true;
		}

		internal bool HasItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			if (this.m_pageItemsAbove == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < this.m_pageItemsAbove.Count; i++)
			{
				if (siblings[this.m_pageItemsAbove[i]] == null)
				{
					if (repeatWithItems == null || repeatWithItems[this.m_pageItemsAbove[i]] == null)
					{
						this.m_pageItemsAbove.RemoveAt(i);
						i--;
					}
					else
					{
						num++;
					}
				}
			}
			if (this.m_pageItemsAbove.Count == 0)
			{
				this.m_pageItemsAbove = null;
				return false;
			}
			if (this.m_pageItemsAbove.Count == num)
			{
				return false;
			}
			return true;
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			this.AdjustOriginFromItemsAbove(siblings, repeatWithItems, false);
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems, bool adjustForRender)
		{
			if (this.m_pageItemsAbove != null)
			{
				double num = -1.7976931348623157E+308;
				double num2 = 1.7976931348623157E+308;
				double num3 = 1.7976931348623157E+308;
				double num4 = 1.7976931348623157E+308;
				double num5 = 0.0;
				PageItem pageItem = null;
				ItemSizes itemSizes = null;
				for (int i = 0; i < this.m_pageItemsAbove.Count; i++)
				{
					pageItem = siblings[this.m_pageItemsAbove[i]];
					if (pageItem == null)
					{
						if (repeatWithItems == null || repeatWithItems[this.m_pageItemsAbove[i]] == null)
						{
							this.m_pageItemsAbove.RemoveAt(i);
							i--;
						}
						continue;
					}
					if (pageItem.ItemState != 0 && this.m_itemState == State.Unknow && pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
					{
						this.m_itemState = State.Below;
					}
					if (adjustForRender)
					{
						if (pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
						{
							itemSizes = pageItem.ItemRenderSizes;
							num5 = this.ItemRenderSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
							goto IL_0125;
						}
						continue;
					}
					itemSizes = pageItem.ItemPageSizes;
					num5 = this.ItemPageSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
					goto IL_0125;
					IL_0125:
					if ((RoundedDouble)itemSizes.DeltaY < num3)
					{
						num3 = itemSizes.DeltaY;
					}
					if ((RoundedDouble)itemSizes.DeltaY > num)
					{
						num = itemSizes.DeltaY;
						num4 = num5;
					}
					else if ((RoundedDouble)itemSizes.DeltaY == num)
					{
						num4 = Math.Min(num4, num5);
					}
					num2 = Math.Min(num2, num5);
				}
				if (num != -1.7976931348623157E+308)
				{
					if (num2 < 0.01 || num2 == 1.7976931348623157E+308)
					{
						num2 = 0.0;
					}
					if (num4 < 0.01 || num4 == 1.7976931348623157E+308)
					{
						num4 = 0.0;
					}
					num += num2 - num4;
					if (num < num3)
					{
						num = num3;
					}
					if (adjustForRender)
					{
						this.ItemRenderSizes.Top += num;
						this.ItemRenderSizes.DeltaY += num;
					}
					else
					{
						this.ItemPageSizes.Top += num;
						this.ItemPageSizes.DeltaY += num;
					}
				}
				if (this.m_pageItemsAbove.Count == 0)
				{
					this.m_pageItemsAbove = null;
				}
			}
		}

		internal void AdjustOriginFromItemsAtLeft(PageItem[] siblings, bool adjustForRender)
		{
			if (this.m_pageItemsLeft != null)
			{
				double num = -1.7976931348623157E+308;
				double num2 = 1.7976931348623157E+308;
				double num3 = 1.7976931348623157E+308;
				double num4 = 1.7976931348623157E+308;
				double num5 = 0.0;
				PageItem pageItem = null;
				ItemSizes itemSizes = null;
				for (int i = 0; i < this.m_pageItemsLeft.Count; i++)
				{
					pageItem = siblings[this.m_pageItemsLeft[i]];
					if (pageItem == null)
					{
						this.m_pageItemsLeft.RemoveAt(i);
						i--;
					}
					else if (pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
					{
						if (adjustForRender)
						{
							itemSizes = pageItem.ItemRenderSizes;
							num5 = this.ItemRenderSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
						}
						else
						{
							itemSizes = pageItem.ItemPageSizes;
							num5 = this.ItemPageSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
						}
						if ((RoundedDouble)itemSizes.DeltaX < num4)
						{
							num4 = itemSizes.DeltaX;
						}
						if ((RoundedDouble)itemSizes.DeltaX > num)
						{
							num = itemSizes.DeltaX;
							num3 = num5;
						}
						else if ((RoundedDouble)itemSizes.DeltaX == num)
						{
							num3 = Math.Min(num3, num5);
						}
						num2 = Math.Min(num2, num5);
					}
				}
				if (num != -1.7976931348623157E+308)
				{
					if (num2 < 0.01 || num2 == 1.7976931348623157E+308)
					{
						num2 = 0.0;
					}
					if (num3 < 0.01 || num3 == 1.7976931348623157E+308)
					{
						num3 = 0.0;
					}
					num += num2 - num3;
					if (num < num4)
					{
						num = num4;
					}
					if (adjustForRender)
					{
						this.ItemRenderSizes.Left += num;
						this.ItemRenderSizes.DeltaX += num;
					}
					else
					{
						this.ItemPageSizes.Left += num;
						this.ItemPageSizes.DeltaX += num;
					}
				}
			}
		}

		internal virtual void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
		}

		internal void AdjustOriginFromRepeatItems(PageItem[] siblings)
		{
			if (this.m_pageItemsAbove != null)
			{
				double num = 1.7976931348623157E+308;
				double num2 = 1.7976931348623157E+308;
				double num3 = 0.0;
				PageItem pageItem = null;
				ItemSizes itemSizes = null;
				for (int i = 0; i < this.m_pageItemsAbove.Count; i++)
				{
					pageItem = siblings[this.m_pageItemsAbove[i]];
					if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
					{
						itemSizes = pageItem.ItemRenderSizes;
						num3 = this.ItemRenderSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
						if (num3 >= 0.0)
						{
							num = Math.Min(num, num3);
						}
						else
						{
							num2 = Math.Min(num2, num3);
						}
					}
				}
				if (num2 != 1.7976931348623157E+308)
				{
					if (num != 1.7976931348623157E+308)
					{
						num2 -= num;
					}
					this.ItemRenderSizes.Top -= num2;
					this.ItemRenderSizes.DeltaY -= num2;
				}
			}
		}

		internal virtual int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			return 0;
		}

		internal void UpdateSizes(double topDelta, PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			this.m_itemPageSizes.UpdateSizes(topDelta, this, siblings, repeatWithItems);
			this.m_itemRenderSizes = null;
			this.m_offset = 0L;
			this.m_rplItemState = 0;
		}

		internal virtual void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				if (itemHelper.ItemPageSizes != null)
				{
					this.m_itemPageSizes = itemHelper.ItemPageSizes.GetNewItem();
				}
				this.m_itemState = itemHelper.State;
				this.m_pageItemsAbove = PageItem.GetNewList(itemHelper.PageItemsAbove);
				this.m_pageItemsLeft = PageItem.GetNewList(itemHelper.PageItemsLeft);
				this.m_defLeftValue = itemHelper.DefLeftValue;
			}
		}

		internal virtual void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)1);
				this.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal virtual PageItemHelper WritePaginationInfo()
		{
			return null;
		}

		internal virtual void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				if (this.m_itemPageSizes != null)
				{
					this.m_itemPageSizes.WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write((byte)3);
				reportPageInfo.Write((byte)this.m_itemState);
				reportPageInfo.Write((byte)21);
				reportPageInfo.Write(this.m_defLeftValue);
				if (this.m_pageItemsAbove != null && this.m_pageItemsAbove.Count > 0)
				{
					reportPageInfo.Write((byte)4);
					reportPageInfo.Write(this.m_pageItemsAbove.Count);
					for (int i = 0; i < this.m_pageItemsAbove.Count; i++)
					{
						reportPageInfo.Write(this.m_pageItemsAbove[i]);
					}
				}
				if (this.m_pageItemsLeft != null && this.m_pageItemsLeft.Count > 0)
				{
					reportPageInfo.Write((byte)5);
					reportPageInfo.Write(this.m_pageItemsLeft.Count);
					for (int j = 0; j < this.m_pageItemsLeft.Count; j++)
					{
						reportPageInfo.Write(this.m_pageItemsLeft[j]);
					}
				}
			}
		}

		internal virtual void WritePaginationInfoProperties(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				if (this.m_itemPageSizes != null)
				{
					itemHelper.ItemPageSizes = this.m_itemPageSizes.WritePaginationInfo();
				}
				itemHelper.State = this.m_itemState;
				itemHelper.DefLeftValue = this.m_defLeftValue;
				itemHelper.PageItemsAbove = PageItem.GetNewList(this.m_pageItemsAbove);
				itemHelper.PageItemsLeft = PageItem.GetNewList(this.m_pageItemsLeft);
			}
		}

		protected void TracePageBreakAtStartIgnored(PageContext context)
		{
			if (this.PageBreak != null && !this.PageBreak.Instance.Disabled)
			{
				if (this.PageBreak.BreakLocation != PageBreakLocation.Start && this.PageBreak.BreakLocation != PageBreakLocation.StartAndEnd)
				{
					return;
				}
				this.TracePageBreakIgnored(context);
			}
		}

		protected void TracePageBreakAtEndIgnored(PageContext context)
		{
			if (this.PageBreak != null && !this.PageBreak.Instance.Disabled)
			{
				if (this.PageBreak.BreakLocation != PageBreakLocation.End && this.PageBreak.BreakLocation != PageBreakLocation.StartAndEnd)
				{
					return;
				}
				this.TracePageBreakIgnored(context);
			}
		}

		private void TracePageBreakIgnored(PageContext context)
		{
			if (!context.Common.RegisteredPBIgnored.Contains(this.SourceID))
			{
				string str = "PR-DIAG [Page {0}] Page break on '{1}' ignored";
				switch (context.IgnorePBReason)
				{
				default:
					return;
				case PageContext.IgnorePBReasonFlag.Repeated:
					return;
				case PageContext.IgnorePBReasonFlag.TablixParent:
					str += " - inside TablixCell";
					break;
				case PageContext.IgnorePBReasonFlag.Toggled:
					str += " - part of toggleable region";
					break;
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, str, context.PageNumber, this.ItemName);
				context.Common.RegisteredPBIgnored.Add(this.SourceID, null);
			}
		}

		protected void TracePageGrownOnKeepTogetherItem(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Item '{1}' kept together - Explicit - Page grown", pageNumber, this.ItemName);
		}

		private void TracePageBreakIgnoredBecauseDisabled(int pageNumber)
		{
			if (this.PageBreak != null && this.PageBreak.Instance.Disabled)
			{
				if (this.PageBreak.BreakLocation != PageBreakLocation.Start && this.PageBreak.BreakLocation != PageBreakLocation.StartAndEnd)
				{
					return;
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – Disable is True", pageNumber, this.ItemName);
			}
		}

		private void TracePageBreakIgnoredAtTop(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – at top of page", pageNumber, this.ItemName);
		}
	}
}
