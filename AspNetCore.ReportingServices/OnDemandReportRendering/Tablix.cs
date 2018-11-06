using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Tablix : DataRegion, IPageBreakItem
	{
		private TablixCorner m_corner;

		private TablixHierarchy m_columns;

		private TablixHierarchy m_rows;

		private TablixBody m_body;

		private int[] m_matrixRowDefinitionMapping;

		private int[] m_matrixColDefinitionMapping;

		private int m_memberCellDefinitionIndex;

		private MatrixMemberInfoCache m_matrixMemberColIndexes;

		private PageBreakLocation? m_propagatedPageBreak = null;

		private BandLayoutOptions m_bandLayout;

		private ReportSizeProperty m_topMargin;

		private ReportSizeProperty m_bottomMargin;

		private ReportSizeProperty m_leftMargin;

		private ReportSizeProperty m_rightMargin;

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (base.m_isOldSnapshot && Type.List == base.m_snapshotDataRegionType && base.DataElementOutput == DataElementOutputTypes.Output)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return base.DataElementOutput;
			}
		}

		public bool CanScroll
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return this.TablixDef.CanScroll;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_renderReportItem).KeepTogether;
				}
				return this.TablixDef.KeepTogether;
			}
		}

		public TablixLayoutDirection LayoutDirection
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (Type.Matrix == base.m_snapshotDataRegionType)
					{
						return (TablixLayoutDirection)this.RenderMatrix.LayoutDirection;
					}
					return TablixLayoutDirection.LTR;
				}
				if (this.TablixDef.LayoutDirection)
				{
					return TablixLayoutDirection.RTL;
				}
				return TablixLayoutDirection.LTR;
			}
		}

		public TablixCorner Corner
		{
			get
			{
				if (this.m_corner == null)
				{
					this.m_corner = new TablixCorner(this);
				}
				return this.m_corner;
			}
		}

		public TablixHierarchy ColumnHierarchy
		{
			get
			{
				if (this.m_columns == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_columns = new TablixHierarchy(this, true);
					}
					else
					{
						this.m_columns = new TablixHierarchy(this, true);
					}
				}
				return this.m_columns;
			}
		}

		public TablixHierarchy RowHierarchy
		{
			get
			{
				if (this.m_rows == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_rows = new TablixHierarchy(this, false);
					}
					else
					{
						this.m_rows = new TablixHierarchy(this, false);
					}
				}
				return this.m_rows;
			}
		}

		public TablixBody Body
		{
			get
			{
				if (this.m_body == null)
				{
					this.m_body = new TablixBody(this);
				}
				return this.m_body;
			}
		}

		public int Columns
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					switch (base.m_snapshotDataRegionType)
					{
					case Type.List:
						return 0;
					case Type.Table:
						return 0;
					case Type.Matrix:
						return this.RenderMatrix.Columns;
					}
				}
				return this.TablixDef.ColumnHeaderRowCount;
			}
		}

		public int Rows
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					switch (base.m_snapshotDataRegionType)
					{
					case Type.List:
						return 0;
					case Type.Table:
						return 0;
					case Type.Matrix:
						return this.RenderMatrix.Rows;
					}
				}
				return this.TablixDef.RowHeaderColumnCount;
			}
		}

		public int GroupsBeforeRowHeaders
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (Type.Matrix == base.m_snapshotDataRegionType)
					{
						return this.RenderMatrix.GroupsBeforeRowHeaders;
					}
					return 0;
				}
				return this.TablixDef.GroupsBeforeRowHeaders;
			}
		}

		public bool RepeatRowHeaders
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return Type.Matrix == base.m_snapshotDataRegionType;
				}
				return this.TablixDef.RepeatRowHeaders;
			}
		}

		public bool RepeatColumnHeaders
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return Type.Matrix == base.m_snapshotDataRegionType;
				}
				return this.TablixDef.RepeatColumnHeaders;
			}
		}

		public bool FixedRowHeaders
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (Type.Matrix == base.m_snapshotDataRegionType)
					{
						return this.RenderMatrix.RowGroupingFixedHeader;
					}
					if (Type.Table == base.m_snapshotDataRegionType)
					{
						if (this.RenderTable.FixedHeader)
						{
							return !this.RenderTable.HasFixedColumnHeaders;
						}
						return false;
					}
					return false;
				}
				return this.TablixDef.FixedRowHeaders;
			}
		}

		public bool FixedColumnHeaders
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (Type.Matrix == base.m_snapshotDataRegionType)
					{
						return this.RenderMatrix.ColumnGroupingFixedHeader;
					}
					if (Type.Table == base.m_snapshotDataRegionType)
					{
						if (this.RenderTable.FixedHeader)
						{
							return this.RenderTable.HasFixedColumnHeaders;
						}
						return false;
					}
					return false;
				}
				return this.TablixDef.FixedColumnHeaders;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return this.TablixDef.OmitBorderOnPageBreak;
			}
		}

		public BandLayoutOptions BandLayout
		{
			get
			{
				if (!base.m_isOldSnapshot && this.TablixDef.BandLayout != null)
				{
					if (this.m_bandLayout == null)
					{
						this.m_bandLayout = new BandLayoutOptions(this.TablixDef.BandLayout);
					}
					return this.m_bandLayout;
				}
				return null;
			}
		}

		public ReportSizeProperty TopMargin
		{
			get
			{
				return this.GetOrCreateMarginProperty(ref this.m_topMargin, this.TablixDef.TopMargin);
			}
		}

		public ReportSizeProperty BottomMargin
		{
			get
			{
				return this.GetOrCreateMarginProperty(ref this.m_bottomMargin, this.TablixDef.BottomMargin);
			}
		}

		public ReportSizeProperty LeftMargin
		{
			get
			{
				return this.GetOrCreateMarginProperty(ref this.m_leftMargin, this.TablixDef.LeftMargin);
			}
		}

		public ReportSizeProperty RightMargin
		{
			get
			{
				return this.GetOrCreateMarginProperty(ref this.m_rightMargin, this.TablixDef.RightMargin);
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (!this.m_propagatedPageBreak.HasValue)
					{
						this.m_propagatedPageBreak = PageBreakLocation.None;
						TablixMemberCollection memberCollection = this.RowHierarchy.MemberCollection;
					}
					return this.m_propagatedPageBreak.Value;
				}
				PageBreak pageBreak = base.PageBreak;
				if (pageBreak.HasEnabledInstance)
				{
					return pageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		public bool HideStaticsIfNoRows
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (Type.Table == base.m_snapshotDataRegionType)
					{
						if (this.RenderTable.TableHeader == null)
						{
							return null == this.RenderTable.TableFooter;
						}
						return false;
					}
					return true;
				}
				return this.TablixDef.HideStaticsIfNoRows;
			}
		}

		public override ReportSize Width
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Matrix == base.m_snapshotDataRegionType && base.m_cachedWidth == null)
				{
					TablixHierarchy columnHierarchy = this.ColumnHierarchy;
					if (columnHierarchy != null && columnHierarchy.MemberCollection != null)
					{
						base.SetCachedWidth(columnHierarchy.MemberCollection.SizeDelta);
					}
				}
				return base.Width;
			}
		}

		public override ReportSize Height
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Matrix == base.m_snapshotDataRegionType && base.m_cachedHeight == null)
				{
					TablixHierarchy rowHierarchy = this.RowHierarchy;
					if (rowHierarchy != null && rowHierarchy.MemberCollection != null)
					{
						base.SetCachedHeight(rowHierarchy.MemberCollection.SizeDelta);
					}
				}
				return base.Height;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix TablixDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)base.m_reportItemDef;
			}
		}

		internal override bool HasDataCells
		{
			get
			{
				if (this.m_body != null)
				{
					return this.m_body.HasRowCollection;
				}
				return false;
			}
		}

		internal override IDataRegionRowCollection RowCollection
		{
			get
			{
				if (this.m_body != null)
				{
					return this.m_body.RowCollection;
				}
				return null;
			}
		}

		internal Type SnapshotTablixType
		{
			get
			{
				return base.m_snapshotDataRegionType;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.List RenderList
		{
			get
			{
				if (base.m_isOldSnapshot && Type.List == base.m_snapshotDataRegionType)
				{
					return (AspNetCore.ReportingServices.ReportRendering.List)base.m_renderReportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Table RenderTable
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Table == base.m_snapshotDataRegionType)
				{
					return (AspNetCore.ReportingServices.ReportRendering.Table)base.m_renderReportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Matrix RenderMatrix
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Matrix == base.m_snapshotDataRegionType)
				{
					return (AspNetCore.ReportingServices.ReportRendering.Matrix)base.m_renderReportItem;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		internal int[] MatrixRowDefinitionMapping
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Matrix == base.m_snapshotDataRegionType && this.m_matrixRowDefinitionMapping == null)
				{
					this.m_matrixRowDefinitionMapping = this.CalculateMatrixDefinitionMapping(((AspNetCore.ReportingServices.ReportProcessing.Matrix)this.RenderMatrix.ReportItemDef).Rows);
				}
				return this.m_matrixRowDefinitionMapping;
			}
		}

		internal int[] MatrixColDefinitionMapping
		{
			get
			{
				if (base.m_isOldSnapshot && Type.Matrix == base.m_snapshotDataRegionType && this.m_matrixColDefinitionMapping == null)
				{
					this.m_matrixColDefinitionMapping = this.CalculateMatrixDefinitionMapping(((AspNetCore.ReportingServices.ReportProcessing.Matrix)this.RenderMatrix.ReportItemDef).Columns);
				}
				return this.m_matrixColDefinitionMapping;
			}
		}

		internal MatrixMemberInfoCache MatrixMemberColIndexes
		{
			get
			{
				return this.m_matrixMemberColIndexes;
			}
			set
			{
				this.m_matrixMemberColIndexes = value;
			}
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix reportItemDef, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.List renderList, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderList, renderingContext)
		{
			base.m_snapshotDataRegionType = Type.List;
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollection, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Table renderTable, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollection, inSubtotal, renderTable, renderingContext)
		{
			base.m_snapshotDataRegionType = Type.Table;
		}

		internal Tablix(IDefinitionPath parentDefinitionPath, int indexIntoParentCollection, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Matrix renderMatrix, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollection, inSubtotal, renderMatrix, renderingContext)
		{
			base.m_snapshotDataRegionType = Type.Matrix;
		}

		private ReportSizeProperty GetOrCreateMarginProperty(ref ReportSizeProperty property, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			if (!base.m_isOldSnapshot && expression != null)
			{
				if (property == null)
				{
					property = new ReportSizeProperty(expression);
				}
				return property;
			}
			return null;
		}

		internal void SetPageBreakLocation(PageBreakLocation pageBreakLocation)
		{
			if (base.m_isOldSnapshot)
			{
				this.m_propagatedPageBreak = pageBreakLocation;
			}
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new TablixInstance(this);
			}
			return base.m_instance;
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (renderReportItem != null)
			{
				this.m_matrixRowDefinitionMapping = null;
				this.m_matrixColDefinitionMapping = null;
				if (Type.Matrix == base.m_snapshotDataRegionType && this.m_corner != null)
				{
					this.m_corner.ResetContext();
				}
				if (this.m_rows != null)
				{
					this.m_rows.ResetContext(true);
				}
				if (this.m_columns != null)
				{
					this.m_columns.ResetContext(true);
				}
			}
			else
			{
				if (Type.Matrix == base.m_snapshotDataRegionType && this.m_corner != null)
				{
					this.m_corner.ResetContext();
				}
				if (this.m_rows != null)
				{
					this.m_rows.ResetContext(false);
				}
				if (this.m_columns != null)
				{
					this.m_columns.ResetContext(false);
				}
			}
		}

		internal int GetCurrentMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex;
		}

		internal int GetAndIncrementMemberCellDefinitionIndex()
		{
			return this.m_memberCellDefinitionIndex++;
		}

		internal void ResetMemberCellDefinitionIndex(int startIndex)
		{
			this.m_memberCellDefinitionIndex = startIndex;
		}

		private int[] CalculateMatrixDefinitionMapping(MatrixHeading heading)
		{
			List<int> list = new List<int>();
			int num = 0;
			this.AddInnerHierarchy(heading, list, ref num);
			return list.ToArray();
		}

		private void AddInnerHierarchy(MatrixHeading heading, List<int> mapping, ref int definitionIndex)
		{
			if (heading == null)
			{
				mapping.Add(definitionIndex++);
			}
			else if (heading.Grouping == null)
			{
				this.AddInnerStatics(heading, mapping, ref definitionIndex);
			}
			else if (heading.Subtotal != null)
			{
				int num = definitionIndex;
				if (Subtotal.PositionType.Before == heading.Subtotal.Position)
				{
					this.AddInnerStatics(heading, mapping, ref definitionIndex);
					definitionIndex = num;
					this.AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
				}
				else
				{
					this.AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
					definitionIndex = num;
					this.AddInnerStatics(heading, mapping, ref definitionIndex);
				}
			}
			else
			{
				this.AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
			}
		}

		private void AddInnerStatics(MatrixHeading heading, List<int> mapping, ref int definitionIndex)
		{
			if (heading == null)
			{
				mapping.Add(definitionIndex++);
			}
			else if (heading.Grouping == null)
			{
				int count = heading.ReportItems.Count;
				for (int i = 0; i < count; i++)
				{
					this.AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
				}
			}
			else
			{
				this.AddInnerHierarchy(heading.SubHeading, mapping, ref definitionIndex);
			}
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_corner != null)
			{
				this.m_corner.SetNewContext();
			}
			if (this.m_rows != null)
			{
				this.m_rows.SetNewContext();
			}
			if (this.m_columns != null)
			{
				this.m_columns.SetNewContext();
			}
			if (base.m_reportItemDef != null)
			{
				((AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)base.m_reportItemDef).ResetTextBoxImpls(base.m_renderingContext.OdpContext);
			}
		}
	}
}
