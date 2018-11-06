using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal abstract class PageItem : IStorable, IPersistable
	{
		[Flags]
		protected enum StateFlags : ushort
		{
			Clear = 0,
			OnThisVerticalPage = 1,
			KeepTogetherVert = 2,
			KeepTogetherHoriz = 4,
			UnresolvedPBS = 8,
			UnresolvedPBE = 0x10,
			UnresolvedKTV = 0x20,
			UnresolvedKTH = 0x40,
			UnresolvedCKTV = 0x80,
			UnresolvedCKTH = 0x100,
			UnresolvedCPB = 0x200,
			SplitsVerticalPage = 0x400,
			ResolveChildren = 0x800,
			Duplicate = 0x1000,
			NeedResolve = 0x2000,
			TablixCellTopItem = 0x4000,
			FullyCreated = 0x8000
		}

		[Flags]
		internal enum RepeatState : byte
		{
			None = 0,
			Vertical = 1,
			Horizontal = 2
		}

		[StaticReference]
		protected ReportItem m_source;

		protected ItemSizes m_itemPageSizes;

		protected PageBreakProperties m_pageBreakProperties;

		protected string m_pageName;

		protected List<int> m_pageItemsAbove;

		protected List<int> m_pageItemsLeft;

		protected StateFlags m_stateFlags;

		protected long m_offset;

		[StaticReference]
		protected RPLElement m_rplElement;

		protected long m_nonSharedOffset = -1L;

		protected byte m_rplItemState;

		private static Declaration m_declaration = PageItem.GetDeclaration();

		internal ReportItem Source
		{
			get
			{
				return this.m_source;
			}
		}

		internal virtual string SourceID
		{
			get
			{
				return this.m_source.ID;
			}
		}

		internal virtual string SourceUniqueName
		{
			get
			{
				return this.m_source.Instance.UniqueName;
			}
		}

		internal ItemSizes ItemPageSizes
		{
			get
			{
				return this.m_itemPageSizes;
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

		public long Offset
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

		public RPLElement RPLElement
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

		internal byte RplItemState
		{
			get
			{
				return this.m_rplItemState;
			}
		}

		internal PageBreakProperties PageBreakProperties
		{
			get
			{
				return this.m_pageBreakProperties;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				if (this.m_pageBreakProperties == null)
				{
					return false;
				}
				if (this.IgnorePageBreaks)
				{
					return false;
				}
				return this.m_pageBreakProperties.PageBreakAtStart;
			}
		}

		internal bool PageBreakAtEnd
		{
			get
			{
				if (this.m_pageBreakProperties == null)
				{
					return false;
				}
				if (this.IgnorePageBreaks)
				{
					return false;
				}
				return this.m_pageBreakProperties.PageBreakAtEnd;
			}
		}

		internal string PageName
		{
			get
			{
				if (this.m_pageName != null && this.IgnorePageBreaks)
				{
					return null;
				}
				return this.m_pageName;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				if (this.m_source == null)
				{
					return false;
				}
				if (this.m_source.Visibility == null)
				{
					return false;
				}
				if (this.m_source.Visibility.ToggleItem != null)
				{
					return true;
				}
				return false;
			}
		}

		internal bool OnThisVerticalPage
		{
			get
			{
				return this.GetFlagValue(StateFlags.OnThisVerticalPage);
			}
			set
			{
				this.SetFlagValue(StateFlags.OnThisVerticalPage, value);
			}
		}

		internal bool SplitsVerticalPage
		{
			get
			{
				return this.GetFlagValue(StateFlags.SplitsVerticalPage);
			}
			set
			{
				this.SetFlagValue(StateFlags.SplitsVerticalPage, value);
			}
		}

		internal bool Duplicate
		{
			get
			{
				return this.GetFlagValue(StateFlags.Duplicate);
			}
			set
			{
				this.SetFlagValue(StateFlags.Duplicate, value);
			}
		}

		internal bool ResolveChildren
		{
			get
			{
				return this.GetFlagValue(StateFlags.ResolveChildren);
			}
			set
			{
				this.SetFlagValue(StateFlags.ResolveChildren, value);
			}
		}

		internal bool KeepTogetherVertical
		{
			get
			{
				return this.GetFlagValue(StateFlags.KeepTogetherVert);
			}
			set
			{
				this.SetFlagValue(StateFlags.KeepTogetherVert, value);
			}
		}

		internal bool KeepTogetherHorizontal
		{
			get
			{
				return this.GetFlagValue(StateFlags.KeepTogetherHoriz);
			}
			set
			{
				this.SetFlagValue(StateFlags.KeepTogetherHoriz, value);
			}
		}

		internal bool UnresolvedPBS
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedPBS);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedPBS, value);
			}
		}

		internal bool UnresolvedPBE
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedPBE);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedPBE, value);
			}
		}

		internal bool UnresolvedKTV
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedKTV);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedKTV, value);
			}
		}

		internal bool UnresolvedKTH
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedKTH);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedKTH, value);
			}
		}

		internal bool UnresolvedCKTV
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedCKTV);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedCKTV, value);
			}
		}

		internal bool UnresolvedCKTH
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedCKTH);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedCKTH, value);
			}
		}

		internal bool UnresolvedCPB
		{
			get
			{
				return this.GetFlagValue(StateFlags.UnresolvedCPB);
			}
			set
			{
				this.SetFlagValue(StateFlags.UnresolvedCPB, value);
			}
		}

		internal bool NeedResolve
		{
			get
			{
				return this.GetFlagValue(StateFlags.NeedResolve);
			}
			set
			{
				this.SetFlagValue(StateFlags.NeedResolve, value);
			}
		}

		internal bool TablixCellTopItem
		{
			get
			{
				return this.GetFlagValue(StateFlags.TablixCellTopItem);
			}
			set
			{
				this.SetFlagValue(StateFlags.TablixCellTopItem, value);
			}
		}

		internal bool FullyCreated
		{
			get
			{
				return this.GetFlagValue(StateFlags.FullyCreated);
			}
			set
			{
				this.SetFlagValue(StateFlags.FullyCreated, value);
			}
		}

		internal bool PBAreUnresolved
		{
			get
			{
				if (!this.UnresolvedPBS && !this.UnresolvedPBE)
				{
					if (this.UnresolvedCPB)
					{
						return true;
					}
					return false;
				}
				return true;
			}
		}

		internal bool KTVIsUnresolved
		{
			get
			{
				if (this.UnresolvedKTV)
				{
					return true;
				}
				if (this.UnresolvedCKTV)
				{
					return true;
				}
				return false;
			}
		}

		internal bool KTHIsUnresolved
		{
			get
			{
				if (this.UnresolvedKTH)
				{
					return true;
				}
				if (this.UnresolvedCKTH)
				{
					return true;
				}
				return false;
			}
		}

		internal virtual double OriginalLeft
		{
			get
			{
				return this.Source.Left.ToMillimeters();
			}
		}

		internal virtual double OriginalWidth
		{
			get
			{
				return this.Source.Width.ToMillimeters();
			}
		}

		internal virtual double OriginalRight
		{
			get
			{
				return this.OriginalLeft + this.OriginalWidth;
			}
		}

		internal virtual Style SharedStyle
		{
			get
			{
				return this.Source.Style;
			}
		}

		internal virtual StyleInstance NonSharedStyle
		{
			get
			{
				return this.Source.Instance.Style;
			}
		}

		public virtual int Size
		{
			get
			{
				return 2 * AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize + 1 + 16 + 2 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_itemPageSizes) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageBreakProperties) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageItemsAbove) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageItemsLeft) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageName);
			}
		}

		internal virtual bool ContentOnPage
		{
			get
			{
				return true;
			}
		}

		internal PageItem()
		{
		}

		protected PageItem(ReportItem source)
		{
			this.m_source = source;
			this.KeepTogetherHorizontal = true;
			this.KeepTogetherVertical = true;
			bool unresolvedKTH = this.UnresolvedKTV = true;
			this.UnresolvedKTH = unresolvedKTH;
			this.FullyCreated = true;
		}

		private bool GetFlagValue(StateFlags aFlag)
		{
			return (int)(this.m_stateFlags & aFlag) > 0;
		}

		private void SetFlagValue(StateFlags aFlag, bool aValue)
		{
			if (aValue)
			{
				this.m_stateFlags |= aFlag;
			}
			else
			{
				this.m_stateFlags &= (StateFlags)(ushort)(~(uint)aFlag);
			}
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(PageItem.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_source);
					writer.Write(value2);
					break;
				}
				case MemberName.ItemPageSizes:
					writer.Write(this.m_itemPageSizes);
					break;
				case MemberName.PageBreakProperties:
					writer.Write(this.m_pageBreakProperties);
					break;
				case MemberName.PageName:
					writer.Write(this.m_pageName);
					break;
				case MemberName.ItemsAbove:
					writer.WriteListOfPrimitives(this.m_pageItemsAbove);
					break;
				case MemberName.ItemsLeft:
					writer.WriteListOfPrimitives(this.m_pageItemsLeft);
					break;
				case MemberName.State:
					writer.Write((ushort)this.m_stateFlags);
					break;
				case MemberName.Offset:
					writer.Write(this.m_offset);
					break;
				case MemberName.RPLElement:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_rplElement);
					writer.Write(value);
					break;
				}
				case MemberName.NonSharedOffset:
					writer.Write(this.m_nonSharedOffset);
					break;
				case MemberName.RPLState:
					writer.Write(this.m_rplItemState);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(PageItem.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Source:
				{
					int id2 = reader.ReadInt32();
					this.m_source = (ReportItem)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.ItemPageSizes:
					this.m_itemPageSizes = (ItemSizes)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakProperties:
					this.m_pageBreakProperties = (PageBreakProperties)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					this.m_pageName = reader.ReadString();
					break;
				case MemberName.ItemsAbove:
					this.m_pageItemsAbove = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.ItemsLeft:
					this.m_pageItemsLeft = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.State:
					this.m_stateFlags = (StateFlags)reader.ReadUInt16();
					break;
				case MemberName.Offset:
					this.m_offset = reader.ReadInt64();
					break;
				case MemberName.RPLElement:
				{
					int id = reader.ReadInt32();
					this.m_rplElement = (RPLElement)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.NonSharedOffset:
					this.m_nonSharedOffset = reader.ReadInt64();
					break;
				case MemberName.RPLState:
					this.m_rplItemState = reader.ReadByte();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ObjectType GetObjectType()
		{
			return ObjectType.PageItem;
		}

		internal static Declaration GetDeclaration()
		{
			if (PageItem.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.ItemPageSizes, ObjectType.ItemSizes));
				list.Add(new MemberInfo(MemberName.PageBreakProperties, ObjectType.PageBreakProperties));
				list.Add(new MemberInfo(MemberName.PageName, ObjectType.String));
				list.Add(new MemberInfo(MemberName.ItemsAbove, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.ItemsLeft, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.State, Token.UInt16));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RPLElement, Token.Int32));
				list.Add(new MemberInfo(MemberName.NonSharedOffset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RPLState, Token.Byte));
				return new Declaration(ObjectType.PageItem, ObjectType.None, list);
			}
			return PageItem.m_declaration;
		}

		internal static PageItem Create(ReportItem source, bool tablixCellParent, PageContext pageContext)
		{
			return PageItem.Create(source, tablixCellParent, false, pageContext);
		}

		internal static PageItem Create(ReportItem source, bool tablixCellParent, bool ignoreKT, PageContext pageContext)
		{
			if (source == null)
			{
				return null;
			}
			PageItem pageItem = null;
			bool flag = true;
			if (source.Visibility != null && source.Instance.Visibility.CurrentlyHidden)
			{
				flag = false;
				pageItem = new HiddenPageItem(source, pageContext, false);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
			{
				pageItem = new TextBox((AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)source, pageContext);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Line)
			{
				pageItem = new Line((AspNetCore.ReportingServices.OnDemandReportRendering.Line)source);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Image)
			{
				pageItem = new Image((AspNetCore.ReportingServices.OnDemandReportRendering.Image)source);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Chart)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Chart source2 = (AspNetCore.ReportingServices.OnDemandReportRendering.Chart)source;
				pageItem = ((!PageItem.TransformToTextBox(source2)) ? ((PageItem)new Chart(source2, pageContext)) : ((PageItem)new TextBox(source2, pageContext)));
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel source3 = (AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)source;
				pageItem = ((!PageItem.TransformToTextBox(source3)) ? ((PageItem)new GaugePanel(source3, pageContext)) : ((PageItem)new TextBox(source3, pageContext)));
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Map)
			{
				pageItem = new Map((AspNetCore.ReportingServices.OnDemandReportRendering.Map)source, pageContext);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
			{
				bool flag2 = false;
				AspNetCore.ReportingServices.OnDemandReportRendering.Tablix source4 = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)source;
				if (PageItem.TransformToTextBox(source4, tablixCellParent, ref flag2))
				{
					pageItem = new TextBox(source4, pageContext);
				}
				else if (flag2)
				{
					flag = false;
					pageItem = new NoRowsItem(source4);
				}
				else
				{
					pageItem = new Tablix(source4, pageContext);
					pageContext.InitCache();
				}
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				pageItem = new Rectangle((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)source, pageContext);
			}
			else if (source is AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)
			{
				bool flag3 = false;
				AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source5 = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)source;
				if (PageItem.TransformToTextBox(source5, tablixCellParent, ref flag3))
				{
					pageItem = new TextBox(source5, pageContext);
				}
				else if (flag3)
				{
					flag = false;
					pageItem = new NoRowsItem(source5);
				}
				else
				{
					pageItem = new SubReport(source5);
				}
			}
			if (flag)
			{
				pageItem.CacheNonSharedProperties(pageContext);
			}
			if (ignoreKT)
			{
				pageItem.KeepTogetherHorizontal = false;
				pageItem.KeepTogetherVertical = false;
				PageItem pageItem2 = pageItem;
				PageItem pageItem3 = pageItem;
				bool unresolvedKTV = pageItem3.UnresolvedKTH = false;
				pageItem2.UnresolvedKTV = unresolvedKTV;
			}
			pageItem.TablixCellTopItem = tablixCellParent;
			return pageItem;
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

		internal void CalculateVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			this.CalculateVertical(pageContext, topInParentSystem, bottomInParentSystem, siblings, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors, (double?)null);
		}

		internal void CalculateVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, double? sourceWidth)
		{
			this.AdjustOriginFromItemsAbove(siblings);
			this.OnThisVerticalPage = false;
			this.UnresolvedCKTV = false;
			this.UnresolvedCPB = false;
			this.ResolveChildren = true;
			if (pageContext.IgnorePageBreaks)
			{
				bool unresolvedPBE = this.UnresolvedPBS = false;
				this.UnresolvedPBE = unresolvedPBE;
			}
			else if (anyAncestorHasKT && (this.PageBreakAtStart || this.PageBreakAtEnd))
			{
				anyAncestorHasKT = false;
				foreach (PageItem ancestor in ancestors)
				{
					ancestor.KeepTogetherVertical = false;
					ancestor.UnresolvedKTV = false;
				}
			}
			if (pageContext.FullOnPage || hasUnpinnedAncestors)
			{
				this.UnresolvedKTV = this.KeepTogetherVertical;
				if (!pageContext.IgnorePageBreaks)
				{
					this.UnresolvedPBS = this.PageBreakAtStart;
					this.UnresolvedPBE = this.PageBreakAtEnd;
				}
			}
			double bottomInParentSystem2 = anyAncestorHasKT ? (topInParentSystem + pageContext.ColumnHeight) : bottomInParentSystem;
			if (this.HitsCurrentVerticalPage(topInParentSystem, bottomInParentSystem2) && (anyAncestorHasKT || hasUnpinnedAncestors || pageContext.FullOnPage || !this.ResolvePageBreakAtStart(pageContext, topInParentSystem, bottomInParentSystem)))
			{
				RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Top);
				if (x < topInParentSystem)
				{
					this.ResetHorizontal(true, sourceWidth);
				}
				this.OnThisVerticalPage = true;
				bool canOverwritePageBreak = pageContext.Common.CanOverwritePageBreak;
				bool canSetPageName = pageContext.Common.CanSetPageName;
				this.DetermineVerticalSize(pageContext, topInParentSystem, bottomInParentSystem, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors);
				if (this.InvalidateVerticalKT(anyAncestorHasKT, pageContext))
				{
					if (anyAncestorHasKT)
					{
						anyAncestorHasKT = false;
						foreach (PageItem ancestor2 in ancestors)
						{
							ancestor2.KeepTogetherVertical = false;
							ancestor2.UnresolvedKTV = false;
						}
					}
					this.KeepTogetherVertical = false;
					this.UnresolvedKTV = false;
				}
				if (anyAncestorHasKT)
				{
					this.UnresolvedKTV = this.KeepTogetherVertical;
				}
				if (!anyAncestorHasKT && !hasUnpinnedAncestors && !pageContext.FullOnPage)
				{
					this.ResolveVertical(pageContext, topInParentSystem, bottomInParentSystem, null, false, canOverwritePageBreak, canSetPageName);
				}
			}
		}

		protected virtual bool InvalidateVerticalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if (anyAncestorHasKT || this.KeepTogetherVertical)
			{
				RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Height);
				if (x > pageContext.ColumnHeight)
				{
					this.TraceInvalidatedKeepTogetherVertical(pageContext);
					return true;
				}
			}
			return false;
		}

		protected virtual void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
		}

		internal virtual bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			if (recalculate)
			{
				this.AdjustOriginFromItemsAbove(siblings);
			}
			return false;
		}

		internal void ResolveVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, PageItem[] siblings, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			if (resolveItem && this.NeedResolve)
			{
				this.ResolveChildren = true;
			}
			if (this.ResolveChildren)
			{
				this.ResolveChildren = false;
				this.AdjustOriginFromItemsAbove(siblings);
				this.OnThisVerticalPage = false;
				if (this.HitsCurrentVerticalPage(topInParentSystem, bottomInParentSystem) && !this.ResolvePageBreakAtStart(pageContext, topInParentSystem, bottomInParentSystem))
				{
					this.OnThisVerticalPage = true;
					this.ResolveKeepTogetherVertical(pageContext, topInParentSystem, bottomInParentSystem, resolveItem, canOverwritePageBreak, canSetPageName);
					if (this.UnresolvedPBE)
					{
						if (this.PageBreakAtEnd)
						{
							if (this.ItemPageSizes.Bottom <= bottomInParentSystem)
							{
								double num = bottomInParentSystem - this.ItemPageSizes.Bottom;
								this.ItemPageSizes.DeltaY += num;
								this.UnresolvedPBE = false;
								pageContext.Common.RegisterPageBreakProperties(this.PageBreakProperties, canOverwritePageBreak);
								if (pageContext.Common.DiagnosticsEnabled && num == 0.0)
								{
									pageContext.Common.TracePageBreakIgnoredAtBottomOfPage(this);
								}
							}
						}
						else
						{
							this.UnresolvedPBE = false;
						}
					}
				}
			}
		}

		internal void CalculateHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			this.CalculateHorizontal(pageContext, leftInParentSystem, rightInParentSystem, siblings, ancestors, ref anyAncestorHasKT, hasUnpinnedAncestors, (double?)null);
		}

		internal void CalculateHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors, double? sourceWidth)
		{
			if (pageContext.FullOnPage || hasUnpinnedAncestors)
			{
				this.UnresolvedKTH = this.KeepTogetherHorizontal;
			}
			this.ResolveChildren = true;
			if (this.OnThisVerticalPage)
			{
				if (pageContext.ResetHorizontal)
				{
					this.ResetHorizontal(false, sourceWidth);
				}
				this.AdjustOriginFromItemsAtLeft(siblings);
				double rightInParentSystem2 = anyAncestorHasKT ? (leftInParentSystem + pageContext.ColumnWidth) : rightInParentSystem;
				if (this.HitsCurrentHorizontalPage(leftInParentSystem, rightInParentSystem2))
				{
					this.DetermineHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors);
					if (this.InvalidateHorizontalKT(anyAncestorHasKT, pageContext))
					{
						if (anyAncestorHasKT)
						{
							anyAncestorHasKT = false;
							foreach (PageItem ancestor in ancestors)
							{
								ancestor.KeepTogetherHorizontal = false;
								ancestor.UnresolvedKTH = false;
							}
						}
						this.KeepTogetherHorizontal = false;
						this.UnresolvedKTH = false;
					}
					if (anyAncestorHasKT)
					{
						this.UnresolvedKTH = this.KeepTogetherHorizontal;
					}
					if (!anyAncestorHasKT && !hasUnpinnedAncestors && !pageContext.FullOnPage)
					{
						this.ResolveHorizontal(pageContext, leftInParentSystem, rightInParentSystem, null, false);
					}
				}
			}
		}

		protected virtual bool InvalidateHorizontalKT(bool anyAncestorHasKT, PageContext pageContext)
		{
			if (anyAncestorHasKT || this.KeepTogetherHorizontal)
			{
				RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Width);
				if (x > pageContext.ColumnWidth)
				{
					this.TraceInvalidatedKeepTogetherHorizontal(pageContext);
					return true;
				}
			}
			return false;
		}

		protected virtual void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
		}

		internal void ResolveHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, PageItem[] siblings, bool resolveItem)
		{
			if (resolveItem && this.NeedResolve)
			{
				this.ResolveChildren = true;
			}
			if (this.ResolveChildren)
			{
				this.ResolveChildren = false;
				if (this.OnThisVerticalPage)
				{
					this.AdjustOriginFromItemsAtLeft(siblings);
					if (this.HitsCurrentHorizontalPage(leftInParentSystem, rightInParentSystem))
					{
						this.ResolveKeepTogetherHorizontal(pageContext, leftInParentSystem, rightInParentSystem, resolveItem);
					}
				}
			}
		}

		private bool ResolvePageBreakAtStart(PageContext pageContext, double topInParentSystem, double bottomInParentSystem)
		{
			if (this.UnresolvedPBS)
			{
				this.UnresolvedPBS = false;
				if (this.PageBreakAtStart)
				{
					if (this.ItemPageSizes.Top > topInParentSystem)
					{
						RoundedDouble roundedDouble = new RoundedDouble(bottomInParentSystem - this.ItemPageSizes.Top);
						if (roundedDouble < pageContext.ColumnHeight)
						{
							this.ItemPageSizes.MoveVertical(roundedDouble.Value);
							pageContext.Common.RegisterPageBreakProperties(this.PageBreakProperties, false);
							return true;
						}
					}
					if (pageContext.Common.DiagnosticsEnabled && this.ItemPageSizes.Top == topInParentSystem)
					{
						pageContext.Common.TracePageBreakIgnoredAtTopOfPage(this);
					}
				}
			}
			return false;
		}

		protected bool StartsOnThisPage(PageContext pageContext, double topInParentSystem, double bottomInParentSystem)
		{
			if (this.OnThisVerticalPage && this.ItemPageSizes.Top >= topInParentSystem)
			{
				return true;
			}
			return false;
		}

		protected virtual bool ResolveKeepTogetherVertical(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, bool resolveItem, bool canOverwritePageBreak, bool canSetPageName)
		{
			if (!this.UnresolvedKTV)
			{
				if (this.StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
				{
					pageContext.Common.SetPageName(this.PageName, canSetPageName);
				}
				return false;
			}
			this.UnresolvedKTV = false;
			if (this.KeepTogetherVertical)
			{
				double num = bottomInParentSystem - this.ItemPageSizes.Top;
				RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Height);
				if (x <= num)
				{
					if (this.StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
					{
						pageContext.Common.SetPageName(this.PageName, canSetPageName);
					}
					return true;
				}
				if (x <= pageContext.ColumnHeight)
				{
					this.ItemPageSizes.MoveVertical(num);
					this.OnThisVerticalPage = false;
					this.TraceKeepTogetherVertical(pageContext);
					return true;
				}
				this.TraceInvalidatedKeepTogetherVertical(pageContext);
			}
			if (this.StartsOnThisPage(pageContext, topInParentSystem, bottomInParentSystem))
			{
				pageContext.Common.SetPageName(this.PageName, canSetPageName);
			}
			return false;
		}

		protected virtual bool ResolveKeepTogetherHorizontal(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, bool resolveItem)
		{
			if (!this.UnresolvedKTH)
			{
				return false;
			}
			this.UnresolvedKTH = false;
			if (this.KeepTogetherHorizontal)
			{
				double num = rightInParentSystem - this.ItemPageSizes.Left;
				RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Width);
				if (x <= num)
				{
					return true;
				}
				if (x <= pageContext.ColumnWidth)
				{
					this.ItemPageSizes.MoveHorizontal(num);
					this.TraceKeepTogetherHorizontal(pageContext);
					return true;
				}
				this.TraceInvalidatedKeepTogetherHorizontal(pageContext);
			}
			return false;
		}

		internal bool AboveCurrentVerticalPage(double topInParentSystem)
		{
			RoundedDouble x = new RoundedDouble(this.ItemPageSizes.Bottom);
			if (x < topInParentSystem)
			{
				return true;
			}
			if (x == topInParentSystem && this.ItemPageSizes.Height > 0.0)
			{
				return true;
			}
			return false;
		}

		internal bool HitsCurrentVerticalPage(double topInParentSystem, double bottomInParentSystem)
		{
			RoundedDouble roundedDouble = new RoundedDouble(this.ItemPageSizes.Bottom);
			if (roundedDouble < topInParentSystem)
			{
				return false;
			}
			if (roundedDouble == topInParentSystem && this.ItemPageSizes.Height > 0.0)
			{
				return false;
			}
			roundedDouble.Value = this.ItemPageSizes.Top;
			if (roundedDouble >= bottomInParentSystem)
			{
				return false;
			}
			return true;
		}

		internal bool HitsCurrentHorizontalPage(double leftInParentSystem, double rightInParentSystem)
		{
			RoundedDouble roundedDouble = new RoundedDouble(this.ItemPageSizes.Right);
			if (roundedDouble < leftInParentSystem)
			{
				return false;
			}
			if (roundedDouble == leftInParentSystem && this.ItemPageSizes.Width > 0.0)
			{
				return false;
			}
			roundedDouble.Value = this.ItemPageSizes.Left;
			if (roundedDouble >= rightInParentSystem)
			{
				return false;
			}
			return true;
		}

		internal virtual bool HitsCurrentPage(double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (this.HitsCurrentVerticalPage(pageTop, pageBottom) && this.HitsCurrentHorizontalPage(pageLeft, pageRight))
			{
				return true;
			}
			return false;
		}

		internal bool Release(double pageBottom, double pageRight)
		{
			RoundedDouble roundedDouble = new RoundedDouble(this.ItemPageSizes.Bottom);
			if (roundedDouble <= pageBottom)
			{
				roundedDouble.Value = this.ItemPageSizes.Right;
				if (roundedDouble <= pageRight)
				{
					return true;
				}
			}
			return false;
		}

		internal virtual bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			if (this.HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				this.WriteStartItemToStream(rplWriter, pageContext);
				this.RegisterTextBoxes(rplWriter, pageContext);
				this.OmitBorderOnPageBreak(rplWriter, pageLeft, pageTop, pageRight, pageBottom);
				return true;
			}
			return false;
		}

		internal virtual void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void ResetHorizontal(bool spanPages, double? width)
		{
			this.ItemPageSizes.Left = this.OriginalLeft;
			if (width.HasValue)
			{
				this.ItemPageSizes.Width = width.Value;
			}
			else
			{
				this.ItemPageSizes.Width = this.OriginalWidth;
			}
		}

		internal virtual void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
		}

		internal void OmitBorderOnPageBreak(double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			byte b = 15;
			b = (byte)(~b);
			this.m_rplItemState &= b;
			RoundedDouble x = new RoundedDouble(this.m_itemPageSizes.Top);
			if (x < pageTop)
			{
				this.m_rplItemState |= 1;
			}
			x = new RoundedDouble(this.m_itemPageSizes.Left);
			if (x < pageLeft)
			{
				this.m_rplItemState |= 4;
			}
			x = new RoundedDouble(this.m_itemPageSizes.Bottom);
			if (x > pageBottom)
			{
				this.m_rplItemState |= 2;
			}
			x = new RoundedDouble(this.m_itemPageSizes.Right);
			if (x > pageRight)
			{
				this.m_rplItemState |= 8;
			}
		}

		internal void AdjustOriginFromItemsAtLeft(PageItem[] siblings)
		{
			if (this.m_pageItemsLeft != null && siblings != null)
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
					else if (pageItem.OnThisVerticalPage)
					{
						itemSizes = pageItem.ItemPageSizes;
						num5 = this.ItemPageSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
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
					this.ItemPageSizes.Left += num;
					this.ItemPageSizes.DeltaX += num;
				}
			}
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings)
		{
			if (this.m_pageItemsAbove != null && siblings != null)
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
						this.m_pageItemsAbove.RemoveAt(i);
						i--;
					}
					else
					{
						itemSizes = pageItem.ItemPageSizes;
						num5 = this.ItemPageSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
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
					this.ItemPageSizes.Top += num;
					this.ItemPageSizes.DeltaY += num;
				}
				if (this.m_pageItemsAbove.Count == 0)
				{
					this.m_pageItemsAbove = null;
				}
			}
		}

		protected void TraceKeepTogetherHorizontal(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && this.Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item '{1}' kept together horizontally - Explicit - Pushed to next page", pageContext.Common.PageNumber, this.Source.Name);
			}
		}

		protected void TraceKeepTogetherVertical(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && this.Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Item '{1}' kept together vertically - Explicit - Pushed to next page", pageContext.Common.PageNumber, this.Source.Name);
			}
		}

		protected void TraceInvalidatedKeepTogetherHorizontal(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && this.Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Horizontal KeepTogether on Item '{1}' not honored - larger than page", pageContext.Common.PageNumber, this.Source.Name);
			}
		}

		protected void TraceInvalidatedKeepTogetherVertical(PageContext pageContext)
		{
			if (pageContext.Common.DiagnosticsEnabled && this.Source != null)
			{
				RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Info, "PR-DIAG [Page {0}] Vertical KeepTogether on Item '{1}' not honored - larger than page", pageContext.Common.PageNumber, this.Source.Name);
			}
		}

		internal void WritePageItemSizes(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((float)this.m_itemPageSizes.Left);
			spbifWriter.Write((float)this.m_itemPageSizes.Top);
			spbifWriter.Write((float)this.m_itemPageSizes.Width);
			spbifWriter.Write((float)this.m_itemPageSizes.Height);
			if (this.m_source == null)
			{
				spbifWriter.Write(0);
			}
			else
			{
				spbifWriter.Write(this.m_source.ZIndex);
			}
			spbifWriter.Write(this.m_rplItemState);
			spbifWriter.Write(this.m_offset);
		}

		internal RPLItemMeasurement WritePageItemSizes()
		{
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Left = (float)this.m_itemPageSizes.Left;
			rPLItemMeasurement.Top = (float)this.m_itemPageSizes.Top;
			rPLItemMeasurement.Width = (float)this.m_itemPageSizes.Width;
			rPLItemMeasurement.Height = (float)this.m_itemPageSizes.Height;
			if (this.m_source != null)
			{
				rPLItemMeasurement.ZIndex = this.m_source.ZIndex;
			}
			rPLItemMeasurement.State = this.m_rplItemState;
			rPLItemMeasurement.Element = (this.m_rplElement as RPLItem);
			return rPLItemMeasurement;
		}

		internal abstract void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext);

		internal virtual void CacheNonSharedProperties(PageContext pageContext)
		{
			if (pageContext.CacheNonSharedProps)
			{
				this.m_nonSharedOffset = 0L;
				if (pageContext.PropertyCacheState != PageContext.CacheState.CountPages)
				{
					if (pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
					{
						BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
						long position = propertyCacheWriter.BaseStream.Position;
						this.WriteNonSharedItemProps(propertyCacheWriter, null, pageContext);
						long position2 = propertyCacheWriter.BaseStream.Position;
						if (pageContext.ItemCacheSharedImageInfo != null)
						{
							propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.StreamName);
							propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.ImageBounderies.StartOffset);
							propertyCacheWriter.Write(pageContext.ItemCacheSharedImageInfo.ImageBounderies.EndOffset);
							pageContext.ItemCacheSharedImageInfo = null;
						}
						this.m_nonSharedOffset = propertyCacheWriter.BaseStream.Position;
						propertyCacheWriter.Write(position);
						propertyCacheWriter.Write(position2);
					}
					else
					{
						this.WriteStartItemToStream(new RPLWriter(), pageContext);
					}
				}
			}
		}

		internal void WriteElementProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			spbifWriter.Write((byte)15);
			this.WriteSharedItemProps(spbifWriter, rplWriter, pageContext, offset);
			if (this.m_nonSharedOffset != -1)
			{
				this.CopyCachedData(rplWriter, pageContext);
			}
			else
			{
				this.WriteNonSharedItemProps(spbifWriter, rplWriter, pageContext);
			}
			spbifWriter.Write((byte)255);
		}

		internal virtual void CopyCachedData(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(this.m_nonSharedOffset, SeekOrigin.Begin);
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (num2 == this.m_nonSharedOffset)
			{
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				this.CopyData(propertyCacheReader, rplWriter, this.m_nonSharedOffset - num);
			}
			else
			{
				this.CopyDataAndResolve(rplWriter, pageContext, num, num2, false);
			}
		}

		internal void CopyDataAndResolve(RPLWriter rplWriter, PageContext pageContext, long startOffset, long endOffset, bool ignoreDelimiter)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(endOffset, SeekOrigin.Begin);
			string key = propertyCacheReader.ReadString();
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (ignoreDelimiter)
			{
				endOffset--;
			}
			propertyCacheReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
			this.CopyData(propertyCacheReader, rplWriter, num - startOffset);
			bool flag = false;
			if (pageContext.SharedImages != null)
			{
				object obj = pageContext.SharedImages[key];
				if (obj != null)
				{
					rplWriter.BinaryWriter.Write((byte)42);
					rplWriter.BinaryWriter.Write((byte)2);
					rplWriter.BinaryWriter.Write((long)obj);
					flag = true;
				}
			}
			else
			{
				pageContext.SharedImages = new Hashtable();
			}
			if (!flag)
			{
				pageContext.SharedImages.Add(key, rplWriter.BinaryWriter.BaseStream.Position);
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				propertyCacheReader.ReadByte();
				if (propertyCacheReader.ReadByte() == 0)
				{
					propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
					this.CopyData(propertyCacheReader, rplWriter, endOffset - num);
					return;
				}
				ItemBoundaries itemBoundaries = pageContext.CacheSharedImages[key] as ItemBoundaries;
				propertyCacheReader.BaseStream.Seek(itemBoundaries.StartOffset, SeekOrigin.Begin);
				this.CopyData(propertyCacheReader, rplWriter, itemBoundaries.EndOffset - itemBoundaries.StartOffset);
			}
			propertyCacheReader.BaseStream.Seek(num2, SeekOrigin.Begin);
			this.CopyData(propertyCacheReader, rplWriter, endOffset - num2);
		}

		internal void CopyData(BinaryReader cacheReader, RPLWriter rplWriter, long length)
		{
			byte[] copyBuffer = rplWriter.CopyBuffer;
			while (length >= copyBuffer.Length)
			{
				cacheReader.Read(copyBuffer, 0, copyBuffer.Length);
				rplWriter.BinaryWriter.Write(copyBuffer, 0, copyBuffer.Length);
				length -= copyBuffer.Length;
			}
			if (length > 0)
			{
				cacheReader.Read(copyBuffer, 0, (int)length);
				rplWriter.BinaryWriter.Write(copyBuffer, 0, (int)length);
			}
		}

		internal void WriteElementProps(RPLElementProps elemProps, PageContext pageContext)
		{
			this.WriteSharedItemProps(elemProps, pageContext);
			this.WriteNonSharedItemProps(elemProps, pageContext);
		}

		protected void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
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
			if (this.m_source != null && !this.m_source.DocumentMapLabel.IsExpression && this.m_source.DocumentMapLabel.Value != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(this.m_source.DocumentMapLabel.Value);
			}
			this.WriteCustomSharedItemProps(spbifWriter, rplWriter, pageContext);
			this.WriteSharedStyle(spbifWriter, null, pageContext);
			spbifWriter.Write((byte)255);
		}

		protected void WriteSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
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
			if (this.m_source != null && !this.m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemPropsDef.Label = this.m_source.DocumentMapLabel.Value;
			}
			this.WriteCustomSharedItemProps(rPLItemPropsDef, pageContext);
			rPLItemPropsDef.SharedStyle = this.WriteSharedStyle(null, pageContext);
		}

		internal virtual void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			if (styleDef == null)
			{
				styleDef = this.m_source.Style;
			}
			if (styleDef != null)
			{
				ReportProperty reportProperty = ((StyleBase)styleDef)[name];
				if (reportProperty != null && reportProperty.IsExpression)
				{
					object obj = ((StyleBaseInstance)style)[name];
					if (obj == null)
					{
						PageItem.WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
					}
					else
					{
						switch (spbifType)
						{
						case 33:
							break;
						case 37:
						{
							int num = (int)obj;
							if (num > 0)
							{
								spbifWriter.Write(spbifType);
								spbifWriter.Write(num);
							}
							break;
						}
						default:
						{
							bool flag = true;
							byte? stylePropByte = PageItem.GetStylePropByte(spbifType, obj, ref flag);
							int? nullable = stylePropByte;
							if (nullable.HasValue)
							{
								spbifWriter.Write(spbifType);
								spbifWriter.Write(stylePropByte.Value);
							}
							else if (flag)
							{
								string text = obj.ToString();
								if (text != null)
								{
									spbifWriter.Write(spbifType);
									spbifWriter.Write(text);
								}
							}
							break;
						}
						}
					}
				}
			}
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			if (styleDef == null)
			{
				styleDef = this.m_source.Style;
			}
			if (styleDef != null)
			{
				ReportProperty reportProperty = ((StyleBase)styleDef)[name];
				if (reportProperty != null && reportProperty.IsExpression)
				{
					object obj = ((StyleBaseInstance)style)[name];
					if (obj == null)
					{
						PageItem.WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
					}
					else
					{
						switch (spbifType)
						{
						case 33:
							break;
						case 37:
						{
							int num = (int)obj;
							if (num > 0)
							{
								rplStyleProps.Add(spbifType, num);
							}
							break;
						}
						default:
						{
							bool flag = true;
							byte? stylePropByte = PageItem.GetStylePropByte(spbifType, obj, ref flag);
							int? nullable = stylePropByte;
							if (nullable.HasValue)
							{
								rplStyleProps.Add(spbifType, stylePropByte.Value);
							}
							else if (flag)
							{
								string text = obj.ToString();
								if (text != null)
								{
									rplStyleProps.Add(spbifType, text);
								}
							}
							break;
						}
						}
					}
				}
			}
		}

		internal static byte? GetStylePropByte(byte spbifType, object styleProp, ref bool convertToString)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate((FontStyles)styleProp);
			case 22:
				return StyleEnumConverter.Translate((FontWeights)styleProp);
			case 24:
				return StyleEnumConverter.Translate((TextDecorations)styleProp);
			case 25:
				return StyleEnumConverter.Translate((TextAlignments)styleProp);
			case 26:
				return StyleEnumConverter.Translate((VerticalAlignments)styleProp);
			case 29:
				return StyleEnumConverter.Translate((Directions)styleProp);
			case 30:
				return StyleEnumConverter.Translate((WritingModes)styleProp);
			case 31:
				return StyleEnumConverter.Translate((UnicodeBiDiTypes)styleProp);
			case 38:
				return StyleEnumConverter.Translate((Calendars)styleProp);
			case 5:
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 6:
			case 7:
			case 8:
			case 9:
				convertToString = false;
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 35:
				return StyleEnumConverter.Translate((BackgroundRepeatTypes)styleProp);
			default:
				return null;
			}
		}

		internal static void WriteStyleReportProperty(ReportProperty styleProp, BinaryWriter spbifWriter, byte spbifType)
		{
			if (styleProp != null)
			{
				if (styleProp is ReportStringProperty)
				{
					ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
					if (reportStringProperty.Value != null)
					{
						spbifWriter.Write(spbifType);
						spbifWriter.Write(reportStringProperty.Value);
					}
				}
				else if (styleProp is ReportSizeProperty)
				{
					ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
					if (reportSizeProperty.Value != null)
					{
						spbifWriter.Write(spbifType);
						spbifWriter.Write(reportSizeProperty.Value.ToString());
					}
				}
				else if (styleProp is ReportColorProperty)
				{
					ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
					if (reportColorProperty.Value != null)
					{
						spbifWriter.Write(spbifType);
						spbifWriter.Write(reportColorProperty.Value.ToString());
					}
				}
				else if (styleProp is ReportIntProperty)
				{
					ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
					if (reportIntProperty.Value > 0)
					{
						spbifWriter.Write(spbifType);
						spbifWriter.Write(reportIntProperty.Value);
					}
				}
				else
				{
					byte? stylePropByte = PageItem.GetStylePropByte(spbifType, styleProp);
					int? nullable = stylePropByte;
					if (nullable.HasValue)
					{
						spbifWriter.Write(spbifType);
						spbifWriter.Write(stylePropByte.Value);
					}
				}
			}
		}

		internal static void WriteStyleReportProperty(ReportProperty styleProp, RPLStyleProps rplStyleProps, byte spbifType)
		{
			if (styleProp != null)
			{
				if (styleProp is ReportStringProperty)
				{
					ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
					if (reportStringProperty.Value != null)
					{
						rplStyleProps.Add(spbifType, reportStringProperty.Value);
					}
				}
				else if (styleProp is ReportSizeProperty)
				{
					ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
					if (reportSizeProperty.Value != null)
					{
						rplStyleProps.Add(spbifType, reportSizeProperty.Value.ToString());
					}
				}
				else if (styleProp is ReportColorProperty)
				{
					ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
					if (reportColorProperty.Value != null)
					{
						rplStyleProps.Add(spbifType, reportColorProperty.Value.ToString());
					}
				}
				else if (styleProp is ReportIntProperty)
				{
					ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
					if (reportIntProperty.Value > 0)
					{
						rplStyleProps.Add(spbifType, reportIntProperty.Value);
					}
				}
				else
				{
					byte? stylePropByte = PageItem.GetStylePropByte(spbifType, styleProp);
					int? nullable = stylePropByte;
					if (nullable.HasValue)
					{
						rplStyleProps.Add(spbifType, stylePropByte.Value);
					}
				}
			}
		}

		internal static byte? GetStylePropByte(byte spbifType, ReportProperty styleProp)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontStyles>)styleProp).Value);
			case 22:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontWeights>)styleProp).Value);
			case 24:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextDecorations>)styleProp).Value);
			case 25:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextAlignments>)styleProp).Value);
			case 26:
				return StyleEnumConverter.Translate(((ReportEnumProperty<VerticalAlignments>)styleProp).Value);
			case 29:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Directions>)styleProp).Value);
			case 30:
				return StyleEnumConverter.Translate(((ReportEnumProperty<WritingModes>)styleProp).Value);
			case 31:
				return StyleEnumConverter.Translate(((ReportEnumProperty<UnicodeBiDiTypes>)styleProp).Value);
			case 38:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Calendars>)styleProp).Value);
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BorderStyles>)styleProp).Value);
			case 35:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BackgroundRepeatTypes>)styleProp).Value);
			default:
				return null;
			}
		}

		internal void WriteStyleProp(Style style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = ((StyleBase)style)[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				PageItem.WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
			}
		}

		internal static void WriteStyleProp(Style style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = ((StyleBase)style)[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				PageItem.WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
			}
		}

		internal void WriteBackgroundImage(Style style, bool writeShared, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (style == null)
			{
				style = this.m_source.Style;
			}
			if (style != null)
			{
				BackgroundImage backgroundImage = style.BackgroundImage;
				if (backgroundImage != null && backgroundImage.Instance != null)
				{
					if (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null)
					{
						return;
					}
					if (backgroundImage.Value != null)
					{
						if (backgroundImage.Value.IsExpression)
						{
							if (!writeShared)
							{
								spbifWriter.Write((byte)33);
								this.WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null, false);
							}
						}
						else if (writeShared && backgroundImage.Value.Value != null)
						{
							spbifWriter.Write((byte)33);
							this.WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null, true);
						}
					}
					if (backgroundImage.BackgroundRepeat != null)
					{
						if (backgroundImage.BackgroundRepeat.IsExpression)
						{
							if (!writeShared)
							{
								spbifWriter.Write((byte)35);
								spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
							}
						}
						else if (writeShared)
						{
							spbifWriter.Write((byte)35);
							spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
						}
					}
				}
			}
		}

		internal void WriteBackgroundImage(Style style, bool writeShared, RPLStyleProps rplStyleProps, PageContext pageContext)
		{
			if (style == null)
			{
				style = this.m_source.Style;
			}
			if (style != null)
			{
				BackgroundImage backgroundImage = style.BackgroundImage;
				if (backgroundImage != null && backgroundImage.Instance != null)
				{
					if (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null)
					{
						return;
					}
					if (backgroundImage.Value != null)
					{
						if (backgroundImage.Value.IsExpression)
						{
							if (!writeShared)
							{
								RPLImageData value = new RPLImageData();
								this.WriteImage(backgroundImage.Instance, null, ref value, pageContext, null);
								rplStyleProps.Add(33, value);
							}
						}
						else if (writeShared && backgroundImage.Value.Value != null)
						{
							RPLImageData value2 = new RPLImageData();
							this.WriteImage(backgroundImage.Instance, null, ref value2, pageContext, null);
							rplStyleProps.Add(33, value2);
						}
					}
					if (backgroundImage.BackgroundRepeat != null)
					{
						if (backgroundImage.BackgroundRepeat.IsExpression)
						{
							if (!writeShared)
							{
								rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
							}
						}
						else if (writeShared)
						{
							rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
						}
					}
				}
			}
		}

		internal void WriteSharedStyle(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			if (style == null)
			{
				style = this.SharedStyle;
			}
			if (style != null)
			{
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				this.WriteItemSharedStyleProps(spbifWriter, style, pageContext);
				this.WriteBorderProps(spbifWriter, style);
				spbifWriter.Write((byte)255);
			}
		}

		internal virtual void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColor, 0);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
			this.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal RPLStyleProps WriteSharedStyle(Style style, PageContext pageContext)
		{
			if (style == null)
			{
				style = this.SharedStyle;
			}
			if (style == null)
			{
				return null;
			}
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			this.WriteItemSharedStyleProps(rPLStyleProps, style, pageContext);
			this.WriteBorderProps(rPLStyleProps, style);
			return rPLStyleProps;
		}

		internal virtual void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal virtual void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
		}

		internal virtual void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
		}

		private void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.SourceUniqueName);
			if (this.m_source != null && this.m_source.DocumentMapLabel.IsExpression && this.m_source.Instance.DocumentMapLabel != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(this.m_source.Instance.DocumentMapLabel);
			}
			this.WriteCustomNonSharedItemProps(spbifWriter, pageContext);
			this.WriteNonSharedStyle(spbifWriter, null, null, pageContext);
			spbifWriter.Write((byte)255);
		}

		private void WriteNonSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
		{
			elemProps.UniqueName = this.SourceUniqueName;
			RPLItemProps rPLItemProps = elemProps as RPLItemProps;
			if (this.m_source != null && this.m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemProps.Label = this.m_source.Instance.DocumentMapLabel;
			}
			this.WriteCustomNonSharedItemProps(rPLItemProps, pageContext);
			rPLItemProps.NonSharedStyle = this.WriteNonSharedStyle(null, null, pageContext);
		}

		internal virtual void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomNonSharedItemProps(RPLElementProps elemProps, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				this.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal virtual void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = this.SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count != 0)
			{
				if (style == null)
				{
					style = this.NonSharedStyle;
				}
				if (style != null)
				{
					bool flag = false;
					spbifWriter.Write((byte)6);
					spbifWriter.Write((byte)1);
					for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
					{
						switch (nonSharedStyleAttributes[i])
						{
						case StyleAttributeNames.BackgroundImage:
						case StyleAttributeNames.BackgroundImageRepeat:
							if (!flag)
							{
								flag = true;
								this.WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
							}
							break;
						default:
							this.WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
							break;
						}
					}
					spbifWriter.Write((byte)255);
				}
			}
		}

		internal virtual void WriteNonSharedStyleWithoutTag(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = this.SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count != 0)
			{
				if (style == null)
				{
					style = this.NonSharedStyle;
				}
				if (style != null)
				{
					bool flag = false;
					spbifWriter.Write((byte)1);
					for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
					{
						switch (nonSharedStyleAttributes[i])
						{
						case StyleAttributeNames.BackgroundImage:
						case StyleAttributeNames.BackgroundImageRepeat:
							if (!flag)
							{
								flag = true;
								this.WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
							}
							break;
						default:
							this.WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
							break;
						}
					}
					spbifWriter.Write((byte)255);
				}
			}
		}

		internal virtual void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				this.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProps(rplStyleProps, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal virtual RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = this.SharedStyle;
			}
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count != 0)
			{
				if (style == null)
				{
					style = this.NonSharedStyle;
				}
				if (style == null)
				{
					return null;
				}
				bool flag = false;
				RPLStyleProps rPLStyleProps = new RPLStyleProps();
				for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
				{
					switch (nonSharedStyleAttributes[i])
					{
					case StyleAttributeNames.BackgroundImage:
					case StyleAttributeNames.BackgroundImageRepeat:
						if (!flag)
						{
							flag = true;
							this.WriteItemNonSharedStyleProps(rPLStyleProps, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
						}
						break;
					default:
						this.WriteNonSharedStyleProp(rPLStyleProps, styleDef, style, nonSharedStyleAttributes[i], pageContext);
						break;
					}
				}
				return rPLStyleProps;
			}
			return null;
		}

		internal virtual void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
		}

		internal virtual void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
		}

		internal void WriteInvalidImage(BinaryWriter spbifWriter, PageContext pageContext, GDIImageProps gdiImageProps)
		{
			long position = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)42);
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable["InvalidImage"];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			hashtable.Add("InvalidImage", position);
			spbifWriter.Write((byte)0);
			System.Drawing.Image image = (Bitmap)HPBProcessing.HPBResourceManager.GetObject("InvalidImage");
			if (image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				spbifWriter.Write((byte)2);
				spbifWriter.Write((int)memoryStream.Length);
				this.WriteStreamContent(memoryStream, spbifWriter);
				if (gdiImageProps == null)
				{
					gdiImageProps = new GDIImageProps(image);
				}
				image.Dispose();
			}
			this.WriteImageProperties(null, "InvalidImage", spbifWriter, gdiImageProps);
			spbifWriter.Write((byte)255);
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, BinaryWriter spbifWriter, PageContext pageContext, GDIImageProps gdiImage, bool writeShared)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			long position = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)42);
			if (text != null)
			{
				this.WriteShareableImages(imageInstance, text, spbifWriter, pageContext, position, gdiImage, writeShared);
			}
			else
			{
				spbifWriter.Write((byte)1);
				this.WriteImageProperties(imageInstance, null, spbifWriter, gdiImage);
				spbifWriter.Write((byte)255);
			}
		}

		internal void WriteInvalidImage(RPLImageProps elemProps, PageContext pageContext, GDIImageProps gdiImageProps)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable["InvalidImage"];
				if (obj != null)
				{
					elemProps.Image = (RPLImageData)obj;
					return;
				}
			}
			RPLImageData rPLImageData = new RPLImageData();
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			rPLImageData.IsShared = true;
			hashtable.Add("InvalidImage", rPLImageData);
			rPLImageData.ImageName = "InvalidImage";
			System.Drawing.Image image = (Bitmap)HPBProcessing.HPBResourceManager.GetObject("InvalidImage");
			if (image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, image.RawFormat);
				rPLImageData.ImageData = memoryStream.ToArray();
				if (gdiImageProps == null)
				{
					rPLImageData.GDIImageProps = new GDIImageProps(image);
				}
				image.Dispose();
			}
			elemProps.Image = rPLImageData;
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, RPLImageProps elemProps, PageContext pageContext, GDIImageProps gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			RPLImageData rPLImageData = new RPLImageData();
			if (text != null)
			{
				this.WriteShareableImages(imageInstance, text, ref rPLImageData, pageContext, gdiImage);
			}
			else
			{
				this.WriteImageProperties(imageInstance, null, rPLImageData, gdiImage);
			}
			elemProps.Image = rPLImageData;
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, ref RPLImageData imageData, PageContext pageContext, GDIImageProps gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			if (text != null)
			{
				this.WriteShareableImages(imageInstance, text, ref imageData, pageContext, gdiImage);
			}
			else
			{
				this.WriteImageProperties(imageInstance, null, imageData, gdiImage);
			}
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, GDIImageProps gdiImage)
		{
			if (streamName != null)
			{
				spbifWriter.Write((byte)1);
				spbifWriter.Write(streamName);
			}
			if (imageInstance != null)
			{
				if (imageInstance.MIMEType != null)
				{
					spbifWriter.Write((byte)0);
					spbifWriter.Write(imageInstance.MIMEType);
				}
				if (imageInstance.ImageData != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write(imageInstance.ImageData.Length);
					spbifWriter.Write(imageInstance.ImageData);
				}
			}
			if (gdiImage != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(gdiImage.Width);
				spbifWriter.Write((byte)4);
				spbifWriter.Write(gdiImage.Height);
				spbifWriter.Write((byte)5);
				spbifWriter.Write(gdiImage.HorizontalResolution);
				spbifWriter.Write((byte)6);
				spbifWriter.Write(gdiImage.VerticalResolution);
				if (gdiImage.RawFormat.Equals(ImageFormat.Bmp))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)0);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Jpeg))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)1);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Gif))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)2);
				}
				else if (gdiImage.RawFormat.Equals(ImageFormat.Png))
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)3);
				}
			}
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, RPLImageData imageData, GDIImageProps gdiImage)
		{
			imageData.ImageName = streamName;
			if (imageInstance != null)
			{
				imageData.ImageMimeType = imageInstance.MIMEType;
				imageData.ImageData = imageInstance.ImageData;
			}
			imageData.GDIImageProps = gdiImage;
		}

		private void WriteStreamContent(Stream sourceStream, BinaryWriter spbifWriter)
		{
			byte[] buffer = new byte[1024];
			long num = 0L;
			int num2 = 0;
			sourceStream.Position = 0L;
			for (; num < sourceStream.Length; num += num2)
			{
				num2 = sourceStream.Read(buffer, 0, 1024);
				spbifWriter.Write(buffer, 0, num2);
			}
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, PageContext pageContext, long offsetStart, GDIImageProps gdiImage, bool writeShared)
		{
			if (this.m_nonSharedOffset == -1 || writeShared)
			{
				Hashtable hashtable = pageContext.SharedImages;
				if (hashtable != null)
				{
					object obj = hashtable[streamName];
					if (obj != null)
					{
						spbifWriter.Write((byte)2);
						spbifWriter.Write((long)obj);
						return;
					}
				}
				if (hashtable == null)
				{
					hashtable = (pageContext.SharedImages = new Hashtable());
				}
				hashtable.Add(streamName, offsetStart);
				spbifWriter.Write((byte)0);
				this.WriteImageProperties(imageInstance, streamName, spbifWriter, gdiImage);
				spbifWriter.Write((byte)255);
			}
			else
			{
				Hashtable hashtable3 = pageContext.CacheSharedImages;
				if (hashtable3 != null)
				{
					object obj2 = hashtable3[streamName];
					if (obj2 != null)
					{
						spbifWriter.Write((byte)2);
						pageContext.ItemCacheSharedImageInfo = new CachedSharedImageInfo(streamName, new ItemBoundaries(offsetStart, spbifWriter.BaseStream.Position));
						return;
					}
				}
				else
				{
					hashtable3 = (pageContext.CacheSharedImages = new Hashtable());
				}
				spbifWriter.Write((byte)0);
				this.WriteImageProperties(imageInstance, streamName, spbifWriter, gdiImage);
				spbifWriter.Write((byte)255);
				ItemBoundaries itemBoundaries = new ItemBoundaries(offsetStart, spbifWriter.BaseStream.Position);
				pageContext.ItemCacheSharedImageInfo = new CachedSharedImageInfo(streamName, itemBoundaries);
				hashtable3.Add(streamName, itemBoundaries);
			}
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, ref RPLImageData imageData, PageContext pageContext, GDIImageProps gdiImage)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable[streamName];
				if (obj != null)
				{
					imageData = (RPLImageData)obj;
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			imageData.IsShared = true;
			hashtable.Add(streamName, imageData);
			this.WriteImageProperties(imageInstance, streamName, imageData, gdiImage);
		}

		internal void WriteActionInfo(ActionInfo actionInfo, BinaryWriter spbifWriter)
		{
			if (actionInfo != null)
			{
				ActionCollection actions = actionInfo.Actions;
				if (actions != null && actions.Count != 0)
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.Action action = null;
					ActionInstance actionInstance = null;
					spbifWriter.Write((byte)7);
					spbifWriter.Write((byte)2);
					spbifWriter.Write(actions.Count);
					for (int i = 0; i < actions.Count; i++)
					{
						action = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Action>)actions)[i];
						actionInstance = action.Instance;
						spbifWriter.Write((byte)3);
						if (action.Hyperlink != null)
						{
							ReportUrl hyperlink = actionInstance.Hyperlink;
							if (hyperlink != null)
							{
								Uri uri = hyperlink.ToUri();
								if ((Uri)null != uri)
								{
									spbifWriter.Write((byte)6);
									spbifWriter.Write(uri.AbsoluteUri);
								}
							}
						}
						spbifWriter.Write((byte)255);
					}
					spbifWriter.Write((byte)255);
				}
			}
		}

		internal static RPLActionInfo WriteActionInfo(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return null;
			}
			ActionCollection actions = actionInfo.Actions;
			if (actions != null && actions.Count != 0)
			{
				RPLActionInfo rPLActionInfo = new RPLActionInfo(actions.Count);
				PageItem.WriteAction(actions, rPLActionInfo);
				return rPLActionInfo;
			}
			return null;
		}

		internal static void WriteAction(ActionCollection actions, RPLActionInfo rplActionInfo)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Action action = null;
			ActionInstance actionInstance = null;
			RPLAction rPLAction = null;
			for (int i = 0; i < actions.Count; i++)
			{
				action = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Action>)actions)[i];
				actionInstance = action.Instance;
				rPLAction = new RPLAction();
				if (action.Hyperlink != null)
				{
					ReportUrl hyperlink = actionInstance.Hyperlink;
					if (hyperlink != null)
					{
						Uri uri = hyperlink.ToUri();
						if ((Uri)null != uri)
						{
							rPLAction.Hyperlink = uri.AbsoluteUri;
						}
					}
				}
				rplActionInfo.Actions[i] = rPLAction;
			}
		}

		internal bool WriteObjectValue(BinaryWriter spbifWriter, byte name, TypeCode typeCode, object value)
		{
			bool result = false;
			spbifWriter.Write(name);
			switch (typeCode)
			{
			case TypeCode.Byte:
				spbifWriter.Write((byte)value);
				break;
			case TypeCode.Decimal:
				spbifWriter.Write((decimal)value);
				break;
			case TypeCode.Double:
				spbifWriter.Write((double)value);
				break;
			case TypeCode.Int16:
				spbifWriter.Write((short)value);
				break;
			case TypeCode.Int32:
				spbifWriter.Write((int)value);
				break;
			case TypeCode.Int64:
				spbifWriter.Write((long)value);
				break;
			case TypeCode.SByte:
				spbifWriter.Write((sbyte)value);
				break;
			case TypeCode.Single:
				spbifWriter.Write((float)value);
				break;
			case TypeCode.UInt16:
				spbifWriter.Write((ushort)value);
				break;
			case TypeCode.UInt32:
				spbifWriter.Write((uint)value);
				break;
			case TypeCode.UInt64:
				spbifWriter.Write((ulong)value);
				break;
			case TypeCode.DateTime:
				spbifWriter.Write(((DateTime)value).ToBinary());
				break;
			case TypeCode.Boolean:
				spbifWriter.Write((bool)value);
				break;
			default:
				spbifWriter.Write(value.ToString());
				result = true;
				break;
			}
			return result;
		}

		internal bool WriteObjectValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			bool result = false;
			switch (typeCode)
			{
			default:
				result = true;
				break;
			case TypeCode.Boolean:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				break;
			}
			return result;
		}
	}
}
