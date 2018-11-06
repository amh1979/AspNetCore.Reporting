using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Rectangle : ReportItem
	{
		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		private ReportItemCollection m_reportItemCollection;

		public override object SharedRenderingInfo
		{
			get
			{
				Hashtable sharedRenderingInfo = base.RenderingContext.RenderingInfoManager.SharedRenderingInfo;
				if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Report)
				{
					return sharedRenderingInfo[((AspNetCore.ReportingServices.ReportProcessing.Report)base.ReportItemDef).BodyID];
				}
				return sharedRenderingInfo[base.ReportItemDef.ID];
			}
			set
			{
				Hashtable sharedRenderingInfo = base.RenderingContext.RenderingInfoManager.SharedRenderingInfo;
				if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Report)
				{
					sharedRenderingInfo[((AspNetCore.ReportingServices.ReportProcessing.Report)base.ReportItemDef).BodyID] = value;
				}
				else
				{
					sharedRenderingInfo[base.ReportItemDef.ID] = value;
				}
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = this.m_reportItemCollection;
				if (this.m_reportItemCollection == null)
				{
					RenderingContext renderingContext = (!base.RenderingContext.InPageSection) ? base.RenderingContext : new RenderingContext(base.RenderingContext, base.UniqueName);
					AspNetCore.ReportingServices.ReportProcessing.ReportItemCollection reportItems;
					if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Report)
					{
						reportItems = ((AspNetCore.ReportingServices.ReportProcessing.Report)base.ReportItemDef).ReportItems;
					}
					else
					{
						Global.Tracer.Assert(base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle);
						reportItems = ((AspNetCore.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).ReportItems;
					}
					ReportItemColInstance reportItemColInstance = null;
					if (base.ReportItemInstance != null)
					{
						if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Report)
						{
							reportItemColInstance = ((ReportInstance)base.ReportItemInstance).ReportItemColInstance;
						}
						else
						{
							Global.Tracer.Assert(base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle);
							reportItemColInstance = ((RectangleInstance)base.ReportItemInstance).ReportItemColInstance;
						}
					}
					reportItemCollection = new ReportItemCollection(reportItems, reportItemColInstance, renderingContext, this.m_childrenNonComputedUniqueNames);
					if (base.RenderingContext.CacheState)
					{
						this.m_reportItemCollection = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		public bool PageBreakAtEnd
		{
			get
			{
				if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).PageBreakAtEnd;
				}
				return false;
			}
		}

		public bool PageBreakAtStart
		{
			get
			{
				if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).PageBreakAtStart;
				}
				return false;
			}
		}

		public override int LinkToChild
		{
			get
			{
				if (base.ReportItemDef is AspNetCore.ReportingServices.ReportProcessing.Rectangle)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.Rectangle)base.ReportItemDef).LinkToChild;
				}
				return -1;
			}
		}

		internal Rectangle(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			this.m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			ReportItemCollection reportItemCollection = this.ReportItemCollection;
			if (reportItemCollection == null)
			{
				return false;
			}
			return reportItemCollection.Search(searchContext);
		}
	}
}
