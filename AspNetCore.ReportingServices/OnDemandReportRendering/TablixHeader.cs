using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixHeader : IDefinitionPath
	{
		private Tablix m_owner;

		private TablixMember m_tablixMember;

		private string m_definitionPath;

		private CellContents m_cellContents;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_cacheRenderReportItem;

		public string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = this.ParentDefinitionPath.DefinitionPath + "xH";
				}
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_tablixMember;
			}
		}

		public ReportSize Size
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					if (this.m_owner.SnapshotTablixType != DataRegion.Type.Matrix)
					{
						return null;
					}
					ShimMatrixMember shimMatrixMember = this.m_tablixMember as ShimMatrixMember;
					if (shimMatrixMember.IsColumn)
					{
						return new ReportSize(shimMatrixMember.CurrentRenderMatrixMember.Height);
					}
					return new ReportSize(shimMatrixMember.CurrentRenderMatrixMember.Width);
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.TablixHeader tablixHeader = this.m_tablixMember.MemberDefinition.TablixHeader;
				if (tablixHeader.SizeForRendering == null)
				{
					tablixHeader.SizeForRendering = new ReportSize(tablixHeader.Size, tablixHeader.SizeValue);
				}
				return tablixHeader.SizeForRendering;
			}
		}

		public CellContents CellContents
		{
			get
			{
				if (this.m_cellContents == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (this.m_owner.SnapshotTablixType == DataRegion.Type.Matrix)
						{
							ShimMatrixMember shimMatrixMember = this.m_tablixMember as ShimMatrixMember;
							AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem = shimMatrixMember.IsStatic ? shimMatrixMember.m_staticOrSubtotal.ReportItem : ((MatrixMember)shimMatrixMember.Group.CurrentShimRenderGroup).ReportItem;
							this.m_cellContents = new CellContents(this, this.m_owner.InSubtotal, renderReportItem, shimMatrixMember.RowSpan, shimMatrixMember.ColSpan, this.m_owner.RenderingContext, shimMatrixMember.SizeDelta, shimMatrixMember.IsColumn);
						}
					}
					else
					{
						this.m_cellContents = new CellContents(this.m_tablixMember.ReportScope, this, this.m_tablixMember.MemberDefinition.TablixHeader.CellContents, this.m_tablixMember.RowSpan, this.m_tablixMember.ColSpan, this.m_owner.RenderingContext);
					}
				}
				else if (this.m_owner.IsOldSnapshot)
				{
					this.OnDemandUpdateCellContents();
				}
				return this.m_cellContents;
			}
		}

		internal TablixHeader(Tablix owner, TablixMember tablixMember)
		{
			this.m_owner = owner;
			this.m_tablixMember = tablixMember;
		}

		internal void SetNewContext()
		{
			if (this.m_cellContents != null)
			{
				this.m_cellContents.SetNewContext();
			}
		}

		internal void ResetCellContents()
		{
			this.m_cacheRenderReportItem = null;
		}

		private void OnDemandUpdateCellContents()
		{
			if (this.m_cacheRenderReportItem == null && this.m_cellContents != null)
			{
				this.m_cacheRenderReportItem = ((ShimMatrixMember)this.m_tablixMember).CurrentRenderMatrixMember.ReportItem;
				this.m_cellContents.UpdateRenderReportItem(this.m_cacheRenderReportItem);
			}
		}
	}
}
