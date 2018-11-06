using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class PageHeadFoot : PageItemContainer
	{
		private new PageSection m_source;

		internal override string SourceUniqueName
		{
			get
			{
				return this.m_source.InstanceUniqueName;
			}
		}

		internal override string SourceID
		{
			get
			{
				return this.m_source.ID;
			}
		}

		internal override ReportElement OriginalSource
		{
			get
			{
				return this.m_source;
			}
		}

		internal PageHeadFoot(PageSection source, ReportSize width, PageContext pageContext)
			: base(null, false)
		{
			if (pageContext != null)
			{
				base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(width, source.Height, source.ID, true);
			}
			else
			{
				base.m_itemPageSizes = new PaddItemSizes(width, source.Height, source.ID);
			}
			this.m_source = source;
		}

		internal void CalculateItem(RPLWriter rplWriter, PageContext pageContext, bool isHeader, Interactivity interactivity, bool native)
		{
			this.WriteStartItemToStream(rplWriter, isHeader, pageContext, native);
			base.CreateChildren(this.m_source.ReportItemCollection, pageContext, base.m_itemPageSizes.Width, base.m_itemPageSizes.Height);
			double num = 0.0;
			if (base.m_children != null)
			{
				double num2 = 0.0;
				PageItem pageItem = null;
				for (int i = 0; i < base.m_children.Length; i++)
				{
					pageItem = base.m_children[i];
					if (pageItem != null)
					{
						pageItem.CalculatePage(rplWriter, null, pageContext, base.m_children, null, 0.0, ref num, interactivity);
						num2 = Math.Max(num2, pageItem.ItemPageSizes.Bottom + base.m_itemPageSizes.PaddingBottom);
					}
				}
				base.ConsumeWhitespaceVertical(base.m_itemPageSizes, num2, pageContext);
			}
			if (pageContext.CancelPage)
			{
				base.m_itemState = State.Below;
				base.m_children = null;
				base.m_rplElement = null;
			}
			else
			{
				if (rplWriter != null)
				{
					this.CreateItemRenderSizes(null, pageContext, false);
					PageItem[] childrenOnPage = null;
					int itemsOnPage = base.CalculateRenderSizes(rplWriter, pageContext, interactivity, (List<int>)null, out childrenOnPage);
					this.WriteEndItemToStream(rplWriter, itemsOnPage, childrenOnPage);
				}
				base.m_indexesLeftToRight = null;
				base.m_children = null;
			}
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, bool isHeader, PageContext pageContext, bool native)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					bool flag = false;
					if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						if (native)
						{
							if (isHeader)
							{
								binaryWriter.Write((byte)4);
							}
							else
							{
								binaryWriter.Write((byte)5);
							}
						}
						else
						{
							binaryWriter.Write((byte)10);
							flag = true;
						}
					}
					else if (isHeader)
					{
						binaryWriter.Write((byte)4);
					}
					else
					{
						binaryWriter.Write((byte)5);
					}
					binaryWriter.Write((byte)15);
					binaryWriter.Write((byte)0);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(this.SourceID);
					if (!flag)
					{
						binaryWriter.Write((byte)44);
						binaryWriter.Write(this.m_source.PrintOnFirstPage);
						if (pageContext.VersionPicker != 0 && pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation)
						{
							binaryWriter.Write((byte)47);
							binaryWriter.Write(this.m_source.PrintBetweenSections);
						}
					}
					Style style = this.m_source.Style;
					if (style != null)
					{
						this.WriteSharedStyle(binaryWriter, style, pageContext, 6);
						binaryWriter.Write((byte)255);
						binaryWriter.Write((byte)1);
						binaryWriter.Write((byte)0);
						binaryWriter.Write(this.SourceUniqueName);
						StyleInstance styleInstance = base.GetStyleInstance(this.m_source, null);
						if (styleInstance != null)
						{
							this.WriteNonSharedStyle(binaryWriter, style, styleInstance, pageContext, 6, null);
						}
					}
					else
					{
						binaryWriter.Write((byte)255);
						binaryWriter.Write((byte)1);
						binaryWriter.Write((byte)0);
						binaryWriter.Write(this.SourceUniqueName);
					}
					binaryWriter.Write((byte)255);
					binaryWriter.Write((byte)255);
				}
				else
				{
					base.m_rplElement = new RPLHeaderFooter();
					base.m_rplElement.ElementProps.Definition.ID = this.SourceID;
					RPLHeaderFooterPropsDef rPLHeaderFooterPropsDef = base.m_rplElement.ElementProps.Definition as RPLHeaderFooterPropsDef;
					rPLHeaderFooterPropsDef.PrintOnFirstPage = this.m_source.PrintOnFirstPage;
					rPLHeaderFooterPropsDef.PrintBetweenSections = this.m_source.PrintBetweenSections;
					base.m_rplElement.ElementProps.UniqueName = this.SourceUniqueName;
					Style style2 = this.m_source.Style;
					if (style2 != null)
					{
						base.m_rplElement.ElementProps.Definition.SharedStyle = this.WriteSharedStyle(style2, pageContext);
						StyleInstance styleInstance2 = base.GetStyleInstance(this.m_source, null);
						if (styleInstance2 != null)
						{
							base.m_rplElement.ElementProps.NonSharedStyle = this.WriteNonSharedStyle(style2, styleInstance2, pageContext, null);
						}
					}
				}
			}
		}

		internal override void WriteEndItemToStream(RPLWriter rplWriter, int itemsOnPage, PageItem[] childrenOnPage)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				int num = (childrenOnPage != null) ? childrenOnPage.Length : 0;
				long value = 0L;
				RPLItemMeasurement[] array = null;
				if (binaryWriter != null)
				{
					value = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(base.m_offset);
					binaryWriter.Write(itemsOnPage);
				}
				else if (itemsOnPage > 0)
				{
					array = new RPLItemMeasurement[itemsOnPage];
					((RPLContainer)base.m_rplElement).Children = array;
				}
				PageItem pageItem = null;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					pageItem = childrenOnPage[i];
					if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
					{
						if (pageItem.ItemState != State.OnPageHidden && !(pageItem is NoRowsItem))
						{
							if (binaryWriter != null)
							{
								pageItem.WritePageItemRenderSizes(binaryWriter);
							}
							else
							{
								array[num2] = pageItem.WritePageItemRenderSizes();
								num2++;
							}
						}
						childrenOnPage[i] = null;
						base.m_children[i] = null;
					}
				}
				if (binaryWriter != null)
				{
					base.m_offset = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(value);
					binaryWriter.Write((byte)255);
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(spbifWriter, style, true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(rplStyleProps, style, true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(spbifWriter, styleDef, false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(rplStyleProps, styleDef, false, pageContext);
				break;
			}
		}
	}
}
