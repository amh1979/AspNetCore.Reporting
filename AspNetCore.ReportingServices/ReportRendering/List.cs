using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class List : DataRegion
	{
		private ListContentCollection m_contents;

		public override bool PageBreakAtEnd
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping == null)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtEnd;
				}
				if (!((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtEnd)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping.PageBreakAtEnd;
				}
				return true;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping == null)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtStart;
				}
				if (!((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtStart)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping.PageBreakAtStart;
				}
				return true;
			}
		}

		public bool GroupBreakAtStart
		{
			get
			{
				return this.Contents[0].PageBreakAtStart;
			}
		}

		public bool GroupBreakAtEnd
		{
			get
			{
				return this.Contents[0].PageBreakAtEnd;
			}
		}

		public ListContentCollection Contents
		{
			get
			{
				ListContentCollection listContentCollection = this.m_contents;
				if (this.m_contents == null)
				{
					listContentCollection = new ListContentCollection(this);
					if (base.RenderingContext.CacheState)
					{
						this.m_contents = listContentCollection;
					}
				}
				return listContentCollection;
			}
		}

		public override bool NoRows
		{
			get
			{
				if (base.ReportItemInstance != null && ((ListInstance)base.ReportItemInstance).ListContents.Count != 0)
				{
					return false;
				}
				return true;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((ListInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal List(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.List reportItemDef, ListInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public bool IsListContentOnThisPage(int contentIndex, int pageNumber, int listStartPage, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((ListInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			if (((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemInstance.ReportItemDef).Grouping != null)
			{
				Global.Tracer.Assert(contentIndex >= 0 && contentIndex < childrenStartAndEndPages.Count);
				if (contentIndex >= childrenStartAndEndPages.Count)
				{
					return false;
				}
				RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[contentIndex];
				startPage = renderingPagesRanges.StartPage;
				endPage = renderingPagesRanges.EndPage;
				if (pageNumber >= startPage)
				{
					return pageNumber <= endPage;
				}
				return false;
			}
			pageNumber -= listStartPage;
			Global.Tracer.Assert(pageNumber >= 0 && pageNumber < childrenStartAndEndPages.Count);
			RenderingPagesRanges renderingPagesRanges2 = childrenStartAndEndPages[pageNumber];
			startPage = pageNumber;
			endPage = pageNumber;
			if (contentIndex >= renderingPagesRanges2.StartRow)
			{
				return contentIndex < renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails;
			}
			return false;
		}

		public void GetListContentOnPage(int page, int listStartPage, out int startChild, out int endChild)
		{
			startChild = -1;
			endChild = -1;
			if (base.ReportItemInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = ((ListInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					if (((AspNetCore.ReportingServices.ReportProcessing.List)base.ReportItemInstance.ReportItemDef).Grouping != null)
					{
						RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startChild, ref endChild);
					}
					else if (childrenStartAndEndPages != null)
					{
						page -= listStartPage;
						Global.Tracer.Assert(page >= 0 && page < childrenStartAndEndPages.Count);
						RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[page];
						startChild = renderingPagesRanges.StartRow;
						endChild = startChild + renderingPagesRanges.NumberOfDetails - 1;
					}
				}
			}
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (!base.SkipSearch && !this.NoRows)
			{
				bool flag = false;
				ListContentCollection contents = this.Contents;
				if (searchContext.ItemStartPage != searchContext.ItemEndPage)
				{
					int num = 0;
					int num2 = 0;
					SearchContext searchContext2 = new SearchContext(searchContext);
					this.GetListContentOnPage(searchContext.SearchPage, searchContext.ItemStartPage, out num, out num2);
					int itemStartPage = default(int);
					int itemEndPage = default(int);
					this.IsListContentOnThisPage(num, searchContext.SearchPage, searchContext.ItemStartPage, out itemStartPage, out itemEndPage);
					searchContext2.ItemStartPage = itemStartPage;
					searchContext2.ItemEndPage = itemEndPage;
					flag = List.SearchPartialList(contents, searchContext2, num, num);
					num++;
					if (!flag && num < num2)
					{
						searchContext2.ItemStartPage = searchContext.SearchPage;
						searchContext2.ItemEndPage = searchContext.SearchPage;
						flag = List.SearchPartialList(contents, searchContext2, num, num2 - 1);
						num = num2;
					}
					if (!flag && num == num2)
					{
						this.IsListContentOnThisPage(num2, searchContext.SearchPage, searchContext.ItemStartPage, out itemStartPage, out itemEndPage);
						searchContext2.ItemStartPage = itemStartPage;
						searchContext2.ItemEndPage = itemEndPage;
						flag = List.SearchPartialList(contents, searchContext2, num2, num2);
					}
				}
				else
				{
					flag = List.SearchFullList(contents, searchContext);
				}
				return flag;
			}
			return false;
		}

		internal static bool SearchPartialList(ListContentCollection contents, SearchContext searchContext, int startChild, int endChild)
		{
			if (contents == null)
			{
				return false;
			}
			bool flag = false;
			ListContent listContent = null;
			while (startChild <= endChild && !flag)
			{
				listContent = contents[startChild];
				flag = listContent.ReportItemCollection.Search(searchContext);
				startChild++;
			}
			return flag;
		}

		internal static bool SearchFullList(ListContentCollection contents, SearchContext searchContext)
		{
			if (contents == null)
			{
				return false;
			}
			bool flag = false;
			ListContent listContent = null;
			for (int i = 0; i < contents.Count; i++)
			{
				if (flag)
				{
					break;
				}
				listContent = contents[i];
				if (!listContent.Hidden)
				{
					flag = listContent.ReportItemCollection.Search(searchContext);
				}
			}
			return flag;
		}
	}
}
