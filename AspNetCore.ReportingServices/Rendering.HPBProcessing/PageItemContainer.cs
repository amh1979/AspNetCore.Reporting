using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class PageItemContainer : PageItem, IComparer, IStorable, IPersistable
	{
		protected PageItem[] m_children;

		protected int[] m_indexesLeftToRight;

		protected double m_rightPadding;

		protected double m_definitionRightPadding;

		protected double m_bottomPadding;

		private static Declaration m_declaration = PageItemContainer.GetDeclaration();

		internal abstract byte RPLFormatType
		{
			get;
		}

		public override int Size
		{
			get
			{
				return base.Size + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_children) + 24 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_indexesLeftToRight);
			}
		}

		internal PageItemContainer()
		{
		}

		internal PageItemContainer(ReportItem source)
			: base(source)
		{
			base.FullyCreated = false;
		}

		internal abstract RPLElement CreateRPLElement();

		internal abstract RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext);

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(PageItemContainer.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Children:
					writer.Write(this.m_children);
					break;
				case MemberName.Indexes:
					writer.Write(this.m_indexesLeftToRight);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(this.m_rightPadding);
					break;
				case MemberName.DefPadding:
					writer.Write(this.m_definitionRightPadding);
					break;
				case MemberName.VerticalPadding:
					writer.Write(this.m_bottomPadding);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(PageItemContainer.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Children:
					this.m_children = reader.ReadArrayOfRIFObjects<PageItem>();
					break;
				case MemberName.Indexes:
					this.m_indexesLeftToRight = reader.ReadInt32Array();
					break;
				case MemberName.HorizontalPadding:
					this.m_rightPadding = reader.ReadDouble();
					break;
				case MemberName.DefPadding:
					this.m_definitionRightPadding = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					this.m_bottomPadding = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.PageItemContainer;
		}

		internal new static Declaration GetDeclaration()
		{
			if (PageItemContainer.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Children, ObjectType.RIFObjectArray, ObjectType.PageItem));
				list.Add(new MemberInfo(MemberName.Indexes, ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.DefPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				return new Declaration(ObjectType.PageItemContainer, ObjectType.PageItem, list);
			}
			return PageItemContainer.m_declaration;
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
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

		private void VerticalDependency()
		{
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < this.m_children.Length; i++)
			{
				pageItem = this.m_children[i];
				itemSizes = pageItem.ItemPageSizes;
				roundedDouble2.Value = itemSizes.Bottom;
				roundedDouble.Value = -1.0;
				for (int j = i + 1; j < this.m_children.Length; j++)
				{
					pageItem2 = this.m_children[j];
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

		private void HorizontalDependecy()
		{
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			PageItem pageItem2 = null;
			ItemSizes itemSizes2 = null;
			RoundedDouble roundedDouble = new RoundedDouble(-1.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < this.m_children.Length; i++)
			{
				pageItem = this.m_children[this.m_indexesLeftToRight[i]];
				itemSizes = pageItem.ItemPageSizes;
				roundedDouble.Value = -1.0;
				roundedDouble2.Value = itemSizes.Right;
				for (int j = i + 1; j < this.m_children.Length; j++)
				{
					pageItem2 = this.m_children[this.m_indexesLeftToRight[j]];
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
		}

		protected abstract void CreateChildren(PageContext pageContext);

		protected void CreateChildren(ReportItemCollection childrenDef, PageContext pageContext)
		{
			if (childrenDef != null && childrenDef.Count != 0 && this.m_children == null)
			{
				double num = 0.0;
				double num2 = 0.0;
				this.m_children = new PageItem[childrenDef.Count + 1];
				this.m_indexesLeftToRight = new int[childrenDef.Count + 1];
				for (int i = 0; i < childrenDef.Count; i++)
				{
					ReportItem source = ((ReportElementCollectionBase<ReportItem>)childrenDef)[i];
					this.m_children[i] = PageItem.Create(source, false, pageContext);
					this.m_indexesLeftToRight[i] = i;
					num = Math.Max(num, this.m_children[i].ItemPageSizes.Right);
					num2 = Math.Max(num2, this.m_children[i].ItemPageSizes.Bottom);
				}
				double num3 = 0.0;
				num3 = ((!pageContext.ConsumeWhitespace) ? Math.Max(num2, base.ItemPageSizes.Height) : num2);
				this.m_children[this.m_children.Length - 1] = new HiddenPageItem(num3, 0.0);
				this.m_indexesLeftToRight[this.m_children.Length - 1] = this.m_children.Length - 1;
				this.m_rightPadding = Math.Max(0.0, base.m_itemPageSizes.Width - num);
				this.m_definitionRightPadding = this.m_rightPadding;
				this.m_bottomPadding = Math.Max(0.0, base.m_itemPageSizes.Height - num2);
				Array.Sort(this.m_indexesLeftToRight, this);
				this.VerticalDependency();
				this.HorizontalDependecy();
			}
		}

		private void ConsumeWhitespaceVertical(ItemSizes itemSizes, double adjustHeightTo)
		{
			double num = adjustHeightTo - itemSizes.Height;
			itemSizes.AdjustHeightTo(adjustHeightTo);
			if (!(num <= 0.0) && !(this.m_bottomPadding <= 0.0))
			{
				double num2 = num - this.m_bottomPadding;
				if (num2 > 0.0)
				{
					itemSizes.AdjustHeightTo(itemSizes.Height - this.m_bottomPadding);
					this.m_bottomPadding = 0.0;
				}
				else
				{
					itemSizes.AdjustHeightTo(itemSizes.Height - num);
					this.m_bottomPadding = 0.0 - num2;
				}
			}
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (this.m_children == null)
			{
				this.CreateChildren(pageContext);
			}
			if (this.m_children == null)
			{
				base.FullyCreated = true;
			}
			else
			{
				ancestors.Add(this);
				bool keepTogetherVertical = base.KeepTogetherVertical;
				bool flag = keepTogetherVertical || anyAncestorHasKT;
				this.DetermineContentVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, ancestors, ref flag, hasUnpinnedAncestors, false, false);
				if (keepTogetherVertical && !flag)
				{
					anyAncestorHasKT = false;
				}
				ancestors.RemoveAt(ancestors.Count - 1);
			}
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
			if (this.m_children == null)
			{
				return false;
			}
			double topInParentSystem2 = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double num = 0.0;
			bool flag = false;
			for (int i = 0; i < this.m_children.Length; i++)
			{
				PageItem pageItem = this.m_children[i];
				if (pageItem != null)
				{
					flag |= pageItem.ResolveDuplicates(pageContext, topInParentSystem2, this.m_children, flag);
					num = Math.Max(num, pageItem.ItemPageSizes.Bottom);
				}
			}
			if (flag)
			{
				if (pageContext.ConsumeWhitespace)
				{
					this.ConsumeWhitespaceVertical(base.ItemPageSizes, num + this.m_bottomPadding);
				}
				else
				{
					base.ItemPageSizes.AdjustHeightTo(num);
				}
				PageItem[] children = this.m_children;
				foreach (PageItem pageItem2 in children)
				{
					if (pageItem2 != null)
					{
						pageItem2.ItemPageSizes.DeltaY = 0.0;
					}
				}
			}
			return flag;
		}

		private void DetermineContentVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			double num = 0.0;
			double topInParentSystem2 = Math.Max(0.0, topInParentSystem - base.ItemPageSizes.Top);
			double bottomInParentSystem2 = bottomInParentSystem - base.ItemPageSizes.Top;
			PageContext pageContext2 = pageContext;
			if (!pageContext.IgnorePageBreaks && base.IgnorePageBreaks)
			{
				pageContext2 = new PageContext(pageContext, pageContext.CacheNonSharedProps);
				pageContext2.IgnorePageBreaks = true;
				pageContext2.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.ToggleableItem;
			}
			base.FullyCreated = true;
			for (int i = 0; i < this.m_children.Length; i++)
			{
				PageItem pageItem = this.m_children[i];
				if (pageItem != null)
				{
					if (resolveState)
					{
						pageItem.ResolveVertical(pageContext2, topInParentSystem2, bottomInParentSystem2, this.m_children, resolveItem, pageContext.Common.CanOverwritePageBreak, pageContext.Common.CanSetPageName);
					}
					else
					{
						pageItem.CalculateVertical(pageContext2, topInParentSystem2, bottomInParentSystem2, this.m_children, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
					}
					if (pageItem.KTVIsUnresolved)
					{
						base.UnresolvedCKTV = true;
					}
					if (pageItem.PBAreUnresolved)
					{
						base.UnresolvedCPB = true;
					}
					if (pageItem.NeedResolve)
					{
						base.NeedResolve = true;
					}
					if (!pageItem.FullyCreated)
					{
						base.FullyCreated = false;
					}
					num = Math.Max(num, pageItem.ItemPageSizes.Bottom);
				}
			}
			if (pageContext.ConsumeWhitespace)
			{
				this.ConsumeWhitespaceVertical(base.ItemPageSizes, num + this.m_bottomPadding);
			}
			else
			{
				base.ItemPageSizes.AdjustHeightTo(num);
			}
			PageItem[] children = this.m_children;
			foreach (PageItem pageItem2 in children)
			{
				if (pageItem2 != null)
				{
					pageItem2.ItemPageSizes.DeltaY = 0.0;
				}
			}
		}

		protected override bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			bool flag = base.ResolveKeepTogetherVertical(pageContext, topInParentSystem, bottomInParentSystem, resolveItem, canOverwritePageBreak, canSetPageName);
			if (this.m_children == null)
			{
				return flag;
			}
			bool flag2 = false;
			if (resolveItem && base.NeedResolve)
			{
				goto IL_003b;
			}
			if (!flag && base.UnresolvedCKTV)
			{
				goto IL_003b;
			}
			if (base.UnresolvedCPB)
			{
				goto IL_003b;
			}
			goto IL_0060;
			IL_0060:
			return flag;
			IL_003b:
			base.UnresolvedCKTV = false;
			base.UnresolvedCPB = false;
			base.NeedResolve = false;
			this.DetermineContentVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, null, ref flag2, false, true, resolveItem);
			goto IL_0060;
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (this.m_children != null)
			{
				ancestors.Add(this);
				if (base.KeepTogetherHorizontal)
				{
					anyAncestorHasKT = true;
				}
				this.DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors, false, false);
				ancestors.RemoveAt(ancestors.Count - 1);
			}
		}

		protected virtual void DetermineContentHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			double num = 0.0;
			double leftInParentSystem2 = Math.Max(0.0, leftInParentSystem - base.ItemPageSizes.Left);
			double rightInParentSystem2 = rightInParentSystem - base.ItemPageSizes.Left;
			bool flag = true;
			for (int i = 0; i < this.m_children.Length; i++)
			{
				PageItem pageItem = this.m_children[this.m_indexesLeftToRight[i]];
				if (pageItem != null)
				{
					if (resolveState)
					{
						pageItem.ResolveHorizontal(pageContext, leftInParentSystem2, rightInParentSystem2, this.m_children, resolveItem);
					}
					else
					{
						pageItem.CalculateHorizontal(pageContext, leftInParentSystem2, rightInParentSystem2, this.m_children, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
					}
					if (pageItem.KTHIsUnresolved)
					{
						base.UnresolvedCKTH = true;
					}
					if (pageItem.NeedResolve)
					{
						base.NeedResolve = true;
					}
					num = Math.Max(num, pageItem.ItemPageSizes.Right);
					if (!(pageItem is HiddenPageItem))
					{
						flag = false;
					}
				}
			}
			if (num > 0.0)
			{
				this.StretchHorizontal(num, pageContext);
			}
			else if (!flag && base.ItemPageSizes.Left >= leftInParentSystem)
			{
				this.StretchHorizontal(num, pageContext);
			}
			PageItem[] children = this.m_children;
			foreach (PageItem pageItem2 in children)
			{
				if (pageItem2 != null)
				{
					pageItem2.ItemPageSizes.DeltaX = 0.0;
				}
			}
		}

		private void StretchHorizontal(double maxRightInThisSystem, PageContext pageContext)
		{
			if (maxRightInThisSystem + this.m_rightPadding > this.OriginalWidth)
			{
				double num = 0.0;
				if (!pageContext.ConsumeWhitespace)
				{
					num = this.m_definitionRightPadding;
				}
				maxRightInThisSystem = ((!(maxRightInThisSystem > this.OriginalWidth - num)) ? this.OriginalWidth : (maxRightInThisSystem + num));
			}
			else
			{
				maxRightInThisSystem += this.m_rightPadding;
			}
			base.ItemPageSizes.AdjustWidthTo(maxRightInThisSystem);
		}

		protected override bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			bool flag = base.ResolveKeepTogetherHorizontal(pageContext, leftInParentSystem, rightInParentSystem, resolveItem);
			if (this.m_children == null)
			{
				return flag;
			}
			if (resolveItem && base.NeedResolve)
			{
				goto IL_002d;
			}
			if (!flag && base.UnresolvedCKTH)
			{
				goto IL_002d;
			}
			goto IL_004a;
			IL_004a:
			return flag;
			IL_002d:
			base.UnresolvedCKTH = false;
			base.NeedResolve = false;
			this.DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, null, false, false, true, resolveItem);
			goto IL_004a;
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (this.HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				this.WriteStartItemToStream(rplWriter, pageContext);
				this.OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				List<PageItem> list = null;
				if (this.m_children != null)
				{
					double pageLeft2 = Math.Max(0.0, pageLeft - base.ItemPageSizes.Left);
					double pageTop2 = Math.Max(0.0, pageTop - base.ItemPageSizes.Top);
					double pageRight2 = pageRight - base.ItemPageSizes.Left;
					double pageBottom2 = pageBottom - base.ItemPageSizes.Top;
					double num = 0.0;
					list = new List<PageItem>();
					for (int i = 0; i < this.m_children.Length; i++)
					{
						PageItem pageItem = this.m_children[i];
						if (pageItem != null)
						{
							if (pageItem.AddToPage(rplWriter, pageContext, pageLeft2, pageTop2, pageRight2, pageBottom2, repeatState))
							{
								if (pageItem.ContentOnPage)
								{
									list.Add(pageItem);
								}
								if (pageItem.Release(pageBottom2, pageRight2))
								{
									if (repeatState == RepeatState.None && (pageItem.ContentOnPage || !pageContext.ConsumeWhitespace || this.m_bottomPadding <= 0.0))
									{
										this.m_children[i] = null;
									}
								}
								else
								{
									num = Math.Max(num, pageItem.OriginalRight);
								}
							}
							else
							{
								num = Math.Max(num, pageItem.OriginalRight);
							}
						}
					}
					this.m_rightPadding = Math.Max(0.0, this.OriginalRight - num);
				}
				this.WriteEndItemToStream(rplWriter, list);
				return true;
			}
			return false;
		}

		internal override void ResetHorizontal(bool spanPages, double? width)
		{
			base.ResetHorizontal(spanPages, width);
			this.m_rightPadding = this.m_definitionRightPadding;
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					binaryWriter.Write(this.RPLFormatType);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, base.m_offset + 1);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = this.CreateRPLElement();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
				else
				{
					base.m_rplElement = this.CreateRPLElement(base.m_rplElement.ElementProps, pageContext);
				}
			}
		}

		internal virtual void WriteEndItemToStream(RPLWriter rplWriter, List<PageItem> itemsOnPage)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				int num = (itemsOnPage != null) ? itemsOnPage.Count : 0;
				long value = 0L;
				RPLItemMeasurement[] array = null;
				if (binaryWriter != null)
				{
					value = binaryWriter.BaseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(base.m_offset);
					binaryWriter.Write(num);
				}
				else
				{
					array = new RPLItemMeasurement[num];
					((RPLContainer)base.m_rplElement).Children = array;
				}
				if (itemsOnPage != null)
				{
					for (int i = 0; i < num; i++)
					{
						PageItem pageItem = itemsOnPage[i];
						if (pageItem != null)
						{
							if (binaryWriter != null)
							{
								pageItem.WritePageItemSizes(binaryWriter);
							}
							else
							{
								array[i] = pageItem.WritePageItemSizes();
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

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, rplStyleProps, pageContext);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, spbifWriter, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, rplStyleProps, pageContext);
				break;
			}
		}
	}
}
