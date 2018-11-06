using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimListCell : ShimCell
	{
		private ListContent m_currentListContents;

		private Rectangle m_shimContainer;

		public override CellContents CellContents
		{
			get
			{
				if (base.m_cellContents == null)
				{
					this.m_shimContainer = new Rectangle(this, base.m_inSubtotal, this.m_currentListContents, base.m_owner.RenderingContext);
					base.m_cellContents = new CellContents(this.m_shimContainer, 1, 1, base.m_owner.RenderingContext);
				}
				return base.m_cellContents;
			}
		}

		internal ShimListCell(Tablix owner)
			: base(owner, 0, 0, owner.InSubtotal)
		{
			this.m_currentListContents = owner.RenderList.Contents[0];
		}

		internal void SetCellContents(ListContent renderContents)
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			this.m_currentListContents = renderContents;
			if (this.m_shimContainer != null)
			{
				base.m_cellContents.SetNewContext();
				this.m_shimContainer.UpdateListContents(this.m_currentListContents);
			}
		}
	}
}
