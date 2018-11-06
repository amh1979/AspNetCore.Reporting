using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixHierarchy : MemberHierarchy<TablixMember>
	{
		private Tablix OwnerTablix
		{
			get
			{
				return base.m_owner as Tablix;
			}
		}

		public TablixMemberCollection MemberCollection
		{
			get
			{
				if (base.m_members == null)
				{
					if (this.OwnerTablix.IsOldSnapshot)
					{
						switch (this.OwnerTablix.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							if (base.m_isColumn)
							{
								base.m_members = new ShimListMemberCollection(this, this.OwnerTablix);
							}
							else
							{
								base.m_members = new ShimListMemberCollection(this, this.OwnerTablix, this.OwnerTablix.RenderList.Contents);
							}
							break;
						case DataRegion.Type.Table:
							this.OwnerTablix.ResetMemberCellDefinitionIndex(0);
							base.m_members = new ShimTableMemberCollection(this, this.OwnerTablix, base.m_isColumn);
							break;
						case DataRegion.Type.Matrix:
						{
							this.OwnerTablix.ResetMemberCellDefinitionIndex(0);
							MatrixMemberCollection renderMemberCollection = base.m_isColumn ? this.OwnerTablix.RenderMatrix.ColumnMemberCollection : this.OwnerTablix.RenderMatrix.RowMemberCollection;
							base.m_members = new ShimMatrixMemberCollection(this, this.OwnerTablix, base.m_isColumn, null, renderMemberCollection, this.CreateMatrixMemberCache());
							break;
						}
						}
						if (!base.m_isColumn)
						{
							this.CalculatePropagatedPageBreak();
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablixDef = this.OwnerTablix.TablixDef;
						if (tablixDef.TablixColumns != null)
						{
							base.m_members = new InternalTablixMemberCollection(this, this.OwnerTablix, null, base.m_isColumn ? tablixDef.TablixColumnMembers : tablixDef.TablixRowMembers);
						}
					}
				}
				return (TablixMemberCollection)base.m_members;
			}
		}

		internal TablixHierarchy(Tablix owner, bool isColumn)
			: base((ReportItem)owner, isColumn)
		{
		}

		private void CalculatePropagatedPageBreak()
		{
			AspNetCore.ReportingServices.ReportRendering.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_owner.RenderReportItem;
			bool thisOrAnscestorHasToggle = dataRegion.SharedHidden == AspNetCore.ReportingServices.ReportRendering.SharedHiddenState.Sometimes;
			PageBreakLocation pageBreakLocation = PageBreakHelper.GetPageBreakLocation(dataRegion.PageBreakAtStart, dataRegion.PageBreakAtEnd);
			if (base.m_members != null && base.m_members.Count > 0)
			{
				pageBreakLocation = PageBreakHelper.MergePageBreakLocations(this.CalculatePropagatedPageBreak(base.m_members, thisOrAnscestorHasToggle, this.OwnerTablix.SnapshotTablixType == DataRegion.Type.Table), pageBreakLocation);
			}
			this.OwnerTablix.SetPageBreakLocation(pageBreakLocation);
		}

		private PageBreakLocation CalculatePropagatedPageBreak(DataRegionMemberCollection<TablixMember> members, bool thisOrAnscestorHasToggle, bool isTable)
		{
			PageBreakLocation result = PageBreakLocation.None;
			bool flag = false;
			ShimTablixMember shimTablixMember = null;
			int num = 0;
			while (num < members.Count)
			{
				ShimTablixMember shimTablixMember2 = (ShimTablixMember)((ReportElementCollectionBase<TablixMember>)members)[num];
				if (shimTablixMember2.IsStatic)
				{
					if (isTable)
					{
						if (shimTablixMember2.RepeatOnNewPage)
						{
							flag = true;
						}
					}
					else if (shimTablixMember2.Children != null && shimTablixMember2.Children.Count > 0)
					{
						result = this.CalculatePropagatedPageBreak(shimTablixMember2.Children, thisOrAnscestorHasToggle, false);
					}
					num++;
					continue;
				}
				shimTablixMember = shimTablixMember2;
				break;
			}
			if (shimTablixMember != null)
			{
				thisOrAnscestorHasToggle |= (shimTablixMember.Visibility != null && shimTablixMember.Visibility.HiddenState == SharedHiddenState.Sometimes);
				PageBreakLocation pageBreakLocation = PageBreakLocation.None;
				AspNetCore.ReportingServices.ReportRendering.Group currentShimRenderGroup = shimTablixMember.Group.CurrentShimRenderGroup;
				if (currentShimRenderGroup != null)
				{
					pageBreakLocation = PageBreakHelper.GetPageBreakLocation(currentShimRenderGroup.PageBreakAtStart, currentShimRenderGroup.PageBreakAtEnd);
				}
				if (shimTablixMember.Children != null)
				{
					pageBreakLocation = PageBreakHelper.MergePageBreakLocations(this.CalculatePropagatedPageBreak(shimTablixMember.Children, thisOrAnscestorHasToggle, isTable), pageBreakLocation);
				}
				shimTablixMember.SetPropagatedPageBreak(pageBreakLocation);
				if ((!isTable || flag) && pageBreakLocation != 0)
				{
					if (!thisOrAnscestorHasToggle)
					{
						result = pageBreakLocation;
					}
					shimTablixMember.SetPropagatedPageBreak(PageBreakLocation.Between);
				}
			}
			return result;
		}

		internal override void ResetContext()
		{
			this.ResetContext(true);
		}

		internal void ResetContext(bool clearCache)
		{
			if (clearCache)
			{
				this.OwnerTablix.ResetMemberCellDefinitionIndex(0);
			}
			if (base.m_members != null && this.OwnerTablix.IsOldSnapshot)
			{
				switch (this.OwnerTablix.SnapshotTablixType)
				{
				case DataRegion.Type.List:
					if (!base.m_isColumn)
					{
						((ShimListMemberCollection)base.m_members).UpdateContext(this.OwnerTablix.RenderList.Contents);
					}
					break;
				case DataRegion.Type.Table:
					if (!base.m_isColumn)
					{
						((ShimTableMemberCollection)base.m_members).UpdateContext();
					}
					break;
				case DataRegion.Type.Matrix:
				{
					MatrixMemberInfoCache matrixMemberCellIndexes = null;
					if (clearCache && base.m_isColumn)
					{
						matrixMemberCellIndexes = this.CreateMatrixMemberCache();
					}
					((ShimMatrixMemberCollection)base.m_members).UpdateContext(matrixMemberCellIndexes);
					break;
				}
				}
			}
		}

		private MatrixMemberInfoCache CreateMatrixMemberCache()
		{
			if (base.m_isColumn)
			{
				MatrixMemberCollection columnMemberCollection = this.OwnerTablix.RenderMatrix.ColumnMemberCollection;
				if (columnMemberCollection.MatrixHeadingDef.SubHeading != null)
				{
					this.OwnerTablix.MatrixMemberColIndexes = new MatrixMemberInfoCache(-1, columnMemberCollection.Count);
				}
				else
				{
					this.OwnerTablix.MatrixMemberColIndexes = new MatrixMemberInfoCache(0, -1);
				}
				return this.OwnerTablix.MatrixMemberColIndexes;
			}
			return null;
		}
	}
}
