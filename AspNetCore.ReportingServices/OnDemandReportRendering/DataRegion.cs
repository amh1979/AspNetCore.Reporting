using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegion : ReportItem, IPageBreakItem, IReportScope, IDataRegion
	{
		internal enum Type
		{
			None,
			List,
			Table,
			Matrix,
			Chart,
			GaugePanel,
			CustomReportItem,
			MapDataRegion
		}

		private ReportStringProperty m_noRowsMessage;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		internal Type m_snapshotDataRegionType;

		public PageBreak PageBreak
		{
			get
			{
				if (this.m_pageBreak == null)
				{
					if (base.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportRendering.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_renderReportItem;
						this.m_pageBreak = new PageBreak(base.m_renderingContext, this.ReportScope, PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd));
					}
					else
					{
						IPageBreakOwner pageBreakOwner = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportItemDef;
						this.m_pageBreak = new PageBreak(base.m_renderingContext, this.ReportScope, pageBreakOwner);
					}
				}
				return this.m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (this.m_pageName == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_pageName = new ReportStringProperty();
					}
					else
					{
						this.m_pageName = new ReportStringProperty(((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportItemDef).PageName);
					}
				}
				return this.m_pageName;
			}
		}

		public ReportStringProperty NoRowsMessage
		{
			get
			{
				if (this.m_noRowsMessage == null)
				{
					if (base.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo noRows = ((AspNetCore.ReportingServices.ReportProcessing.DataRegion)base.m_renderReportItem.ReportItemDef).NoRows;
						this.m_noRowsMessage = new ReportStringProperty(noRows);
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo noRowsMessage = ((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportItemDef).NoRowsMessage;
						this.m_noRowsMessage = new ReportStringProperty(noRowsMessage);
					}
				}
				return this.m_noRowsMessage;
			}
		}

		public string DataSetName
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_renderReportItem).DataSetName;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef).DataSetName;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_isOldSnapshot && this.m_snapshotDataRegionType == Type.List)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					AspNetCore.ReportingServices.ReportRendering.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_renderReportItem;
					return PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd);
				}
				PageBreak pageBreak = this.PageBreak;
				if (pageBreak.HasEnabledInstance)
				{
					return pageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		internal Type DataRegionType
		{
			get
			{
				return this.m_snapshotDataRegionType;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return (IReportScopeInstance)base.Instance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return (IRIFReportScope)base.m_reportItemDef;
			}
		}

		internal override IReportScope ReportScope
		{
			get
			{
				return this;
			}
		}

		bool IDataRegion.HasDataCells
		{
			get
			{
				return this.HasDataCells;
			}
		}

		internal abstract bool HasDataCells
		{
			get;
		}

		IDataRegionRowCollection IDataRegion.RowCollection
		{
			get
			{
				return this.RowCollection;
			}
		}

		internal abstract IDataRegionRowCollection RowCollection
		{
			get;
		}

		internal DataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(null, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal DataRegion(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItem renderDataRegion, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderDataRegion, renderingContext)
		{
		}

		public int[] GetRepeatSiblings()
		{
			if (base.m_isOldSnapshot)
			{
				return ((AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_renderReportItem).GetRepeatSiblings();
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef;
			if (dataRegion.RepeatSiblings == null)
			{
				return new int[0];
			}
			int[] array = new int[dataRegion.RepeatSiblings.Count];
			dataRegion.RepeatSiblings.CopyTo(array);
			return array;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.SetNewContext();
			}
			if (!base.m_isOldSnapshot)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.ReportItemDef;
				dataRegion.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
