using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportItemCollection
	{
		private ReportItem[] m_reportItems;

		private AspNetCore.ReportingServices.ReportProcessing.ReportItemCollection m_reportItemColDef;

		private ReportItemColInstance m_reportItemColInstance;

		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		private RenderingContext m_renderingContext;

		public ReportItem this[int index]
		{
			get
			{
				if (0 <= index && index < this.Count)
				{
					ReportItem reportItem = null;
					if (this.m_reportItems == null || this.m_reportItems[index] == null)
					{
						int num = 0;
						bool flag = false;
						AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef = null;
						this.m_reportItemColDef.GetReportItem(index, out flag, out num, out reportItemDef);
						NonComputedUniqueNames nonComputedUniqueNames = null;
						ReportItemInstance reportItemInstance = null;
						if (!flag)
						{
							if (this.m_childrenNonComputedUniqueNames != null)
							{
								nonComputedUniqueNames = this.m_childrenNonComputedUniqueNames[num];
							}
						}
						else if (this.m_reportItemColInstance != null)
						{
							reportItemInstance = this.m_reportItemColInstance[num];
						}
						reportItem = ReportItem.CreateItem(index, reportItemDef, reportItemInstance, this.m_renderingContext, nonComputedUniqueNames);
						if (this.m_renderingContext.CacheState)
						{
							if (this.m_reportItems == null)
							{
								this.m_reportItems = new ReportItem[this.Count];
							}
							this.m_reportItems[index] = reportItem;
						}
					}
					else
					{
						reportItem = this.m_reportItems[index];
					}
					return reportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_reportItemColDef.Count;
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				return this.m_renderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_reportItemColDef.ID];
			}
			set
			{
				this.m_renderingContext.RenderingInfoManager.SharedRenderingInfo[this.m_reportItemColDef.ID] = value;
			}
		}

		internal ReportItemCollection(AspNetCore.ReportingServices.ReportProcessing.ReportItemCollection reportItemColDef, ReportItemColInstance reportItemColInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
		{
			if (reportItemColInstance != null)
			{
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(renderingContext.ChunkManager, renderingContext.InPageSection);
				Global.Tracer.Assert(childrenNonComputedUniqueNames == null || null == instanceInfo.ChildrenNonComputedUniqueNames);
				if (childrenNonComputedUniqueNames == null)
				{
					childrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
				}
			}
			this.m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
			this.m_reportItemColInstance = reportItemColInstance;
			this.m_reportItemColDef = reportItemColDef;
			this.m_renderingContext = renderingContext;
		}

		public void GetReportItemStartAndEndPages(int currentPage, int index, out int startPage, out int endPage)
		{
			if (0 <= index && index < this.Count)
			{
				startPage = currentPage;
				endPage = currentPage;
				if (this.m_reportItemColInstance != null)
				{
					this.m_reportItemColInstance.GetReportItemStartAndEndPages(index, ref startPage, ref endPage);
				}
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
		}

		internal bool Search(SearchContext searchContext)
		{
			if (this.m_reportItemColDef.Count == 0)
			{
				return false;
			}
			bool flag = false;
			int itemStartPage = 0;
			int itemEndPage = 0;
			ReportItem reportItem = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			int num = 0;
			while (!flag && num < this.m_reportItemColDef.Count)
			{
				reportItem = this[num];
				if (searchContext.ItemStartPage != searchContext.ItemEndPage)
				{
					this.GetReportItemStartAndEndPages(searchContext.SearchPage, num, out itemStartPage, out itemEndPage);
					searchContext2.ItemStartPage = itemStartPage;
					searchContext2.ItemEndPage = itemEndPage;
					if (searchContext2.IsItemOnSearchPage)
					{
						flag = this.SearchRepeatedSiblings(reportItem as DataRegion, searchContext2);
						if (!flag)
						{
							flag = reportItem.Search(searchContext2);
						}
					}
				}
				else
				{
					flag = reportItem.Search(searchContext2);
				}
				num++;
			}
			return flag;
		}

		private bool SearchRepeatedSiblings(DataRegion dataRegion, SearchContext searchContext)
		{
			if (dataRegion == null)
			{
				return false;
			}
			bool flag = false;
			int[] repeatSiblings = dataRegion.GetRepeatSiblings();
			if (repeatSiblings != null)
			{
				int num = 0;
				SearchContext searchContext2 = new SearchContext(searchContext);
				int num2 = 0;
				while (!flag && num2 < repeatSiblings.Length)
				{
					num = repeatSiblings[num2];
					flag = this[num].Search(searchContext2);
					num2++;
				}
			}
			return flag;
		}
	}
}
