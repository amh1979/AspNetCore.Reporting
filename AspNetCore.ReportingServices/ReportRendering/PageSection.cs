using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class PageSection : ReportItem
	{
		internal const string PageHeaderUniqueNamePrefix = "ph";

		internal const string PageFooterUniqueNamePrefix = "pf";

		private ReportItemCollection m_reportItems;

		private AspNetCore.ReportingServices.ReportProcessing.PageSection m_pageSectionDef;

		private bool m_pageDef;

		private PageSectionInstance m_pageSectionInstance;

		public bool PrintOnFirstPage
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.PageSection)base.ReportItemDef).PrintOnFirstPage;
			}
		}

		public bool PrintOnLastPage
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.PageSection)base.ReportItemDef).PrintOnLastPage;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = this.m_reportItems;
				if (this.m_reportItems == null)
				{
					reportItemCollection = new ReportItemCollection(this.m_pageSectionDef.ReportItems, (this.m_pageSectionInstance == null) ? null : this.m_pageSectionInstance.ReportItemColInstance, base.RenderingContext, null);
					if (base.RenderingContext.CacheState)
					{
						this.m_reportItems = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		internal PageSection(string uniqueName, AspNetCore.ReportingServices.ReportProcessing.PageSection pageSectionDef, PageSectionInstance pageSectionInstance, Report report, RenderingContext renderingContext, bool pageDef)
			: base(uniqueName, 0, pageSectionDef, pageSectionInstance, renderingContext)
		{
			this.m_pageSectionDef = pageSectionDef;
			this.m_pageSectionInstance = pageSectionInstance;
			this.m_pageDef = pageDef;
		}

		public ReportItem Find(string uniqueName)
		{
			if (uniqueName != null && uniqueName.Length > 0)
			{
				if (uniqueName.Equals(base.UniqueName))
				{
					return this;
				}
				char[] separator = new char[1]
				{
					'a'
				};
				string[] array = uniqueName.Split(separator);
				if (array != null && array.Length >= 2)
				{
					object obj = (this.m_pageSectionInstance != null) ? ((ISearchByUniqueName)this.m_pageSectionInstance) : ((ISearchByUniqueName)this.m_pageSectionDef);
					NonComputedUniqueNames nonComputedUniqueNames = null;
					int num = -1;
					for (int i = 1; i < array.Length; i++)
					{
						IIndexInto indexInto = obj as IIndexInto;
						if (indexInto == null)
						{
							obj = null;
							break;
						}
						num = ReportItem.StringToInt(array[i]);
						NonComputedUniqueNames nonComputedUniqueNames2 = null;
						obj = indexInto.GetChildAt(num, out nonComputedUniqueNames2);
						if (nonComputedUniqueNames == null)
						{
							nonComputedUniqueNames = nonComputedUniqueNames2;
							continue;
						}
						if (nonComputedUniqueNames.ChildrenUniqueNames == null || num < 0 || num >= nonComputedUniqueNames.ChildrenUniqueNames.Length)
						{
							return null;
						}
						nonComputedUniqueNames = nonComputedUniqueNames.ChildrenUniqueNames[num];
					}
					if (obj == null)
					{
						return null;
					}
					if (obj is AspNetCore.ReportingServices.ReportProcessing.ReportItem)
					{
						AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef = (AspNetCore.ReportingServices.ReportProcessing.ReportItem)obj;
						return ReportItem.CreateItem(uniqueName, reportItemDef, null, base.RenderingContext, nonComputedUniqueNames);
					}
					ReportItemInstance reportItemInstance = (ReportItemInstance)obj;
					return ReportItem.CreateItem(uniqueName, reportItemInstance.ReportItemDef, reportItemInstance, base.RenderingContext, nonComputedUniqueNames);
				}
				return null;
			}
			return null;
		}

		internal override bool Search(SearchContext searchContext)
		{
			ReportItemCollection reportItemCollection = this.ReportItemCollection;
			if (reportItemCollection == null)
			{
				return false;
			}
			return reportItemCollection.Search(searchContext);
		}
	}
}
