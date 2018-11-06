using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemContainer : PageItem, IComparer
	{
		protected PageItem[] m_children;

		protected int[] m_indexesTopToBottom;

		protected RepeatWithItem[] m_repeatWithItems;

		protected int[] m_indexesLeftToRight;

		protected bool m_itemsCreated;

		protected double m_prevPageEnd;

		private PageItem m_rightEdgeItem;

		internal PageItemContainer(ReportItem source, bool createForRepeat)
			: base(source)
		{
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
			if (this.m_children[num] != null && this.m_children[num2] != null)
			{
				if (this.m_children[num].ItemPageSizes.Left == this.m_children[num2].ItemPageSizes.Left)
				{
					return 0;
				}
				if (this.m_children[num].ItemPageSizes.Left < this.m_children[num2].ItemPageSizes.Left)
				{
					return -1;
				}
				return 1;
			}
			return 0;
		}

		private void VerticalDependency(PageItem[] children)
		{
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < children.Length; i++)
			{
				pageItem = children[i];
				if (pageItem != null)
				{
					itemSizes = pageItem.ItemPageSizes;
					roundedDouble2.Value = itemSizes.Bottom;
					roundedDouble.Value = -1.0;
					for (int j = i + 1; j < children.Length; j++)
					{
						pageItem2 = children[j];
						if (pageItem2 != null)
						{
							itemSizes2 = pageItem2.ItemPageSizes;
							if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Top)
							{
								break;
							}
							if (roundedDouble2 <= itemSizes2.Top)
							{
								if (pageItem2.PageItemsAbove == null)
								{
									pageItem2.PageItemsAbove = new List<int>();
								}
								pageItem2.PageItemsAbove.Add(i);
								if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Bottom)
								{
									roundedDouble.Value = itemSizes2.Bottom;
								}
							}
						}
					}
				}
			}
		}

		private void HorizontalDependecy()
		{
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			int num = 0;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			this.m_rightEdgeItem.PageItemsLeft = null;
			for (int i = 0; i < this.m_children.Length; i++)
			{
				pageItem = this.m_children[this.m_indexesLeftToRight[i]];
				if (pageItem != null)
				{
					itemSizes = pageItem.ItemPageSizes;
					roundedDouble.Value = -1.0;
					roundedDouble2.Value = itemSizes.Right;
					for (num = i + 1; num < this.m_children.Length; num++)
					{
						pageItem2 = this.m_children[this.m_indexesLeftToRight[num]];
						if (pageItem2 != null)
						{
							itemSizes2 = pageItem2.ItemPageSizes;
							if (roundedDouble >= 0.0 && roundedDouble <= itemSizes2.Left)
							{
								break;
							}
							if (roundedDouble2 <= itemSizes2.Left)
							{
								if (pageItem2.PageItemsLeft == null)
								{
									pageItem2.PageItemsLeft = new List<int>();
								}
								pageItem2.PageItemsLeft.Add(this.m_indexesLeftToRight[i]);
								if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Right)
								{
									roundedDouble.Value = itemSizes2.Right;
								}
							}
						}
					}
					if (num == this.m_children.Length)
					{
						pageItem2 = this.m_rightEdgeItem;
						itemSizes2 = pageItem2.ItemPageSizes;
						if ((roundedDouble < 0.0 || roundedDouble > itemSizes2.Left) && roundedDouble2 <= itemSizes2.Left)
						{
							if (pageItem2.PageItemsLeft == null)
							{
								pageItem2.PageItemsLeft = new List<int>();
							}
							pageItem2.PageItemsLeft.Add(this.m_indexesLeftToRight[i]);
							if (roundedDouble < 0.0 || roundedDouble > itemSizes2.Right)
							{
								roundedDouble.Value = itemSizes2.Right;
							}
						}
					}
				}
			}
		}

		private bool IsStaticTablix(AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix, TablixMember colMemberParent)
		{
			if (tablix == null && colMemberParent == null)
			{
				return true;
			}
			TablixMember tablixMember = null;
			TablixMemberCollection tablixMemberCollection = null;
			tablixMemberCollection = ((colMemberParent != null) ? colMemberParent.Children : tablix.ColumnHierarchy.MemberCollection);
			if (tablixMemberCollection == null)
			{
				return true;
			}
			for (int i = 0; i < tablixMemberCollection.Count; i++)
			{
				tablixMember = ((ReportElementCollectionBase<TablixMember>)tablixMemberCollection)[i];
				if (!tablixMember.IsStatic)
				{
					return false;
				}
				if (tablixMember.Visibility != null && tablixMember.Visibility.HiddenState == SharedHiddenState.Sometimes)
				{
					return false;
				}
				if (!this.IsStaticTablix(null, tablixMember))
				{
					return false;
				}
			}
			return true;
		}

		internal bool CreateChildren(ReportItemCollection childrenDef, PageContext pageContext, double parentWidth, double parentHeight)
		{
			return this.CreateChildren(childrenDef, pageContext, parentWidth, parentHeight, false);
		}

		internal bool CreateChildren(ReportItemCollection childrenDef, PageContext pageContext, double parentWidth, double parentHeight, bool isSimple)
		{
			if (childrenDef != null && childrenDef.Count != 0)
			{
				double num = 0.0;
				double num2 = 0.0;
				bool flag = isSimple;
				ReportItem reportItem = null;
				this.m_children = new PageItem[childrenDef.Count];
				this.m_indexesLeftToRight = new int[childrenDef.Count];
				for (int i = 0; i < childrenDef.Count; i++)
				{
					this.m_indexesLeftToRight[i] = i;
					reportItem = ((ReportElementCollectionBase<ReportItem>)childrenDef)[i];
					this.m_children[i] = PageItem.Create(reportItem, pageContext, false, false);
					if (flag)
					{
						if (reportItem.Visibility != null && reportItem.Visibility.HiddenState == SharedHiddenState.Sometimes)
						{
							flag = false;
						}
						else
						{
							AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = reportItem as AspNetCore.ReportingServices.OnDemandReportRendering.Tablix;
							flag = this.IsStaticTablix(tablix, null);
							if (flag)
							{
								bool flag2 = false;
								this.VerifyNoRows(reportItem, ref flag2);
								if (flag2)
								{
									flag = false;
								}
							}
						}
					}
					if (reportItem.RepeatedSibling)
					{
						if (this.m_repeatWithItems == null)
						{
							this.m_repeatWithItems = new RepeatWithItem[childrenDef.Count];
						}
						this.m_repeatWithItems[i] = new RepeatWithItem(PageItem.Create(reportItem, pageContext, false, true), pageContext);
					}
					num = Math.Max(num, this.m_children[i].ItemPageSizes.Right);
					num2 = Math.Max(num2, this.m_children[i].ItemPageSizes.Bottom);
				}
				this.m_rightEdgeItem = new EdgePageItem(0.0, Math.Max(parentWidth, num), this.SourceID, pageContext);
				num = Math.Max(0.0, parentWidth - num);
				num2 = Math.Max(0.0, parentHeight - num2);
				base.m_itemPageSizes.SetPaddings(num, num2);
				Array.Sort(this.m_indexesLeftToRight, this);
				this.VerticalDependency(this.m_children);
				this.HorizontalDependecy();
				this.ResolveOverlappingItems(pageContext);
				return flag;
			}
			return isSimple;
		}

		internal bool CreateChildrenFromPaginationState(ReportItemCollection childrenDef, PageContext pageContext, PageItemContainerHelper itemHelper, bool isSimple)
		{
			if (itemHelper != null && itemHelper.Children != null && itemHelper.Children.Length != 0)
			{
				if (childrenDef != null && childrenDef.Count != 0)
				{
					bool flag = isSimple;
					ReportItem reportItem = null;
					int num = 0;
					PageItemHelper[] children = itemHelper.Children;
					int[] indexesTopToBottom = itemHelper.IndexesTopToBottom;
					this.m_children = new PageItem[childrenDef.Count];
					RSTrace.RenderingTracer.Assert(this.m_children.Length == children.Length, "Mismatch between the number of children on page and the saved number of children for the page.");
					for (int i = 0; i < childrenDef.Count; i++)
					{
						num = i;
						if (indexesTopToBottom != null)
						{
							num = indexesTopToBottom[i];
						}
						reportItem = ((ReportElementCollectionBase<ReportItem>)childrenDef)[num];
						if (children[i] != null)
						{
							this.m_children[i] = PageItem.Create(reportItem, pageContext, false, false);
							this.m_children[i].UpdateItem(children[i]);
						}
						if (flag)
						{
							if (reportItem.Visibility != null && reportItem.Visibility.HiddenState == SharedHiddenState.Sometimes)
							{
								flag = false;
							}
							else
							{
								AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = reportItem as AspNetCore.ReportingServices.OnDemandReportRendering.Tablix;
								flag = this.IsStaticTablix(tablix, null);
								if (flag)
								{
									bool flag2 = false;
									this.VerifyNoRows(reportItem, ref flag2);
									if (flag2)
									{
										flag = false;
									}
								}
							}
						}
						if (reportItem.RepeatedSibling)
						{
							if (this.m_repeatWithItems == null)
							{
								this.m_repeatWithItems = new RepeatWithItem[childrenDef.Count];
							}
							this.m_repeatWithItems[i] = new RepeatWithItem(PageItem.Create(reportItem, pageContext, false, true), pageContext);
						}
					}
					return flag;
				}
				return isSimple;
			}
			return isSimple;
		}

		internal void ResolveRepeatWithFromPaginationState(PageItemContainerHelper itemHelper, RPLWriter rplWriter, PageContext pageContext)
		{
			if (itemHelper != null && this.m_repeatWithItems != null && this.m_repeatWithItems.Length != 0)
			{
				for (int i = 0; i < this.m_repeatWithItems.Length; i++)
				{
					if (this.m_repeatWithItems[i] != null)
					{
						this.m_repeatWithItems[i].UpdateItem(itemHelper.RepeatWithItems[i], rplWriter, pageContext);
					}
				}
			}
		}

		internal void ResolveRepeatWith(ReportItemCollection childrenDef, PageContext pageContext)
		{
            if (childrenDef == null || this.m_repeatWithItems == null)
            {
                return;
            }
            RPLWriter rplWriter = new RPLWriter();
            PageContext pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Repeated);
            for (int i = 0; i < childrenDef.Count; i++)
            {
                DataRegion dataRegion = childrenDef[i] as DataRegion;
                if (dataRegion != null)
                {
                    int num = -1;
                    int[] repeatSiblings = dataRegion.GetRepeatSiblings();
                    if (repeatSiblings != null && repeatSiblings.Length > 0)
                    {
                        PageItem[] array = new PageItem[repeatSiblings.Length + 1];
                        for (int j = 0; j < repeatSiblings.Length; j++)
                        {
                            int num2 = repeatSiblings[j];
                            if (num2 < i)
                            {
                                if (this.m_repeatWithItems[num2] != null)
                                {
                                    array[j] = this.m_repeatWithItems[num2].SourcePageItem;
                                }
                            }
                            else
                            {
                                if (num < 0)
                                {
                                    num = j;
                                    array[j] = PageItem.Create(dataRegion, pageContext2, false, true);
                                }
                                if (this.m_repeatWithItems[num2] != null)
                                {
                                    array[j + 1] = this.m_repeatWithItems[num2].SourcePageItem;
                                }
                            }
                        }
                        if (num < 0)
                        {
                            num = array.Length - 1;
                            array[num] = PageItem.Create(dataRegion, pageContext2, false, true);
                        }
                        this.VerticalDependency(array);
                        for (int k = 0; k < array.Length; k++)
                        {
                            if (num == k)
                            {
                                array[k].AdjustOriginFromItemsAbove(array, null);
                            }
                            else if (array[k] != null)
                            {
                                array[k].CalculateRepeatWithPage(rplWriter, pageContext2, array);
                            }
                        }
                        foreach (int num2 in repeatSiblings)
                        {
                            //int num2;
                            if (this.m_children[num2] != null)
                            {
                                this.m_repeatWithItems[num2].UpdateCreateState(array[num], i, this.m_children[num2].PageItemsAbove, pageContext2);
                            }
                        }
                    }
                }
            }
        }

		private void ResolveOverlappingItems(PageContext pageContext)
		{
			PageItem pageItem = null;
			bool flag = false;
			bool flag2 = false;
			double num = 0.0;
			PageItem[] array = null;
			bool flag3 = false;
			this.DetectOverlappingItems(pageContext, out flag2, out flag);
			while (flag2)
			{
				num = 0.0;
				if (flag)
				{
					for (int i = 0; i < this.m_children.Length; i++)
					{
						pageItem = this.m_children[i];
						if (pageItem != null)
						{
							pageItem.AdjustOriginFromItemsAbove(this.m_children, null, false);
							num = Math.Max(num, pageItem.ItemPageSizes.Bottom + base.ItemPageSizes.PaddingBottom);
						}
					}
					base.ItemPageSizes.AdjustHeightTo(num);
				}
				else
				{
					for (int j = 0; j < this.m_children.Length; j++)
					{
						pageItem = this.m_children[this.m_indexesLeftToRight[j]];
						if (pageItem != null)
						{
							pageItem.AdjustOriginFromItemsAtLeft(this.m_children, false);
							num = Math.Max(num, pageItem.ItemPageSizes.Right + base.ItemPageSizes.PaddingRight);
						}
					}
					base.ItemPageSizes.AdjustWidthTo(num);
					this.m_rightEdgeItem.ItemPageSizes.Left = Math.Max(this.m_rightEdgeItem.ItemPageSizes.Left, num);
				}
				for (int k = 0; k < this.m_children.Length; k++)
				{
					pageItem = this.m_children[k];
					if (pageItem != null)
					{
						if (flag)
						{
							pageItem.ItemPageSizes.DeltaY = 0.0;
							pageItem.PageItemsAbove = null;
						}
						else
						{
							pageItem.ItemPageSizes.DeltaX = 0.0;
							pageItem.PageItemsLeft = null;
						}
					}
				}
				if (flag)
				{
					this.UpdateChildren(ref array);
					this.VerticalDependency(this.m_children);
				}
				else
				{
					if (this.m_indexesTopToBottom != null)
					{
						for (int l = 0; l < this.m_indexesLeftToRight.Length; l++)
						{
							this.m_indexesLeftToRight[l] = l;
						}
					}
					Array.Sort(this.m_indexesLeftToRight, this);
					this.HorizontalDependecy();
					flag3 = true;
				}
				this.DetectOverlappingItems(pageContext, out flag2, out flag);
			}
			if (flag3)
			{
				for (int m = 0; m < this.m_children.Length; m++)
				{
					if (this.m_children[m] != null)
					{
						this.m_children[m].DefLeftValue = this.m_children[m].ItemPageSizes.Left;
					}
				}
			}
		}

		private void UpdateChildren(ref PageItem[] defChildren)
		{
			if (this.m_indexesTopToBottom == null)
			{
				this.m_indexesTopToBottom = new int[this.m_children.Length];
				defChildren = new PageItem[this.m_children.Length];
				for (int i = 0; i < this.m_children.Length; i++)
				{
					this.m_indexesTopToBottom[i] = i;
					defChildren[i] = this.m_children[i];
				}
			}
			Array.Sort(this.m_indexesTopToBottom, new SortPageItemIndexesOnTop(defChildren));
			for (int j = 0; j < this.m_children.Length; j++)
			{
				this.m_children[j] = defChildren[this.m_indexesTopToBottom[j]];
			}
		}

		private static bool IsStraightLine(Line line)
		{
			if (line == null)
			{
				return false;
			}
			if (line.ItemPageSizes.Width != 0.0)
			{
				return line.ItemPageSizes.Height == 0.0;
			}
			return true;
		}

		private void RemoveItem(PageItem item, int index)
		{
			this.m_children[index] = null;
			if (item.RepeatedSibling)
			{
				this.m_repeatWithItems[index] = null;
			}
		}

		private void DetectOverlappingItems(PageContext pageContext, out bool itemsOverlap, out bool verticalOverlap)
		{
			itemsOverlap = false;
			verticalOverlap = false;
			PageItem pageItem = null;
			PageItem pageItem2 = null;
			Line line = null;
			Line line2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0, true);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0, true);
			RoundedDouble roundedDouble3 = new RoundedDouble(0.0, true);
			RoundedDouble roundedDouble4 = new RoundedDouble(0.0, true);
			for (int i = 0; i < this.m_children.Length; i++)
			{
				if (itemsOverlap)
				{
					break;
				}
				pageItem = this.m_children[i];
				if (pageItem != null && !pageItem.HiddenForOverlap(pageContext))
				{
					line = (pageItem as Line);
					if (!PageItemContainer.IsStraightLine(line))
					{
						roundedDouble.Value = pageItem.ItemPageSizes.Bottom;
						roundedDouble2.Value = pageItem.ItemPageSizes.Right;
						for (int j = i + 1; j < this.m_children.Length; j++)
						{
							if (itemsOverlap)
							{
								break;
							}
							pageItem2 = this.m_children[j];
							if (pageItem2 != null && !pageItem.HiddenForOverlap(pageContext))
							{
								line2 = (pageItem2 as Line);
								if (!PageItemContainer.IsStraightLine(line2))
								{
									roundedDouble3.Value = pageItem2.ItemPageSizes.Bottom;
									roundedDouble4.Value = pageItem2.ItemPageSizes.Right;
									if (roundedDouble <= pageItem2.ItemPageSizes.Top)
									{
										break;
									}
									if (!(roundedDouble2 <= pageItem2.ItemPageSizes.Left) && !(roundedDouble4 <= pageItem.ItemPageSizes.Left))
									{
										if (line != null)
										{
											this.RemoveItem(line, i);
										}
										else if (line2 != null)
										{
											this.RemoveItem(line2, j);
										}
										else
										{
											itemsOverlap = true;
											double num = roundedDouble.Value - pageItem2.ItemPageSizes.Top;
											double overlapY = Math.Min(num, pageItem2.ItemPageSizes.Height);
											if ((RoundedDouble)pageItem.ItemPageSizes.Left <= pageItem2.ItemPageSizes.Left)
											{
												this.MoveItems(roundedDouble2, pageItem2, pageItem2, overlapY, num, ref verticalOverlap);
											}
											else
											{
												this.MoveItems(roundedDouble4, pageItem, pageItem2, overlapY, num, ref verticalOverlap);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private void MoveItems(RoundedDouble pinItemRight, PageItem moveItemRight, PageItem moveItemDown, double overlapY, double deltaY, ref bool verticalOverlap)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (pinItemRight <= moveItemRight.ItemPageSizes.Right)
			{
				num = pinItemRight.Value - moveItemRight.ItemPageSizes.Left;
				num2 = num;
			}
			else
			{
				num = moveItemRight.ItemPageSizes.Width;
				num2 = pinItemRight.Value - moveItemRight.ItemPageSizes.Left;
			}
			if (num <= overlapY)
			{
				moveItemRight.ItemPageSizes.MoveHorizontal(num2);
			}
			else
			{
				verticalOverlap = true;
				moveItemDown.ItemPageSizes.MoveVertical(deltaY);
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			return true;
		}

		internal override void CreateItemRenderSizes(ItemSizes contentSize, PageContext pageContext, bool createForRepeat)
		{
			if (contentSize == null)
			{
				if (pageContext != null)
				{
					if (createForRepeat)
					{
						base.m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(base.m_itemPageSizes, true, true);
					}
					else
					{
						base.m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(base.m_itemPageSizes, true, true);
					}
				}
				if (base.m_itemRenderSizes == null)
				{
					base.m_itemRenderSizes = new PaddItemSizes((PaddItemSizes)base.m_itemPageSizes);
				}
			}
			else
			{
				base.m_itemRenderSizes = contentSize;
			}
		}

		internal PageItem[] BringRepeatedWithOnPage(RPLWriter rplWriter, List<int> repeatedSiblings, PageContext pageContext)
		{
            if (this.m_children == null)
            {
                return null;
            }
            if (repeatedSiblings == null)
            {
                return this.m_children;
            }
            PageItem[] array = new PageItem[this.m_children.Length];
            for (int i = 0; i < this.m_children.Length; i++)
            {
                PageItem pageItem = this.m_children[i];
                if (pageItem != null && ((pageItem.ItemState != PageItem.State.Below && pageItem.ItemState != PageItem.State.TopNextPage) || this.m_repeatWithItems[i] == null || repeatedSiblings.BinarySearch(i) < 0))
                {
                    array[i] = this.m_children[i];
                }
            }
            List<int> list = null;
            double num = 0.0;
            for (int j = repeatedSiblings.Count - 1; j >= 0; j--)
            {
                int num2 = repeatedSiblings[j];
                if (array[num2] == null)
                {
                    RepeatWithItem repeatWithItem = this.m_repeatWithItems[num2];
                    if (repeatWithItem != null)
                    {
                        PageItem pageItem2 = this.m_children[repeatWithItem.DataRegionIndex];
                        if (!repeatWithItem.AddOnPage(pageItem2.ItemRenderSizes, array, num2, ref list, ref num))
                        {
                            repeatedSiblings.RemoveAt(j);
                        }
                        else
                        {
                            array[num2] = repeatWithItem.SourcePageItem;
                        }
                    }
                }
                else
                {
                    repeatedSiblings.RemoveAt(j);
                }
            }
            if (num < 0.0)
            {
                foreach (PageItem pageItem in array)
                {
                    if (pageItem != null && pageItem.ItemState != PageItem.State.Below && pageItem.ItemState != PageItem.State.TopNextPage)
                    {
                        pageItem.ItemRenderSizes.Top -= num;
                    }
                }
            }
            for (int l = 0; l < repeatedSiblings.Count; l++)
            {
                int num2 = repeatedSiblings[l];
                RepeatWithItem repeatWithItem = this.m_repeatWithItems[num2];
                if (repeatWithItem != null)
                {
                    repeatWithItem.WriteRepeatWithToPage(rplWriter, pageContext);
                }
            }
            if (list != null)
            {
                for (int m = 0; m < list.Count; m++)
                {
                    array[list[m]].AdjustOriginFromRepeatItems(array);
                }
            }
            return array;
        }

		protected void ConsumeWhitespaceHorizontal(ItemSizes itemSizes, double adjustWidthTo, PageContext pageContext)
		{
			if (!pageContext.ConsumeContainerWhitespace)
			{
				itemSizes.AdjustWidthTo(adjustWidthTo);
			}
			else
			{
				double num = adjustWidthTo - itemSizes.Width;
				itemSizes.AdjustWidthTo(adjustWidthTo);
				if (!(num <= 0.0) && !(itemSizes.PaddingRight <= 0.0))
				{
					double num2 = num - itemSizes.PaddingRight;
					if (num2 > 0.0)
					{
						itemSizes.AdjustWidthTo(itemSizes.PadWidth);
						itemSizes.PaddingRight = 0.0;
					}
					else
					{
						itemSizes.AdjustWidthTo(itemSizes.Width - num);
						itemSizes.PaddingRight = 0.0 - num2;
					}
				}
			}
		}

		protected void ConsumeWhitespaceVertical(ItemSizes itemSizes, double adjustHeightTo, PageContext pageContext)
		{
			if (!pageContext.ConsumeContainerWhitespace)
			{
				itemSizes.AdjustHeightTo(adjustHeightTo);
			}
			else
			{
				double num = adjustHeightTo - itemSizes.Height;
				itemSizes.AdjustHeightTo(adjustHeightTo);
				if (!(num <= 0.0) && !(itemSizes.PaddingBottom <= 0.0))
				{
					double num2 = num - itemSizes.PaddingBottom;
					if (num2 > 0.0)
					{
						itemSizes.AdjustHeightTo(itemSizes.PadHeight);
						itemSizes.PaddingBottom = 0.0;
					}
					else
					{
						itemSizes.AdjustHeightTo(itemSizes.Height - num);
						itemSizes.PaddingBottom = 0.0 - num2;
					}
				}
			}
		}

		internal void CalculateRepeatWithRenderSizes(PageContext pageContext)
		{
			if (this.m_children != null)
			{
				PageItem pageItem = null;
				double num = base.m_itemRenderSizes.PaddingBottom;
				for (int i = 0; i < this.m_children.Length; i++)
				{
					pageItem = this.m_children[i];
					if (pageItem != null)
					{
						pageItem.AdjustOriginFromItemsAbove(this.m_children, null, true);
						num = Math.Max(num, pageItem.ItemRenderSizes.Bottom + base.m_itemRenderSizes.PaddingBottom);
					}
				}
				this.ConsumeWhitespaceVertical(base.m_itemRenderSizes, num, pageContext);
				num = base.m_itemRenderSizes.PaddingRight;
				for (int j = 0; j < this.m_children.Length; j++)
				{
					pageItem = this.m_children[this.m_indexesLeftToRight[j]];
					if (pageItem != null)
					{
						pageItem.AdjustOriginFromItemsAtLeft(this.m_children, true);
						num = Math.Max(num, pageItem.ItemRenderSizes.Right + base.m_itemRenderSizes.PaddingRight);
					}
				}
				if (this.m_rightEdgeItem != null)
				{
					if (pageContext != null)
					{
						this.m_rightEdgeItem.ItemRenderSizes = pageContext.GetSharedRenderEdgeItemSizesElement(this.m_rightEdgeItem.ItemPageSizes);
					}
					else
					{
						this.m_rightEdgeItem.ItemRenderSizes = new ItemSizes(this.m_rightEdgeItem.ItemPageSizes);
					}
					this.m_rightEdgeItem.AdjustOriginFromItemsAtLeft(this.m_children, true);
					this.ConsumeWhitespaceHorizontal(base.m_itemRenderSizes, this.m_rightEdgeItem.ItemRenderSizes.Right, pageContext);
				}
				else
				{
					this.ConsumeWhitespaceHorizontal(base.m_itemRenderSizes, num, pageContext);
				}
			}
		}

		internal int CalculateRenderSizes(RPLWriter rplWriter, PageContext pageContext, Interactivity interactivity, List<int> repeatedSiblings, out PageItem[] childrenOnPage)
		{
            childrenOnPage = this.BringRepeatedWithOnPage(rplWriter, repeatedSiblings, pageContext);
            if (childrenOnPage == null)
            {
                return 0;
            }
            double num = this.m_itemRenderSizes.PaddingBottom;
            int num2 = 0;
            for (int i = 0; i < childrenOnPage.Length; i++)
            {
                PageItem pageItem = childrenOnPage[i];
                if (pageItem != null && pageItem.ItemState != PageItem.State.Below && pageItem.ItemState != PageItem.State.TopNextPage)
                {
                    if (pageItem.ItemState != PageItem.State.OnPageHidden && !(pageItem is NoRowsItem))
                    {
                        num2++;
                    }
                    RegisterItem.RegisterPageItem(pageItem, pageContext, pageContext.EvaluatePageHeaderFooter, interactivity);
                    pageItem.AdjustOriginFromItemsAbove(childrenOnPage, this.m_repeatWithItems, true);
                    num = Math.Max(num, pageItem.ItemRenderSizes.Bottom + this.m_itemRenderSizes.PaddingBottom);
                }
            }
            this.ConsumeWhitespaceVertical(this.m_itemRenderSizes, num, pageContext);
            num = this.m_itemRenderSizes.PaddingRight;
            for (int j = 0; j < childrenOnPage.Length; j++)
            {
                PageItem pageItem = childrenOnPage[this.m_indexesLeftToRight[j]];
                if (pageItem != null && pageItem.ItemState != PageItem.State.Below && pageItem.ItemState != PageItem.State.TopNextPage)
                {
                    pageItem.AdjustOriginFromItemsAtLeft(childrenOnPage, true);
                    num = Math.Max(num, pageItem.ItemRenderSizes.Right + this.m_itemRenderSizes.PaddingRight);
                }
            }
            if (this.m_rightEdgeItem != null)
            {
                if (pageContext != null)
                {
                    this.m_rightEdgeItem.ItemRenderSizes = pageContext.GetSharedRenderEdgeItemSizesElement(this.m_rightEdgeItem.ItemPageSizes);
                }
                else
                {
                    this.m_rightEdgeItem.ItemRenderSizes = new ItemSizes(this.m_rightEdgeItem.ItemPageSizes);
                }
                this.m_rightEdgeItem.AdjustOriginFromItemsAtLeft(childrenOnPage, true);
                this.ConsumeWhitespaceHorizontal(this.m_itemRenderSizes, this.m_rightEdgeItem.ItemRenderSizes.Right, pageContext);
            }
            else
            {
                this.ConsumeWhitespaceHorizontal(this.m_itemRenderSizes, num, pageContext);
            }
            return num2;
        }

		internal void UpdateItemPageState(PageContext pageContext, bool omitBorderOnPageBreak)
		{
			if (base.m_itemState == State.SpanPages)
			{
				if (omitBorderOnPageBreak)
				{
					base.m_rplItemState |= 1;
				}
				base.m_itemState = State.OnPage;
				PageItem pageItem = null;
				for (int i = 0; i < this.m_children.Length; i++)
				{
					pageItem = this.m_children[i];
					if (pageItem != null)
					{
						pageItem.UpdateSizes(this.m_prevPageEnd, this.m_children, this.m_repeatWithItems);
						if (pageItem.ItemState == State.Below)
						{
							pageItem.ItemState = State.Unknow;
						}
						else if (pageItem.ItemState == State.TopNextPage)
						{
							pageItem.ItemState = State.OnPage;
						}
					}
				}
			}
			double num = this.m_prevPageEnd - base.ItemPageSizes.PadHeight;
			base.m_itemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height - this.m_prevPageEnd);
			if (num > 0.0)
			{
				base.m_itemPageSizes.PaddingBottom = base.m_itemPageSizes.Height;
			}
			this.m_prevPageEnd = 0.0;
			if (this.m_repeatWithItems != null)
			{
				for (int j = 0; j < this.m_repeatWithItems.Length; j++)
				{
					if (this.m_repeatWithItems[j] != null)
					{
						this.m_repeatWithItems[j].UpdateSizes(pageContext);
					}
				}
			}
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				PageItemContainerHelper pageItemContainerHelper = itemHelper as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null, "This should be a container object");
				this.m_itemsCreated = pageItemContainerHelper.ItemsCreated;
				this.m_prevPageEnd = pageItemContainerHelper.PrevPageEnd;
				if (pageItemContainerHelper.RightEdgeItem != null)
				{
					this.m_rightEdgeItem = new EdgePageItem(pageItemContainerHelper.RightEdgeItem.ItemPageSizes.Top, pageItemContainerHelper.RightEdgeItem.ItemPageSizes.Left, this.SourceID, null);
					this.m_rightEdgeItem.UpdateItem(pageItemContainerHelper.RightEdgeItem);
				}
				if (pageItemContainerHelper.IndexesLeftToRight != null)
				{
					this.m_indexesLeftToRight = PageItem.GetNewArray(pageItemContainerHelper.IndexesLeftToRight);
				}
				if (pageItemContainerHelper.IndexesTopToBottom != null)
				{
					this.m_indexesTopToBottom = PageItem.GetNewArray(pageItemContainerHelper.IndexesTopToBottom);
				}
			}
		}

		internal void ResolveVerticalDependencyList(List<int> pageItemsAbove, int index)
		{
			if (pageItemsAbove != null && this.m_repeatWithItems != null)
			{
				List<int> list = null;
				for (int i = 0; i < pageItemsAbove.Count; i++)
				{
					if (this.m_repeatWithItems[pageItemsAbove[i]] != null && list == null)
					{
						list = new List<int>();
						list.Add(pageItemsAbove[i]);
					}
				}
				if (list != null)
				{
					for (int j = index + 1; j < this.m_children.Length; j++)
					{
						this.FixItemsAbove(this.m_children[j], index, list);
					}
				}
			}
		}

		private void FixItemsAbove(PageItem item, int index, List<int> repeatedItems)
		{
			if (item != null)
			{
				List<int> pageItemsAbove = item.PageItemsAbove;
				if (pageItemsAbove != null)
				{
					int num = pageItemsAbove.BinarySearch(index);
					if (num >= 0)
					{
						pageItemsAbove.RemoveAt(num);
						Hashtable hashtable = new Hashtable();
						this.CollectAllItems(pageItemsAbove, hashtable);
						if (hashtable.Count > 0)
						{
							for (int i = 0; i < repeatedItems.Count; i++)
							{
								if (hashtable.ContainsKey(repeatedItems[i]))
								{
									repeatedItems.RemoveAt(i);
									i--;
								}
							}
						}
						int num2 = 0;
						for (int j = 0; j < repeatedItems.Count; j++)
						{
							while (num2 < pageItemsAbove.Count)
							{
								if (pageItemsAbove[num2] < repeatedItems[j])
								{
									num2++;
									continue;
								}
								pageItemsAbove.Insert(num2, repeatedItems[j]);
								num2++;
								break;
							}
							if (num2 == pageItemsAbove.Count)
							{
								pageItemsAbove.Add(repeatedItems[j]);
								num2++;
							}
						}
					}
				}
			}
		}

		private void CollectAllItems(List<int> srcList, Hashtable destList)
		{
			if (srcList != null)
			{
				PageItem pageItem = null;
				for (int i = 0; i < srcList.Count; i++)
				{
					if (!destList.ContainsKey(srcList[i]))
					{
						destList.Add(srcList[i], srcList[i]);
					}
					pageItem = this.m_children[srcList[i]];
					if (pageItem != null)
					{
						this.CollectAllItems(pageItem.PageItemsAbove, destList);
					}
				}
			}
		}

		internal void ReleaseChildrenOnPage()
		{
			if (this.m_children != null)
			{
				PageItem pageItem = null;
				for (int i = 0; i < this.m_children.Length; i++)
				{
					pageItem = this.m_children[i];
					if (pageItem != null && (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd || pageItem.ItemState == State.OnPageHidden))
					{
						if (!pageItem.RepeatedSibling)
						{
							this.ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
						}
						this.m_children[i] = null;
					}
				}
			}
		}

		private void VerifyNoRows(ReportItem reportItem, ref bool noRows)
		{
			if (reportItem != null)
			{
				DataRegion dataRegion = reportItem as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					if (dataRegionInstance.NoRows)
					{
						noRows = true;
					}
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = reportItem as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
					if (subReport != null)
					{
						SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
						if (!subReportInstance.NoRows && !subReportInstance.ProcessedWithError)
						{
							return;
						}
						noRows = true;
					}
				}
			}
		}

		internal virtual void WriteEndItemToStream(RPLWriter rplWriter, int itemsOnPage, PageItem[] childrenOnPage)
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
						if (pageItem.ItemState == State.OnPageHidden || pageItem is NoRowsItem)
						{
							if (!pageItem.RepeatedSibling)
							{
								this.ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
							}
							childrenOnPage[i] = null;
							this.m_children[i] = null;
						}
						else
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
							if (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd)
							{
								if (!pageItem.RepeatedSibling)
								{
									this.ResolveVerticalDependencyList(pageItem.PageItemsAbove, i);
								}
								childrenOnPage[i] = null;
								this.m_children[i] = null;
							}
							else
							{
								pageItem.ItemRenderSizes = null;
							}
						}
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

		internal void WriteRepeatWithEndItemToStream(RPLWriter rplWriter, int itemsOnPage)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				int num = (this.m_children != null) ? this.m_children.Length : 0;
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
					pageItem = this.m_children[i];
					if (pageItem != null && pageItem.ItemState != State.OnPageHidden)
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

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)5);
				this.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(5);
			this.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		internal override void WritePaginationInfoProperties(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				PageItemContainerHelper pageItemContainerHelper = itemHelper as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null, "This should be a container object");
				base.WritePaginationInfoProperties(pageItemContainerHelper);
				pageItemContainerHelper.ItemsCreated = this.m_itemsCreated;
				pageItemContainerHelper.PrevPageEnd = this.m_prevPageEnd;
				if (this.m_rightEdgeItem != null)
				{
					pageItemContainerHelper.RightEdgeItem = this.m_rightEdgeItem.WritePaginationInfo();
				}
				pageItemContainerHelper.IndexesLeftToRight = PageItem.GetNewArray(this.m_indexesLeftToRight);
				pageItemContainerHelper.IndexesTopToBottom = PageItem.GetNewArray(this.m_indexesTopToBottom);
				if (this.m_children != null && this.m_children.Length > 0)
				{
					pageItemContainerHelper.Children = new PageItemHelper[this.m_children.Length];
					for (int i = 0; i < this.m_children.Length; i++)
					{
						if (this.m_children[i] != null)
						{
							pageItemContainerHelper.Children[i] = this.m_children[i].WritePaginationInfo();
						}
					}
				}
				if (this.m_repeatWithItems != null && this.m_repeatWithItems.Length > 0)
				{
					pageItemContainerHelper.RepeatWithItems = new PageItemRepeatWithHelper[this.m_repeatWithItems.Length];
					for (int j = 0; j < this.m_repeatWithItems.Length; j++)
					{
						if (this.m_repeatWithItems[j] != null)
						{
							pageItemContainerHelper.RepeatWithItems[j] = this.m_repeatWithItems[j].WritePaginationInfo();
						}
					}
				}
			}
		}

		internal override void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)6);
				reportPageInfo.Write(this.m_itemsCreated);
				reportPageInfo.Write((byte)11);
				reportPageInfo.Write(this.m_prevPageEnd);
				if (this.m_rightEdgeItem != null)
				{
					reportPageInfo.Write((byte)9);
					this.m_rightEdgeItem.WritePaginationInfo(reportPageInfo);
				}
				if (this.m_indexesLeftToRight != null && this.m_indexesLeftToRight.Length > 0)
				{
					reportPageInfo.Write((byte)7);
					reportPageInfo.Write(this.m_indexesLeftToRight.Length);
					for (int i = 0; i < this.m_indexesLeftToRight.Length; i++)
					{
						reportPageInfo.Write(this.m_indexesLeftToRight[i]);
					}
				}
				if (this.m_indexesTopToBottom != null && this.m_indexesTopToBottom.Length > 0)
				{
					reportPageInfo.Write((byte)20);
					reportPageInfo.Write(this.m_indexesTopToBottom.Length);
					for (int j = 0; j < this.m_indexesTopToBottom.Length; j++)
					{
						reportPageInfo.Write(this.m_indexesTopToBottom[j]);
					}
				}
				if (this.m_children != null && this.m_children.Length > 0)
				{
					reportPageInfo.Write((byte)10);
					reportPageInfo.Write(this.m_children.Length);
					for (int k = 0; k < this.m_children.Length; k++)
					{
						if (this.m_children[k] != null)
						{
							this.m_children[k].WritePaginationInfo(reportPageInfo);
						}
						else
						{
							reportPageInfo.Write((byte)14);
							reportPageInfo.Write((byte)255);
						}
					}
				}
				if (this.m_repeatWithItems != null && this.m_repeatWithItems.Length > 0)
				{
					reportPageInfo.Write((byte)8);
					reportPageInfo.Write(this.m_repeatWithItems.Length);
					for (int l = 0; l < this.m_repeatWithItems.Length; l++)
					{
						if (this.m_repeatWithItems[l] != null)
						{
							this.m_repeatWithItems[l].WritePaginationInfo(reportPageInfo);
						}
						else
						{
							reportPageInfo.Write((byte)14);
							reportPageInfo.Write((byte)255);
						}
					}
				}
			}
		}
	}
}
